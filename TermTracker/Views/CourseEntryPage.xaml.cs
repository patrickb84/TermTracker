using System;
using System.Collections.Generic;
using TermTracker.Models;
using Xamarin.Forms;

namespace TermTracker.Views
{
    [QueryProperty(nameof(CourseId), nameof(CourseId))]
    public partial class CourseEntryPage : ContentPage
    {
        public string CourseId
        {
            set { Load(value); }
        }

        public CourseEntryPage()
        {
            InitializeComponent();
            BindingContext = new Course();
        }

        public CourseEntryPage(int termId)
        {
            InitializeComponent();
            BindingContext = new Course() { CourseTermId = termId };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            var courseStatuses = CourseStatuses.Statuses;
            foreach (string status in courseStatuses)
            {
                statusPicker.Items.Add(status);
            }
        }

        async void Load(string courseId)
        {
            try
            {
                int id = Convert.ToInt32(courseId);
                Course course = await App.Database.GetCourseAsync(id);
                BindingContext = course;
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to load course.");
            }
        }

        async void SaveButton_Click(object sender, EventArgs e)
        {
            var course = (Course)BindingContext;

            if (string.IsNullOrWhiteSpace(course.CourseTitle))
            {
                await DisplayAlert("Required Field",
                    "The term must have a title.", "OK");
                return;
            }
            if (DateTime.Compare(course.CourseStartDate,
                course.CourseEndDate) != -1)
            {
                await DisplayAlert("Incorrect Dates",
                    "The start date must come before the end date.", "OK");
                return;
            }
            if (string.IsNullOrWhiteSpace(course.InstructorName))
            {
                await DisplayAlert("Required Field",
                    "Instructor must have a name.", "OK");
                return;
            }
            if (string.IsNullOrWhiteSpace(course.InstructorPhone))
            {
                await DisplayAlert("Required Field",
                    "Instructor must have a phone number.", "OK");
                return;
            }
            if (string.IsNullOrWhiteSpace(course.InstructorEmail))
            {
                await DisplayAlert("Required Field",
                    "Instructor must have an email.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(course.Status))
            {
                course.Status = "Plan To Take";
            }

            await App.Database.SaveCourseAsync(course);
            await Navigation.PopAsync();
        }

        async void DeleteButton_Click(object sender, EventArgs e)
        {
            bool response = await DisplayAlert("Delete this course?",
                "This action is irreversible.", "Delete", "Cancel");
            if (response)
            {
                var item = (Course)BindingContext;
                await App.Database.DeleteCourseAsync(item);

                // Navigate backwards
                await Shell.Current.GoToAsync("../..");
            }
        }
    }
}