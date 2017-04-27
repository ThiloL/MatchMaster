using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchMaster
{
    public static class Global
    {
        public const string Manufacturer = "COREBYTE";
        public const string Product = "MatchMaster";

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

        public static string DatabaseFolder()
        {
            return Path.Combine(Path.Combine(Environment.ExpandEnvironmentVariables("%PROGRAMDATA%"), Manufacturer), Product);
        }

        public static string DatabaseMdfPath()
        {
            return Path.Combine(DatabaseFolder(), $"{Product}.mdf");
        }

    }
}
