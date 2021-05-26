using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Xamarin.Forms;
using TermTracker.Models;

namespace TermTracker.Views
{
    public partial class TermsPage : ContentPage
    {
        public TermsPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            TermsCollection.ItemsSource = await App.Database.GetTermsAsync();

            if (TermsCollection.SelectedItem != null)
            {
                TermsCollection.SelectedItem = null;
            }
        }

        async void AddTerm_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(TermEntryPage));
        }

        async void Terms_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.Count != 0)
            {
                Term term = (Term)e.CurrentSelection.FirstOrDefault();
                await Shell.Current.GoToAsync(
                    $"{nameof(TermDetailPage)}?{nameof(TermDetailPage.TermId)}={term.TermId}");
            }
        }

    }
}