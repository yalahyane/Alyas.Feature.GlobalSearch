using Alyas.Feature.GlobalSearch.Configuration;
using Alyas.Feature.GlobalSearch.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<OrderCloudConfiguration>(
    builder.Configuration.GetSection("OrderCloudConfiguration")
);

builder.Services.Configure<SolrConfiguration>(
    builder.Configuration.GetSection("SolrConfiguration")
);

builder.Services.AddTransient<IAuthenticationService, AuthenticationService>();
builder.Services.AddTransient<IOrderCloudService, OrderCloudService>();
builder.Services.AddTransient<ISolrService, SolrService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
