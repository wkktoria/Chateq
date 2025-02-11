using Chateq.API.Extensions;
using Chateq.API.Hubs;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token.",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer",
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            []
        }
    });
});

builder.Services.AddConfiguration(configuration);
builder.Services.AddServices();
builder.Services.AddOptions(configuration);

var origin = configuration.GetValue<string>("Origin") ?? throw new NullReferenceException("The origin is missing.");
builder.Services.AddCors(options => options.AddPolicy("CorsPolicy",
    policyBuilder =>
    {
        policyBuilder.WithOrigins(origin).AllowAnyHeader().AllowAnyMethod().SetIsOriginAllowed(host => true)
            .AllowCredentials();
    }));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");

app.UseHttpsRedirection();

app.MapHub<MessageHub>("/messageHub");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();