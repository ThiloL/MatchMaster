using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MatchMaster
{
    public static class DbCreator
    {
        /// <summary>
        /// Drop (if) and Create Database with MDF and LDF
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="filename_without_extension"></param>
        /// <returns></returns>
        public static bool Create(string db_name,string folder)
        {
            try
            {
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, $"{Global.Product}", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            SqlConnection connection = new SqlConnection($@"Server=(LocalDB)\{Global.Product}");
            using (connection)
            {
                try
                {

                    connection.Open();

                    string sql = null;

                    sql = $@"
IF EXISTS(SELECT * FROM sys.databases WHERE name='{db_name}')
	BEGIN
        DROP DATABASE [{db_name}]
    END
";
                    SqlCommand command = new SqlCommand(sql, connection);
                    command.ExecuteNonQuery();

                    sql = string.Format(@"
                    CREATE DATABASE
                        [{0}]
                    ON PRIMARY (
                       NAME={0}_data,
                       FILENAME = '{1}\{0}.mdf'
                    )
                    LOG ON (
                        NAME={0}_log,
                        FILENAME = '{1}\{0}.ldf'
                    )",
                        db_name,
                        folder
                    );

                    command.CommandText = sql;
                    command.ExecuteNonQuery();

                    return true;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, $"{Global.Product}", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
        }
    }
}
