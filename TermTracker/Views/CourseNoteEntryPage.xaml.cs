using System;
using System.Collections.Generic;
using TermTracker.Models;
using Xamarin.Forms;
using Xamarin.Essentials;
using System.Threading.Tasks;

namespace TermTracker.Views
{
    [QueryProperty(nameof(NoteId), nameof(NoteId))]
    public partial class CourseNoteEntryPage : ContentPage
    {
        public string NoteId
        {
            get { return NoteId; }
            set { Load(value); }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            NoteEditor.Focus();
        }

        public CourseNoteEntryPage()
        {
            InitializeComponent();
            BindingContext = new Note();
            ExistingNoteLayout.IsVisible = true;
            NewNoteLayout.IsVisible = false;
        }

        public CourseNoteEntryPage(int courseId)
        {
            InitializeComponent();
            BindingContext = new Note() { NoteCourseId = courseId };
            ExistingNoteLayout.IsVisible = false;
            NewNoteLayout.IsVisible = true;
        }

        async void Load(string termId)
        {
            try
            {
                int id = Convert.ToInt32(termId);
                Note note = await App.Database.GetNoteAsync(id);
                BindingContext = note;
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to load note.");
            }
        }

        async void SaveButton_Click(object sender, EventArgs e)
        {
            var note = (Note)BindingContext;

            if (string.IsNullOrWhiteSpace(note.Text))
            {
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                note.CreatedDate = DateTime.Now;
                await App.Database.SaveNoteAsync(note);
                await Shell.Current.GoToAsync("..");
            }
        }

        async void DeleteButton_Click(object sender, EventArgs e)
        {
            bool response = await DisplayAlert("Delete this note?",
                "This action is irreversible.", "Delete", "Cancel");
            if (response)
            {
                var note = (Note)BindingContext;
                await App.Database.DeleteNoteAsync(note);

                await Shell.Current.GoToAsync("..");
            }
        }

        async void Share_Clicked(System.Object sender, System.EventArgs e)
        {
            var note = (Note)BindingContext;
            if (!string.IsNullOrEmpty(note.Text))
            {
                await Share.RequestAsync(new ShareTextRequest
                {
                    Text = note.Text,
                    Title = "Course Note from TermTracker"
                });
            }
            else
            {
                await DisplayAlert("Not allowed", "Note must have text in order to share.", "Ok");
            }
        }
    }
}
