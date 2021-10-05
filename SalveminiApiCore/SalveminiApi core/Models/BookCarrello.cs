using System;
using System.Collections.Generic;

namespace SalveminiApi_core.Models
{
    public partial class BookCarrello
    {
        public int Id { get; set; }
        public int IdUtente { get; set; }
        public int IdLibro { get; set; }

        public virtual BookLibri IdLibroNavigation { get; set; }
        public virtual BookUtenti IdUtenteNavigation { get; set; }
    }
}
