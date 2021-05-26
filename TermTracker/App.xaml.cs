using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using TermTracker.Views;
using TermTracker.Data;

namespace TermTracker
{
    public partial class App : Application
    {
        static Database database;

        public static Database Database
        {
            get
            {
                if (database == null)
                {
                    database = new Database(Path.Combine(Environment
                        .GetFolderPath(Environment.SpecialFolder
                        .LocalApplicationData), "TermTracker.db3"));
                }
                return database;
            }
        }

        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
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