using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchMaster
{
    [Table("Matches")]
    public class Match 
    {
        //private ICollection<Shooter> _match_schooters;

        [Key , DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MatchID { get; set; }

        [Required]
        public string Title { get; set; }

        [Display(Name = "End")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "Start")]
        public DateTime? EndDate { get; set; }

        public ICollection<Shooter> MatchShooters { get; set; }

        public Match()
        {
            this.MatchShooters = new List<Shooter>();
            this.StartDate = DateTime.Today;
            this.EndDate = DateTime.Today;
        }

        public override string ToString()
        {
            return string.Format("{0} ({1}-{2})",this.Title, this.StartDate?.ToString("d"), this.EndDate?.ToString("d"));
        }
    }
}
