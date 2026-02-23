using System.ComponentModel.DataAnnotations;

namespace EticaretUygulamasi.Models
{
    public class SiparisDetay
    {
        [Key]
        public int SiparisDetayId { get; set; }

        public int SiparisId { get; set; }
        public Siparis? Siparis { get; set; }

        public int UrunId { get; set; }
        public Urun? Urun { get; set; }

        [Required]
        public int Miktar { get; set; }

        public decimal BirimFiyat { get; set; }

        public decimal ToplamFiyat { get; set; }
    }
} 