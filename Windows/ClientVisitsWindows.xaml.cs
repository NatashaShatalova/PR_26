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
    /// Логика взаимодействия для ClientVisitsWindows.xaml
    /// </summary>
    public partial class ClientVisitsWindows : Window
    {
        Client client;

        public ClientVisitsWindows(Client client)
        {
            this.client = client;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lvVisits.ItemsSource = db.GetContext().ClientService.ToList().Where(c => c.ClientID == client.ID);
        }

        private void lvVisits_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            new DocumentsListWindow(((ClientService)lvVisits.SelectedItem).ID).ShowDialog();
        }
    }
}
