var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddCors();

builder.Services.AddIdentityServices(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseCors(policy => policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200"));

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

try
{
    var dbContext = services.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();
    await Seed.SeedUsers(dbContext);
}
catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Error occurred during migration");
}

await app.RunAsync();
