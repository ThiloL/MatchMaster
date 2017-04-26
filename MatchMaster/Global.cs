using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchMaster
{
    public static class Global
    {
        public static ShooterWindow sw;
        public static MatchWindow mw;
        public static MatchShooters ms;
        public static PrintStuff ps;
        public static CategoryWindow cw;

        private static Match _current_match = null;

        public static Match CurrentMatch
        {
            get { return _current_match; }
            set { _current_match = (Match)value; }
        }

    }
}
