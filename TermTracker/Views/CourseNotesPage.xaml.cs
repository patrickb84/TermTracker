using System;
using System.Collections.Generic;
using System.Linq;
using TermTracker.Models;
using Xamarin.Forms;

namespace TermTracker.Views
{
    [QueryProperty(nameof(CourseId), nameof(CourseId))]
    public partial class CourseNotesPage : ContentPage
    {
        public string CourseId
        {
            set { Load(value); }
        }
        public string NoteCourseId;

        public CourseNotesPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (!string.IsNullOrEmpty(NoteCourseId))
            {
                int id = Convert.ToInt32(NoteCourseId);
                NotesCollection.ItemsSource = await App.Database.GetNotesAsync(id);
            }
        }

        async void Load(string courseId)
        {
            try
            {
                NoteCourseId = courseId;
                int id = Convert.ToInt32(courseId);
                NotesCollection.ItemsSource = await App.Database.GetNotesAsync(id);
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to load Notes.");
            }
        }

        async void AddNote_Clicked(System.Object sender, System.EventArgs e)
        {
            var cnp = new CourseNoteEntryPage(Convert.ToInt32(NoteCourseId));
            await Navigation.PushAsync(cnp);

            //await Shell.Current.GoToAsync(nameof(CourseNoteEntryPage));
        }

        async void NotesCollection_SelectionChanged(System.Object sender, Xamarin.Forms.SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.Count != 0)
            {
                Note n = (Note)e.CurrentSelection.FirstOrDefault();
                await Shell.Current.GoToAsync(
                    $"{nameof(CourseNoteEntryPage)}?{nameof(CourseNoteEntryPage.NoteId)}={n.NoteId}");
            }
        }
    }
}
