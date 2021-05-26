using System;
using System.IO;
using TermTracker.Models;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Linq;

namespace TermTracker.Views
{
    [QueryProperty(nameof(TermId), nameof(TermId))]
    public partial class TermDetailPage : ContentPage
    {
        public Term ActiveTerm { get; set; }
        public string TermId
        {
            set
            {
                Load(value);
            }
        }

        public TermDetailPage()
        {
            InitializeComponent();
            BindingContext = new Term();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (CoursesCollection.SelectedItem != null)
            {
                CoursesCollection.SelectedItem = null;
            }
        }

        async void Load(string termId)
        {
            try
            {
                int id = Convert.ToInt32(termId);
                ActiveTerm = await App.Database.GetTermAsync(id);
                CoursesCollection.ItemsSource = await App.Database.GetCoursesAsync(id);
                BindingContext = ActiveTerm;
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to load Term.");
            }
        }

        async void AddCourseButton_Clicked(object sender, EventArgs e)
        {
            var courseEntryPage = new CourseEntryPage(ActiveTerm.TermId);
            await Navigation.PushAsync(courseEntryPage);
        }

        async void EditTerm_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(
                $"{nameof(TermEntryPage)}?{nameof(TermEntryPage.TermId)}={ActiveTerm.TermId}");
        }

        async void CoursesCollection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.Count != 0)
            {
                Course c = (Course)e.CurrentSelection.FirstOrDefault();
                await Shell.Current.GoToAsync(
                    $"{nameof(CourseDetailPage)}?{nameof(CourseDetailPage.CourseId)}={c.CourseId}");
            }
        }
    }
}