using System.ComponentModel.DataAnnotations;

namespace EticaretUygulamasi.Models
{
    public class Siparis
    {
        [Key]
        public int SiparisId { get; set; }

        public int KullaniciId { get; set; }
        public Kullanici? Kullanici { get; set; }

        [Required]
        public DateTime SiparisTarihi { get; set; }

        public decimal ToplamTutar { get; set; }

        public string? SiparisDurumu { get; set; } // Beklemede, Onaylandı, Kargoda, Tamamlandı

        [Required(ErrorMessage = "Adres alanı boş bırakılamaz")]
        public string? Adres { get; set; }

        [Required(ErrorMessage = "Telefon alanı boş bırakılamaz")]
        public string? Telefon { get; set; }


        public string? TakipNumarasi { get; set; } // Kargo takip numarası

        // Navigation property
        public ICollection<SiparisDetay>? SiparisDetaylar { get; set; } = new List<SiparisDetay>();
    }
} 