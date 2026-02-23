using Microsoft.EntityFrameworkCore;
using EticaretUygulamasi.Data;
using EticaretUygulamasi.Models;
using Microsoft.AspNetCore.Http.Features;
using BCrypt.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

// Veritabanı bağlantısı
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Session kullanımı için
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = ".EticaretUygulamasi.Session";
});

// Maksimum dosya boyutu ayarı
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024; // 10MB
});

var app = builder.Build();

// SEED DATA
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();

    // Admin kullanıcı oluştur
    if (!context.Kullanicilar.Any(k => k.Email == "admin@example.com"))
    {
        var adminKullanici = new Kullanici
        {
            Ad = "Admin",
            Soyad = "Kullanıcı",
            Email = "admin@example.com",
            Sifre = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
            Adres = "Admin Adresi",
            Telefon = "5555555555",
            AdminMi = true
        };
        context.Kullanicilar.Add(adminKullanici);
        context.SaveChanges();
    }

    // if (!context.Urunler.Any()) // Bu satırı yorum satırı yapıyoruz veya siliyoruz
    {
        // Kategorileri ekle (varsa tekrar eklenmez, hata vermez)
        if (!context.Kategoriler.Any())
        {
            var kategoriler = new List<Kategori>
            {
                new Kategori { KategoriAdi = "Bilgisayar & Donanım", Aciklama = "Laptop, SSD, Ekran Kartı, Monitör ve İşlemci ürünleri" },
                new Kategori { KategoriAdi = "Telefon & Aksesuarlar", Aciklama = "Akıllı Telefon, Powerbank, Kulaklık ve Akıllı Saat ürünleri" },
                new Kategori { KategoriAdi = "Oyun & Konsol", Aciklama = "Oyun Konsolu, Kulaklık, Klavye ve Oyun Koltuğu ürünleri" },
                new Kategori { KategoriAdi = "Elektronik Aletler", Aciklama = "Akıllı TV, Android TV Box, Robot Süpürge ve Drone ürünleri" },
                new Kategori { KategoriAdi = "Fotoğraf & Video", Aciklama = "DSLR Kamera, Aynasız Kamera, Aksiyon Kamerası ve Tripod ürünleri" },
            };

            // Mevcut kategorileri temizle ve yeni kategorileri ekle (Geçici çözüm için)
            // context.Kategoriler.RemoveRange(context.Kategoriler);
            // context.SaveChanges();

            context.Kategoriler.AddRange(kategoriler);
            context.SaveChanges();

            // Kategorileri tekrar çek (ID'ler için)
            var kategorilerDb = context.Kategoriler.ToList();

            // Ürünleri temizle ve yeniden ekle (Geçici çözüm için)
            // context.Urunler.RemoveRange(context.Urunler);
            // context.SaveChanges();

            // Ürünleri ekle
            var urunler = new List<Urun>
            {
                // Bilgisayar & Donanım
                new Urun { UrunAdi = "Lenovo IdeaPad 3 15ADA6 AMD Ryzen 5", Fiyat = 12999, Aciklama = "15.6 inç, AMD Ryzen 5, 8GB RAM, 256GB SSD", ResimUrl = "/images/leno.jpg", StokMiktari = 10, KategoriId = kategorilerDb.First(k => k.KategoriAdi == "Bilgisayar & Donanım").KategoriId },
                new Urun { UrunAdi = "Asus TUF Gaming F15 FX506HF", Fiyat = 15999, Aciklama = "15.6 inç, Intel Core i5, 8GB RAM, 512GB SSD", ResimUrl = "/images/as.webp", StokMiktari = 12, KategoriId = kategorilerDb.First(k => k.KategoriAdi == "Bilgisayar & Donanım").KategoriId },
                new Urun { UrunAdi = "HP Victus 15-fa0003NT", Fiyat = 14999, Aciklama = "15.6 inç, AMD Ryzen 5, 16GB RAM, 512GB SSD", ResimUrl = "/images/hp.webp", StokMiktari = 8, KategoriId = kategorilerDb.First(k => k.KategoriAdi == "Bilgisayar & Donanım").KategoriId },
                new Urun { UrunAdi = "MSI GF63 Thin 11SC", Fiyat = 16999, Aciklama = "15.6 inç, Intel Core i5, 8GB RAM, 512GB SSD", ResimUrl = "/images/msı.jpg", StokMiktari = 15, KategoriId = kategorilerDb.First(k => k.KategoriAdi == "Bilgisayar & Donanım").KategoriId },
                new Urun { UrunAdi = "Apple MacBook Air M2", Fiyat = 42999, Aciklama = "13.6 inç Liquid Retina Display, M2 Chip, 8GB RAM, 256GB SSD", ResimUrl = "/images/ap.jpg", StokMiktari = 20, KategoriId = kategorilerDb.First(k => k.KategoriAdi == "Bilgisayar & Donanım").KategoriId },
                new Urun { UrunAdi = "Kingston 16GB DDR4 3200MHz RAM", Fiyat = 999, Aciklama = "16GB DDR4, 3200MHz", ResimUrl = "/images/ki.webp",  StokMiktari = 50, KategoriId = kategorilerDb.First(k => k.KategoriAdi == "Bilgisayar & Donanım").KategoriId },
                new Urun { UrunAdi = "Samsung 980 PRO 1TB NVMe SSD", Fiyat = 3999, Aciklama = "1TB NVMe SSD, 7000MB/s Okuma, 5000MB/s Yazma", ResimUrl = "/images/sa.jpg", StokMiktari = 30, KategoriId = kategorilerDb.First(k => k.KategoriAdi == "Bilgisayar & Donanım").KategoriId },
                new Urun { UrunAdi = "Intel Core i7-12700K İşlemci", Fiyat = 8999, Aciklama = "Intel Core i7-12700K, 12 Çekirdek, 3.6GHz", ResimUrl = "/images/in.jpg", StokMiktari = 25, KategoriId = kategorilerDb.First(k => k.KategoriAdi == "Bilgisayar & Donanım").KategoriId },
                new Urun { UrunAdi = "NVIDIA GeForce RTX 4060 Ekran Kartı", Fiyat = 12999, Aciklama = "8GB GDDR6, Ray Tracing, DLSS 3.0", ResimUrl = "/images/nv.jpg", StokMiktari = 20, KategoriId = kategorilerDb.First(k => k.KategoriAdi == "Bilgisayar & Donanım").KategoriId },
                new Urun { UrunAdi = "ASUS Prime B660M-A Anakart", Fiyat = 3999, Aciklama = "Intel B660, DDR4, PCIe 4.0", ResimUrl = "/images/asu.jpg", StokMiktari = 15, KategoriId = kategorilerDb.First(k => k.KategoriAdi == "Bilgisayar & Donanım").KategoriId },

                // Telefon & Aksesuarlar
                new Urun { UrunAdi = "iPhone 15 Pro Max 256GB", Fiyat = 59999, Aciklama = "6.7 inç Super Retina XDR, A17 Pro, 256GB", ResimUrl = "/images/ap1.jpg", StokMiktari = 30, KategoriId = kategorilerDb.First(k => k.KategoriAdi == "Telefon & Aksesuarlar").KategoriId },
                new Urun { UrunAdi = "Samsung Galaxy S24 Ultra", Fiyat = 54999, Aciklama = "6.8 inç Dynamic AMOLED, Snapdragon 8 Gen 3, 256GB", ResimUrl = "/images/sam.webp", StokMiktari = 25, KategoriId = kategorilerDb.First(k => k.KategoriAdi == "Telefon & Aksesuarlar").KategoriId },
                new Urun { UrunAdi = "Xiaomi Redmi Note 13 Pro", Fiyat = 12999, Aciklama = "6.67 inç AMOLED, MediaTek Dimensity 7200, 128GB", ResimUrl = "/images/xi.jpg", StokMiktari = 40, KategoriId = kategorilerDb.First(k => k.KategoriAdi == "Telefon & Aksesuarlar").KategoriId },
                new Urun { UrunAdi = "Apple AirPods Pro 2. Nesil", Fiyat = 6999, Aciklama = "Aktif Gürültü Engelleme, Uzamsal Ses, USB-C", ResimUrl = "/images/air.jpg", StokMiktari = 60, KategoriId = kategorilerDb.First(k => k.KategoriAdi == "Telefon & Aksesuarlar").KategoriId },
                new Urun { UrunAdi = "Baseus 20W USB-C Hızlı Şarj Cihazı", Fiyat = 299, Aciklama = "20W Hızlı Şarj, USB-C", ResimUrl = "/images/şa.jpg", StokMiktari = 100, KategoriId = kategorilerDb.First(k => k.KategoriAdi == "Telefon & Aksesuarlar").KategoriId },
                new Urun { UrunAdi = "Spigen iPhone 15 Pro Kılıf", Fiyat = 399, Aciklama = "Koruyucu Kılıf, iPhone 15 Pro", ResimUrl = "/images/kı.jpg", StokMiktari = 80, KategoriId = kategorilerDb.First(k => k.KategoriAdi == "Telefon & Aksesuarlar").KategoriId },
                new Urun { UrunAdi = "Samsung Galaxy Buds2 Pro", Fiyat = 4999, Aciklama = "Aktif Gürültü Engelleme, Kablosuz Kulaklık", ResimUrl = "/images/Sam.jpg", StokMiktari = 50, KategoriId = kategorilerDb.First(k => k.KategoriAdi == "Telefon & Aksesuarlar").KategoriId },
                new Urun { UrunAdi = "Xiaomi 10000 mAh Powerbank", Fiyat = 999, Aciklama = "10000mAh, 22.5W Hızlı Şarj", ResimUrl = "/images/Xii.jpg", StokMiktari = 70, KategoriId = kategorilerDb.First(k => k.KategoriAdi == "Telefon & Aksesuarlar").KategoriId },
                new Urun { UrunAdi = "Baseus Araç Telefon Tutucu", Fiyat = 199, Aciklama = "Araç Telefon Tutucu, Evrensel", ResimUrl = "/images/arac.jpg", StokMiktari = 120, KategoriId = kategorilerDb.First(k => k.KategoriAdi == "Telefon & Aksesuarlar").KategoriId },
                new Urun { UrunAdi = "Ugreen USB-C to HDMI Dönüştürücü", Fiyat = 299, Aciklama = "USB-C to HDMI, 4K@60Hz", ResimUrl = "/ images/ugr.jpg", StokMiktari = 90, KategoriId = kategorilerDb.First(k => k.KategoriAdi == "Telefon & Aksesuarlar").KategoriId },

                // Oyun & Konsol
                new Urun { UrunAdi = "Sony PlayStation 5 (Disk Sürüm", Fiyat = 19999, Aciklama = "825GB SSD, DualSense Controller, 4K UHD Blu-ray", ResimUrl = "/images/sonyy.jpg", StokMiktari = 25, KategoriId = kategorilerDb.First(k => k.KategoriAdi == "Oyun & Konsol").KategoriId },
                new Urun { UrunAdi = "Microsoft Xbox Series X", Fiyat = 18999, Aciklama = "1TB SSD, 4K UHD Blu-ray, 120fps", ResimUrl = "/images/xbo.jpg", StokMiktari = 20, KategoriId = kategorilerDb.First(k => k.KategoriAdi == "Oyun & Konsol").KategoriId },
                new Urun { UrunAdi = "Nintendo Switch OLED", Fiyat = 9999, Aciklama = "7 inç OLED Ekran, 64GB Depolama", ResimUrl = "/images/nine.jpg", StokMiktari = 30, KategoriId = kategorilerDb.First(k => k.KategoriAdi == "Oyun & Konsol").KategoriId },
                new Urun { UrunAdi = "Logitech G29 Direksiyon Seti", Fiyat = 4999, Aciklama = "Direksiyon Seti, Pedal, PS4/PS5/PC", ResimUrl = "/images/logi.jpg", StokMiktari = 15, KategoriId = kategorilerDb.First(k => k.KategoriAdi == "Oyun & Konsol").KategoriId },
                new Urun { UrunAdi = "DualSense PS5 Kablosuz Kumanda", Fiyat = 1999, Aciklama = "Kablosuz Kumanda, PS5", ResimUrl = "/images/dua.jpg", StokMiktari = 40, KategoriId = kategorilerDb.First(k => k.KategoriAdi == "Oyun & Konsol").KategoriId },
                new Urun { UrunAdi = "Xbox Kablosuz Kumanda (Robot White)", Fiyat = 1999, Aciklama = "Kablosuz Kumanda, Xbox Series X/S", ResimUrl = "/images/robot.jpg", StokMiktari = 35, KategoriId = kategorilerDb.First(k => k.KategoriAdi == "Oyun & Konsol").KategoriId },
                new Urun { UrunAdi = "FIFA 24 PS5", Fiyat = 999, Aciklama = "FIFA 24, PS5", ResimUrl = "/images/fif.jpg", StokMiktari = 50, KategoriId = kategorilerDb.First(k => k.KategoriAdi == "Oyun & Konsol").KategoriId },
                new Urun { UrunAdi = "Red Dead Redemption 2 Xbox", Fiyat = 999, Aciklama = "Red Dead Redemption 2, Xbox", ResimUrl = "/images/red.jpg", StokMiktari = 45, KategoriId = kategorilerDb.First(k => k.KategoriAdi == "Oyun & Konsol").KategoriId },
                new Urun { UrunAdi = "The Legend of Zelda: Tears of the Kingdom (Switch)", Fiyat = 999, Aciklama = "The Legend of Zelda: Tears of the Kingdom, Switch", ResimUrl = "/images/rt.jpg", StokMiktari = 40, KategoriId = kategorilerDb.First(k => k.KategoriAdi == "Oyun & Konsol").KategoriId },
                new Urun { UrunAdi = "PS5 Taşıma Çantası", Fiyat = 499, Aciklama = "Taşıma Çantası, PS5", ResimUrl = "/images/pst.jpg", StokMiktari = 60, KategoriId = kategorilerDb.First(k => k.KategoriAdi == "Oyun & Konsol").KategoriId },

                // Elektronik Aletler
                new Urun { UrunAdi = "Xiaomi Mi Vacuum Mop 2 Pro", Fiyat = 4999, Aciklama = "Robot Süpürge, 3000Pa Emiş, 5200mAh Pil", ResimUrl = "/images/xiaomi.jpg", StokMiktari = 20, KategoriId = kategorilerDb.First(k => k.KategoriAdi == "Elektronik Aletler").KategoriId },
                new Urun { UrunAdi = "Arzum Okka Minio Türk Kahvesi Makinesi", Fiyat = 1999, Aciklama = "Türk Kahvesi Makinesi, 550W", ResimUrl = "/images/ok.jpg", StokMiktari = 30, KategoriId = kategorilerDb.First(k => k.KategoriAdi == "Elektronik Aletler").KategoriId },
                new Urun { UrunAdi = "Tefal Easy Fry Yağsız Fritöz", Fiyat = 2999, Aciklama = "Yağsız Fritöz, 4.2L, 2000W", ResimUrl = "/images/tef.jpg", StokMiktari = 25, KategoriId = kategorilerDb.First(k => k.KategoriAdi == "Elektronik Aletler").KategoriId },
                new Urun { UrunAdi = "Philips 5000 Serisi Buharlı Ütü", Fiyat = 1499, Aciklama = "Buharlı Ütü, 2400W", ResimUrl = "/images/phi.jpg", StokMiktari = 40, KategoriId = kategorilerDb.First(k => k.KategoriAdi == "Elektronik Aletler").KategoriId },
                new Urun { UrunAdi = "Dyson V15 Detect Kablosuz Süpürge", Fiyat = 19999, Aciklama = "Kablosuz Süpürge, 60 dk Pil Ömrü", ResimUrl = "/images/dys.jpg", StokMiktari = 15, KategoriId = kategorilerDb.First(k => k.KategoriAdi == "Elektronik Aletler").KategoriId },
                new Urun { UrunAdi = "Fakir Kaave Türk Kahve Makinesi", Fiyat = 1499, Aciklama = "Türk Kahvesi Makinesi, 550W", ResimUrl = "/images/fakir.jpg", StokMiktari = 35, KategoriId = kategorilerDb.First(k => k.KategoriAdi == "Elektronik Aletler").KategoriId },
              
                new Urun { UrunAdi = "Anker Nebula Capsule Mini Projeksiyon", Fiyat = 4999, Aciklama = "Mini Projeksiyon, 100 ANSI Lumen", ResimUrl = "/images/anker.webp", StokMiktari = 15, KategoriId = kategorilerDb.First(k => k.KategoriAdi == "Elektronik Aletler").KategoriId },

                // Fotoğraf & Video
                new Urun { UrunAdi = "Canon EOS 250D DSLR Kamera", Fiyat = 14999, Aciklama = "24.1MP APS-C Sensör, 4K Video", ResimUrl = "/images/can.jpg", StokMiktari = 20, KategoriId = kategorilerDb.First(k => k.KategoriAdi == "Fotoğraf & Video").KategoriId },
                new Urun { UrunAdi = "Sony ZV-E10 Vlog Kamera", Fiyat = 19999, Aciklama = "24.2MP APS-C Sensör, 4K Video", ResimUrl = "/images/soni.jpg", StokMiktari = 15, KategoriId = kategorilerDb.First(k => k.KategoriAdi == "Fotoğraf & Video").KategoriId },
                new Urun { UrunAdi = "GoPro HERO12 Black", Fiyat = 8999, Aciklama = "5.3K/60fps Video, 27MP Fotoğraf", ResimUrl = "/images/go.jpg", StokMiktari = 30, KategoriId = kategorilerDb.First(k => k.KategoriAdi == "Fotoğraf & Video").KategoriId },
                new Urun { UrunAdi = "DJI Osmo Pocket 3", Fiyat = 14999, Aciklama = "4K/60fps Video, 3-Akslı Stabilizasyon", ResimUrl = "/images/dj.jpg", StokMiktari = 25, KategoriId = kategorilerDb.First(k => k.KategoriAdi == "Fotoğraf & Video").KategoriId },
               
                new Urun { UrunAdi = "Manfrotto Tripod MKCOMPACTLT", Fiyat = 2999, Aciklama = "Alüminyum Tripod, 4kg Yük Kapasitesi", ResimUrl = "/images/man.jpg", StokMiktari = 25, KategoriId = kategorilerDb.First(k => k.KategoriAdi == "Fotoğraf & Video").KategoriId },
                new Urun { UrunAdi = "Sandisk Extreme PRO SDXC 128GB Hafıza Kartı", Fiyat = 999, Aciklama = "128GB, 170MB/s Okuma, 90MB/s Yazma", ResimUrl = "/images/sand.jpg", StokMiktari = 50, KategoriId = kategorilerDb.First(k => k.KategoriAdi == "Fotoğraf & Video").KategoriId },
               
            };
            context.Urunler.AddRange(urunler);
            context.SaveChanges();
        }
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession(); 
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
