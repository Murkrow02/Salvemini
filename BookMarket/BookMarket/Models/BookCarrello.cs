using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BookMarket.Models
{
    public partial class BookCarrello
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? IdUtente { get; set; }
        
        [Required]
        public int IdLibro { get; set; }

        [ForeignKey("IdLibro")]
        public virtual BookLibri Libro { get; set; }

        [ForeignKey("IdUtente")]
        public virtual BookUtenti Utente { get; set; }
    }
}
