using Microsoft.EntityFrameworkCore;
using SipintarBeltim.Models;

namespace SipintarBeltim.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.Roles.AnyAsync())
            return; // Already seeded

        // ===== ROLES =====
        var adminRole = new Role
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Name = "Administrator",
            Description = "Akses penuh ke seluruh modul dan fitur sistem",
            IsProtected = true,
            CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };
        var operatorRole = new Role
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            Name = "Operator",
            Description = "Input data kewaspadaan, potensi konflik, peristiwa, WNA/TKA",
            IsProtected = false,
            CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };
        var viewerRole = new Role
        {
            Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            Name = "Viewer",
            Description = "Hanya dapat melihat dashboard dan laporan",
            IsProtected = false,
            CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        db.Roles.AddRange(adminRole, operatorRole, viewerRole);

        // ===== MODULS & MENUS =====
        var moduls = CreateModulsAndMenus();
        db.Moduls.AddRange(moduls);

        await db.SaveChangesAsync();

        // ===== ROLE PERMISSIONS =====
        // Collect all menus (flat) using their unique Guid to avoid duplicates
        var allMenusFlat = new List<Menu>();
        var seenMenuIds = new HashSet<Guid>();
        foreach (var modul in moduls)
        {
            foreach (var menu in modul.Menus)
            {
                if (seenMenuIds.Add(menu.Id))
                    allMenusFlat.Add(menu);
                foreach (var child in menu.Children)
                {
                    if (seenMenuIds.Add(child.Id))
                        allMenusFlat.Add(child);
                }
            }
        }

        // Administrator: full access
        foreach (var menu in allMenusFlat)
        {
            db.RolePermissions.Add(new RolePermission
            {
                RoleId = adminRole.Id,
                MenuId = menu.Id,
                CanView = true,
                CanCreate = true,
                CanUpdate = true,
                CanDelete = true
            });
        }

        // Operator: limited access — use menu Id directly to avoid key collisions
        var operatorMenuGuids = new HashSet<Guid>
        {
            Guid.Parse("b0000001-0000-0000-0000-000000000001"), // Dashboard
            Guid.Parse("b0000004-0000-0000-0000-000000000001"), // Form Kewaspadaan Dini
            Guid.Parse("b0000004-0000-0000-0000-000000000002"), // EWS Dashboard
            Guid.Parse("b0000005-0000-0000-0000-000000000001"), // Form Potensi Konflik
            Guid.Parse("b0000005-0000-0000-0000-000000000002"), // EWS Potensi Konflik
            Guid.Parse("b0000006-0000-0000-0000-000000000001"), // Form Peristiwa Konflik
            Guid.Parse("b0000006-0000-0000-0000-000000000002"), // EWS Peristiwa Konflik
            Guid.Parse("b0000007-0000-0000-0000-000000000001"), // Warga Negara Asing (parent)
            Guid.Parse("b0000007-0000-0000-0000-000000000002"), // Tenaga Kerja Asing (parent)
            Guid.Parse("b0000007-0000-0000-0000-000000000011"), // Form WNA
            Guid.Parse("b0000007-0000-0000-0000-000000000012"), // EWS WNA
            Guid.Parse("b0000007-0000-0000-0000-000000000021"), // Form TKA
            Guid.Parse("b0000007-0000-0000-0000-000000000022"), // EWS TKA
            Guid.Parse("b0000008-0000-0000-0000-000000000001"), // Tindak Lanjut
            Guid.Parse("b0000009-0000-0000-0000-000000000002"), // Notifikasi
        };

        var operatorEwsGuids = new HashSet<Guid>
        {
            Guid.Parse("b0000001-0000-0000-0000-000000000001"), // Dashboard
            Guid.Parse("b0000004-0000-0000-0000-000000000002"), // EWS Dashboard
            Guid.Parse("b0000005-0000-0000-0000-000000000002"), // EWS Potensi Konflik
            Guid.Parse("b0000006-0000-0000-0000-000000000002"), // EWS Peristiwa Konflik
            Guid.Parse("b0000007-0000-0000-0000-000000000012"), // EWS WNA
            Guid.Parse("b0000007-0000-0000-0000-000000000022"), // EWS TKA
        };

        foreach (var menu in allMenusFlat)
        {
            if (operatorMenuGuids.Contains(menu.Id))
            {
                var isEws = operatorEwsGuids.Contains(menu.Id);
                db.RolePermissions.Add(new RolePermission
                {
                    RoleId = operatorRole.Id,
                    MenuId = menu.Id,
                    CanView = true,
                    CanCreate = !isEws,
                    CanUpdate = !isEws,
                    CanDelete = false
                });
            }
        }

        // Viewer: view only on dashboards and reports
        var viewerMenuGuids = new HashSet<Guid>
        {
            Guid.Parse("b0000001-0000-0000-0000-000000000001"), // Dashboard
            Guid.Parse("b0000004-0000-0000-0000-000000000002"), // EWS Dashboard
            Guid.Parse("b0000005-0000-0000-0000-000000000002"), // EWS Potensi Konflik
            Guid.Parse("b0000006-0000-0000-0000-000000000002"), // EWS Peristiwa Konflik
            Guid.Parse("b0000007-0000-0000-0000-000000000012"), // EWS WNA
            Guid.Parse("b0000007-0000-0000-0000-000000000022"), // EWS TKA
            Guid.Parse("b0000008-0000-0000-0000-000000000001"), // Tindak Lanjut
            Guid.Parse("b0000009-0000-0000-0000-000000000001"), // Laporan Periodik
            Guid.Parse("b0000009-0000-0000-0000-000000000002"), // Notifikasi
        };

        foreach (var menu in allMenusFlat)
        {
            if (viewerMenuGuids.Contains(menu.Id))
            {
                db.RolePermissions.Add(new RolePermission
                {
                    RoleId = viewerRole.Id,
                    MenuId = menu.Id,
                    CanView = true,
                    CanCreate = false,
                    CanUpdate = false,
                    CanDelete = false
                });
            }
        }

        await db.SaveChangesAsync();

        // ===== USERS =====
        var users = new List<User>
        {
            new User
            {
                Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                FullName = "Admin SIPINTAR",
                Email = "admin@sipintar.go.id",
                IsActive = true,
                RoleId = adminRole.Id,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new User
            {
                Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                Username = "operator",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("operator123"),
                FullName = "Budi Santoso",
                Email = "operator@sipintar.go.id",
                IsActive = true,
                RoleId = operatorRole.Id,
                CreatedAt = new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new User
            {
                Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                Username = "pimpinan",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("pimpinan123"),
                FullName = "Bupati Belitung Timur",
                Email = "pimpinan@sipintar.go.id",
                IsActive = true,
                RoleId = viewerRole.Id,
                CreatedAt = new DateTime(2024, 3, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new User
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                Username = "operator2",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("operator123"),
                FullName = "Siti Rahayu",
                Email = "siti@sipintar.go.id",
                IsActive = true,
                RoleId = operatorRole.Id,
                CreatedAt = new DateTime(2024, 4, 15, 0, 0, 0, DateTimeKind.Utc)
            },
            new User
            {
                Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                Username = "viewer2",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("viewer123"),
                FullName = "Camat Manggar",
                Email = "camat.manggar@sipintar.go.id",
                IsActive = false,
                RoleId = viewerRole.Id,
                CreatedAt = new DateTime(2024, 5, 10, 0, 0, 0, DateTimeKind.Utc)
            },
            new User
            {
                Id = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"),
                Username = "superadmin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("superadmin123"),
                FullName = "Super Admin SIPINTAR",
                Email = "superadmin@sipintar.go.id",
                IsActive = true,
                RoleId = adminRole.Id,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        };

        db.Users.AddRange(users);
        await db.SaveChangesAsync();
    }

    private static List<Modul> CreateModulsAndMenus()
    {
        var moduls = new List<Modul>();

        // 1. Administrasi
        var modAdmin = new Modul
        {
            Id = Guid.Parse("a0000001-0000-0000-0000-000000000001"),
            Name = "Administrasi",
            Description = "Manajemen pengguna, role, dan audit trail",
            Idx = 1
        };
        modAdmin.Menus = new List<Menu>
        {
            new Menu { Id = Guid.Parse("b0000001-0000-0000-0000-000000000001"), Name = "Dashboard", Description = "Halaman utama dashboard", Idx = 1, ModulId = modAdmin.Id },
            new Menu { Id = Guid.Parse("b0000001-0000-0000-0000-000000000002"), Name = "Role Management", Description = "Kelola role dan permission", Idx = 2, ModulId = modAdmin.Id },
            new Menu { Id = Guid.Parse("b0000001-0000-0000-0000-000000000003"), Name = "User Management", Description = "Kelola pengguna sistem", Idx = 3, ModulId = modAdmin.Id },
            new Menu { Id = Guid.Parse("b0000001-0000-0000-0000-000000000004"), Name = "Audit Trail", Description = "Riwayat aktivitas sistem", Idx = 4, ModulId = modAdmin.Id },
        };
        moduls.Add(modAdmin);

        // 2. Master Data
        var modMaster = new Modul
        {
            Id = Guid.Parse("a0000001-0000-0000-0000-000000000002"),
            Name = "Master Data",
            Description = "Data referensi utama aplikasi",
            Idx = 2
        };
        modMaster.Menus = new List<Menu>
        {
            new Menu { Id = Guid.Parse("b0000002-0000-0000-0000-000000000001"), Name = "Aspek", Description = "Data aspek konflik", Idx = 1, ModulId = modMaster.Id },
            new Menu { Id = Guid.Parse("b0000002-0000-0000-0000-000000000002"), Name = "Jenis Konflik", Description = "Klasifikasi jenis konflik", Idx = 2, ModulId = modMaster.Id },
            new Menu { Id = Guid.Parse("b0000002-0000-0000-0000-000000000003"), Name = "Instansi", Description = "Data instansi terkait", Idx = 3, ModulId = modMaster.Id },
            new Menu { Id = Guid.Parse("b0000002-0000-0000-0000-000000000004"), Name = "Wilayah", Description = "Data wilayah administratif", Idx = 4, ModulId = modMaster.Id },
        };
        moduls.Add(modMaster);

        // 3. Matriks Risiko
        var modRisiko = new Modul
        {
            Id = Guid.Parse("a0000001-0000-0000-0000-000000000003"),
            Name = "Matriks Risiko",
            Description = "Konfigurasi level dan matriks risiko",
            Idx = 3
        };
        modRisiko.Menus = new List<Menu>
        {
            new Menu { Id = Guid.Parse("b0000003-0000-0000-0000-000000000001"), Name = "Level Kemungkinan", Description = "Level kemungkinan risiko", Idx = 1, ModulId = modRisiko.Id },
            new Menu { Id = Guid.Parse("b0000003-0000-0000-0000-000000000002"), Name = "Level Dampak", Description = "Level dampak risiko", Idx = 2, ModulId = modRisiko.Id },
            new Menu { Id = Guid.Parse("b0000003-0000-0000-0000-000000000003"), Name = "Level Risiko", Description = "Definisi level risiko", Idx = 3, ModulId = modRisiko.Id },
            new Menu { Id = Guid.Parse("b0000003-0000-0000-0000-000000000004"), Name = "Matriks Risiko", Description = "Konfigurasi matriks risiko", Idx = 4, ModulId = modRisiko.Id },
        };
        moduls.Add(modRisiko);

        // 4. Kewaspadaan Dini
        var modKewaspadaan = new Modul
        {
            Id = Guid.Parse("a0000001-0000-0000-0000-000000000004"),
            Name = "Kewaspadaan Dini",
            Description = "Form dan monitoring kewaspadaan dini",
            Idx = 4
        };
        modKewaspadaan.Menus = new List<Menu>
        {
            new Menu { Id = Guid.Parse("b0000004-0000-0000-0000-000000000001"), Name = "Form Kewaspadaan Dini", Description = "Input data kewaspadaan dini", Idx = 1, ModulId = modKewaspadaan.Id },
            new Menu { Id = Guid.Parse("b0000004-0000-0000-0000-000000000002"), Name = "EWS Dashboard", Description = "Dashboard early warning system", Idx = 2, ModulId = modKewaspadaan.Id },
        };
        moduls.Add(modKewaspadaan);

        // 5. Potensi Konflik
        var modPotensi = new Modul
        {
            Id = Guid.Parse("a0000001-0000-0000-0000-000000000005"),
            Name = "Potensi Konflik",
            Description = "Pencatatan dan monitoring potensi konflik",
            Idx = 5
        };
        modPotensi.Menus = new List<Menu>
        {
            new Menu { Id = Guid.Parse("b0000005-0000-0000-0000-000000000001"), Name = "Form Potensi Konflik", Description = "Input data potensi konflik", Idx = 1, ModulId = modPotensi.Id },
            new Menu { Id = Guid.Parse("b0000005-0000-0000-0000-000000000002"), Name = "EWS Potensi Konflik", Description = "Dashboard EWS potensi konflik", Idx = 2, ModulId = modPotensi.Id },
        };
        moduls.Add(modPotensi);

        // 6. Peristiwa Konflik
        var modPeristiwa = new Modul
        {
            Id = Guid.Parse("a0000001-0000-0000-0000-000000000006"),
            Name = "Peristiwa Konflik",
            Description = "Pencatatan dan monitoring peristiwa konflik",
            Idx = 6
        };
        modPeristiwa.Menus = new List<Menu>
        {
            new Menu { Id = Guid.Parse("b0000006-0000-0000-0000-000000000001"), Name = "Form Peristiwa Konflik", Description = "Input data peristiwa konflik", Idx = 1, ModulId = modPeristiwa.Id },
            new Menu { Id = Guid.Parse("b0000006-0000-0000-0000-000000000002"), Name = "EWS Peristiwa Konflik", Description = "Dashboard EWS peristiwa konflik", Idx = 2, ModulId = modPeristiwa.Id },
        };
        moduls.Add(modPeristiwa);

        // 7. WNA & TKA
        var modWnaTka = new Modul
        {
            Id = Guid.Parse("a0000001-0000-0000-0000-000000000007"),
            Name = "WNA & TKA",
            Description = "Monitoring warga negara asing dan tenaga kerja asing",
            Idx = 7
        };
        var menuWna = new Menu { Id = Guid.Parse("b0000007-0000-0000-0000-000000000001"), Name = "Warga Negara Asing", Description = "Data WNA", Idx = 1, ModulId = modWnaTka.Id };
        var menuTka = new Menu { Id = Guid.Parse("b0000007-0000-0000-0000-000000000002"), Name = "Tenaga Kerja Asing", Description = "Data TKA", Idx = 2, ModulId = modWnaTka.Id };

        menuWna.Children = new List<Menu>
        {
            new Menu { Id = Guid.Parse("b0000007-0000-0000-0000-000000000011"), Name = "Form WNA", Description = "Input data WNA", Idx = 1, ModulId = modWnaTka.Id, ParentId = menuWna.Id },
            new Menu { Id = Guid.Parse("b0000007-0000-0000-0000-000000000012"), Name = "EWS WNA", Description = "Dashboard EWS WNA", Idx = 2, ModulId = modWnaTka.Id, ParentId = menuWna.Id },
        };
        menuTka.Children = new List<Menu>
        {
            new Menu { Id = Guid.Parse("b0000007-0000-0000-0000-000000000021"), Name = "Form TKA", Description = "Input data TKA", Idx = 1, ModulId = modWnaTka.Id, ParentId = menuTka.Id },
            new Menu { Id = Guid.Parse("b0000007-0000-0000-0000-000000000022"), Name = "EWS TKA", Description = "Dashboard EWS TKA", Idx = 2, ModulId = modWnaTka.Id, ParentId = menuTka.Id },
        };

        modWnaTka.Menus = new List<Menu> { menuWna, menuTka };
        moduls.Add(modWnaTka);

        // 8. Tindak Lanjut
        var modTindakLanjut = new Modul
        {
            Id = Guid.Parse("a0000001-0000-0000-0000-000000000008"),
            Name = "Tindak Lanjut",
            Description = "Tindak lanjut dan keputusan",
            Idx = 8
        };
        modTindakLanjut.Menus = new List<Menu>
        {
            new Menu { Id = Guid.Parse("b0000008-0000-0000-0000-000000000001"), Name = "Tindak Lanjut & Keputusan", Description = "Kelola tindak lanjut", Idx = 1, ModulId = modTindakLanjut.Id },
        };
        moduls.Add(modTindakLanjut);

        // 9. Laporan & Pengaturan
        var modLaporan = new Modul
        {
            Id = Guid.Parse("a0000001-0000-0000-0000-000000000009"),
            Name = "Laporan & Pengaturan",
            Description = "Laporan periodik dan pengaturan sistem",
            Idx = 9
        };
        modLaporan.Menus = new List<Menu>
        {
            new Menu { Id = Guid.Parse("b0000009-0000-0000-0000-000000000001"), Name = "Laporan Periodik", Description = "Generate laporan periodik", Idx = 1, ModulId = modLaporan.Id },
            new Menu { Id = Guid.Parse("b0000009-0000-0000-0000-000000000002"), Name = "Notifikasi", Description = "Pengaturan notifikasi", Idx = 2, ModulId = modLaporan.Id },
            new Menu { Id = Guid.Parse("b0000009-0000-0000-0000-000000000003"), Name = "General Setting", Description = "Pengaturan umum aplikasi", Idx = 3, ModulId = modLaporan.Id },
            new Menu { Id = Guid.Parse("b0000009-0000-0000-0000-000000000004"), Name = "Pengaturan Tampilan", Description = "Kustomisasi tampilan", Idx = 4, ModulId = modLaporan.Id },
        };
        moduls.Add(modLaporan);

        return moduls;
    }
}
