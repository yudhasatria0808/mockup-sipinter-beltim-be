using Microsoft.EntityFrameworkCore;
using SipintarBeltim.Data;

namespace SipintarBeltim.Endpoints;

public static class DashboardEndpoints
{
    public static void MapDashboardEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/dashboard")
            .WithTags("Dashboard").RequireAuthorization();

        // GET /api/dashboard/stats — Aggregated statistics
        group.MapGet("/stats", async (AppDbContext db) =>
        {
            // Kewaspadaan stats (disetujui)
            var kewaspadaanAll = await db.KewaspadaanDinis
                .Where(k => k.Status == "disetujui")
                .ToListAsync();

            var kewaspadaanStats = new
            {
                total = kewaspadaanAll.Count,
                sangatTinggi = kewaspadaanAll.Count(k => k.TingkatRisiko == "Sangat Tinggi"),
                tinggi = kewaspadaanAll.Count(k => k.TingkatRisiko == "Tinggi"),
                sedang = kewaspadaanAll.Count(k => k.TingkatRisiko == "Sedang"),
                rendah = kewaspadaanAll.Count(k => k.TingkatRisiko == "Rendah"),
            };

            // Potensi Konflik stats (disetujui)
            var potensiAll = await db.PotensiKonfliks
                .Where(p => p.Status == "disetujui")
                .ToListAsync();

            var potensiStats = new
            {
                total = potensiAll.Count,
                sangatTinggi = potensiAll.Count(p => p.TingkatRisiko == "Sangat Tinggi"),
                tinggi = potensiAll.Count(p => p.TingkatRisiko == "Tinggi"),
                sedang = potensiAll.Count(p => p.TingkatRisiko == "Sedang"),
                rendah = potensiAll.Count(p => p.TingkatRisiko == "Rendah"),
            };

            // Peristiwa Konflik stats (disetujui)
            var peristiwaAll = await db.PeristiwaKonfliks
                .Where(p => p.Status == "disetujui")
                .ToListAsync();

            var peristiwaStats = new
            {
                total = peristiwaAll.Count,
                sangatTinggi = peristiwaAll.Count(p => p.TingkatRisiko == "Sangat Tinggi"),
                tinggi = peristiwaAll.Count(p => p.TingkatRisiko == "Tinggi"),
                sedang = peristiwaAll.Count(p => p.TingkatRisiko == "Sedang"),
                rendah = peristiwaAll.Count(p => p.TingkatRisiko == "Rendah"),
                totalKorbanKritis = peristiwaAll.Sum(p => p.KorbanKritis),
                totalKorbanLuka = peristiwaAll.Sum(p => p.KorbanLukaLuka),
                totalKorbanMengungsi = peristiwaAll.Sum(p => p.KorbanMengungsi),
                totalKerugian = peristiwaAll.Sum(p => p.KerugianMateril),
            };

            // WNA & TKA counts
            var wnaTotal = await db.WargaNegaraAsings.CountAsync();
            var tkaTotal = await db.TenagaKerjaAsings.CountAsync();

            // Pending approval counts
            var pendingKewaspadaan = await db.KewaspadaanDinis.CountAsync(k => k.Status == "menunggu");
            var pendingPotensi = await db.PotensiKonfliks.CountAsync(p => p.Status == "menunggu");
            var pendingPeristiwa = await db.PeristiwaKonfliks.CountAsync(p => p.Status == "menunggu");

            return Results.Ok(new
            {
                success = true,
                data = new
                {
                    kewaspadaan = kewaspadaanStats,
                    potensiKonflik = potensiStats,
                    peristiwaKonflik = peristiwaStats,
                    wna = new { total = wnaTotal },
                    tka = new { total = tkaTotal },
                    pendingApproval = new
                    {
                        kewaspadaan = pendingKewaspadaan,
                        potensiKonflik = pendingPotensi,
                        peristiwaKonflik = pendingPeristiwa,
                        total = pendingKewaspadaan + pendingPotensi + pendingPeristiwa,
                    }
                }
            });
        })
        .WithName("GetDashboardStats")
        .WithOpenApi();

