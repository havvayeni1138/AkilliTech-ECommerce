using System.ComponentModel.DataAnnotations;

namespace EticaretUygulamasi.Models
{
    public class Kullanici
    {
        [Key]
        public int KullaniciId { get; set; }

        [Required(ErrorMessage = "Ad zorunludur.")]
        [StringLength(50, ErrorMessage = "Ad en fazla 50 karakter olabilir.")]
        public string? Ad { get; set; }

        [Required(ErrorMessage = "Soyad zorunludur.")]
        [StringLength(50, ErrorMessage = "Soyad en fazla 50 karakter olabilir.")]
        public string? Soyad { get; set; }

        [Required(ErrorMessage = "E-posta zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
        public string? Sifre { get; set; }

        public string? Telefon { get; set; }

        public string? Adres { get; set; }

        public bool AdminMi { get; set; } = false;
    }
} 