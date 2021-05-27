using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace TermTracker.Views
{
    public partial class NotificationsPage : ContentPage
    {
        public NotificationsPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            NotificationsCollection.ItemsSource = await App.Database.GetNotificationsAsync();
        }
    }
}
