using System.ComponentModel.DataAnnotations;

namespace EticaretUygulamasi.Models
{
    public class KargoDetay
    {
        [Key]
        public int KargoDetayId { get; set; }

        public int KargoId { get; set; }
        public Kargo? Kargo { get; set; }

        public int UrunId { get; set; }
        public Urun? Urun { get; set; }

        [Required]
        public int Miktar { get; set; }

        public string? Durum { get; set; } // Paketlendi, Yolda, Teslim Edildi
    }
} 