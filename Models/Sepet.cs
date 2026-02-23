using System.ComponentModel.DataAnnotations;

namespace EticaretUygulamasi.Models
{
    public class Sepet
    {
        [Key]
        public int SepetId { get; set; }

        public int KullaniciId { get; set; }
        public Kullanici? Kullanici { get; set; }

        public ICollection<SepetDetay>? SepetDetaylar { get; set; } = new List<SepetDetay>();
    }
} 