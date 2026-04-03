using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;

namespace Ticaret_Apı.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UrunController : ControllerBase
    {
        public string db_ = $"Data Source={Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\TicaretDb.db";

        public UrunController()
        {
            using (SqliteConnection conn = new SqliteConnection(db_))
            {
                conn.Open();
                string sql = @"
                CREATE TABLE IF NOT EXISTS Kategoriler (Id INTEGER PRIMARY KEY AUTOINCREMENT, Ad TEXT);
                CREATE TABLE IF NOT EXISTS Urunler (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT, 
                    Ad TEXT, 
                    Aciklama TEXT, 
                    Fiyat REAL, 
                    Stok INTEGER, 
                    SatilanAdet INTEGER DEFAULT 0,
                    KategoriId INTEGER, 
                    ResimUrl TEXT,
                    FOREIGN KEY (KategoriId) REFERENCES Kategoriler(Id)
                );";
                using (var cmd = new SqliteCommand(sql, conn)) { cmd.ExecuteNonQuery(); }
                try
                {
                    using (var cmd = new SqliteCommand("ALTER TABLE Urunler ADD COLUMN SatilanAdet INTEGER DEFAULT 0", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                catch { }
            }
        }

        [HttpGet("Kategoriler")]
        public IActionResult GetKategoriler()
        {
            var liste = new List<object>();
            using (SqliteConnection conn = new SqliteConnection(db_))
            {
                conn.Open();
                using (SqliteCommand cmd = new SqliteCommand("Select * From Kategoriler", conn))
                using (SqliteDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read()) liste.Add(new { id = rd[0], ad = rd[1] });
                }
            }
            return Ok(liste);
        }

        [HttpGet("UrunlerGetir")]
        public IActionResult GetUrunler()
        {
            var liste = new List<object>();
            using (SqliteConnection conn = new SqliteConnection(db_))
            {
                conn.Open();
                string sql = "SELECT u.Id, u.Ad, u.Aciklama, u.Fiyat, u.Stok, k.Ad, u.ResimUrl FROM Urunler u JOIN Kategoriler k ON u.KategoriId = k.Id";
                using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                using (SqliteDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read()) liste.Add(new { id = rd[0], ad = rd[1], aciklama = rd[2], fiyat = rd[3], stok = rd[4], kategori = rd[5], resimUrl = rd[6] });
                }
            }
            return Ok(liste);
        }

        [HttpGet("KategoriGetir")]
        public IActionResult GetUrunlerByKategori(int kategoriId)
        {
            var liste = new List<object>();
            using (SqliteConnection conn = new SqliteConnection(db_))
            {
                conn.Open();
                string sql = "SELECT u.Id, u.Ad, u.Aciklama, u.Fiyat, u.Stok, k.Ad, u.ResimUrl FROM Urunler u JOIN Kategoriler k ON u.KategoriId = k.Id WHERE u.KategoriId = @kid";
                using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@kid", kategoriId);
                    using (SqliteDataReader rd = cmd.ExecuteReader())
                    {
                        while (rd.Read()) liste.Add(new { id = rd[0], ad = rd[1], aciklama = rd[2], fiyat = rd[3], stok = rd[4], kategori = rd[5], resimUrl = rd[6] });
                    }
                }
            }
            return Ok(liste);
        }

        [HttpPost("UrunEkle")]
        public IActionResult UrunEkle(string ad, string? aciklama, string fiyat, string stok, string KategoriId, string? ResimUrl)
        {
            using (SqliteConnection conn = new SqliteConnection(db_))
            {
                conn.Open();
                string sql = "INSERT INTO Urunler (Ad, Aciklama, Fiyat, Stok, KategoriId, ResimUrl, SatilanAdet) VALUES (@ad, @aciklama, @fiyat, @stok, @KategoriId, @ResimUrl, 0)";
                using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@ad", (object)ad ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@aciklama", (object)aciklama ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@fiyat", (object)fiyat ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@stok", (object)stok ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@KategoriId", (object)KategoriId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ResimUrl", (object)ResimUrl ?? DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
            return Ok("başarılı");
        }

        [HttpPost("SatisYap/{id}")]
        public IActionResult SatisYap(int id)
        {
            using (SqliteConnection conn = new SqliteConnection(db_))
            {
                conn.Open();
                string sql = "UPDATE Urunler SET Stok = Stok - 1, SatilanAdet = SatilanAdet + 1 WHERE Id = @id AND Stok > 0";
                using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    if (cmd.ExecuteNonQuery() == 0) return BadRequest("Stok yok!");
                }
            }
            return Ok("başarılı");
        }

        [HttpGet("Istatistikler")]
        public IActionResult GetIstatistikler()
        {
            using (SqliteConnection conn = new SqliteConnection(db_))
            {
                conn.Open();
                string sql = "SELECT COUNT(Id), IFNULL(SUM(Stok), 0), IFNULL(SUM(SatilanAdet * Fiyat), 0) FROM Urunler";
                using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                using (SqliteDataReader rd = cmd.ExecuteReader())
                {
                    if (rd.Read()) return Ok(new { toplamUrun = rd[0], toplamStok = rd[1], toplamSatis = rd[2] });
                }
            }
            return Ok(new { toplamUrun = 0, toplamStok = 0, toplamSatis = 0 });
        }

        [HttpPost("Kategori_Ekle")]
        public IActionResult KategoriEkle(string Ad)
        {
            using (SqliteConnection conn = new SqliteConnection(db_))
            {
                conn.Open();
                using (SqliteCommand cmd = new SqliteCommand("Insert Into Kategoriler (Ad) Values (@ad)", conn))
                {
                    cmd.Parameters.AddWithValue("@ad", Ad);
                    cmd.ExecuteNonQuery();
                }
            }
            return Ok("başarılı");
        }

        [HttpPut("UrunGuncelle")]
        public IActionResult UrunGuncelle(int id, double yeniFiyat, int yeniStok)
        {
            using (SqliteConnection conn = new SqliteConnection(db_))
            {
                conn.Open();
                using (SqliteCommand cmd = new SqliteCommand("UPDATE Urunler SET Fiyat = @f, Stok = @s WHERE Id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@f", yeniFiyat);
                    cmd.Parameters.AddWithValue("@s", yeniStok);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
            return Ok("başarılı");
        }
        [HttpPost("UrunEkle")]
        public async Task<IActionResult> UrunEkle([FromForm] string ad, [FromForm] string? aciklama, [FromForm] double fiyat, [FromForm] int stok, [FromForm] int KategoriId, IFormFile? resimDosyası)
        {
            string dosyaYolu = "";

            if (resimDosyası != null && resimDosyası.Length > 0)
            {
                var klasörYolu = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(klasörYolu)) Directory.CreateDirectory(klasörYolu);

                var dosyaAdi = Guid.NewGuid().ToString() + Path.GetExtension(resimDosyası.FileName);
                var tamYol = Path.Combine(klasörYolu, dosyaAdi);

                using (var stream = new FileStream(tamYol, FileMode.Create))
                {
                    await resimDosyası.CopyToAsync(stream);
                }
                dosyaYolu = "https://localhost:7130/uploads/" + dosyaAdi; 
            }

  
            using (SqliteConnection conn = new SqliteConnection(db_))
            {
                conn.Open();
                string sql = "INSERT INTO Urunler (Ad, Aciklama, Fiyat, Stok, KategoriId, ResimUrl, SatilanAdet) VALUES (@ad, @ac, @f, @s, @k, @r, 0)";
                using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@ad", ad);
                    cmd.Parameters.AddWithValue("@ac", aciklama ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@f", fiyat);
                    cmd.Parameters.AddWithValue("@s", stok);
                    cmd.Parameters.AddWithValue("@k", KategoriId);
                    cmd.Parameters.AddWithValue("@r", dosyaYolu);
                    cmd.ExecuteNonQuery();
                }
            }
            return Ok("Ürün ve Resim Başarıyla Eklendi!");
        }

        [HttpPost("GirisYap")]
        public IActionResult Login([FromForm] string kullanıcıAdı, [FromForm] string sifre)
        {

            if (kullanıcıAdı == "admin" && sifre == "tulay.")
            {
                return Ok(new { mesaj = "Hoş geldin", yetki = "admin" });
            }
            return BadRequest("Kullanıcı adı veya şifre yanlış!");
        }

        [HttpDelete("UrunSil")]
        public IActionResult UrunSil(int id)
        {
            using (SqliteConnection conn = new SqliteConnection(db_))
            {
                conn.Open();
                using (SqliteCommand cmd = new SqliteCommand("Delete From Urunler Where Id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    if (cmd.ExecuteNonQuery() == 0) return NotFound();
                }
            }
            return Ok("başarılı");
        }
    }
}