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
using System.Globalization;
using System.Text.RegularExpressions;
using sqlite_utility;
namespace import_export
{
    public partial class MainWindow : Window
    {
        PersianCalendar cal = new PersianCalendar();
        Sqlite_Utility sqlite = new Sqlite_Utility("datas.db");
        string Table_key { get; set; } = "import";
        Int32 total ;
        public MainWindow()
        {
            InitializeComponent();
            add.Content = FindResource("add_pic");
            change.Content = FindResource("change_pic");
            search.Content = FindResource("search_pic");
            print_date.Content = $"{cal.GetYear(DateTime.Now)} / {cal.GetMonth(DateTime.Now)} / {cal.GetDayOfMonth(DateTime.Now)} - {cal.GetDayOfWeek(DateTime.Now)}";
            ListView_rearrange($"SELECT * FROM {Table_key} ORDER BY ID DESC");
        }
        private void ListView_rearrange(string command)
        {
            total = 0;
            list.Items.Clear();
            sqlite.sql_command_read(command);
            while (sqlite.check_reader_and_close())
            {
                list.Items.Add(new Data_Binding
                {
                    ID = sqlite.reader.GetInt32(0),
                    Item = sqlite.reader.GetString(1),
                    Quantity = sqlite.reader.GetInt16(2),
                    Date = $"{sqlite.reader.GetInt16(3)}" +
                    $"/{sqlite.reader.GetInt16(4)}/{sqlite.reader.GetInt16(5)} - {sqlite.reader.GetInt16(6)}:{sqlite.reader.GetInt16(7)}",
                    Cost = sqlite.reader.GetInt32(8),
                    Details = (sqlite.reader.GetString(9) != "") ? "notes" : ""
                });
                total += sqlite.reader.GetInt32(8);
            }
            total_cost.Content = $"Total : {total.ToString()}";
        }
        private void Change_Click(object sender, RoutedEventArgs e)
        {
            if(Table_key == "import")
            {
                Table_key = "export";
                Table_name.Content = "EXPORT";
            }
            else
            {
                Table_key = "import";
                Table_name.Content = "IMPORT";
            }
            ListView_rearrange($"SELECT * FROM {Table_key} ORDER BY ID DESC");
        }       
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            TemplateWin form = new Add(Table_key);
            form.ShowDialog();
            ListView_rearrange($"SELECT * FROM {Table_key} ORDER BY ID DESC");
        }
        private void Search_Click(object sender , RoutedEventArgs e)
        {
            switch(Search_Parameters_Check())
            {
                case 0:
                    MessageBox.Show("Invalid Search Parameters .");
                    break;
                case 1:
                    ListView_rearrange($"SELECT * FROM {Table_key} ORDER BY ID DESC");
                    break;
                case 2:
                    string searchQuery = $"SELECT * FROM {Table_key} WHERE ";
                    if (c_name.IsChecked == true)
                    {
                        searchQuery += $"item LIKE \'%{t_name.Text}%\' ";
                        if (c_year.IsChecked == true || c_month.IsChecked == true || c_day.IsChecked == true || c_quantity.IsChecked == true)
                            searchQuery += $"AND ";
                    }
                    if (c_year.IsChecked == true)
                    {
                        searchQuery += $"year = {int.Parse(t_year.Text)} ";
                        if (c_month.IsChecked == true || c_day.IsChecked == true || c_quantity.IsChecked == true)
                            searchQuery += $"AND ";
                    }
                    if (c_month.IsChecked == true)
                    {
                        searchQuery += $"month = {int.Parse(t_month.Text)} ";
                        if (c_day.IsChecked == true || c_quantity.IsChecked == true)
                            searchQuery += $"AND ";
                    }
                    if (c_day.IsChecked == true)
                    {
                        searchQuery += $"day = {int.Parse(t_day.Text)} ";
                        if (c_quantity.IsChecked == true)
                            searchQuery += $"AND ";
                    }
                    if (c_quantity.IsChecked == true)
                    {
                        searchQuery += $"quantity = {int.Parse(t_quantity.Text)} ";
                    }
                    searchQuery += "ORDER BY ID DESC";
                    ListView_rearrange(searchQuery);
                    break;
            }                  
        }
        private void CheckBox_Check_changed(object sender, RoutedEventArgs e)
        {
            if (sender.Equals(c_name))
            {
                if (c_name.IsChecked == true)                   
                    t_name.IsEnabled = true;
                else
                {
                    t_name.Text = "";
                    t_name.IsEnabled = false;
                }
            }                
            else if (sender.Equals(c_year))
            {
                if (c_year.IsChecked == true)
                    t_year.IsEnabled = true;
                else
                {
                    t_year.Text = "";
                    t_year.IsEnabled = false;
                }
            }            
            else if (sender.Equals(c_month))
            {
                if (c_month.IsChecked == true)
                    t_month.IsEnabled = true;
                else
                {
                    t_month.Text = "";
                    t_month.IsEnabled = false;
                }
            }
            else if (sender.Equals(c_day))
            {
                if (c_day.IsChecked == true)
                    t_day.IsEnabled = true;
                else
                {
                    t_day.Text = "";
                    t_day.IsEnabled = false;
                }
            }
            else if (sender.Equals(c_quantity))
            {
                if (c_quantity.IsChecked == true)
                    t_quantity.IsEnabled = true;
                else
                {
                    t_quantity.Text = "";
                    t_quantity.IsEnabled = false;
                }
            }
        }
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (list.SelectedItems.Count > 0)
            {
                TemplateWin form = new Item_description(Table_key, (Data_Binding)list.SelectedItem);
                form.ShowDialog();
                ListView_rearrange($"SELECT * FROM {Table_key} ORDER BY ID DESC");
            }           
        }
        private int Search_Parameters_Check()
        {
            int status = 1;
            if (c_name.IsChecked == true)
            {
                if (t_name.Text == "")
                {
                    return 0;
                }
                status = 2;
            }
            if (c_year.IsChecked == true)
            {                
                if (!Regex_Check(@"^\d{1,}$" , t_year.Text))
                {
                    return 0;
                }
                status = 2;
            }
            if (c_month.IsChecked == true)
            {
                if (!Regex_Check(@"^\d{1,}$", t_month.Text))
                {
                    return 0;
                }
                status = 2;
            }
            if (c_day.IsChecked == true)
            {
                if (!Regex_Check(@"^\d{1,}$", t_day.Text))
                {
                    return 0;
                }
                status = 2;
            }
            if (c_quantity.IsChecked == true)
            {
                if (!Regex_Check(@"^\d{1,}$", t_quantity.Text))
                {
                    return 0;
                }
                status = 2;
            }
            return status;
        }
        private bool Regex_Check(string pattern, string input)
        {
            Regex regex = new Regex(pattern);
            return regex.IsMatch(input);
        }
    }
    public class Data_Binding
    {
        public Int32 ID { get; set; }
        public string Item { get; set; }
        public int Quantity { get; set; }
        public string Date { get; set; }
        public Int32 Cost { get; set; }
        public string Details { get; set; }
    }
}
