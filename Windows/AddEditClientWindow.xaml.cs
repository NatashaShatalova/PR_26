using CarServiceApp.Model.DAO;
using CarServiceApp.Model.DataSource;
using CarServiceApp.Utils;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Логика взаимодействия для AddEditClientWindow.xaml
    /// </summary>
    public partial class AddEditClientWindow : Window
    {
        Client curClient = ClientEditInfoUtils.client;
        string clientPhotoPath = "\\Resources\\ClientsPhoto\\";
        long maxPhotoLength = 2097152; //2mb

        public AddEditClientWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cbGender.ItemsSource = db.GetContext().Gender.ToList();

            if (ClientEditInfoUtils.typeCode == 1)
            {
                loadInfo();
            }
            else if(ClientEditInfoUtils.typeCode == 0)
            {
                hideBtn();
            }

            lockBtn();
        }

        private void hideBtn()
        {
            lvTags.Visibility = Visibility.Collapsed;
            bAddTag.Visibility = Visibility.Collapsed;
            bDeleteTag.Visibility = Visibility.Collapsed;
        }

        private void lockBtn()
        {
            if (lvTags.SelectedItems.Count != 1) bDeleteTag.IsEnabled = false;
            else bDeleteTag.IsEnabled = true;
        }

        private void loadInfo()
        {
            try
            {
                iPhoto.Source = new BitmapImage(new Uri(clientPhotoPath + curClient.PhotoPath, UriKind.Relative));

                tbId.Text = curClient.ID.ToString();
                tbLastName.Text = curClient.LastName;
                tbFirstName.Text = curClient.FirstName;
                tbPatronymic.Text = curClient.Patronymic;
                cbGender.SelectedItem = curClient.Gender;
                dpBirthDate.SelectedDate = curClient.Birthday;
                tbPhone.Text = curClient.Phone;
                tbMail.Text = curClient.Email;
                tbPathLogo.Text = curClient.PhotoPath;

                updTags();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void updTags()
        {
            lvTags.ItemsSource = db.GetContext().Client.ToList().Find(c => c.ID == curClient.ID).Tag;
        }

        private void bDeleteTag_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                curClient.Tag.Remove((Tag)lvTags.SelectedItem);
                db.GetContext().SaveChanges();
                updTags();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void bAddTag_Click(object sender, RoutedEventArgs e)
        {
            new AddTagsWindow(curClient).ShowDialog();
            db.GetContext().SaveChanges();
            updTags();
        }

        private void lvTags_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lockBtn();
        }

        private void bDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"Удалить клиента " + curClient.LastName + " " + curClient.FirstName + "?",
                "Подтверждение", MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (visitsIsExist(curClient))
                {
                    MessageBox.Show("Удаление данного клиента невозможно!\n\nПричина: присутствует история посещений", "Ошибка");
                    return;
                }

                db.GetContext().Client.Remove(curClient);
                db.GetContext().SaveChanges();
                Close();
            }
        }

        private bool visitsIsExist(Client client)
        {
            if(db.GetContext().ClientService.ToList().Find(v => v.ClientID == client.ID) == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void bReady_Click(object sender, RoutedEventArgs e)
        {
            if (!checkErrorInput())
            {
                MessageBox.Show("Поля не заполнены!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!EmailValidator.IsValidEmail(tbMail.Text))
            {
                MessageBox.Show("Неверно введен E-Mail!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var gender = (Gender)cbGender.SelectedItem;

                if (ClientEditInfoUtils.typeCode == 1)
                {
                    var curEditClient = db.GetContext().Client.ToList().Find(i => i.ID == curClient.ID);

                    curEditClient.LastName = tbLastName.Text;
                    curEditClient.FirstName = tbFirstName.Text;
                    curEditClient.Patronymic = tbPatronymic.Text;
                    curEditClient.GenderCode = gender.Code.ToString();
                    curEditClient.Birthday = dpBirthDate.SelectedDate;
                    curEditClient.Phone = tbPhone.Text;
                    curEditClient.Email = tbMail.Text;
                    curEditClient.PhotoPath = tbPathLogo.Text;

                    db.GetContext().SaveChanges();

                    MessageBox.Show("Запись обновлена", "Оповещение", MessageBoxButton.OK);

                    Close();
                }
                else if (ClientEditInfoUtils.typeCode == 0)
                {
                    Client newClient = new Client()
                    {
                        FirstName = tbFirstName.Text,
                        LastName = tbLastName.Text,
                        Patronymic = tbPatronymic.Text,
                        Birthday = dpBirthDate.SelectedDate, 
                        RegistrationDate = DateTime.Now, 
                        Email = tbMail.Text,
                        Phone = tbPhone.Text,
                        GenderCode = gender.Code,
                        PhotoPath = tbPathLogo.Text
                    };

                    db.GetContext().Client.Add(newClient);
                    db.GetContext().SaveChanges();

                    MessageBox.Show("Запись добавлена", "Оповещение", MessageBoxButton.OK);

                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private bool checkErrorInput()
        {
            return
                tbFirstName.Text != null &&
                tbLastName.Text != null &&
                tbMail.Text != null &&
                tbPhone.Text != null &&
                dpBirthDate.SelectedDate != null &&
                cbGender.SelectedItem != null;
        }

        private void bLogo_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg";
            if (openFileDialog.ShowDialog() == true)
            {
                Uri fileUri = new Uri(openFileDialog.FileName);

                if (new FileInfo(fileUri.LocalPath).Length > maxPhotoLength)
                {
                    MessageBox.Show($"Выбранный Вами файл не должен превышать {byteToMegabyte(maxPhotoLength)} МБ", "Ошибка");
                    return;
                }

                iPhoto.Source = new BitmapImage(fileUri);
                tbPathLogo.Text = fileUri.LocalPath;
            }
        }

        private long byteToMegabyte(long bytes)
        {
            return (long)Math.Round(Convert.ToDouble(bytes / 1048576), 1);
        }

        private void onlyNumberInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }

        private void onlyTextInput(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.A && e.Key <= Key.Z ||
                e.Key == Key.Space ||
                e.Key == Key.OemMinus)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void phoneInput(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9 ||
                e.Key == Key.Space ||
                e.Key == Key.OemMinus ||
                e.Key == Key.OemPlus ||
                e.Key == Key.OemOpenBrackets ||
                e.Key == Key.OemCloseBrackets)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }
    }
}
