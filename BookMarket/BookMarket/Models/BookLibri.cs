using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BookMarket.Models
{
    public partial class BookLibri
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(200), MinLength(2)]
        public string Nome { get; set; }
        [Required]
        [MaxLength(13), MinLength(2)]
        public string Codice { get; set; }
        [Required]
        [MaxLength(100), MinLength(2)]
        public string Materia { get; set; }
        public int IdUtente { get; set; }
        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal? Prezzo { get; set; }
        public int? IdAcquirente { get; set; }
        [Required]
        public DateTime DataCaricamento { get; set; }
        public bool? Venduto { get; set; }

        [ForeignKey("IdAcquirente")]
        public virtual BookUtenti Acquirente { get; set; }
        [ForeignKey("IdUtente")]
        public virtual BookUtenti Utente { get; set; }
        public virtual ICollection<BookCarrello> BookCarrello { get; set; }
    }
}
