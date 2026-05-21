using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SipintarBeltim.Data;
using SipintarBeltim.Endpoints;
using SipintarBeltim.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header. Contoh: \"Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// SQLite Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "SipintarBeltimSecretKey2024VeryLongKeyForSecurity!";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "SipintarBeltim",
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "SipintarBeltimApp",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });
builder.Services.AddAuthorization();

// CORS - allow frontend access
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Auto-migrate and seed database on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    await DbSeeder.SeedAsync(db);
    await MasterDataSeeder.SeedAsync(db);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

// ===== ENDPOINTS =====

// Health check
app.MapGet("/", () => new { status = "ok", service = "SiPintar Beltim API" })
   .WithName("HealthCheck")
   .WithOpenApi();

// Auth endpoints
app.MapAuthEndpoints();

// User endpoints
app.MapUserEndpoints();

// Role endpoints
app.MapRoleEndpoints();

// Audit Trail endpoints
app.MapAuditTrailEndpoints();

// Master Data endpoints
app.MapAspekEndpoints();
app.MapJenisKonflikEndpoints();
app.MapInstansiEndpoints();
app.MapWilayahEndpoints();

// Matriks Risiko endpoints
app.MapLevelKemungkinanEndpoints();
app.MapLevelDampakEndpoints();
app.MapLevelRisikoEndpoints();
app.MapMatriksRisikoEndpoints();

// Kewaspadaan Dini endpoints
app.MapKewaspadaanEndpoints();

// Potensi Konflik endpoints
app.MapPotensiKonflikEndpoints();

// Peristiwa Konflik endpoints
app.MapPeristiwaKonflikEndpoints();

// WNA & TKA endpoints
app.MapWnaEndpoints();
app.MapTkaEndpoints();

// ===== PELANGGAN (existing) =====
var pelangganGroup = app.MapGroup("/api/pelanggan").WithTags("Pelanggan");

pelangganGroup.MapGet("/", async (AppDbContext db) =>
{
    var data = await db.Pelanggan.ToListAsync();
    return Results.Ok(new { success = true, data });
})
.WithName("GetPelanggan")
.WithOpenApi();

pelangganGroup.MapGet("/{id}", async (int id, AppDbContext db) =>
{
    var pelanggan = await db.Pelanggan.FindAsync(id);
    if (pelanggan is null)
        return Results.NotFound(new { success = false, message = "Pelanggan tidak ditemukan" });

    return Results.Ok(new { success = true, data = pelanggan });
})
.WithName("GetPelangganById")
.WithOpenApi();

pelangganGroup.MapPost("/", async (Pelanggan pelanggan, AppDbContext db) =>
{
    pelanggan.CreatedAt = DateTime.UtcNow;
    db.Pelanggan.Add(pelanggan);
    await db.SaveChangesAsync();

    return Results.Created($"/api/pelanggan/{pelanggan.Id}", new { success = true, data = pelanggan });
})
.WithName("CreatePelanggan")
.WithOpenApi();

pelangganGroup.MapPut("/{id}", async (int id, Pelanggan input, AppDbContext db) =>
{
    var pelanggan = await db.Pelanggan.FindAsync(id);
    if (pelanggan is null)
        return Results.NotFound(new { success = false, message = "Pelanggan tidak ditemukan" });

    pelanggan.Nama = input.Nama;
    pelanggan.Alamat = input.Alamat;
    pelanggan.NomorSambungan = input.NomorSambungan;
    pelanggan.NoTelp = input.NoTelp;
    pelanggan.Aktif = input.Aktif;

    await db.SaveChangesAsync();
    return Results.Ok(new { success = true, data = pelanggan });
})
.WithName("UpdatePelanggan")
.WithOpenApi();

pelangganGroup.MapDelete("/{id}", async (int id, AppDbContext db) =>
{
    var pelanggan = await db.Pelanggan.FindAsync(id);
    if (pelanggan is null)
        return Results.NotFound(new { success = false, message = "Pelanggan tidak ditemukan" });

    db.Pelanggan.Remove(pelanggan);
    await db.SaveChangesAsync();
    return Results.Ok(new { success = true, message = "Pelanggan berhasil dihapus" });
})
.WithName("DeletePelanggan")
.WithOpenApi();

app.Run();
