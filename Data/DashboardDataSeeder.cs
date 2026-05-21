using Microsoft.EntityFrameworkCore;
using SipintarBeltim.Models;

namespace SipintarBeltim.Data;

/// <summary>
/// Seeds sample data for Kewaspadaan Dini, Potensi Konflik, and Peristiwa Konflik
/// so the dashboard has meaningful data to display.
/// </summary>
public static class DashboardDataSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        // Only seed if no kewaspadaan data exists
        if (await db.KewaspadaanDinis.AnyAsync())
            return;

        await SeedKewaspadaanDini(db);
        await SeedPotensiKonflik(db);
        await SeedPeristiwaKonflik(db);
    }

    private static async Task SeedKewaspadaanDini(AppDbContext db)
    {
        var kecamatanDesa = new (string kec, string desa)[]
        {
            ("Manggar", "Padang"),
            ("Manggar", "Kurnia Jaya"),
            ("Gantung", "Selingsing"),
            ("Dendang", "Dendang"),
            ("Kelapa Kampit", "Mayang"),
            ("Kelapa Kampit", "Air Kelik"),
            ("Simpang Renggiang", "Renggiang"),
            ("Simpang Pesak", "Tanjung Batu Itam"),
            ("Damar", "Mengkubang"),
            ("Manggar", "Lalang"),
        };

        var aspekList = new[] { "Ideologi", "Politik", "Ekonomi", "Sosial Budaya", "Hankam" };
        var risikoList = new[] { "Rendah", "Sedang", "Tinggi", "Sangat Tinggi" };
        var kemungkinanLevels = new[] { "Sangat Jarang", "Jarang", "Sedang", "Sering", "Sangat Sering" };
        var dampakLevels = new[] { "Tidak Signifikan", "Minor", "Moderat", "Mayor", "Katastrofik" };

        var items = new List<KewaspadaanDini>();
        var random = new Random(42); // Fixed seed for reproducibility

        for (int i = 0; i < kecamatanDesa.Length; i++)
        {
            var (kec, desa) = kecamatanDesa[i];
            var aspek = aspekList[i % aspekList.Length];
            var risiko = risikoList[i % risikoList.Length];
            var kemungkinan = kemungkinanLevels[random.Next(kemungkinanLevels.Length)];
            var dampak = dampakLevels[random.Next(dampakLevels.Length)];

            items.Add(new KewaspadaanDini
            {
                Periode = DateTime.UtcNow.AddDays(-random.Next(1, 90)),
                Aspek = aspek,
                Kabupaten = "Belitung Timur",
                Kecamatan = kec,
                Desa = desa,
                AlamatDetail = $"Jl. Utama Desa {desa}",
                TitikKoordinat = GetCoordinate(kec, random),
                SumberInformasi = "Laporan masyarakat",
                KemungkinanAncamanLevel = kemungkinan,
                KemungkinanAncamanDeskripsi = $"Potensi ancaman {aspek.ToLower()} di wilayah {desa}, {kec}",
                Hambatan = "Keterbatasan akses informasi",
                Tantangan = "Koordinasi antar instansi",
                Gangguan = "Provokasi pihak tertentu",
                PrediksiDampakLevel = dampak,
                PrediksiDampakDeskripsi = $"Dampak terhadap stabilitas {aspek.ToLower()} di kecamatan {kec}",
                Rekomendasi = "Perlu koordinasi dengan aparat keamanan dan tokoh masyarakat setempat",
                TingkatRisiko = risiko,
                Status = "disetujui",
                ApprovedBy = "Admin SIPINTAR",
                ApprovedAt = DateTime.UtcNow.AddDays(-random.Next(1, 30)),
                CreatedBy = "Admin SIPINTAR",
                CreatedAt = DateTime.UtcNow.AddDays(-random.Next(30, 120)),
            });
        }

        // Add a few with "menunggu" status
        items.Add(new KewaspadaanDini
        {
            Periode = DateTime.UtcNow.AddDays(-5),
            Aspek = "Ekonomi",
            Kabupaten = "Belitung Timur",
            Kecamatan = "Manggar",
            Desa = "Mekar Jaya",
            SumberInformasi = "Laporan petugas",
            KemungkinanAncamanLevel = "Sedang",
            KemungkinanAncamanDeskripsi = "Potensi konflik ekonomi terkait distribusi bantuan sosial",
            PrediksiDampakLevel = "Moderat",
            PrediksiDampakDeskripsi = "Ketidakpuasan masyarakat terhadap distribusi bantuan",
            Rekomendasi = "Sosialisasi mekanisme distribusi bantuan",
            TingkatRisiko = "Sedang",
            Status = "menunggu",
            CreatedBy = "Operator EWS",
            CreatedAt = DateTime.UtcNow.AddDays(-5),
        });

        items.Add(new KewaspadaanDini
        {
            Periode = DateTime.UtcNow.AddDays(-2),
            Aspek = "Hankam",
            Kabupaten = "Belitung Timur",
            Kecamatan = "Kelapa Kampit",
            Desa = "Air Kelik",
            SumberInformasi = "Intelijen daerah",
            KemungkinanAncamanLevel = "Sering",
            KemungkinanAncamanDeskripsi = "Aktivitas tambang ilegal meningkat",
            PrediksiDampakLevel = "Mayor",
            PrediksiDampakDeskripsi = "Kerusakan lingkungan dan konflik warga",
            Rekomendasi = "Penertiban dan penegakan hukum",
            TingkatRisiko = "Tinggi",
            Status = "menunggu",
            CreatedBy = "Operator EWS",
            CreatedAt = DateTime.UtcNow.AddDays(-2),
        });

        db.KewaspadaanDinis.AddRange(items);
        await db.SaveChangesAsync();
    }

    private static async Task SeedPotensiKonflik(AppDbContext db)
    {
        var random = new Random(123);
        var aspekList = new[] { "Ideologi", "Politik", "Ekonomi", "Sosial Budaya", "Hankam" };
        var risikoList = new[] { "Rendah", "Sedang", "Tinggi", "Sangat Tinggi" };

        var items = new List<PotensiKonflik>
        {
            new()
            {
                Periode = DateTime.UtcNow.AddDays(-45),
                Aspek = "Ekonomi",
                Kabupaten = "Belitung Timur",
                Kecamatan = "Kelapa Kampit",
                Desa = "Mayang",
                TitikKoordinat = "-2.82, 107.95",
                SumberInformasi = "Laporan masyarakat",
                NamaPotensiKonflik = "Sengketa lahan perkebunan PT. ARC dengan warga adat",
                KemungkinanLevel = "Sering",
                KemungkinanDeskripsi = "Klaim lahan adat oleh warga terhadap area perkebunan",
                SumberSebabPermasalahan = "Tumpang tindih sertifikat lahan",
                LatarBelakangMasalah = "Warga mengklaim lahan adat yang telah diberikan HGU kepada perusahaan",
                DampakLevel = "Mayor",
                DampakDeskripsi = "Potensi bentrokan fisik antara warga dan security perusahaan",
                UpayaPenanganan = "Mediasi oleh Pemda dan BPN",
                Rekomendasi = "Percepat penyelesaian sengketa melalui jalur hukum",
                TingkatRisiko = "Sangat Tinggi",
                Status = "disetujui",
                ApprovedBy = "Admin SIPINTAR",
                ApprovedAt = DateTime.UtcNow.AddDays(-40),
                CreatedBy = "Admin SIPINTAR",
                CreatedAt = DateTime.UtcNow.AddDays(-50),
            },
            new()
            {
                Periode = DateTime.UtcNow.AddDays(-30),
                Aspek = "Sosial Budaya",
                Kabupaten = "Belitung Timur",
                Kecamatan = "Manggar",
                Desa = "Kurnia Jaya",
                TitikKoordinat = "-2.88, 108.27",
                SumberInformasi = "Tokoh masyarakat",
                NamaPotensiKonflik = "Gesekan antar kelompok pemuda di media sosial",
                KemungkinanLevel = "Sedang",
                KemungkinanDeskripsi = "Provokasi di media sosial memicu ketegangan antar kelompok",
                SumberSebabPermasalahan = "Hoax dan ujaran kebencian di media sosial",
                LatarBelakangMasalah = "Persaingan antar kelompok pemuda yang dipicu konten provokatif",
                DampakLevel = "Moderat",
                DampakDeskripsi = "Potensi tawuran antar kelompok",
                UpayaPenanganan = "Pendekatan tokoh pemuda dan patroli siber",
                Rekomendasi = "Sosialisasi literasi digital dan mediasi antar kelompok",
                TingkatRisiko = "Sedang",
                Status = "disetujui",
                ApprovedBy = "Admin SIPINTAR",
                ApprovedAt = DateTime.UtcNow.AddDays(-25),
                CreatedBy = "Operator EWS",
                CreatedAt = DateTime.UtcNow.AddDays(-35),
            },
            new()
            {
                Periode = DateTime.UtcNow.AddDays(-20),
                Aspek = "Politik",
                Kabupaten = "Belitung Timur",
                Kecamatan = "Gantung",
                Desa = "Selingsing",
                TitikKoordinat = "-2.96, 108.08",
                SumberInformasi = "Laporan petugas",
                NamaPotensiKonflik = "Ketegangan menjelang Pilkades",
                KemungkinanLevel = "Sedang",
                KemungkinanDeskripsi = "Persaingan antar calon kepala desa memicu polarisasi warga",
                SumberSebabPermasalahan = "Politik identitas dan money politics",
                LatarBelakangMasalah = "Dua calon kuat dengan basis massa yang fanatik",
                DampakLevel = "Minor",
                DampakDeskripsi = "Ketegangan sosial di tingkat RT/RW",
                UpayaPenanganan = "Sosialisasi Pilkades damai oleh Bawaslu",
                Rekomendasi = "Pengawasan ketat dan mediasi antar pendukung",
                TingkatRisiko = "Rendah",
                Status = "disetujui",
                ApprovedBy = "Admin SIPINTAR",
                ApprovedAt = DateTime.UtcNow.AddDays(-15),
                CreatedBy = "Operator EWS",
                CreatedAt = DateTime.UtcNow.AddDays(-25),
            },
            new()
            {
                Periode = DateTime.UtcNow.AddDays(-15),
                Aspek = "Hankam",
                Kabupaten = "Belitung Timur",
                Kecamatan = "Simpang Pesak",
                Desa = "Tanjung Batu Itam",
                TitikKoordinat = "-3.10, 107.88",
                SumberInformasi = "TNI AL",
                NamaPotensiKonflik = "Konflik nelayan lokal vs kapal asing",
                KemungkinanLevel = "Sangat Sering",
                KemungkinanDeskripsi = "Kapal asing berulang kali masuk perairan nelayan lokal",
                SumberSebabPermasalahan = "Lemahnya pengawasan perairan",
                LatarBelakangMasalah = "Penangkapan ikan ilegal oleh kapal asing di zona tangkap nelayan lokal",
                DampakLevel = "Katastrofik",
                DampakDeskripsi = "Kerugian ekonomi besar bagi nelayan dan potensi konflik fisik di laut",
                UpayaPenanganan = "Patroli gabungan TNI AL dan Polair",
                Rekomendasi = "Peningkatan frekuensi patroli dan koordinasi dengan negara tetangga",
                TingkatRisiko = "Sangat Tinggi",
                Status = "disetujui",
                ApprovedBy = "Admin SIPINTAR",
                ApprovedAt = DateTime.UtcNow.AddDays(-10),
                CreatedBy = "Admin SIPINTAR",
                CreatedAt = DateTime.UtcNow.AddDays(-20),
            },
            new()
            {
                Periode = DateTime.UtcNow.AddDays(-10),
                Aspek = "Ekonomi",
                Kabupaten = "Belitung Timur",
                Kecamatan = "Simpang Renggiang",
                Desa = "Renggiang",
                TitikKoordinat = "-2.92, 107.98",
                SumberInformasi = "Dinas Tenaga Kerja",
                NamaPotensiKonflik = "Ketegangan warga pendatang vs lokal soal akses lahan",
                KemungkinanLevel = "Jarang",
                KemungkinanDeskripsi = "Warga pendatang membuka lahan tanpa izin",
                SumberSebabPermasalahan = "Ketimpangan akses ekonomi",
                LatarBelakangMasalah = "Warga pendatang dari luar daerah membuka lahan pertanian di area yang diklaim warga lokal",
                DampakLevel = "Moderat",
                DampakDeskripsi = "Ketegangan sosial dan potensi pengusiran",
                UpayaPenanganan = "Mediasi oleh camat dan tokoh adat",
                Rekomendasi = "Pemetaan lahan dan penertiban izin",
                TingkatRisiko = "Sedang",
                Status = "disetujui",
                ApprovedBy = "Admin SIPINTAR",
                ApprovedAt = DateTime.UtcNow.AddDays(-7),
                CreatedBy = "Operator EWS",
                CreatedAt = DateTime.UtcNow.AddDays(-15),
            },
            new()
            {
                Periode = DateTime.UtcNow.AddDays(-5),
                Aspek = "Hankam",
                Kabupaten = "Belitung Timur",
                Kecamatan = "Manggar",
                Desa = "Lalang",
                TitikKoordinat = "-2.87, 108.28",
                SumberInformasi = "Polres Belitung Timur",
                NamaPotensiKonflik = "Pencemaran sungai oleh tambang ilegal",
                KemungkinanLevel = "Sering",
                KemungkinanDeskripsi = "Aktivitas tambang timah ilegal mencemari sungai",
                SumberSebabPermasalahan = "Tambang ilegal tanpa AMDAL",
                LatarBelakangMasalah = "Warga mengeluh air sungai tercemar limbah tambang, mengancam sumber air bersih",
                DampakLevel = "Mayor",
                DampakDeskripsi = "Kerusakan lingkungan dan ancaman kesehatan warga",
                UpayaPenanganan = "Penertiban oleh Satpol PP dan Polres",
                Rekomendasi = "Penegakan hukum tegas dan rehabilitasi lingkungan",
                TingkatRisiko = "Tinggi",
                Status = "disetujui",
                ApprovedBy = "Admin SIPINTAR",
                ApprovedAt = DateTime.UtcNow.AddDays(-3),
                CreatedBy = "Admin SIPINTAR",
                CreatedAt = DateTime.UtcNow.AddDays(-8),
            },
            // Menunggu approval
            new()
            {
                Periode = DateTime.UtcNow.AddDays(-1),
                Aspek = "Sosial Budaya",
                Kabupaten = "Belitung Timur",
                Kecamatan = "Dendang",
                Desa = "Dendang",
                SumberInformasi = "Laporan warga",
                NamaPotensiKonflik = "Sengketa batas wilayah antar desa",
                KemungkinanLevel = "Jarang",
                KemungkinanDeskripsi = "Perselisihan batas wilayah antara dua desa",
                SumberSebabPermasalahan = "Peta batas desa yang tidak jelas",
                LatarBelakangMasalah = "Kedua desa mengklaim area yang sama untuk pembangunan fasilitas umum",
                DampakLevel = "Minor",
                DampakDeskripsi = "Ketegangan antar warga desa",
                UpayaPenanganan = "Musyawarah tingkat kecamatan",
                Rekomendasi = "Penetapan batas desa definitif oleh BPN",
                TingkatRisiko = "Rendah",
                Status = "menunggu",
                CreatedBy = "Operator EWS",
                CreatedAt = DateTime.UtcNow.AddDays(-1),
            },
        };

        db.PotensiKonfliks.AddRange(items);
        await db.SaveChangesAsync();
    }

    private static async Task SeedPeristiwaKonflik(AppDbContext db)
    {
        var items = new List<PeristiwaKonflik>
        {
            new()
            {
                Periode = DateTime.UtcNow.AddDays(-60),
                NamaPeristiwa = "Bentrokan warga vs security PT. ARC di Kelapa Kampit",
                SumberSebabKonflik = "Sengketa lahan adat",
                LatarBelakangKejadian = "Warga memblokir akses jalan perkebunan sebagai protes atas klaim lahan adat yang tidak ditanggapi",
                DeskripsiAkibatPeristiwa = "Terjadi bentrokan fisik antara warga dan pihak security perusahaan",
                KorbanKritis = 0,
                KorbanLukaLuka = 3,
                KorbanMengungsi = 0,
                KerugianMateril = 15000000,
                UpayaPenanganan = "Polres menerjunkan personel untuk mengamankan lokasi",
                UpayaPemulihan = "Mediasi oleh Pemda dengan melibatkan BPN",
                Kabupaten = "Belitung Timur",
                Kecamatan = "Kelapa Kampit",
                Desa = "Mayang",
                TitikKoordinat = "-2.82, 107.95",
                SumberInformasi = "Polres Belitung Timur",
                Keterangan = "Situasi sudah kondusif setelah mediasi",
                SaranTindakLanjut = "Percepat penyelesaian sengketa lahan melalui jalur hukum",
                TingkatRisiko = "Tinggi",
                Status = "disetujui",
                ApprovedBy = "Admin SIPINTAR",
                ApprovedAt = DateTime.UtcNow.AddDays(-55),
                CreatedBy = "Admin SIPINTAR",
                CreatedAt = DateTime.UtcNow.AddDays(-60),
            },
            new()
            {
                Periode = DateTime.UtcNow.AddDays(-40),
                NamaPeristiwa = "Tawuran antar kelompok pemuda di Manggar",
                SumberSebabKonflik = "Provokasi media sosial",
                LatarBelakangKejadian = "Saling ejek di media sosial berujung pertemuan fisik di lapangan",
                DeskripsiAkibatPeristiwa = "Tawuran melibatkan sekitar 20 orang dari dua kelompok",
                KorbanKritis = 0,
                KorbanLukaLuka = 5,
                KorbanMengungsi = 0,
                KerugianMateril = 5000000,
                UpayaPenanganan = "Polsek Manggar membubarkan dan mengamankan 8 orang",
                UpayaPemulihan = "Pembinaan oleh Babinsa dan tokoh pemuda",
                Kabupaten = "Belitung Timur",
                Kecamatan = "Manggar",
                Desa = "Kurnia Jaya",
                TitikKoordinat = "-2.88, 108.27",
                SumberInformasi = "Polsek Manggar",
                Keterangan = "8 orang diamankan, 5 luka ringan",
                SaranTindakLanjut = "Pembinaan berkelanjutan dan patroli rutin",
                TingkatRisiko = "Sedang",
                Status = "disetujui",
                ApprovedBy = "Admin SIPINTAR",
                ApprovedAt = DateTime.UtcNow.AddDays(-35),
                CreatedBy = "Operator EWS",
                CreatedAt = DateTime.UtcNow.AddDays(-40),
            },
            new()
            {
                Periode = DateTime.UtcNow.AddDays(-20),
                NamaPeristiwa = "Pengejaran kapal ikan asing di perairan Simpang Pesak",
                SumberSebabKonflik = "Illegal fishing",
                LatarBelakangKejadian = "Nelayan lokal mengejar dan memblokir kapal asing yang menangkap ikan di zona tangkap mereka",
                DeskripsiAkibatPeristiwa = "Tabrakan perahu nelayan dengan kapal asing, 1 perahu rusak berat",
                KorbanKritis = 1,
                KorbanLukaLuka = 2,
                KorbanMengungsi = 0,
                KerugianMateril = 75000000,
                UpayaPenanganan = "TNI AL dan Polair melakukan evakuasi dan pengamanan",
                UpayaPemulihan = "Bantuan perbaikan perahu dan santunan korban",
                Kabupaten = "Belitung Timur",
                Kecamatan = "Simpang Pesak",
                Desa = "Tanjung Batu Itam",
                TitikKoordinat = "-3.10, 107.88",
                SumberInformasi = "TNI AL Pos Belitung Timur",
                Keterangan = "Kapal asing berhasil diamankan, 1 nelayan kritis",
                SaranTindakLanjut = "Peningkatan patroli dan kerjasama bilateral",
                TingkatRisiko = "Sangat Tinggi",
                Status = "disetujui",
                ApprovedBy = "Admin SIPINTAR",
                ApprovedAt = DateTime.UtcNow.AddDays(-18),
                CreatedBy = "Admin SIPINTAR",
                CreatedAt = DateTime.UtcNow.AddDays(-20),
            },
            new()
            {
                Periode = DateTime.UtcNow.AddDays(-10),
                NamaPeristiwa = "Protes pekerja tambang timah di Kelapa Kampit",
                SumberSebabKonflik = "Pemutusan kontrak sepihak",
                LatarBelakangKejadian = "50 pekerja tambang di-PHK tanpa pesangon yang layak",
                DeskripsiAkibatPeristiwa = "Aksi blokir jalan dan demo di depan kantor perusahaan",
                KorbanKritis = 0,
                KorbanLukaLuka = 0,
                KorbanMengungsi = 0,
                KerugianMateril = 10000000,
                UpayaPenanganan = "Mediasi tripartit oleh Disnaker",
                UpayaPemulihan = "Negosiasi pesangon dan pelatihan kerja",
                Kabupaten = "Belitung Timur",
                Kecamatan = "Kelapa Kampit",
                Desa = "Air Kelik",
                TitikKoordinat = "-2.83, 107.96",
                SumberInformasi = "Dinas Tenaga Kerja",
                Keterangan = "Aksi damai, tidak ada kekerasan",
                SaranTindakLanjut = "Pengawasan ketenagakerjaan dan mediasi lanjutan",
                TingkatRisiko = "Sedang",
                Status = "disetujui",
                ApprovedBy = "Admin SIPINTAR",
                ApprovedAt = DateTime.UtcNow.AddDays(-8),
                CreatedBy = "Operator EWS",
                CreatedAt = DateTime.UtcNow.AddDays(-10),
            },
            // Menunggu approval
            new()
            {
                Periode = DateTime.UtcNow.AddDays(-3),
                NamaPeristiwa = "Perusakan pos jaga tambang di Damar",
                SumberSebabKonflik = "Penolakan warga terhadap aktivitas tambang",
                LatarBelakangKejadian = "Warga merusak pos jaga perusahaan tambang yang dianggap mencemari lingkungan",
                DeskripsiAkibatPeristiwa = "Pos jaga dirusak, 2 kendaraan operasional dibakar",
                KorbanKritis = 0,
                KorbanLukaLuka = 1,
                KorbanMengungsi = 0,
                KerugianMateril = 50000000,
                UpayaPenanganan = "Polsek Damar mengamankan lokasi",
                UpayaPemulihan = "Dialog dengan warga dan perusahaan",
                Kabupaten = "Belitung Timur",
                Kecamatan = "Damar",
                Desa = "Mengkubang",
                TitikKoordinat = "-3.05, 108.10",
                SumberInformasi = "Polsek Damar",
                Keterangan = "5 orang diamankan sebagai tersangka",
                SaranTindakLanjut = "Audit lingkungan dan dialog publik",
                TingkatRisiko = "Tinggi",
                Status = "menunggu",
                CreatedBy = "Operator EWS",
                CreatedAt = DateTime.UtcNow.AddDays(-3),
            },
        };

        db.PeristiwaKonfliks.AddRange(items);
        await db.SaveChangesAsync();
    }

    private static string GetCoordinate(string kecamatan, Random random)
    {
        var coords = kecamatan switch
        {
            "Manggar" => (-2.88 + (random.NextDouble() - 0.5) * 0.02, 108.27 + (random.NextDouble() - 0.5) * 0.02),
            "Gantung" => (-2.96 + (random.NextDouble() - 0.5) * 0.02, 108.08 + (random.NextDouble() - 0.5) * 0.02),
            "Dendang" => (-2.78 + (random.NextDouble() - 0.5) * 0.02, 108.15 + (random.NextDouble() - 0.5) * 0.02),
            "Kelapa Kampit" => (-2.82 + (random.NextDouble() - 0.5) * 0.02, 107.95 + (random.NextDouble() - 0.5) * 0.02),
            "Damar" => (-3.05 + (random.NextDouble() - 0.5) * 0.02, 108.10 + (random.NextDouble() - 0.5) * 0.02),
            "Simpang Renggiang" => (-2.92 + (random.NextDouble() - 0.5) * 0.02, 107.98 + (random.NextDouble() - 0.5) * 0.02),
            "Simpang Pesak" => (-3.10 + (random.NextDouble() - 0.5) * 0.02, 107.88 + (random.NextDouble() - 0.5) * 0.02),
            _ => (-2.90, 108.05),
        };
        return $"{coords.Item1:F4}, {coords.Item2:F4}";
    }
}
