using CarServiceApp.Model.DAO;
using CarServiceApp.Model.DataSource;
using CarServiceApp.Utils;
using CarServiceApp.Windows;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CarServiceApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для ClientListPage.xaml
    /// </summary>
    public partial class ClientListPage : Page
    {
        public ClientListPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            updListAndPage();
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            searchSortFilter();
        }

        private void cbFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            searchSortFilter();
        }

        private void cbSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            searchSortFilter();
        }

        //edit
        private void lvClients_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(lvClients.SelectedItems.Count == 1)
            {
                try
                {
                    ClientEditInfoUtils.typeCode = 1;
                    ClientEditInfoUtils.client = (Client)lvClients.SelectedItem;
                    new AddEditClientWindow().ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }

                clearFilters();
                updList();
            }
        }

        //add
        private void bAddClient_Click(object sender, RoutedEventArgs e)
        {
            ClientEditInfoUtils.typeCode = 0;
            new AddEditClientWindow().ShowDialog();
            clearFilters();
            updList();
        }

        //prevPage
        private void bPageBack_Click(object sender, RoutedEventArgs e)
        {
            if (lvPages.SelectedIndex != 0 && lvPages.SelectedIndex != -1) lvPages.SelectedItem = lvPages.SelectedIndex--;
        }

        //nextPage
        private void bPageNext_Click(object sender, RoutedEventArgs e)
        {
            if (lvPages.SelectedIndex != (getPageCount() + 1)) lvPages.SelectedItem = lvPages.SelectedIndex++;
        }

        //selectPage
        private void lvPages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvPages.SelectedIndex == 0) PageUtils.skipCount = 0;
            else PageUtils.skipCount = lvPages.SelectedIndex * PageUtils.takeCount;
            updList();
            searchSortFilter();
        }

        private void pageLoad()
        {
            List<PageUtils.PageNumber> pageList = new List<PageUtils.PageNumber>();
            int pageCount = getPageCount();

            for (int i = 1; i <= pageCount; i++)
            {
                pageList.Add(new PageUtils.PageNumber(i));
            }

            if (lvPages != null) lvPages.ItemsSource = pageList;
            lvPages.SelectedIndex = 0;
        }

        private int getPageCount()
        {
            int fullCount = db.GetContext().Client.ToList().Count;
            //db.GetContext().Client.ToList().Count
            PageUtils.clientCount = fullCount;
            int remainder = fullCount % PageUtils.takeCount;
            int pageCount = (fullCount - remainder) / PageUtils.takeCount;

            if (remainder > 0) return pageCount++;
            else return pageCount;
        }
        
        //SelectCount
        private void cbClientsCount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cbItem = ((ComboBoxItem)cbClientsCount.SelectedItem).Content.ToString();
            if (cbItem == "Все") PageUtils.takeCount = PageUtils.clientCount;
            else PageUtils.takeCount = Convert.ToInt32(cbItem);

            updListAndPage();
        }

        //ShowBirthday
        private void bShowBirthDate_Click(object sender, RoutedEventArgs e)
        {
            new BirthdayListWindow().ShowDialog();
        }

        //searchSortFilter
        private void searchSortFilter()
        {
            int genderId = 0;
            string sortParametr = "0";

            if (cbFilter == null || cbSort == null || cbFilter.SelectedItem == null || cbSort.SelectedItem == null)
            {
                updList();
                return;
            }

            genderId = Convert.ToInt32(((ComboBoxItem)cbFilter.SelectedItem).Tag.ToString());
            sortParametr = ((ComboBoxItem)cbSort.SelectedItem).Tag.ToString();

            try
            {
                if ((cbSort.SelectedItem == null || cbSort.SelectedIndex == 0) && (tbSearch.Text == "" || tbSearch.Text == null) && (cbFilter.SelectedItem == null || genderId == 0))
                {
                    updList();
                    return;
                }

                if ((cbFilter.SelectedItem == null || genderId == 0) && (cbSort.SelectedItem == null || cbSort.SelectedIndex == 0))
                {
                    search();
                    return;
                }

                if ((cbFilter.SelectedItem == null || genderId == 0) && (tbSearch.Text == "" || tbSearch.Text == null))
                {
                    sort(sortParametr);
                    return;
                }

                if ((cbSort.SelectedItem == null || cbSort.SelectedIndex == 0) && (tbSearch.Text == "" || tbSearch.Text == null))
                {
                    filter(genderId);
                    return;
                }

                if (cbSort.SelectedItem == null || cbSort.SelectedIndex == 0)
                {
                    searchFilter(genderId);
                    return;
                }

                if(cbFilter.SelectedItem == null || genderId == 0)
                {
                    searchSort(sortParametr);
                    return;
                }

                if (tbSearch.Text == "" || tbSearch.Text == null)
                {
                    sortFilter(genderId, sortParametr);
                    return;
                }

                switch (cbSort.SelectedIndex)
                {
                    case 0: searchFilter(genderId); return;
                    case 1: lvClients.ItemsSource = db.GetContext().Client.ToList().FindAll(i => i.LastName.Contains(tbSearch.Text) || i.Email.Contains(tbSearch.Text) || i.Phone.Contains(tbSearch.Text)).Where(g => g.GenderCode == genderId.ToString()).OrderBy(i => i.LastName).Skip(PageUtils.skipCount).Take(PageUtils.takeCount); break;
                    case 2: lvClients.ItemsSource = db.GetContext().Client.ToList().FindAll(i => i.LastName.Contains(tbSearch.Text) || i.Email.Contains(tbSearch.Text) || i.Phone.Contains(tbSearch.Text)).Where(g => g.GenderCode == genderId.ToString()).OrderByDescending(i => i.LastVisit).Skip(PageUtils.skipCount).Take(PageUtils.takeCount); break;
                    case 3: lvClients.ItemsSource = db.GetContext().Client.ToList().FindAll(i => i.LastName.Contains(tbSearch.Text) || i.Email.Contains(tbSearch.Text) || i.Phone.Contains(tbSearch.Text)).Where(g => g.GenderCode == genderId.ToString()).OrderByDescending(i => i.VisitCount).Skip(PageUtils.skipCount).Take(PageUtils.takeCount); break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void sortFilter(int genderId, string sortParametr)
        {
            if (genderId == 0)
            {
                sort(sortParametr);
                return;
            }

            switch (cbSort.SelectedIndex)
            {
                case 0: filter(genderId); break;
                case 1: lvClients.ItemsSource = db.GetContext().Client.ToList().Where(g => g.GenderCode == genderId.ToString()).OrderBy(i => i.LastName).Skip(PageUtils.skipCount).Take(PageUtils.takeCount); break;
                case 2: lvClients.ItemsSource = db.GetContext().Client.ToList().Where(g => g.GenderCode == genderId.ToString()).OrderByDescending(i => i.LastVisit).Skip(PageUtils.skipCount).Take(PageUtils.takeCount); break;
                case 3: lvClients.ItemsSource = db.GetContext().Client.ToList().Where(g => g.GenderCode == genderId.ToString()).OrderByDescending(i => i.VisitCount).Skip(PageUtils.skipCount).Take(PageUtils.takeCount); break;
            }
        }

        private void filter(int genderId)
        {
            lvClients.ItemsSource = db.GetContext().Client.ToList().Where(g => g.GenderCode == genderId.ToString()).Skip(PageUtils.skipCount).Take(PageUtils.takeCount);
        }

        private void sort(string sortParametr)
        {
            switch (cbSort.SelectedIndex)
            {
                case 0: return;
                case 1: lvClients.ItemsSource = db.GetContext().Client.ToList().OrderBy(i => i.LastName).Skip(PageUtils.skipCount).Take(PageUtils.takeCount); break;
                case 2: lvClients.ItemsSource = db.GetContext().Client.ToList().OrderByDescending(i => i.LastVisit).Skip(PageUtils.skipCount).Take(PageUtils.takeCount); break;
                case 3: lvClients.ItemsSource = db.GetContext().Client.ToList().OrderByDescending(i => i.VisitCount).Skip(PageUtils.skipCount).Take(PageUtils.takeCount); break;
            }
        }

        private void searchFilter(int genderId)
        {
            lvClients.ItemsSource = db.GetContext().Client.ToList().FindAll(i => i.LastName.Contains(tbSearch.Text) || i.Email.Contains(tbSearch.Text) || i.Phone.Contains(tbSearch.Text)).Where(g => g.GenderCode == genderId.ToString()).Skip(PageUtils.skipCount).Take(PageUtils.takeCount);
        }

        private void searchSort(string sortParametr)
        {
            switch (cbSort.SelectedIndex)
            {
                case 0: search(); return;
                case 1: lvClients.ItemsSource = db.GetContext().Client.ToList().FindAll(i => i.LastName.Contains(tbSearch.Text) || i.Email.Contains(tbSearch.Text) || i.Phone.Contains(tbSearch.Text)).OrderBy(i => i.LastName).Skip(PageUtils.skipCount).Take(PageUtils.takeCount); break;
                case 2: lvClients.ItemsSource = db.GetContext().Client.ToList().FindAll(i => i.LastName.Contains(tbSearch.Text) || i.Email.Contains(tbSearch.Text) || i.Phone.Contains(tbSearch.Text)).OrderByDescending(i => i.LastVisit).Skip(PageUtils.skipCount).Take(PageUtils.takeCount); break;
                case 3: lvClients.ItemsSource = db.GetContext().Client.ToList().FindAll(i => i.LastName.Contains(tbSearch.Text) || i.Email.Contains(tbSearch.Text) || i.Phone.Contains(tbSearch.Text)).OrderByDescending(i => i.VisitCount).Skip(PageUtils.skipCount).Take(PageUtils.takeCount); break;
            }
        }

        private void search()
        {
            lvClients.ItemsSource = db.GetContext().Client.ToList().FindAll(i => i.LastName.Contains(tbSearch.Text) || i.Email.Contains(tbSearch.Text) || i.Phone.Contains(tbSearch.Text)).Skip(PageUtils.skipCount).Take(PageUtils.takeCount);
        }

        //update
        private void updList()
        {
            if(lvClients == null)
            {
                return;
            }

            lvClients.ItemsSource = db.GetContext().Client.ToList().Skip(PageUtils.skipCount).Take(PageUtils.takeCount);
            updClientCountInfo();
            //clearFilters();
        }

        private void updClientCountInfo()
        {
            int showCount = PageUtils.takeCount, fullcount = PageUtils.clientCount;
            if (showCount > fullcount) showCount = fullcount;
            tbClientCount.Text = $"{showCount} из {fullcount}";
        }

        private void updListAndPage()
        {
            updList();
            pageLoad();
        }

        //clearFields
        private void clearFilters()
        {
            cbFilter.SelectedIndex = 2;
            cbSort.SelectedItem = 0;
            tbSearch.Text = "";
        }

        private void bShowVisits_Click(object sender, RoutedEventArgs e)
        {
            new ClientVisitsWindows((Client)lvClients.SelectedItem).ShowDialog();
        }
    }
}
