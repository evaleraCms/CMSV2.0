using CMS.Api.Core.Attributes;
using CMS.Api.Core.Filter;
using CMS.Core.Common;
using CMS.Core.Extensions;
using CorrelationId.DependencyInjection;
using Microsoft.AspNetCore.Server.Kestrel;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.ComponentModel.Design;

var builder = WebApplication.CreateBuilder(args);
AppData.Configuration = builder.Configuration;
builder.Services.AddScoped<SanitizedResponseFilter>();
builder.Services.AddScoped<SanitizeRequestFilter>();

builder.Services.AddControllers(opt => {
    opt.Filters.AddService<SanitizedResponseFilter>(-3);
    opt.Filters.AddService<SanitizeRequestFilter>(-2);
}).AddNewtonsoftJson(opt => { 
    opt.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
    opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    opt.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
    opt.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();

});

builder.Services.AddHttpContextAccessor();
builder.Services.Configure<IISServerOptions>(opt => {
    opt.MaxRequestBodySize = int.MaxValue;
});
builder.Services.Configure<KestrelServerOptions>(opt => {
    opt.Limits.MaxRequestBodySize = int.MaxValue;
});



builder.Services.AddHttpsRedirection(options => {
    options.HttpsPort = 443; // or your specific HTTPS port
});

builder.Services.AddDefaultCorrelationId();
builder.Services.AddScoped<IServiceContainer,ServiceContainer>();
AppDomain.CurrentDomain.SetData("ContentRootPath",builder.Environment.ContentRootPath);
//TODO: add all the registars here
// var registars = new DependencyRegistrar();
string[] endpoints = new[] { "v1" };
var appEndpoints = builder.Configuration["Endpoints"]?.Split(';') ?? Array.Empty<string>();
builder.Services.AddSwaggerGen(c => {
  c.CustomSchemaIds(s=> s.FullName?.Replace("+","."));
    foreach (var appEndPoint in endpoints)
    {
        var endpoint = endpoints.FirstOrDefault(q => q.EqualsIgnoreCase(appEndPoint));
        if (endpoint.IsEmpty()) continue;

        if (endpoint == "v1") {
            c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo()
            {
                Title = "CMS REST API",
                Version = "v1",                 
                Description = "CMS REST API for version 1",
                Contact = new Microsoft.OpenApi.Models.OpenApiContact()
                {
                    Name = "CMS Team",
                    Email = "CustomerSupport@cms.com"
                }
            });
        }


    }
    c.OperationFilter<CustomHeaderSwaggerAttribute>();
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});
builder.Services.AddSwaggerGenNewtonsoftSupport();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseHttpsRedirection();

var pathBase = builder.Configuration["PathBase"];
//Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    
    app.UseSwagger();
    
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "CMS.RestApi v1");
        options.RoutePrefix = ""; // Swagger UI at root
    });
}

app.UseAuthorization();

app.MapControllers();

app.Run();

