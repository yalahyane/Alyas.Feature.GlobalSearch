using CommonServiceLocator;
using SolrNet;
using SolrNet.Impl;
using SolrNet.Impl.DocumentPropertyVisitors;
using SolrNet.Impl.FacetQuerySerializers;
using SolrNet.Impl.FieldParsers;
using SolrNet.Impl.FieldSerializers;
using SolrNet.Impl.QuerySerializers;
using SolrNet.Impl.ResponseParsers;
using SolrNet.Mapping;
using SolrNet.Mapping.Validation;
using SolrNet.Mapping.Validation.Rules;
using SolrNet.Schema;
using SolrNet.Utils;

namespace Alyas.Feature.GlobalSearch
{
    public static class SolrIndexManager
    {
        public static readonly Container Container = new();

        static SolrIndexManager() => InitContainer();

        public static void InitContainer()
        {
            ServiceLocator.SetLocatorProvider(() => Container);
            Container.Clear();
            var mapper = new MemoizingMappingManager(new AttributesMappingManager());
            Container.Register((Converter<IContainer, IReadOnlyMappingManager>)(_ => mapper));
            var fieldParser = new DefaultFieldParser();
            Container.Register((Converter<IContainer, ISolrFieldParser>)(_ => fieldParser));
            var fieldSerializer = new DefaultFieldSerializer();
            Container.Register((Converter<IContainer, ISolrFieldSerializer>)(_ => fieldSerializer));
            Container.Register((Converter<IContainer, ISolrQuerySerializer>)(c => new DefaultQuerySerializer(c.GetInstance<ISolrFieldSerializer>())));
            Container.Register((Converter<IContainer, ISolrFacetQuerySerializer>)(c => new DefaultFacetQuerySerializer(c.GetInstance<ISolrQuerySerializer>(), c.GetInstance<ISolrFieldSerializer>())));
            Container.Register((Converter<IContainer, ISolrDocumentPropertyVisitor>)(c => new DefaultDocumentVisitor(c.GetInstance<IReadOnlyMappingManager>(), c.GetInstance<ISolrFieldParser>())));
            var solrSchemaParser = new SolrSchemaParser();
            Container.Register((Converter<IContainer, ISolrSchemaParser>)(_ => solrSchemaParser));
            var solrDIHStatusParser = new SolrDIHStatusParser();
            Container.Register((Converter<IContainer, ISolrDIHStatusParser>)(_ => solrDIHStatusParser));
            var headerParser = new HeaderResponseParser<string>();
            Container.Register((Converter<IContainer, ISolrHeaderResponseParser>)(_ => headerParser));
            var extractResponseParser = new ExtractResponseParser(headerParser);
            Container.Register((Converter<IContainer, ISolrExtractResponseParser>)(_ => extractResponseParser));
            Container.Register(typeof(MappedPropertiesIsInSolrSchemaRule).FullName, (Converter<IContainer, IValidationRule>)(_ => new MappedPropertiesIsInSolrSchemaRule()));
            Container.Register(typeof(RequiredFieldsAreMappedRule).FullName, (Converter<IContainer, IValidationRule>)(_ => new RequiredFieldsAreMappedRule()));
            Container.Register(typeof(UniqueKeyMatchesMappingRule).FullName, (Converter<IContainer, IValidationRule>)(_ => new UniqueKeyMatchesMappingRule()));
            Container.Register(typeof(MultivaluedMappedToCollectionRule).FullName, (Converter<IContainer, IValidationRule>)(_ => new MultivaluedMappedToCollectionRule()));
            Container.Register((Converter<IContainer, IMappingValidator>)(c => new MappingValidator(c.GetInstance<IReadOnlyMappingManager>(), c.GetAllInstances<IValidationRule>().ToArray())));
            Container.Register((Converter<IContainer, ISolrStatusResponseParser>)(_ => new SolrStatusResponseParser()));
        }

        public static void Init<T>(string serverURL, string indexName) => Init<T>(new SolrConnection(serverURL + "/" + indexName), indexName);

