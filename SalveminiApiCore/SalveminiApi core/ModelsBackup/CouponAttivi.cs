using System;
using System.Collections.Generic;

namespace SalveminiApi_core.Models
{
    public partial class CouponAttivi
    {
        public int Id { get; set; }
        public int IdUtente { get; set; }
        public int IdCoupon { get; set; }

        public virtual Coupon IdCouponNavigation { get; set; }
        public virtual Utenti IdUtenteNavigation { get; set; }
    }
}
