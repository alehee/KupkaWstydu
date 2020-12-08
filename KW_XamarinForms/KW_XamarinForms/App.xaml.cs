using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KW_XamarinForms
{
    public partial class App : Application
    {
        public static Database database;

        public static Database Database
        {
            get
            {
                if (database == null)
                {
                    database = new Database(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "database.db3"));
                }
                return database;
            }
        }

        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }


    }
}
