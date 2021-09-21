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
using System.Text.RegularExpressions;
using System.Globalization;
using sqlite_utility;
namespace import_export
{
    public partial class TemplateWin : Window
    {
        PersianCalendar cal = new PersianCalendar();
        protected Sqlite_Utility sqlite = new Sqlite_Utility("datas.db");
        protected string Table_key { get; set; }
        protected int[] date_formatted;
        public TemplateWin(string key)
        {
            InitializeComponent();
            Table_key = key;
        }
        public virtual void Textbox_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender.Equals(date))
            {
                date.Foreground = Brushes.Black;
                date.Text = $"{cal.GetYear(DateTime.Now)}/{cal.GetMonth(DateTime.Now)}/{cal.GetDayOfMonth(DateTime.Now)}" +
                    $" - {cal.GetHour(DateTime.Now)}:{cal.GetMinute(DateTime.Now)}";
            }
            else if (sender.Equals(name))
            {
                name.Text = "";
                name.Foreground = Brushes.Black;
            }
            else if (sender.Equals(quantity))
            {
                quantity.Text = "";
                quantity.Foreground = Brushes.Black;
            }
            else if (sender.Equals(cost))
            {
                cost.Text = "";
                cost.Foreground = Brushes.Black;
            }
            else if (sender.Equals(details))
            {
                details.Text = "";
                details.Foreground = Brushes.Black;
            }
        }
        public  virtual void Confirm_Click(object sender, RoutedEventArgs e)
        {

        }
        public bool Regex_Check(string pattern, string input)
        {
            Regex regex = new Regex(pattern);
            return regex.IsMatch(input);
        }
        public bool Date_Check()
        {
            char[] split = new char[] { '/', '-', ':' };
            string[] date_splited;
            date_splited = date.Text.Split(split);
            date_formatted = Array.ConvertAll(date_splited, int.Parse);
            if (date_formatted[1] < 13 && date_formatted[1] > 0)
            {
                if (date_formatted[2] > 0 && date_formatted[2] < 32)
                {
                    if (date_formatted[3] < 24 && date_formatted[3] >= 0)
                    {
                        if (date_formatted[4] < 60 && date_formatted[4] >= 0)
                        {
                            return true;
                        }
                        else
                            return false;
                    }
                    else
                        return false;
                }
                else
                    return false;
            }
            else
                return false;
        }
    }
    public partial class Add : TemplateWin
    {
        public Add(string key) : base(key)
        {                       
            Title = "Add";
            confirm.Content = new Image()
            {
                Source = new BitmapImage(new Uri($"{Environment.CurrentDirectory}/images/confirm.png"))
            };
        }        
        public override void Confirm_Click(object sender, RoutedEventArgs e)
        {
            if (name.Text != "" && name.Text != "Item Name")
            {
                if (Regex_Check(@"^\d{4}\s{0,1}\/\s{0,1}\d{1,2}\s{0,1}\/\s{0,1}\d{1,2}\s{0,1}-\s{0,1}\d{1,2}\s{0,1}\:\s{0,1}\d{1,2}$", date.Text) && Date_Check())
                {
                    if (quantity.Text != "0" && Regex_Check(@"^\d{1,}", quantity.Text))
                    {
                        if (Regex_Check(@"^\d{1,}", cost.Text))
                        {
                            sqlite.SQL_NonQuery($"INSERT INTO {Table_key} ( item , quantity , year , month , day ,hour , minute  , cost , notes ) VALUES (\"{name.Text}\" ," +
                                $" {int.Parse(quantity.Text)} , {date_formatted[0]} , {date_formatted[1]} , {date_formatted[2]} , {date_formatted[3]} , {date_formatted[4]} ," +
                                $" {int.Parse(cost.Text)} , \"{(details.Text != "Details (Max 300 characters)" ? details.Text : "")}\" )");
                            this.Close();
                        }
                        else
                            MessageBox.Show("Invalid Cost .");
                    }
                    else
                        MessageBox.Show("Invalid Quantity .");
                }
                else
                    MessageBox.Show("Invalid Date .");
            }
            else
                MessageBox.Show("Invalid Name .");
        }       
    }
    public partial class Item_description : TemplateWin
    {
        private Int32 ID;
        public Item_description(string key , Data_Binding data) : base(key)
        {
            Title = "Description";
            confirm.Content = new Image()
            {
                Source = new BitmapImage(new Uri($"{Environment.CurrentDirectory}/images/update.png"))
            };
            ID = data.ID;
            name.Text = data.Item;
            date.Text = data.Date;
            quantity.Text = data.Quantity.ToString();
            cost.Text = data.Cost.ToString();
            sqlite.SQL_Query($"SELECT notes FROM {Table_key} WHERE ID = {data.ID}");
            while(sqlite.Row_available())
            {               
                if (( details.Text = sqlite.Read_Row().GetString(0) ) != "")                
                    details.Foreground = Brushes.Gray;
                else
                    details.Foreground = Brushes.Black;
            }            
        }
        public override void Confirm_Click(object sender, RoutedEventArgs e)
        {
            if (name.Text != "" && name.Text != "Item Name")
            {
                if (Regex_Check(@"^\d{4}\s{0,1}\/\s{0,1}\d{1,2}\s{0,1}\/\s{0,1}\d{1,2}\s{0,1}-\s{0,1}\d{1,2}\s{0,1}\:\s{0,1}\d{1,2}$", date.Text) && Date_Check())
                {
                    if (quantity.Text != "0" && Regex_Check(@"^\d{1,}", quantity.Text))
                    {
                        if (Regex_Check(@"^\d{1,}", cost.Text))
                        {
                            sqlite.SQL_Query($"UPDATE {Table_key} set item = \"{name.Text}\" , quantity = {int.Parse(quantity.Text)} , year = {date_formatted[0]} ," +
                                $" month = {date_formatted[1]} , day = {date_formatted[2]} ,hour = {date_formatted[3]} , minute = {date_formatted[4]} ," +
                                $" cost = {int.Parse(cost.Text)}  , notes = \"{(details.Text != "Details (Max 300 characters)" ? details.Text : "")}\" WHERE ID = {ID}"); 
                            this.Close();
                        }
                        else
                            MessageBox.Show("Invalid Cost .");
                    }
                    else
                        MessageBox.Show("Invalid Quantity .");
                }
                else
                    MessageBox.Show("Invalid Date .");
            }
            else
                MessageBox.Show("Invalid Name .");
        }
        public override void Textbox_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender.Equals(date))
                date.Foreground = Brushes.Black;
            else if (sender.Equals(name))
                name.Foreground = Brushes.Black;
            else if (sender.Equals(quantity))
                quantity.Foreground = Brushes.Black;
            else if (sender.Equals(cost))           
                cost.Foreground = Brushes.Black;           
            else if (sender.Equals(details))          
                details.Foreground = Brushes.Black;            
        }
    }
}
