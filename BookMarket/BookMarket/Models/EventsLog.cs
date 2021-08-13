using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BookMarket.Models
{
    public partial class EventsLog
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(3000), MinLength(2)]
        public string Evento { get; set; }
        [Required]
        public DateTime Data { get; set; }
    }
}
