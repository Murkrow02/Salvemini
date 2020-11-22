using System;
using System.Collections.Generic;

namespace SalveminiApi_core.Models
{
    public partial class Utenti
    {
        public Utenti()
        {
            Avvisi = new HashSet<Avvisi>();
            CoinGuadagnate = new HashSet<CoinGuadagnate>();
            CoinSpese = new HashSet<CoinSpese>();
            Commenti = new HashSet<Commenti>();
            Coupon = new HashSet<Coupon>();
            CouponAttivi = new HashSet<CouponAttivi>();
            Notifiche = new HashSet<Notifiche>();
            VotiSondaggi = new HashSet<VotiSondaggi>();
        }

        public int Id { get; set; }
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public string Sesso { get; set; }
        public int Classe { get; set; }
        public string Corso { get; set; }
        public int Stato { get; set; }
        public DateTime Creazione {internal get; set; }
        public int SCoin { get; set; }
        public int AdsWatched {internal get; set; }
        public DateTime? LastAdWatched {internal get; set; }
        public string Immagine { get; set; }
        public string ArgoToken {internal get; set; }

        public virtual FlappyClassifica FlappyClassifica { internal get; set; }
        public virtual ICollection<Avvisi> Avvisi { internal get; set; }
        public virtual ICollection<CoinGuadagnate> CoinGuadagnate { internal get; set; }
        public virtual ICollection<CoinSpese> CoinSpese { internal get; set; }
        public virtual ICollection<Commenti> Commenti { internal get; set; }
        public virtual ICollection<Coupon> Coupon { internal get; set; }
        public virtual ICollection<CouponAttivi> CouponAttivi { internal get; set; }
        public virtual ICollection<Notifiche> Notifiche { internal get; set; }
        public virtual ICollection<VotiSondaggi> VotiSondaggi { internal get; set; }
        public virtual ICollection<Sondaggi> Sondaggi { internal get; set; }

    }


    public class AuthUser
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public string Sesso { get; set; }
        public int Classe { get; set; }
        public string Corso { get; set; }
        public int Stato { get; set; }
        public DateTime Creazione { internal get; set; }
        public int SCoin { get; set; }
        public int AdsWatched { internal get; set; }
        public DateTime? LastAdWatched { internal get; set; }
        public string Immagine { get; set; }
        public string ArgoToken {  get; set; }
    }
}