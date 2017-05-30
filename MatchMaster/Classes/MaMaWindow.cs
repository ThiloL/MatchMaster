using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MatchMaster
{
    public class MaMaWindow : Window
    {
        string title;

        public MaMaWindow(string Title)
        {
            title = Title;
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            MinHeight = App.ScreenHeight / 2;
            Width = App.ScreenWidth / 2;
            MinWidth = App.ScreenWidth / 3;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            this.Title = title + " - " + Global.CurrentMatch?.ToString();
        }

        //protected override void OnClosed(EventArgs e)
        //{
        //    base.OnClosed(e);

            
        //}
    }
}
