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

namespace MatchMaster
{
    /// <summary>
    /// Interaction logic for Category.xaml
    /// </summary>
    public partial class CategoryWindow : Window
    {
        private MatchMasterContext _ctx = new MatchMasterContext();

        public CategoryWindow()
        {
            InitializeComponent();

            this.MinHeight = App.ScreenHeight / 2;
            this.Width = App.ScreenWidth / 2;
            this.MinWidth = App.ScreenWidth / 3;

            Refresh();
        }

        private void Refresh()
        {
            var c = from x in _ctx.Categories
                    orderby x.Name
                    select x;

            CategoriesGrid.ItemsSource = c.ToList();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (Details.DataContext == null) return;

            Category c = (Details.DataContext as Category);

            if (MessageBox.Show($"Do you really want to delete this Category?\n\n{c.ToString()}", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _ctx.Categories.Remove(_ctx.Categories.First(x => x.CategoryID.Equals(c.CategoryID)));
                _ctx.SaveChanges();
                Refresh();
            }
        }

        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            Category c = new Category() { Name = "new Category" };
            _ctx.Categories.Add(c);
            _ctx.SaveChanges();
            Refresh();
            CategoriesGrid.SelectedItem = c;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            this.CategoryDetailsBg.UpdateSources();
            _ctx.SaveChanges();
        }
    }
}
