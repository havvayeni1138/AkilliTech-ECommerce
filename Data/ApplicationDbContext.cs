using Microsoft.EntityFrameworkCore;
using EticaretUygulamasi.Models;

namespace EticaretUygulamasi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Urun> Urunler { get; set; }
        public DbSet<Kullanici> Kullanicilar { get; set; }
        public DbSet<Sepet> Sepetler { get; set; }
        public DbSet<Kategori> Kategoriler { get; set; }
        public DbSet<Siparis> Siparisler { get; set; }
        public DbSet<SiparisDetay> SiparisDetaylar { get; set; }
        public DbSet<Kargo> Kargolar { get; set; }
        public DbSet<KargoDetay> KargoDetaylar { get; set; }
        public DbSet<SepetDetay> SepetDetaylar { get; set; }
        public DbSet<SepetUrunu> SepetUrunleri { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Decimal alanlar için hassasiyet ayarları
            modelBuilder.Entity<Siparis>()
                .Property(s => s.ToplamTutar)
                .HasPrecision(18, 2);

            modelBuilder.Entity<SiparisDetay>()
                .Property(sd => sd.BirimFiyat)
                .HasPrecision(18, 2);

            modelBuilder.Entity<SiparisDetay>()
                .Property(sd => sd.ToplamFiyat)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Urun>()
                .Property(u => u.Fiyat)
                .HasPrecision(18, 2);

            // İlişkileri tanımlama
            modelBuilder.Entity<Sepet>()
                .HasOne(s => s.Kullanici)
                .WithMany()
                .HasForeignKey(s => s.KullaniciId);

            modelBuilder.Entity<Urun>()
                .HasOne(u => u.Kategori)
                .WithMany(k => k.Urunler)
                .HasForeignKey(u => u.KategoriId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Siparis>()
                .HasOne(s => s.Kullanici)
                .WithMany()
                .HasForeignKey(s => s.KullaniciId);

            modelBuilder.Entity<SiparisDetay>()
                .HasOne(sd => sd.Siparis)
                .WithMany(s => s.SiparisDetaylar)
                .HasForeignKey(sd => sd.SiparisId);

            modelBuilder.Entity<SiparisDetay>()
                .HasOne(sd => sd.Urun)
                .WithMany()
                .HasForeignKey(sd => sd.UrunId);

            modelBuilder.Entity<Kargo>()
                .HasOne(k => k.Siparis)
                .WithMany()
                .HasForeignKey(k => k.SiparisId);

            modelBuilder.Entity<KargoDetay>()
                .HasOne(kd => kd.Kargo)
                .WithMany(k => k.KargoDetaylar)
                .HasForeignKey(kd => kd.KargoId);

            modelBuilder.Entity<KargoDetay>()
                .HasOne(kd => kd.Urun)
                .WithMany()
                .HasForeignKey(kd => kd.UrunId);

            // SepetDetay için ilişki tanımlamaları
            modelBuilder.Entity<SepetDetay>()
                .HasOne(sd => sd.Sepet)
                .WithMany(s => s.SepetDetaylar)
                .HasForeignKey(sd => sd.SepetId);

            modelBuilder.Entity<SepetDetay>()
                .HasOne(sd => sd.Urun)
                .WithMany()
                .HasForeignKey(sd => sd.UrunId);

            // Kullanıcı
            modelBuilder.Entity<Kullanici>()
                .HasIndex(k => k.Email)
                .IsUnique();

            // Kategori
            modelBuilder.Entity<Kategori>()
                .HasIndex(k => k.KategoriAdi)
                .IsUnique();

            // SepetUrunu
            modelBuilder.Entity<SepetUrunu>()
                .HasOne(su => su.Urun)
                .WithMany()
                .HasForeignKey(su => su.UrunId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
} 