using Microsoft.EntityFrameworkCore;
using SipintarBeltim.Models;

namespace SipintarBeltim.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Modul> Moduls => Set<Modul>();
    public DbSet<Menu> Menus => Set<Menu>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<AuditTrail> AuditTrails => Set<AuditTrail>();
    public DbSet<Pelanggan> Pelanggan => Set<Pelanggan>();

    // Master Data
    public DbSet<Aspek> Aspeks => Set<Aspek>();
    public DbSet<JenisKonflik> JenisKonfliks => Set<JenisKonflik>();
    public DbSet<Instansi> Instansis => Set<Instansi>();
    public DbSet<Wilayah> Wilayahs => Set<Wilayah>();

    // Matriks Risiko
    public DbSet<LevelKemungkinan> LevelKemungkinans => Set<LevelKemungkinan>();
    public DbSet<LevelDampak> LevelDampaks => Set<LevelDampak>();
    public DbSet<LevelRisiko> LevelRisikos => Set<LevelRisiko>();
    public DbSet<MatriksRisiko> MatriksRisikos => Set<MatriksRisiko>();

    // Kewaspadaan Dini
    public DbSet<KewaspadaanDini> KewaspadaanDinis => Set<KewaspadaanDini>();

    // Potensi Konflik
    public DbSet<PotensiKonflik> PotensiKonfliks => Set<PotensiKonflik>();

    // Peristiwa Konflik
    public DbSet<PeristiwaKonflik> PeristiwaKonfliks => Set<PeristiwaKonflik>();

    // WNA & TKA
    public DbSet<WargaNegaraAsing> WargaNegaraAsings => Set<WargaNegaraAsing>();
    public DbSet<TenagaKerjaAsing> TenagaKerjaAsings => Set<TenagaKerjaAsing>();

    // Forum Komunikasi Pimpinan Daerah
    public DbSet<ForumTopik> ForumTopiks => Set<ForumTopik>();
    public DbSet<ForumKomentar> ForumKomentars => Set<ForumKomentar>();
    public DbSet<ForumArahan> ForumArahans => Set<ForumArahan>();
    public DbSet<ForumTindakLanjut> ForumTindakLanjuts => Set<ForumTindakLanjut>();
    public DbSet<ForumPengumuman> ForumPengumumans => Set<ForumPengumuman>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasOne(e => e.Role)
                  .WithMany(r => r.Users)
                  .HasForeignKey(e => e.RoleId);
        });

        // Menu self-referencing
        modelBuilder.Entity<Menu>(entity =>
        {
            entity.HasOne(e => e.Parent)
                  .WithMany(e => e.Children)
                  .HasForeignKey(e => e.ParentId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Modul)
                  .WithMany(m => m.Menus)
                  .HasForeignKey(e => e.ModulId);
        });

        // RolePermission
        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasIndex(e => new { e.RoleId, e.MenuId }).IsUnique();

            entity.HasOne(e => e.Role)
                  .WithMany(r => r.RolePermissions)
                  .HasForeignKey(e => e.RoleId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Menu)
                  .WithMany(m => m.RolePermissions)
                  .HasForeignKey(e => e.MenuId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Wilayah self-referencing
        modelBuilder.Entity<Wilayah>(entity =>
        {
            entity.HasIndex(e => e.KodeBps).IsUnique();
            entity.HasOne(e => e.Parent)
                  .WithMany(e => e.Children)
                  .HasForeignKey(e => e.ParentId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // MatriksRisiko
        modelBuilder.Entity<MatriksRisiko>(entity =>
        {
            entity.HasIndex(e => new { e.KemungkinanId, e.DampakId }).IsUnique();

            entity.HasOne(e => e.Kemungkinan)
                  .WithMany()
                  .HasForeignKey(e => e.KemungkinanId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Dampak)
                  .WithMany()
                  .HasForeignKey(e => e.DampakId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.LevelRisiko)
                  .WithMany()
                  .HasForeignKey(e => e.LevelRisikoId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // LevelKemungkinan unique skor
        modelBuilder.Entity<LevelKemungkinan>(entity =>
        {
            entity.HasIndex(e => e.Skor).IsUnique();
        });

        // LevelDampak unique skor
        modelBuilder.Entity<LevelDampak>(entity =>
        {
            entity.HasIndex(e => e.Skor).IsUnique();
        });

        // Forum Komentar
        modelBuilder.Entity<ForumKomentar>(entity =>
        {
            entity.HasOne(e => e.ForumTopik)
                  .WithMany(t => t.Komentars)
                  .HasForeignKey(e => e.ForumTopikId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Forum Arahan
        modelBuilder.Entity<ForumArahan>(entity =>
        {
            entity.HasOne(e => e.ForumTopik)
                  .WithMany()
                  .HasForeignKey(e => e.ForumTopikId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // Forum Tindak Lanjut
        modelBuilder.Entity<ForumTindakLanjut>(entity =>
        {
            entity.HasOne(e => e.ForumArahan)
                  .WithMany(a => a.TindakLanjuts)
                  .HasForeignKey(e => e.ForumArahanId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Pelanggan seed (existing)
        modelBuilder.Entity<Pelanggan>().HasData(
            new Pelanggan { Id = 1, Nama = "Ahmad Sudirman", Alamat = "Jl. Merdeka No. 1", NomorSambungan = "001-001", Aktif = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Pelanggan { Id = 2, Nama = "Siti Rahayu", Alamat = "Jl. Pahlawan No. 5", NomorSambungan = "001-002", Aktif = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Pelanggan { Id = 3, Nama = "Budi Santoso", Alamat = "Jl. Kartini No. 12", NomorSambungan = "002-001", Aktif = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        );
    }
}
