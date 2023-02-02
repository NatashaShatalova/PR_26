using CarServiceApp.Model.DAO;
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
    /// Логика взаимодействия для DocumentsListWindow.xaml
    /// </summary>
    public partial class DocumentsListWindow : Window
    {
        public DocumentsListWindow(int clientServiceId)
        {
            InitializeComponent();

            lbDocs.ItemsSource = db.GetContext().DocumentByService.ToList().Where(c => c.ClientServiceID == clientServiceId);
        }
    }
}
