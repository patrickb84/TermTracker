using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using TermTracker.Views;
using TermTracker.Data;
using Plugin.LocalNotifications;
using TermTracker.Models;

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
                        .LocalApplicationData), "TermTracker2.db3"));
                }
                return database;
            }
        }

        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
        }

        protected override async void OnStart()
        {
            var activeNotifications = await App.Database.GetNotificationsAsync();
            foreach (Notification n in activeNotifications)
            {
                CrossLocalNotifications.Current.Show(n.Title, n.Body, n.NotificationId, n.Schedule);
            }
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}