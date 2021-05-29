
using System;
using System.IO;
using TermTracker.Models;
using Xamarin.Forms;

namespace TermTracker.Views
{
    [QueryProperty(nameof(TermId), nameof(TermId))]
    public partial class TermEntryPage : ContentPage
    {
        public string TermId
        {
            set { Load(value); }
        }

        public TermEntryPage()
        {
            InitializeComponent();
            BindingContext = new Term();

            EndDatePicker.Date = DateTime.Now;
            StartDatePicker.Date = DateTime.Now;
        }

        async void Load(string termId)
        {
            try
            {
                int id = Convert.ToInt32(termId);
                Term term = await App.Database.GetTermAsync(id);
                BindingContext = term;
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to load term.");
            }
        }

        async void SaveButton_Click(object sender, EventArgs e)
        {
            var term = (Term)BindingContext;

            if (string.IsNullOrWhiteSpace(term.TermTitle))
            {
                await DisplayAlert("Required Field",
                    "The term must have a title.", "OK");
                return;
            }
            if (DateTime.Compare(term.TermStartDate, term.TermEndDate) != -1)
            {
                await DisplayAlert("Incorrect Dates",
                    "The start date must come before the end date.", "OK");
                return;
            }

            await App.Database.SaveTermAsync(term);
            await Shell.Current.GoToAsync("..");
        }

        async void DeleteButton_Click(object sender, EventArgs e)
        {
            bool response = await DisplayAlert("Delete this term?",
                "This action is irreversible.", "Delete", "Cancel");
            if (response)
            {
                var term = (Term)BindingContext;
                await App.Database.FullDeleteTermAsync(term);

                // Navigate backwards
                await Shell.Current.GoToAsync("../..");
            }
        }
    }
}