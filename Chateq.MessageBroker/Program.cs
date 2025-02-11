using Chateq.Core.Domain;
using Chateq.Core.Domain.Options;
using Chateq.MessageBroker;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.Configure<KafkaOption>(options
    => configuration.GetSection(nameof(KafkaOption)).Bind(options));

var connectionString = configuration.GetValue<string>("ConnectionString");
builder.Services.AddDbContextFactory<ChateqDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddHostedService<KafkaConsumer>();

var app = builder.Build();
app.Run();