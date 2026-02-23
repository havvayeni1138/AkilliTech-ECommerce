using System.ComponentModel.DataAnnotations;

namespace EticaretUygulamasi.Models
{
    public class SepetDetay
    {
        [Key]
        public int SepetDetayId { get; set; }

        public int SepetId { get; set; }
        public Sepet? Sepet { get; set; }

        public int UrunId { get; set; }
        public Urun? Urun { get; set; }

        public int Miktar { get; set; }
    }
} 