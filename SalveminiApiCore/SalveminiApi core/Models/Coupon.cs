using System;
using System.Collections.Generic;

namespace SalveminiApi_core.Models
{
    public partial class Coupon
    {
        public Coupon()
        {
            CouponAttivi = new HashSet<CouponAttivi>();
        }

        public int Codice { get; set; }
        public decimal? XAttivazione { get; set; }
        public decimal? YAttivazione { get; set; }
        public decimal? Raggio { get; set; }
        public int IdCreatore { get; set; }
        public int Valore { get; set; }
        public DateTime Creazione { get; set; }
        public string Nome { get; set; }
        public bool? Attivo { get; set; }

        public virtual Utenti IdCreatoreNavigation { get; set; }
        public virtual ICollection<CouponAttivi> CouponAttivi { get; set; }
    }
}