        // GET /api/dashboard/map — Map data with coordinates per kecamatan
        group.MapGet("/map", async (AppDbContext db) =>
        {
            // Get all kecamatan from wilayah table
            var kecamatanList = await db.Wilayahs
                .Where(w => w.Tipe == "Kecamatan")
                .Select(w => new
                {
                    w.Id,
                    w.Nama,
                    w.KodeBps,
                    w.Latitude,
                    w.Longitude,
                })
                .ToListAsync();

            // Get approved data grouped by kecamatan
            var kewaspadaanByKec = await db.KewaspadaanDinis
                .Where(k => k.Status == "disetujui")
                .GroupBy(k => k.Kecamatan)
                .Select(g => new
                {
                    Kecamatan = g.Key,
                    Total = g.Count(),
                    SangatTinggi = g.Count(k => k.TingkatRisiko == "Sangat Tinggi"),
                    Tinggi = g.Count(k => k.TingkatRisiko == "Tinggi"),
                    Sedang = g.Count(k => k.TingkatRisiko == "Sedang"),
                    Rendah = g.Count(k => k.TingkatRisiko == "Rendah"),
                })
                .ToListAsync();

            var potensiByKec = await db.PotensiKonfliks
                .Where(p => p.Status == "disetujui")
                .GroupBy(p => p.Kecamatan)
                .Select(g => new
                {
                    Kecamatan = g.Key,
                    Total = g.Count(),
                    SangatTinggi = g.Count(p => p.TingkatRisiko == "Sangat Tinggi"),
                    Tinggi = g.Count(p => p.TingkatRisiko == "Tinggi"),
                    Sedang = g.Count(p => p.TingkatRisiko == "Sedang"),
                    Rendah = g.Count(p => p.TingkatRisiko == "Rendah"),
                })
                .ToListAsync();

            var peristiwaByKec = await db.PeristiwaKonfliks
                .Where(p => p.Status == "disetujui")
                .GroupBy(p => p.Kecamatan)
                .Select(g => new
                {
                    Kecamatan = g.Key,
                    Total = g.Count(),
                    SangatTinggi = g.Count(p => p.TingkatRisiko == "Sangat Tinggi"),
                    Tinggi = g.Count(p => p.TingkatRisiko == "Tinggi"),
                    Sedang = g.Count(p => p.TingkatRisiko == "Sedang"),
                    Rendah = g.Count(p => p.TingkatRisiko == "Rendah"),
                    TotalKorban = g.Sum(p => p.KorbanKritis + p.KorbanLukaLuka + p.KorbanMengungsi),
                })
                .ToListAsync();

            var wnaByKec = await db.WargaNegaraAsings
                .GroupBy(w => w.Kecamatan)
                .Select(g => new { Kecamatan = g.Key, Total = g.Count() })
                .ToListAsync();

            var tkaByKec = await db.TenagaKerjaAsings
                .GroupBy(t => t.Kecamatan)
                .Select(g => new { Kecamatan = g.Key, Total = g.Count() })
                .ToListAsync();

            // Get individual points with coordinates for heatmap
            var heatmapPoints = new List<object>();

            // Kewaspadaan points
            var kewaspadaanPoints = await db.KewaspadaanDinis
                .Where(k => k.Status == "disetujui" && k.TitikKoordinat != null)
                .Select(k => new { k.TitikKoordinat, k.TingkatRisiko, Type = "kewaspadaan" })
                .ToListAsync();

            foreach (var p in kewaspadaanPoints)
            {
                var coords = ParseCoordinates(p.TitikKoordinat);
                if (coords != null)
                {
                    heatmapPoints.Add(new
                    {
                        latitude = coords.Value.lat,
                        longitude = coords.Value.lng,
                        intensity = GetIntensity(p.TingkatRisiko),
                        type = p.Type,
                    });
                }
            }

            // Potensi points
            var potensiPoints = await db.PotensiKonfliks
                .Where(p => p.Status == "disetujui" && p.TitikKoordinat != null)
                .Select(p => new { p.TitikKoordinat, p.TingkatRisiko, Type = "potensi" })
                .ToListAsync();

            foreach (var p in potensiPoints)
            {
                var coords = ParseCoordinates(p.TitikKoordinat);
                if (coords != null)
                {
                    heatmapPoints.Add(new
                    {
                        latitude = coords.Value.lat,
                        longitude = coords.Value.lng,
                        intensity = GetIntensity(p.TingkatRisiko),
                        type = p.Type,
                    });
                }
            }

            // Peristiwa points
            var peristiwaPoints = await db.PeristiwaKonfliks
                .Where(p => p.Status == "disetujui" && p.TitikKoordinat != null)
                .Select(p => new { p.TitikKoordinat, p.TingkatRisiko, Type = "peristiwa" })
                .ToListAsync();

            foreach (var p in peristiwaPoints)
            {
                var coords = ParseCoordinates(p.TitikKoordinat);
                if (coords != null)
                {
                    heatmapPoints.Add(new
                    {
                        latitude = coords.Value.lat,
                        longitude = coords.Value.lng,
                        intensity = GetIntensity(p.TingkatRisiko),
                        type = p.Type,
                    });
                }
            }

            // Build kecamatan map data
            var allKecamatanNames = kewaspadaanByKec.Select(k => k.Kecamatan)
                .Union(potensiByKec.Select(p => p.Kecamatan))
                .Union(peristiwaByKec.Select(p => p.Kecamatan))
                .Union(kecamatanList.Select(k => k.Nama))
                .Distinct()
                .ToList();

            var mapData = allKecamatanNames.Select(kecNama =>
            {
                var wilayah = kecamatanList.FirstOrDefault(w =>
                    w.Nama.Equals(kecNama, StringComparison.OrdinalIgnoreCase));
                var kew = kewaspadaanByKec.FirstOrDefault(k =>
                    k.Kecamatan.Equals(kecNama, StringComparison.OrdinalIgnoreCase));
                var pot = potensiByKec.FirstOrDefault(p =>
                    p.Kecamatan.Equals(kecNama, StringComparison.OrdinalIgnoreCase));
                var per = peristiwaByKec.FirstOrDefault(p =>
                    p.Kecamatan.Equals(kecNama, StringComparison.OrdinalIgnoreCase));
                var wna = wnaByKec.FirstOrDefault(w =>
                    w.Kecamatan.Equals(kecNama, StringComparison.OrdinalIgnoreCase));
                var tka = tkaByKec.FirstOrDefault(t =>
                    t.Kecamatan.Equals(kecNama, StringComparison.OrdinalIgnoreCase));

                var totalKonflik = (kew?.Total ?? 0) + (pot?.Total ?? 0) + (per?.Total ?? 0);
                var risikoTinggi = (kew?.SangatTinggi ?? 0) + (kew?.Tinggi ?? 0)
                    + (pot?.SangatTinggi ?? 0) + (pot?.Tinggi ?? 0)
                    + (per?.SangatTinggi ?? 0) + (per?.Tinggi ?? 0);
                var risikoSedang = (kew?.Sedang ?? 0) + (pot?.Sedang ?? 0) + (per?.Sedang ?? 0);
                var risikoRendah = (kew?.Rendah ?? 0) + (pot?.Rendah ?? 0) + (per?.Rendah ?? 0);

                return new
                {
                    kecamatan = kecNama,
                    kodeBps = wilayah?.KodeBps,
                    latitude = wilayah?.Latitude,
                    longitude = wilayah?.Longitude,
                    totalKonflik,
                    risikoTinggi,
                    risikoSedang,
                    risikoRendah,
                    kewaspadaan = kew?.Total ?? 0,
                    potensiKonflik = pot?.Total ?? 0,
                    peristiwaKonflik = per?.Total ?? 0,
                    wna = wna?.Total ?? 0,
                    tka = tka?.Total ?? 0,
                };
            })
            .Where(k => k.totalKonflik > 0 || k.latitude != null)
            .OrderByDescending(k => k.totalKonflik)
            .ToList();

            return Results.Ok(new
            {
                success = true,
                data = new
                {
                    kecamatan = mapData,
                    heatmapPoints,
                }
            });
        })
        .WithName("GetDashboardMap")
        .WithOpenApi();

