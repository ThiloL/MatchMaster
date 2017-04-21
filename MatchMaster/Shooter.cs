using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MatchMaster
{
    [Table("Shooters")]
    public class Shooter
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ShooterID { get; set; }

        [Display(Name = "Surname"), Required]
        public string Surname { get; set; }

        [Display(Name = "First Name"), Required]
        public string FirstName { get; set; }

        [Display(Name = "Nickname")]
        public string Nickname { get; set; }

        [Display(Name = "Birthday")]
        public DateTime? Birthday { get; set; }

        [Display(Name = "Category")]
        public string Category { get; set; }

        public virtual ICollection<MatchParticipation> MatchParticipations { get; set; }

        public Shooter()
        {
            MatchParticipations = new List<MatchParticipation>();
        }

        public override string ToString()
        {
            if (Nickname == null) return string.Format("{0}, {1}", this.Surname, this.FirstName);
            return string.Format("{0} - {1}, {2}", this.Nickname, this.Surname, this.FirstName);
        }

    }
}
