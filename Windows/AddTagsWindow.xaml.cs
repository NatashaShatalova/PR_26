using CarServiceApp.Model.DAO;
using CarServiceApp.Model.DataSource;
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

namespace CarServiceApp.Windows
{
    /// <summary>
    /// Логика взаимодействия для AddTagsWindow.xaml
    /// </summary>
    public partial class AddTagsWindow : Window
    {
        Client selectedClient;

        public AddTagsWindow(Client client)
        {
            selectedClient = client;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lbTags.ItemsSource = db.GetContext().Tag.ToList();
        }

        private void bReady_Click(object sender, RoutedEventArgs e)
        {
            if(lbTags.SelectedItems.Count == 0)
            {
                Close();
                return;
            }

            foreach(var tag in lbTags.SelectedItems)
            {
                selectedClient.Tag.Add((Tag)tag);
            }

            db.GetContext().SaveChanges();
            Close();
        }
    }
}
