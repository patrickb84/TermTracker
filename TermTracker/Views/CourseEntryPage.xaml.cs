using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plugin.LocalNotifications;
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
            var c = (Course)BindingContext;

            if (string.IsNullOrWhiteSpace(c.CourseTitle))
            {
                await DisplayAlert("Required Field",
                    "The term must have a title.", "OK");
                return;
            }
            if (DateTime.Compare(c.CourseStartDate,
                c.CourseEndDate) != -1)
            {
                await DisplayAlert("Incorrect Dates",
                    "The start date must come before the end date.", "OK");
                return;
            }
            if (string.IsNullOrWhiteSpace(c.InstructorName))
            {
                await DisplayAlert("Required Field",
                    "Instructor must have a name.", "OK");
                return;
            }
            if (string.IsNullOrWhiteSpace(c.InstructorPhone))
            {
                await DisplayAlert("Required Field",
                    "Instructor must have a phone number.", "OK");
                return;
            }
            if (string.IsNullOrWhiteSpace(c.InstructorEmail))
            {
                await DisplayAlert("Required Field",
                    "Instructor must have an email.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(c.Status))
            {
                c.Status = "Plan To Take";
            }

            c.NotificationStartId = await NotificationAction(c.NotificationStartId, SwitchTermStart.IsToggled, true);
            c.NotificationEndId = await NotificationAction(c.NotificationEndId, SwitchTermEnd.IsToggled, false);

            await App.Database.SaveCourseAsync(c);
            await Navigation.PopAsync();
        }

        async void DeleteButton_Click(object sender, EventArgs e)
        {
            bool response = await DisplayAlert("Delete this course?",
                "This action is irreversible.", "Delete", "Cancel");
            if (response)
            {
                var c = (Course)BindingContext;
                await App.Database.FullDeleteCourseAsync(c);
                await Shell.Current.GoToAsync("../..");
            }
        }

        async Task<string> NotificationAction(string notificationRelationId, bool turnOn, bool isStartDate)
        {
            bool notificationExists = !string.IsNullOrEmpty(notificationRelationId);
            Course c = (Course)BindingContext;

            if (!notificationExists && turnOn)
            {
                // Notification doesn't exist
                var n = new Notification()
                {
                    Title = c.CourseTitle,
                    Body = isStartDate ? $"{c.CourseTitle} will begin." : $"{c.CourseTitle} will end.",
                    Schedule = isStartDate ? c.CourseStartDate : c.CourseEndDate,
                    RelationshipId = DateTime.Now.ToString("yyyyMMddHHmmss")
                };

                await App.Database.SaveNotificationAsync(n);
                return n.RelationshipId;
            }
            else if (notificationExists && turnOn)
            {
                // There's an existing notification
                var n = await App.Database.GetNotificationAsync(notificationRelationId);
                var existing = n.Schedule;
                var update = isStartDate ? c.CourseStartDate : c.CourseEndDate;
                if (DateTime.Compare(existing, update) != 0)
                {
                    var nf = await App.Database.GetNotificationAsync(notificationRelationId);
                    nf.Schedule = update;
                    await App.Database.SaveNotificationAsync(nf);
                    return nf.RelationshipId;
                }
                else
                    return notificationRelationId;
            }
            else if (!notificationExists && !turnOn)
            {
                return null;
            }
            else if (notificationExists && !turnOn)
            {
                // Turn off notification
                var n = await App.Database.GetNotificationAsync(notificationRelationId);
                CrossLocalNotifications.Current.Cancel(n.NotificationId);
                await App.Database.FullDeleteNotificationAsync(n);
                return null;
            }

            return notificationRelationId;
        }
    }
}