using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchMaster
{
    [Table("MatchParticipations")]
    public class MatchParticipation
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MatchParticipationId { get; set; }

        public int Posse { get; set; }
        public Boolean IsSpeedTicket { get; set; }
        public bool IsMatchDQ { get; set; }

        public int? ShooterID { get; set; }
        public int? MatchID { get; set; }

        public virtual Shooter Shooter { get; set; }
        public virtual Match Match { get; set; }

        public MatchParticipation()
        {
            // Default: Posse number 0
            this.Posse = 0;

            // Default: kein not Speed Ticket
            this.IsSpeedTicket = false;
        }
    }
}
