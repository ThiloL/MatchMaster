using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchMaster
{
    public static class Global
    {
        private static Match _current_match = null;

        public static Match CurrentMatch
        {
            get { return _current_match; }
            set { _current_match = (Match)value; }
        }

    }
}
