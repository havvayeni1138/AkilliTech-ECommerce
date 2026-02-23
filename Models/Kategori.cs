using System.ComponentModel.DataAnnotations;

namespace EticaretUygulamasi.Models
{
    public class Kategori
    {
        [Key]
        public int KategoriId { get; set; }

        [Required(ErrorMessage = "Kategori adı zorunludur.")]
        [StringLength(100, ErrorMessage = "Kategori adı en fazla 100 karakter olabilir.")]
        public string? KategoriAdi { get; set; }

        public string? Aciklama { get; set; }

        public bool AktifMi { get; set; } = true;

        // Navigation property
        public ICollection<Urun>? Urunler { get; set; } = new List<Urun>();
    }
} 