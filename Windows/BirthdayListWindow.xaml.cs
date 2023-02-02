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
    /// Логика взаимодействия для BirthdayListWindow.xaml
    /// </summary>
    public partial class BirthdayListWindow : Window
    {
        public BirthdayListWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            List<Client> birthdayList = new List<Client>();

            foreach (var client in db.GetContext().Client.ToList())
            {
                if(client.Birthday.Value.Month == DateTime.Now.Month)
                {
                    birthdayList.Add(client);
                }
            }

            lbClients.ItemsSource = birthdayList;
        }
    }
}