        // GET /api/dashboard/heatmap — Heatmap data (aspek x kecamatan matrix)
        group.MapGet("/heatmap", async (AppDbContext db) =>
        {
            // Get all aspek from master data
            var aspekList = await db.Aspeks
                .OrderBy(a => a.Nama)
                .Select(a => a.Nama)
                .ToListAsync();

            // Get kecamatan list
            var kecamatanNames = await db.Wilayahs
                .Where(w => w.Tipe == "Kecamatan")
                .OrderBy(w => w.Nama)
                .Select(w => w.Nama)
                .ToListAsync();

            // If no wilayah data, get from existing records
            if (kecamatanNames.Count == 0)
            {
                kecamatanNames = await db.KewaspadaanDinis
                    .Where(k => k.Status == "disetujui")
                    .Select(k => k.Kecamatan)
                    .Union(db.PotensiKonfliks
                        .Where(p => p.Status == "disetujui")
                        .Select(p => p.Kecamatan))
                    .Distinct()
                    .OrderBy(k => k)
                    .ToListAsync();
            }

            // Build matrix: for each aspek x kecamatan, count occurrences
            var kewaspadaanData = await db.KewaspadaanDinis
                .Where(k => k.Status == "disetujui")
                .GroupBy(k => new { k.Aspek, k.Kecamatan })
                .Select(g => new { g.Key.Aspek, g.Key.Kecamatan, Count = g.Count() })
                .ToListAsync();

            var potensiData = await db.PotensiKonfliks
                .Where(p => p.Status == "disetujui")
                .GroupBy(p => new { p.Aspek, p.Kecamatan })
                .Select(g => new { g.Key.Aspek, g.Key.Kecamatan, Count = g.Count() })
                .ToListAsync();

            // Build the matrix
            var matrix = aspekList.Select(aspek =>
            {
                var row = new Dictionary<string, object> { { "aspek", aspek } };
                foreach (var kec in kecamatanNames)
                {
                    var kewCount = kewaspadaanData
                        .Where(k => k.Aspek.Equals(aspek, StringComparison.OrdinalIgnoreCase)
                            && k.Kecamatan.Equals(kec, StringComparison.OrdinalIgnoreCase))
                        .Sum(k => k.Count);
                    var potCount = potensiData
                        .Where(p => p.Aspek.Equals(aspek, StringComparison.OrdinalIgnoreCase)
                            && p.Kecamatan.Equals(kec, StringComparison.OrdinalIgnoreCase))
                        .Sum(p => p.Count);
                    row[kec] = kewCount + potCount;
                }
                return row;
            }).ToList();

            return Results.Ok(new
            {
                success = true,
                data = new
                {
                    aspekList,
                    kecamatanList = kecamatanNames,
                    matrix,
                }
            });
        })
        .WithName("GetDashboardHeatmap")
        .WithOpenApi();

