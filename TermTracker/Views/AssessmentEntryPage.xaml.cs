using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plugin.LocalNotifications;
using TermTracker.Models;
using Xamarin.Forms;

namespace TermTracker.Views
{
    [QueryProperty(nameof(AssessmentId), nameof(AssessmentId))]
    public partial class AssessmentEntryPage : ContentPage
    {
        public string AssessmentId
        {
            set { Load(value); }
        }

        public AssessmentEntryPage()
        {
            InitializeComponent();
            BindingContext = new Assessment();
            SetButtonLayoutToNew(false);
        }

        public AssessmentEntryPage(string assessmentType, int courseId)
        {
            InitializeComponent();
            BindingContext = new Assessment
            {
                AssessmentCourseId = courseId,
                AssessmentType = assessmentType
            };
            SetButtonLayoutToNew(true);

            AssesmentDatePicker.Date = DateTime.Now;
        }

        async void Load(string assessmentId)
        {
            try
            {
                int id = Convert.ToInt32(assessmentId);
                Assessment a = await App.Database.GetAssessmentAsync(id);
                BindingContext = a;
                SetButtonLayoutToNew(false);
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to load assessment.");
            }
        }

        void SetButtonLayoutToNew(bool saveNew)
        {
            BtnSaveNew.IsVisible = saveNew;
            BtnsEdit.IsVisible = !saveNew;
        }

        async void SaveButton_Click(object sender, EventArgs e)
        {
            var assessmentId = DateTime.Now.ToString("yyyyMMddHHmmss");
            var active = (Assessment)BindingContext;
            active.Ref = assessmentId;

            if (string.IsNullOrWhiteSpace(active.AssessmentTitle))
            {
                await DisplayAlert("Required Field", "The assessment must have a title.", "OK");
                return;
            }

            var course = await App.Database.GetCourseAsync(active.AssessmentCourseId);

            if (active.AssessmentType == "OBJECTIVE") course.O_AssessmentRef = active.Ref;
            if (active.AssessmentType == "PERFORMANCE") course.P_AssessmentRef = active.Ref;

            active.NotificationDueId = await NotificationAction(active.NotificationDueId, ToggleNotify.IsToggled);

            await App.Database.SaveCourseAsync(course);
            await App.Database.SaveAssessmentAsync(active);
            await Navigation.PopAsync();
        }

        async void DeleteButton_Click(object sender, EventArgs e)
        {
            bool response = await DisplayAlert("Delete this assessment?",
                "This action is irreversible.", "Delete", "Cancel");
            if (response)
            {
                var a = (Assessment)BindingContext;
                var course = await App.Database.GetCourseAsync(a.AssessmentCourseId);

                if (a.AssessmentType == "OBJECTIVE")
                {
                    course.O_AssessmentRef = null;
                }
                else
                {
                    course.P_AssessmentRef = null;
                }
                await App.Database.SaveCourseAsync(course);
                await App.Database.FullDeleteAssessmentAsync(a);

                await Shell.Current.GoToAsync("..");
            }
        }

        async Task<string> NotificationAction(string notificationRelationId, bool turnOn)
        {
            bool notificationExists = !string.IsNullOrEmpty(notificationRelationId);
            Assessment a = (Assessment)BindingContext;

            if (!notificationExists && turnOn)
            {
                // Notification doesn't exist
                var n = new Notification()
                {
                    Title = a.AssessmentTitle,
                    Body = $"{a.AssessmentTitle} is due.",
                    Schedule = a.DueDate,
                    RelationshipId = DateTime.Now.ToString("yyyyMMddHHmmss")
                };

                await App.Database.SaveNotificationAsync(n);
                return n.RelationshipId;
            }
            else if (notificationExists && turnOn)
            {
                // There's an existing notification
                var prevNotification = await App.Database.GetNotificationAsync(notificationRelationId);
                var prevDate = prevNotification.Schedule;
                var upDate = a.DueDate;

                bool DatesNotEqual = DateTime.Compare(prevDate, upDate) != 0;
                bool TitleNotEqual = a.AssessmentTitle != prevNotification.Title;

                if (DatesNotEqual || TitleNotEqual)
                {
                    prevNotification.Title = a.AssessmentTitle;
                    prevNotification.Body = $"{a.AssessmentTitle} is due.";
                    prevNotification.Schedule = upDate;

                    await App.Database.SaveNotificationAsync(prevNotification);
                    return prevNotification.RelationshipId;
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
                // Turn off
                var n = await App.Database.GetNotificationAsync(notificationRelationId);
                CrossLocalNotifications.Current.Cancel(n.NotificationId);
                await App.Database.FullDeleteNotificationAsync(n);
                return null;
            }

            return notificationRelationId;
        }
    }
}