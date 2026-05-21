using Microsoft.EntityFrameworkCore;
using SipintarBeltim.Models;

namespace SipintarBeltim.Data;

public static class MasterDataSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.Aspeks.AnyAsync())
            return; // Already seeded

        await SeedAspek(db);
        await SeedJenisKonflik(db);
        await SeedInstansi(db);
        await SeedWilayah(db);
        await SeedLevelKemungkinan(db);
        await SeedLevelDampak(db);
        await SeedLevelRisiko(db);
        await SeedMatriksRisiko(db);
    }

    private static async Task SeedAspek(AppDbContext db)
    {
        var data = new List<Aspek>
        {
            new() { Id = Guid.Parse("c0000001-0000-0000-0000-000000000001"), Nama = "Ideologi", Deskripsi = "Aspek yang berkaitan dengan ideologi dan dasar negara" },
            new() { Id = Guid.Parse("c0000001-0000-0000-0000-000000000002"), Nama = "Politik", Deskripsi = "Aspek yang berkaitan dengan sistem dan kehidupan politik" },
            new() { Id = Guid.Parse("c0000001-0000-0000-0000-000000000003"), Nama = "Ekonomi", Deskripsi = "Aspek yang berkaitan dengan perekonomian nasional" },
            new() { Id = Guid.Parse("c0000001-0000-0000-0000-000000000004"), Nama = "Sosial Budaya", Deskripsi = "Aspek yang berkaitan dengan kehidupan sosial dan budaya masyarakat" },
            new() { Id = Guid.Parse("c0000001-0000-0000-0000-000000000005"), Nama = "Hankam", Deskripsi = "Aspek yang berkaitan dengan pertahanan dan keamanan negara" },
        };
        db.Aspeks.AddRange(data);
        await db.SaveChangesAsync();
    }

    private static async Task SeedJenisKonflik(AppDbContext db)
    {
        var data = new List<JenisKonflik>
        {
            new() { Id = Guid.Parse("c0000002-0000-0000-0000-000000000001"), Nama = "Konflik Lahan", Deskripsi = "Perselisihan yang terjadi akibat perebutan, sengketa, atau klaim kepemilikan atas lahan dan tanah antara individu, kelompok, atau institusi." },
            new() { Id = Guid.Parse("c0000002-0000-0000-0000-000000000002"), Nama = "Konflik Agama", Deskripsi = "Konflik yang dipicu oleh perbedaan keyakinan, praktik keagamaan, atau intoleransi antar umat beragama dalam suatu wilayah." },
            new() { Id = Guid.Parse("c0000002-0000-0000-0000-000000000003"), Nama = "Konflik Tenaga Kerja", Deskripsi = "Perselisihan antara pekerja dan pemberi kerja atau antar kelompok pekerja terkait upah, kondisi kerja, hak-hak buruh, atau pemutusan hubungan kerja." },
            new() { Id = Guid.Parse("c0000002-0000-0000-0000-000000000004"), Nama = "Konflik Politik Lokal", Deskripsi = "Konflik yang muncul dari persaingan kekuasaan, perbedaan kepentingan politik, atau ketidakpuasan terhadap kebijakan pemerintah di tingkat daerah." },
        };
        db.JenisKonfliks.AddRange(data);
        await db.SaveChangesAsync();
    }

    private static async Task SeedInstansi(AppDbContext db)
    {
        var data = new List<Instansi>
        {
            new() { Id = Guid.Parse("c0000003-0000-0000-0000-000000000001"), Nama = "Kementerian Dalam Negeri", Deskripsi = "Kementerian yang menangani urusan pemerintahan dalam negeri." },
            new() { Id = Guid.Parse("c0000003-0000-0000-0000-000000000002"), Nama = "Kementerian Luar Negeri", Deskripsi = "Kementerian yang menangani hubungan diplomatik dan luar negeri." },
            new() { Id = Guid.Parse("c0000003-0000-0000-0000-000000000003"), Nama = "Badan Intelijen Negara", Deskripsi = "Lembaga pemerintah yang bertugas di bidang intelijen negara." },
            new() { Id = Guid.Parse("c0000003-0000-0000-0000-000000000004"), Nama = "Kepolisian Negara RI", Deskripsi = "Lembaga negara yang berperan dalam memelihara keamanan dan ketertiban masyarakat." },
            new() { Id = Guid.Parse("c0000003-0000-0000-0000-000000000005"), Nama = "Tentara Nasional Indonesia", Deskripsi = "Angkatan bersenjata Republik Indonesia yang bertugas menjaga pertahanan negara." },
        };
        db.Instansis.AddRange(data);
        await db.SaveChangesAsync();
    }

    private static async Task SeedWilayah(AppDbContext db)
    {
        // Provinsi
        var provinsi = new Wilayah { Id = Guid.Parse("d0000019-0000-0000-0000-000000000000"), Tipe = "Provinsi", Nama = "Kepulauan Bangka Belitung", KodeBps = "19", Latitude = -2.7411, Longitude = 106.4406 };
        db.Wilayahs.Add(provinsi);
        await db.SaveChangesAsync();

        // Kabupaten
        var kabBelitung = new Wilayah { Id = Guid.Parse("d0001902-0000-0000-0000-000000000000"), Tipe = "Kabupaten", Nama = "Kabupaten Belitung", KodeBps = "1902", ParentId = provinsi.Id, Latitude = -2.8670, Longitude = 107.7000 };
        var kabBeltim = new Wilayah { Id = Guid.Parse("d0001906-0000-0000-0000-000000000000"), Tipe = "Kabupaten", Nama = "Kabupaten Belitung Timur", KodeBps = "1906", ParentId = provinsi.Id, Latitude = -2.9000, Longitude = 108.0500 };
        db.Wilayahs.AddRange(kabBelitung, kabBeltim);
        await db.SaveChangesAsync();

        // Kecamatan - Kabupaten Belitung
        var kecTanjungpandan = new Wilayah { Id = Guid.Parse("d0190201-0000-0000-0000-000000000000"), Tipe = "Kecamatan", Nama = "Tanjungpandan", KodeBps = "190201", ParentId = kabBelitung.Id, Latitude = -2.7456, Longitude = 107.6384 };
        var kecMembalong = new Wilayah { Id = Guid.Parse("d0190202-0000-0000-0000-000000000000"), Tipe = "Kecamatan", Nama = "Membalong", KodeBps = "190202", ParentId = kabBelitung.Id, Latitude = -3.0800, Longitude = 107.7200 };
        var kecSelatNasik = new Wilayah { Id = Guid.Parse("d0190203-0000-0000-0000-000000000000"), Tipe = "Kecamatan", Nama = "Selat Nasik", KodeBps = "190203", ParentId = kabBelitung.Id, Latitude = -2.8500, Longitude = 107.3500 };
        var kecSijuk = new Wilayah { Id = Guid.Parse("d0190204-0000-0000-0000-000000000000"), Tipe = "Kecamatan", Nama = "Sijuk", KodeBps = "190204", ParentId = kabBelitung.Id, Latitude = -2.6200, Longitude = 107.5800 };
        var kecBadau = new Wilayah { Id = Guid.Parse("d0190205-0000-0000-0000-000000000000"), Tipe = "Kecamatan", Nama = "Badau", KodeBps = "190205", ParentId = kabBelitung.Id, Latitude = -2.9500, Longitude = 107.8500 };
        db.Wilayahs.AddRange(kecTanjungpandan, kecMembalong, kecSelatNasik, kecSijuk, kecBadau);
        await db.SaveChangesAsync();

        // Kecamatan - Kabupaten Belitung Timur
        var kecManggar = new Wilayah { Id = Guid.Parse("d0190601-0000-0000-0000-000000000000"), Tipe = "Kecamatan", Nama = "Manggar", KodeBps = "190601", ParentId = kabBeltim.Id, Latitude = -2.8800, Longitude = 108.2700 };
        var kecGantung = new Wilayah { Id = Guid.Parse("d0190602-0000-0000-0000-000000000000"), Tipe = "Kecamatan", Nama = "Gantung", KodeBps = "190602", ParentId = kabBeltim.Id, Latitude = -2.9600, Longitude = 108.0800 };
        var kecDendang = new Wilayah { Id = Guid.Parse("d0190603-0000-0000-0000-000000000000"), Tipe = "Kecamatan", Nama = "Dendang", KodeBps = "190603", ParentId = kabBeltim.Id, Latitude = -2.7800, Longitude = 108.1500 };
        var kecKelapaKampit = new Wilayah { Id = Guid.Parse("d0190604-0000-0000-0000-000000000000"), Tipe = "Kecamatan", Nama = "Kelapa Kampit", KodeBps = "190604", ParentId = kabBeltim.Id, Latitude = -2.8200, Longitude = 107.9500 };
        var kecDamar = new Wilayah { Id = Guid.Parse("d0190605-0000-0000-0000-000000000000"), Tipe = "Kecamatan", Nama = "Damar", KodeBps = "190605", ParentId = kabBeltim.Id, Latitude = -3.0500, Longitude = 108.1000 };
        var kecSimpangRenggiang = new Wilayah { Id = Guid.Parse("d0190606-0000-0000-0000-000000000000"), Tipe = "Kecamatan", Nama = "Simpang Renggiang", KodeBps = "190606", ParentId = kabBeltim.Id, Latitude = -2.9200, Longitude = 107.9800 };
        var kecSimpangPesak = new Wilayah { Id = Guid.Parse("d0190607-0000-0000-0000-000000000000"), Tipe = "Kecamatan", Nama = "Simpang Pesak", KodeBps = "190607", ParentId = kabBeltim.Id, Latitude = -3.1000, Longitude = 107.8800 };
        db.Wilayahs.AddRange(kecManggar, kecGantung, kecDendang, kecKelapaKampit, kecDamar, kecSimpangRenggiang, kecSimpangPesak);
        await db.SaveChangesAsync();

        // Kelurahan - Kecamatan Tanjungpandan
        var kelurahan = new List<Wilayah>
        {
            new() { Id = Guid.Parse("d1902010-0010-0000-0000-000000000000"), Tipe = "Kelurahan", Nama = "Tanjungpandan", KodeBps = "1902010001", ParentId = kecTanjungpandan.Id, Latitude = -2.7456, Longitude = 107.6384 },
            new() { Id = Guid.Parse("d1902010-0020-0000-0000-000000000000"), Tipe = "Kelurahan", Nama = "Kampung Damai", KodeBps = "1902010002", ParentId = kecTanjungpandan.Id, Latitude = -2.7500, Longitude = 107.6400 },
            new() { Id = Guid.Parse("d1902010-0030-0000-0000-000000000000"), Tipe = "Kelurahan", Nama = "Lesung Batang", KodeBps = "1902010003", ParentId = kecTanjungpandan.Id, Latitude = -2.7600, Longitude = 107.6300 },
            new() { Id = Guid.Parse("d1902010-0040-0000-0000-000000000000"), Tipe = "Kelurahan", Nama = "Pangkal Lalang", KodeBps = "1902010004", ParentId = kecTanjungpandan.Id, Latitude = -2.7350, Longitude = 107.6450 },
            new() { Id = Guid.Parse("d1902010-0050-0000-0000-000000000000"), Tipe = "Kelurahan", Nama = "Parit", KodeBps = "1902010005", ParentId = kecTanjungpandan.Id, Latitude = -2.7550, Longitude = 107.6350 },
            new() { Id = Guid.Parse("d1902010-0060-0000-0000-000000000000"), Tipe = "Kelurahan", Nama = "Paal Satu", KodeBps = "1902010006", ParentId = kecTanjungpandan.Id, Latitude = -2.7480, Longitude = 107.6420 },
            new() { Id = Guid.Parse("d1902010-0070-0000-0000-000000000000"), Tipe = "Kelurahan", Nama = "Air Saga", KodeBps = "1902010007", ParentId = kecTanjungpandan.Id, Latitude = -2.7650, Longitude = 107.6500 },
        };
        db.Wilayahs.AddRange(kelurahan);

        // Desa - Kecamatan Tanjungpandan
        var desaTanjungpandan = new List<Wilayah>
        {
            new() { Id = Guid.Parse("d1902010-0080-0000-0000-000000000000"), Tipe = "Desa", Nama = "Perawas", KodeBps = "1902010008", ParentId = kecTanjungpandan.Id, Latitude = -2.7700, Longitude = 107.6600 },
            new() { Id = Guid.Parse("d1902010-0090-0000-0000-000000000000"), Tipe = "Desa", Nama = "Aik Rayak", KodeBps = "1902010009", ParentId = kecTanjungpandan.Id, Latitude = -2.7800, Longitude = 107.6700 },
            new() { Id = Guid.Parse("d1902010-0100-0000-0000-000000000000"), Tipe = "Desa", Nama = "Terong", KodeBps = "1902010010", ParentId = kecTanjungpandan.Id, Latitude = -2.7900, Longitude = 107.6800 },
        };
        db.Wilayahs.AddRange(desaTanjungpandan);

        // Desa - Kecamatan Manggar
        var desaManggar = new List<Wilayah>
        {
            new() { Id = Guid.Parse("d1906010-0010-0000-0000-000000000000"), Tipe = "Desa", Nama = "Padang", KodeBps = "1906010001", ParentId = kecManggar.Id, Latitude = -2.8900, Longitude = 108.2600 },
            new() { Id = Guid.Parse("d1906010-0020-0000-0000-000000000000"), Tipe = "Desa", Nama = "Lalang", KodeBps = "1906010002", ParentId = kecManggar.Id, Latitude = -2.8700, Longitude = 108.2800 },
            new() { Id = Guid.Parse("d1906010-0030-0000-0000-000000000000"), Tipe = "Desa", Nama = "Baru", KodeBps = "1906010003", ParentId = kecManggar.Id, Latitude = -2.8600, Longitude = 108.2900 },
            new() { Id = Guid.Parse("d1906010-0040-0000-0000-000000000000"), Tipe = "Desa", Nama = "Kurnia Jaya", KodeBps = "1906010004", ParentId = kecManggar.Id, Latitude = -2.8500, Longitude = 108.3000 },
            new() { Id = Guid.Parse("d1906010-0050-0000-0000-000000000000"), Tipe = "Desa", Nama = "Mekar Jaya", KodeBps = "1906010005", ParentId = kecManggar.Id, Latitude = -2.9000, Longitude = 108.2500 },
        };
        db.Wilayahs.AddRange(desaManggar);
        await db.SaveChangesAsync();
    }

    private static async Task SeedLevelKemungkinan(AppDbContext db)
    {
        var data = new List<LevelKemungkinan>
        {
            new() { Id = Guid.Parse("e0000001-0000-0000-0000-000000000001"), Nama = "Sangat Jarang", Skor = 1, Deskripsi = "Kejadian hampir tidak pernah terjadi, kemungkinan < 10%" },
            new() { Id = Guid.Parse("e0000001-0000-0000-0000-000000000002"), Nama = "Jarang", Skor = 2, Deskripsi = "Kejadian kadang terjadi, kemungkinan 10%–30%" },
            new() { Id = Guid.Parse("e0000001-0000-0000-0000-000000000003"), Nama = "Sedang", Skor = 3, Deskripsi = "Kejadian cukup sering terjadi, kemungkinan 30%–60%" },
            new() { Id = Guid.Parse("e0000001-0000-0000-0000-000000000004"), Nama = "Sering", Skor = 4, Deskripsi = "Kejadian sering terjadi, kemungkinan 60%–90%" },
            new() { Id = Guid.Parse("e0000001-0000-0000-0000-000000000005"), Nama = "Sangat Sering", Skor = 5, Deskripsi = "Kejadian hampir pasti terjadi, kemungkinan > 90%" },
        };
        db.LevelKemungkinans.AddRange(data);
        await db.SaveChangesAsync();
    }

    private static async Task SeedLevelDampak(AppDbContext db)
    {
        var data = new List<LevelDampak>
        {
            new() { Id = Guid.Parse("e0000002-0000-0000-0000-000000000001"), Nama = "Tidak Signifikan", Skor = 1, Deskripsi = "Dampak sangat kecil, tidak mempengaruhi operasional" },
            new() { Id = Guid.Parse("e0000002-0000-0000-0000-000000000002"), Nama = "Minor", Skor = 2, Deskripsi = "Dampak kecil, gangguan operasional ringan dan mudah dipulihkan" },
            new() { Id = Guid.Parse("e0000002-0000-0000-0000-000000000003"), Nama = "Moderat", Skor = 3, Deskripsi = "Dampak sedang, gangguan operasional yang memerlukan perhatian" },
            new() { Id = Guid.Parse("e0000002-0000-0000-0000-000000000004"), Nama = "Mayor", Skor = 4, Deskripsi = "Dampak besar, kerugian signifikan dan sulit dipulihkan" },
            new() { Id = Guid.Parse("e0000002-0000-0000-0000-000000000005"), Nama = "Katastrofik", Skor = 5, Deskripsi = "Dampak sangat besar, mengancam keberlangsungan organisasi" },
        };
        db.LevelDampaks.AddRange(data);
        await db.SaveChangesAsync();
    }

    private static async Task SeedLevelRisiko(AppDbContext db)
    {
        var data = new List<LevelRisiko>
        {
            new() { Id = Guid.Parse("e0000003-0000-0000-0000-000000000001"), Nama = "Sangat Rendah", Warna = "#22c55e", SkorMin = 1, SkorMax = 4 },
            new() { Id = Guid.Parse("e0000003-0000-0000-0000-000000000002"), Nama = "Rendah", Warna = "#84cc16", SkorMin = 5, SkorMax = 8 },
            new() { Id = Guid.Parse("e0000003-0000-0000-0000-000000000003"), Nama = "Sedang", Warna = "#eab308", SkorMin = 9, SkorMax = 12 },
            new() { Id = Guid.Parse("e0000003-0000-0000-0000-000000000004"), Nama = "Tinggi", Warna = "#f97316", SkorMin = 13, SkorMax = 19 },
            new() { Id = Guid.Parse("e0000003-0000-0000-0000-000000000005"), Nama = "Sangat Tinggi", Warna = "#ef4444", SkorMin = 20, SkorMax = 25 },
        };
        db.LevelRisikos.AddRange(data);
        await db.SaveChangesAsync();
    }

    private static async Task SeedMatriksRisiko(AppDbContext db)
    {
        // Get all level kemungkinan and dampak
        var kemungkinanList = await db.LevelKemungkinans.OrderBy(l => l.Skor).ToListAsync();
        var dampakList = await db.LevelDampaks.OrderBy(l => l.Skor).ToListAsync();
        var risikoList = await db.LevelRisikos.OrderBy(l => l.SkorMin).ToListAsync();

        // Generate matriks: skor = kemungkinan.Skor * dampak.Skor → map to level risiko
        var matriksList = new List<MatriksRisiko>();
        foreach (var k in kemungkinanList)
        {
            foreach (var d in dampakList)
            {
                var skor = k.Skor * d.Skor;
                var levelRisiko = GetLevelRisikoByScore(risikoList, skor);
                if (levelRisiko != null)
                {
                    matriksList.Add(new MatriksRisiko
                    {
                        KemungkinanId = k.Id,
                        DampakId = d.Id,
                        LevelRisikoId = levelRisiko.Id
                    });
                }
            }
        }

        db.MatriksRisikos.AddRange(matriksList);
        await db.SaveChangesAsync();
    }

    private static LevelRisiko? GetLevelRisikoByScore(List<LevelRisiko> levels, int skor)
    {
        return levels.FirstOrDefault(l => skor >= l.SkorMin && skor <= l.SkorMax);
    }
}
