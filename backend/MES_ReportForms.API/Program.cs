using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using MES_ReportForms.Core;
using MES_ReportForms.API;
using MES_ReportForms.API.Middlewares;
using MES_ReportForms.API.Filters;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.FileProviders;
using OfficeOpenXml;
using Microsoft.AspNetCore.Http.Connections;

var builder = WebApplication.CreateBuilder(args);

ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
 
builder.Services.AddLogging(log => log.AddLog4Net());

builder.Services.AddLocalization(option => option.ResourcesPath = "Resources");
  
builder.Services.AddControllers();
 
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    foreach (FieldInfo field in typeof(ApiVersionInfo).GetFields())
    {
        if (field.FieldType == typeof(ApiVersionInfo.ApiVersionDetail))
        {
            var apiVersionDetail = (ApiVersionInfo.ApiVersionDetail)field.GetValue(null);
            options.SwaggerDoc(field.Name, new OpenApiInfo
            {
                Version = apiVersionDetail?.Version ?? "V1",
                Title = apiVersionDetail?.Title ?? "MES_ReportForms Service WebAPI",
                Description = apiVersionDetail?.Description,
            });
        }
        else
        {
            options.SwaggerDoc(field.Name, new OpenApiInfo
            {
                Version = "V1",
                Title = "MES_ReportForms Service WebAPI",
                Description = ""
            });
        }
    }

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename), true); 
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{typeof(MES_ReportForms.Core.Constants).Assembly.GetName().Name}.xml"));
    options.SchemaFilter<SwaggerEnumSchemaFilter>();
    options.UseInlineDefinitionsForEnums();

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "请输入token,格式为 Bearer xxxxxxxx（注意中间必须有空格）",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    //添加安全要求
    options.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme{
                Reference =new OpenApiReference{
                    Type = ReferenceType.SecurityScheme,
                    Id ="Bearer"
                }
            },new string[]{ }
        }
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = TokenParameter.Issuer,
                ValidAudience = TokenParameter.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TokenParameter.Secret))
            };
        });

// Add MediatR, to support command pattern.
builder.Services.AddMediatR(c =>
    c.RegisterServicesFromAssembly(typeof(Program).Assembly)
);

// 注入Core实例-EF
builder.Services.AddCoreServices(builder.Configuration);

builder.Services.AddSignalR();  

var app = builder.Build();

app.UseStaticFiles();
 
app.UseMiddleware<LoggingMiddleware>();

app.UseMiddleware<GlobalExceptionMiddleware>(app.Environment.IsDevelopment());

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        foreach (FieldInfo field in typeof(ApiVersionInfo).GetFields())
        {
            // 用相对路径，可兼容IIS部署ANCM虚拟目录路径问题
            c.SwaggerEndpoint($"{field.Name}/swagger.json", $"{field.Name}");
            //c.SwaggerEndpoint($"/swagger/{field.Name}/swagger.json", $"{field.Name}");
            c.DefaultModelExpandDepth(2);
            c.DefaultModelsExpandDepth(0);
        }
        c.ShowCommonExtensions();
        c.EnableFilter();
    });
}

app.UseAuthentication();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseRequestLocalization();

app.Run();
 
public partial class Program { }