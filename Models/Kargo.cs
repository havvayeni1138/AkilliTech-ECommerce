using System.ComponentModel.DataAnnotations;

namespace EticaretUygulamasi.Models
{
    public class Kargo
    {
        [Key]
        public int KargoId { get; set; }

        public int SiparisId { get; set; }
        public Siparis? Siparis { get; set; }

        [Required]
        public string? KargoFirmasi { get; set; }

        [Required]
        public string? TakipNumarasi { get; set; }

        public DateTime GonderimTarihi { get; set; }

        public DateTime? TeslimTarihi { get; set; }

        public string? Durum { get; set; } // Hazırlanıyor, Yolda, Teslim Edildi

        // Navigation property
        public ICollection<KargoDetay>? KargoDetaylar { get; set; } = new List<KargoDetay>();
    }
} 