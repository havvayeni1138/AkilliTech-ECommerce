using System.ComponentModel.DataAnnotations;

namespace EticaretUygulamasi.Models
{
    public class Urun
    {
        [Key]
        public int UrunId { get; set; }

        [Required(ErrorMessage = "Ürün adı zorunludur.")]
        [StringLength(100, ErrorMessage = "Ürün adı en fazla 100 karakter olabilir.")]
        public string? UrunAdi { get; set; }

        [Required(ErrorMessage = "Fiyat zorunludur.")]
        [Range(0, double.MaxValue, ErrorMessage = "Fiyat 0'dan büyük olmalıdır.")]
        public decimal Fiyat { get; set; }

        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        public string? Aciklama { get; set; }

        public string? ResimUrl { get; set; }

        public int StokMiktari { get; set; }

        public bool AktifMi { get; set; } = true;

        // Kategori ilişkisi
        [Required(ErrorMessage = "Kategori seçimi zorunludur.")]
        public int KategoriId { get; set; }

        public Kategori? Kategori { get; set; }
    }
} 