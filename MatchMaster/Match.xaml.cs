using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using System.Data;
using System.Data.SqlClient;

namespace MatchMaster
{
    /// <summary>
    /// Interaction logic for Match.xaml
    /// </summary>
    public partial class MatchWindow : Window
    {
        public MatchWindow()
        {
            InitializeComponent();
            FillGrid();
        }

        private void FillGrid()
        {
            using (SqlConnection con = new SqlConnection(Properties.Settings.Default.MMConnectionString))
            {
                const string s = "select iddesc as ID, title as Title, [start] as [Start], [end] as [End] from match ORDER BY [ID] desc";
                SqlCommand c = new SqlCommand(s, con);
                SqlDataAdapter da = new SqlDataAdapter(c);
                DataTable dt = new DataTable("Match");
                da.Fill(dt);
                GvMatch.ItemsSource = dt.DefaultView;
            }
        }
    }
}