        // GET /api/dashboard/trend — Monthly trend data
        group.MapGet("/trend", async (int? months, AppDbContext db) =>
        {
            var monthCount = months ?? 6;
            var startDate = DateTime.UtcNow.AddMonths(-monthCount);

            var kewaspadaanTrend = await db.KewaspadaanDinis
                .Where(k => k.Status == "disetujui" && k.CreatedAt >= startDate)
                .ToListAsync();

            var potensiTrend = await db.PotensiKonfliks
                .Where(p => p.Status == "disetujui" && p.CreatedAt >= startDate)
                .ToListAsync();

            var peristiwaTrend = await db.PeristiwaKonfliks
                .Where(p => p.Status == "disetujui" && p.CreatedAt >= startDate)
                .ToListAsync();

            // Group by month
            var trendData = new List<object>();
            for (int i = monthCount - 1; i >= 0; i--)
            {
                var monthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1).AddMonths(-i);
                var monthEnd = monthStart.AddMonths(1);

                trendData.Add(new
                {
                    bulan = monthStart.ToString("yyyy-MM"),
                    bulanLabel = monthStart.ToString("MMM yyyy"),
                    kewaspadaan = kewaspadaanTrend.Count(k => k.CreatedAt >= monthStart && k.CreatedAt < monthEnd),
                    potensiKonflik = potensiTrend.Count(p => p.CreatedAt >= monthStart && p.CreatedAt < monthEnd),
                    peristiwaKonflik = peristiwaTrend.Count(p => p.CreatedAt >= monthStart && p.CreatedAt < monthEnd),
                });
            }

            return Results.Ok(new { success = true, data = trendData });
        })
        .WithName("GetDashboardTrend")
        .WithOpenApi();

        // GET /api/dashboard/recent — Recent items from all categories
        group.MapGet("/recent", async (int? limit, AppDbContext db) =>
        {
            var take = limit ?? 10;

            var kewaspadaanRecent = await db.KewaspadaanDinis
                .OrderByDescending(k => k.CreatedAt)
                .Take(take)
                .Select(k => new
                {
                    id = k.Id.ToString(),
                    type = "kewaspadaan",
                    kecamatan = k.Kecamatan,
                    desa = k.Desa,
                    aspek = k.Aspek,
                    tingkatRisiko = k.TingkatRisiko,
                    deskripsi = k.KemungkinanAncamanDeskripsi,
                    status = k.Status,
                    tanggal = k.CreatedAt.ToString("o"),
                })
                .ToListAsync();

            var potensiRecent = await db.PotensiKonfliks
                .OrderByDescending(p => p.CreatedAt)
                .Take(take)
                .Select(p => new
                {
                    id = p.Id.ToString(),
                    type = "potensi",
                    kecamatan = p.Kecamatan,
                    desa = p.Desa,
                    aspek = p.Aspek,
                    tingkatRisiko = p.TingkatRisiko,
                    deskripsi = p.NamaPotensiKonflik,
                    status = p.Status,
                    tanggal = p.CreatedAt.ToString("o"),
                })
                .ToListAsync();

            var peristiwaRecent = await db.PeristiwaKonfliks
                .OrderByDescending(p => p.CreatedAt)
                .Take(take)
                .Select(p => new
                {
                    id = p.Id.ToString(),
                    type = "peristiwa",
                    kecamatan = p.Kecamatan,
                    desa = p.Desa,
                    aspek = "-",
                    tingkatRisiko = p.TingkatRisiko,
                    deskripsi = p.NamaPeristiwa,
                    status = p.Status,
                    tanggal = p.CreatedAt.ToString("o"),
                })
                .ToListAsync();

            // Merge and sort
            var allItems = kewaspadaanRecent
                .Cast<object>()
                .Concat(potensiRecent.Cast<object>())
                .Concat(peristiwaRecent.Cast<object>())
                .ToList();

            // Sort by tanggal descending (using reflection or dynamic)
            var sorted = allItems
                .OrderByDescending(item =>
                {
                    var prop = item.GetType().GetProperty("tanggal");
                    return prop?.GetValue(item)?.ToString() ?? "";
                })
                .Take(take)
                .ToList();

            return Results.Ok(new { success = true, data = sorted });
        })
        .WithName("GetDashboardRecent")
        .WithOpenApi();

        // GET /api/dashboard/aspek-distribution — Distribution by aspek
        group.MapGet("/aspek-distribution", async (AppDbContext db) =>
        {
            var kewaspadaanByAspek = await db.KewaspadaanDinis
                .Where(k => k.Status == "disetujui")
                .GroupBy(k => k.Aspek)
                .Select(g => new { Aspek = g.Key, Count = g.Count() })
                .ToListAsync();

            var potensiByAspek = await db.PotensiKonfliks
                .Where(p => p.Status == "disetujui")
                .GroupBy(p => p.Aspek)
                .Select(g => new { Aspek = g.Key, Count = g.Count() })
                .ToListAsync();

            var allAspek = kewaspadaanByAspek.Select(k => k.Aspek)
                .Union(potensiByAspek.Select(p => p.Aspek))
                .Distinct()
                .ToList();

            var distribution = allAspek.Select(aspek => new
            {
                aspek,
                kewaspadaan = kewaspadaanByAspek.FirstOrDefault(k => k.Aspek == aspek)?.Count ?? 0,
                potensiKonflik = potensiByAspek.FirstOrDefault(p => p.Aspek == aspek)?.Count ?? 0,
                total = (kewaspadaanByAspek.FirstOrDefault(k => k.Aspek == aspek)?.Count ?? 0)
                    + (potensiByAspek.FirstOrDefault(p => p.Aspek == aspek)?.Count ?? 0),
            })
            .OrderByDescending(d => d.total)
            .ToList();

            return Results.Ok(new { success = true, data = distribution });
        })
        .WithName("GetDashboardAspekDistribution")
        .WithOpenApi();
    }

    private static (double lat, double lng)? ParseCoordinates(string? koordinat)
    {
        if (string.IsNullOrWhiteSpace(koordinat)) return null;

        // Try parsing formats: "-2.88, 108.27" or "-2.88,108.27"
        var parts = koordinat.Split(',', StringSplitOptions.TrimEntries);
        if (parts.Length == 2
            && double.TryParse(parts[0], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var lat)
            && double.TryParse(parts[1], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var lng))
        {
            return (lat, lng);
        }
        return null;
    }

    private static double GetIntensity(string tingkatRisiko)
    {
        return tingkatRisiko switch
        {
            "Sangat Tinggi" => 1.0,
            "Tinggi" => 0.75,
            "Sedang" => 0.5,
            "Rendah" => 0.25,
            _ => 0.3,
        };
    }
}
