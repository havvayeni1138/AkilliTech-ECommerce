using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EticaretUygulamasi.Models
{
    public class SepetUrunu
    {
        [Key]
        public int SepetUrunuId { get; set; }

        [Required]
        public string SessionId { get; set; }

        [Required]
        public int UrunId { get; set; }

        [Required]
        public int Miktar { get; set; }

        [Required]
        public DateTime EklenmeTarihi { get; set; }

        [ForeignKey("UrunId")]
        public Urun Urun { get; set; }
    }
} 