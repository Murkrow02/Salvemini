using System;
using System.Collections.Generic;

namespace SalveminiApi_core.Models
{
    public partial class BookLibri
    {
        public BookLibri()
        {
            BookCarrello = new HashSet<BookCarrello>();
        }

        public int Id { get; set; }
        public string Nome { get; set; }
        public string Codice { get; set; }
        public string Materia { get; set; }
        public int IdUtente { get; set; }
        public decimal? Prezzo { get; set; }
        public int? CompratoDa { get; set; }
        public DateTime DataCaricamento { get; set; }
        public bool? Venduto { get; set; }

        public virtual BookUtenti CompratoDaNavigation { get; set; }
        public virtual BookUtenti IdUtenteNavigation { get; set; }
        public virtual ICollection<BookCarrello> BookCarrello { get; set; }
    }
}
