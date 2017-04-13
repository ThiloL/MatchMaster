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
        [Key , DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MatchID { get; set; }

        public string Title { get; set; }

        [Display(Name = "End")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "Start")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Number of Stages")]
        public int NumberOfStages { get; set; }

        [Display(Name = "Number of Posses")]
        public int NumberOfPosses { get; set; }

        public virtual ICollection<MatchParticipation> MatchParticipations { get; set; }

        public Match()
        {
            this.MatchParticipations = new List<MatchParticipation>();
            this.StartDate = DateTime.Today;
            this.EndDate = DateTime.Today;
        }

        public override string ToString()
        {
            return string.Format("{0} ({1}-{2})",this.Title, this.StartDate?.ToString("d"), this.EndDate?.ToString("d"));
        }
    }
}
