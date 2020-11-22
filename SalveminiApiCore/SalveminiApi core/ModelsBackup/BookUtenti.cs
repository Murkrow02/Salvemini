using System;
using System.Collections.Generic;

namespace SalveminiApi_core.Models
{
    public partial class BookUtenti
    {
        public BookUtenti()
        {
            BookCarrello = new HashSet<BookCarrello>();
            BookLibriCompratoDaNavigation = new HashSet<BookLibri>();
            BookLibriIdUtenteNavigation = new HashSet<BookLibri>();
        }

        public int Id { get; set; }
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public string Mail { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        public string Ip { get; set; }
        public DateTime? AppuntamentoRilascio { get; set; }
        public DateTime? AppuntamentoRitiro { get; set; }
        public string MailToken { get; set; }
        public DateTime? LastMailSent { get; set; }
        public DateTime? AppuntamentoFinale { get; set; }

        public virtual ICollection<BookCarrello> BookCarrello { get; set; }
        public virtual ICollection<BookLibri> BookLibriCompratoDaNavigation { get; set; }
        public virtual ICollection<BookLibri> BookLibriIdUtenteNavigation { get; set; }
    }
}
