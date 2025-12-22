using MedCore;
using MedCore.Interfaces;
using MedORM.Migrations;
using MedORM.ORM;
using MedORM.Repo;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

var builder = WebApplication.CreateBuilder(args);

// dev
//var connectionString = builder.Configuration.GetConnectionString("PostgreSQL")
//    ?? throw new InvalidOperationException("Connection string 'PostgreSQL' not found."); 

//check this
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddSingleton<MedDbContext>(sp =>
    new MedDbContext(connectionString));


builder.Services.AddScoped<IRepositoryFactory, RepositoryFactory>();

/// zašto ovako registriramo repozitorije umjesto preko factory-ja?
builder.Services.AddScoped<IPatientRepository>(sp =>
{
    var context = sp.GetRequiredService<MedDbContext>();
    return new PatientRepository(context);
});

builder.Services.AddScoped<IDoctorRepository>(sp =>
{
    var context = sp.GetRequiredService<MedDbContext>();
    return new DoctorRepository(context);
});

//builder.Services.AddControllers();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter()
        );
        options.JsonSerializerOptions.TypeInfoResolver =
            new DefaultJsonTypeInfoResolver();
    });


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Medical API",
        Version = "v1",
        Description = "Medicinski sustav – ORM demo"
    });

    //options.UseInlineDefinitionsForEnums();
    options.UseAllOfToExtendReferenceSchemas();

    options.SchemaGeneratorOptions.UseInlineDefinitionsForEnums = true;
});

builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
{
    options.JsonSerializerOptions.TypeInfoResolver =
        new DefaultJsonTypeInfoResolver();
});


var app = builder.Build();


if (app.Environment.IsDevelopment()) // check this
{
    Console.WriteLine("Running database migrations");
    var migrationRunner = new MigrationExecutor(connectionString);
    migrationRunner.RunMigrations();
    Console.WriteLine("Migrations completed!");
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();