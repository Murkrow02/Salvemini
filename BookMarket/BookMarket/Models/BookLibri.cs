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
        public int IdProprietario { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? Prezzo { get; set; }
        public int? IdAcquirente { get; set; }
        [Required]
        public DateTime DataCaricamento { get; set; }
        public bool? Venduto { get; set; }

        [ForeignKey("IdProprietario")]
        public virtual BookUtenti Acquirente { get; set; }

        [ForeignKey("IdUtente")]
        public virtual BookUtenti Proprietario { get; set; }
        public virtual IEnumerable<BookCarrello> BookCarrello { get; set; }
    }
}
