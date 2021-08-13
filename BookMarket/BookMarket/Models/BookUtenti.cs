using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BookMarket.Models
{
    public partial class BookUtenti
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(30), MinLength(2)]
        public string Nome { get; set; }
        [Required]
        [MaxLength(50), MinLength(2)]
        public string Cognome { get; set; }
        [Required]
        [MaxLength(200), MinLength(2)]
        public string Mail { get; set; }
        [Required]
        [MaxLength(2000), MinLength(2)]
        public string Password { get; set; }
        [Required]
        public string Token { get; set; }
        [Required]
        [MaxLength(200), MinLength(2)]
        public string Ip { get; set; }
        public DateTime? AppuntamentoRilascio { get; set; }
        public DateTime? AppuntamentoRitiro { get; set; }
        public string MailToken { get; set; }
        public DateTime? LastMailSent { get; set; }
        public DateTime? AppuntamentoFinale { get; set; }

        public virtual IEnumerable<BookCarrello> BookCarrello { get; set; }
        public virtual IEnumerable<BookLibri> LibriVenduti { get; set; }
        public virtual IEnumerable<BookLibri> LibriRegistrati { get; set; }
    }
}
