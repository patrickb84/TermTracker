﻿
using System;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TermTracker.Views
{
    public partial class AboutPage : ContentPage
    {
        public AboutPage()
        {
            InitializeComponent();
        }

        async void OnButtonClicked(object sender, EventArgs e)
        {
            // Launch the specified URL in the system browser.
            await Launcher.OpenAsync("https://github.com/patrickb84/TermTracker");
        }
    }
}