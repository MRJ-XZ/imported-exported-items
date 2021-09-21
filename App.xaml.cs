using System;
using System.IO;
using System.Windows;

namespace import_export
{
    public partial class App : Application
    {
        public App()
        {
            if (File_Check().Length > 55)
            {
                MessageBox.Show(File_Check() + "\n---Program Terminated---" , "Error" , MessageBoxButton.OK , MessageBoxImage.Error);
                this.Shutdown(0);
            }
        }       
        private string File_Check()
        {
            string result = "Loading Files failed .\nFollowing Files are missing :";
            if (!File.Exists("sqlite-utility.dll"))
                result += "\n-sqlite-utility.dll";
            if (!File.Exists("SQLite.Interop.dll"))
                result += "\n-SQLite.Interop.dll";
            if (!File.Exists("System.Data.SQLite.dll"))
                result += "\n-System.Data.SQLite.dll";
            if (!File.Exists("images/add.png"))
                result += "\n-images/add.png";
            if (!File.Exists("images/change.png"))
                result += "\n-images/change.png";
            if (!File.Exists("images/search.png"))
                result += "\n-images/search.png";
            if (!File.Exists("images/update.png"))
                result += "\n-images/update.png";
            if (!File.Exists("images/confirm.png"))
                result += "\n-images/confirm.png";
            return result;
        }
    }
}