        public static void Init<T>(ISolrConnection connection, string indexName)
        {
            Container.Register(indexName, _ => connection);
            var activator = new SolrDocumentActivator<T>();
            Container.Register((Converter<IContainer, ISolrDocumentActivator<T>>)(_ => activator));
            Container.Register(ChooseDocumentResponseParser<T>);
            Container.Register((Converter<IContainer, ISolrAbstractResponseParser<T>>)(c => new DefaultResponseParser<T>(c.GetInstance<ISolrDocumentResponseParser<T>>())));
            Container.Register((Converter<IContainer, ISolrMoreLikeThisHandlerQueryResultsParser<T>>)(c => new SolrMoreLikeThisHandlerQueryResultsParser<T>(c.GetAllInstances<ISolrAbstractResponseParser<T>>().ToArray())));
            Container.Register((Converter<IContainer, ISolrQueryExecuter<T>>)(c => new SolrQueryExecuter<T>(c.GetInstance<ISolrAbstractResponseParser<T>>(), connection, c.GetInstance<ISolrQuerySerializer>(), c.GetInstance<ISolrFacetQuerySerializer>(), c.GetInstance<ISolrMoreLikeThisHandlerQueryResultsParser<T>>())));
            Container.Register(ChooseDocumentSerializer<T>);
            Container.Register((Converter<IContainer, ISolrBasicOperations<T>>)(c => new SolrBasicServer<T>(connection, c.GetInstance<ISolrQueryExecuter<T>>(), c.GetInstance<ISolrDocumentSerializer<T>>(), c.GetInstance<ISolrSchemaParser>(), c.GetInstance<ISolrHeaderResponseParser>(), c.GetInstance<ISolrQuerySerializer>(), c.GetInstance<ISolrDIHStatusParser>(), c.GetInstance<ISolrExtractResponseParser>())));
            Container.Register((Converter<IContainer, ISolrBasicReadOnlyOperations<T>>)(c => new SolrBasicServer<T>(connection, c.GetInstance<ISolrQueryExecuter<T>>(), c.GetInstance<ISolrDocumentSerializer<T>>(), c.GetInstance<ISolrSchemaParser>(), c.GetInstance<ISolrHeaderResponseParser>(), c.GetInstance<ISolrQuerySerializer>(), c.GetInstance<ISolrDIHStatusParser>(), c.GetInstance<ISolrExtractResponseParser>())));
            Container.Register((Converter<IContainer, ISolrOperations<T>>)(c => new SolrServer<T>(c.GetInstance<ISolrBasicOperations<T>>(), Container.GetInstance<IReadOnlyMappingManager>(), Container.GetInstance<IMappingValidator>())));
            Container.Register((Converter<IContainer, ISolrReadOnlyOperations<T>>)(c => new SolrServer<T>(c.GetInstance<ISolrBasicOperations<T>>(), Container.GetInstance<IReadOnlyMappingManager>(), Container.GetInstance<IMappingValidator>())));
            var key2 = nameof(ISolrCoreAdmin) + indexName;
            Container.Register(key2, (Converter<IContainer, ISolrCoreAdmin>)(c => new SolrCoreAdmin(connection, c.GetInstance<ISolrHeaderResponseParser>(), c.GetInstance<ISolrStatusResponseParser>())));
        }

        private static ISolrDocumentSerializer<T> ChooseDocumentSerializer<T>(
          IServiceLocator c)
        {
            return typeof(T) == typeof(Dictionary<string, object>) ? (ISolrDocumentSerializer<T>)new SolrDictionarySerializer(c.GetInstance<ISolrFieldSerializer>()) : new SolrDocumentSerializer<T>(c.GetInstance<IReadOnlyMappingManager>(), c.GetInstance<ISolrFieldSerializer>());
        }

        private static ISolrDocumentResponseParser<T> ChooseDocumentResponseParser<T>(
          IServiceLocator c)
        {
            return typeof(T) == typeof(Dictionary<string, object>) ? (ISolrDocumentResponseParser<T>)new SolrDictionaryDocumentResponseParser(c.GetInstance<ISolrFieldParser>()) : new SolrDocumentResponseParser<T>(c.GetInstance<IReadOnlyMappingManager>(), c.GetInstance<ISolrDocumentPropertyVisitor>(), c.GetInstance<ISolrDocumentActivator<T>>());
        }
    }
}
