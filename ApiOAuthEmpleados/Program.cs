using ApiOAuthEmpleados.Data;
using ApiOAuthEmpleados.Helpers;
using ApiOAuthEmpleados.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NSwag.Generation.Processors.Security;
using NSwag;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<HelperOAuthToken>();

HelperOAuthToken helper = new HelperOAuthToken(builder.Configuration);

builder.Services.AddAuthentication(helper.GetAuthenticationOptions())
    .AddJwtBearer(helper.GetJwtOptions());



string connectionString =
    builder.Configuration.GetConnectionString("SqlServer");
builder.Services.AddTransient<RepositoryEmpleados>();
builder.Services.AddDbContext<EmpleadosContext>
    (options => options.UseSqlServer(connectionString));



// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

//builder.Services.AddSwaggerGen(options =>
//{
//    options.SwaggerDoc("v1", new OpenApiInfo
//    {
//        Title="Api ", Version="v1"
//        , Description = "Seguridad Api"

//    });
//});

builder.Services.AddOpenApiDocument(document =>
{
    document.Title = "Api OAuth Empleados";
    document.Description = "Api Timers 2023. Ejemolo OAuth";
    // CONFIGURAMOS LA SEGURIDAD JWT PARA SWAGGER,
    // PERMITE AÑADIR EL TOKEN JWT A LA CABECERA.
    document.AddSecurity("JWT", Enumerable.Empty<string>(),
        new NSwag.OpenApiSecurityScheme
        {
            Type = OpenApiSecuritySchemeType.ApiKey,
            Name = "Authorization",
            In = OpenApiSecurityApiKeyLocation.Header,
            Description = "Copia y pega el Token en el campo 'Value:' así: Bearer {Token JWT}."
        }
    );
    document.OperationProcessors.Add(
        new AspNetCoreOperationSecurityScopeProcessor("JWT"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.


app.UseOpenApi();
app.UseSwaggerUI(options =>
{
    options.InjectStylesheet("/css/bootstrap.css");
    options.InjectStylesheet("/css/material3x.css");
    options.SwaggerEndpoint(
        url: "/swagger/v1/swagger.json", name: "Api v1");
    options.RoutePrefix = "";
    options.DocExpansion(DocExpansion.None);
});

//app.UseSwagger();
//app.UseSwaggerUI(options =>
//{
//    options.SwaggerEndpoint("/swagger/v1/swagger.json"
//        , "Api OAuth Empleados");
//    options.RoutePrefix = "";
//}

//   );
//if (app.Environment.IsDevelopment())
//{
   
//}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
