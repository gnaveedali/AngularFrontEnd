using Microsoft.Extensions.FileProviders;
using Standard_Trading_API.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using Microsoft.OpenApi.Models;
using Data_Access_Layer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.




builder.Services.AddControllers().AddNewtonsoftJson(
                x =>
                {
                    x.SerializerSettings.ContractResolver = new DefaultContractResolver();
                    x.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                }
                )
               .AddJsonOptions(
                //x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve
                options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                }

                );
builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.SuppressConsumesConstraintForFormFileParameters = true;
    options.SuppressInferBindingSourcesForParameters = true;
    options.SuppressModelStateInvalidFilter = true;
    options.SuppressMapClientErrors = true;
    options.ClientErrorMapping[StatusCodes.Status404NotFound].Link =
        "https://httpstatuses.com/404";
});

// Get connection string and assign to DBEntity
string connString = builder.Configuration.GetConnectionString("DefaultConnection");
DBEntity.conStr = connString;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      builder =>
                      {
                          builder.WithOrigins("*");
                          builder.WithHeaders("*");
                          builder.WithMethods("*");
                      });
});
// Add services to the container.





builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

//builder.Services.AddSwaggerGen(options =>
//{
//    //options.CustomSchemaIds(type => type.ToString());
//     options.CustomSchemaIds(x => x.FullName);
//    //options.CustomSchemaIds(type => $"{type.Name}_{System.Guid.NewGuid()}");


//});

builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});


// CUSTOME SERVICES STARTS
builder.Services.AddTransient<Data_Access_Layer.Api_Url_Service>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


// custom service for password hash maker
builder.Services.AddTransient<PasswordHashService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCors(MyAllowSpecificOrigins);

app.MapControllers();

app.Run();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Excel")),
    RequestPath = "/Excel"
});


