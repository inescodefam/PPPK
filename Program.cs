using MedCore;
using MedCore.Interfaces;
using MedORM.Migrations;
using MedORM.ORM;
using MedORM.Repo;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddScoped<MedDbContext>(sp => // nova konekcija za svaki upit a ne singleton jedna konekcija za sve, puca kod paralelnih upita
    new MedDbContext(connectionString));


builder.Services.AddScoped<IRepositoryFactory, RepositoryFactory>();
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

    options.UseAllOfToExtendReferenceSchemas();

    options.SchemaGeneratorOptions.UseInlineDefinitionsForEnums = true;
});

builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
{
    options.JsonSerializerOptions.TypeInfoResolver =
        new DefaultJsonTypeInfoResolver();
});


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    var entityAssembly = Assembly.Load("MedCore");
    var migrationRunner = new MigrationExecutor(connectionString, entityAssembly);
    migrationRunner.RunMigrations();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();