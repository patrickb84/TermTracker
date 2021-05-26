using System;
using System.Collections.Generic;
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

            if (active.AssessmentType == "OBJECTIVE")
                course.O_AssessmentRef = active.Ref;
            if (active.AssessmentType == "PERFORMANCE")
                course.P_AssessmentRef = active.Ref;

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
                await App.Database.DeleteAssessmentAsync(a);

                await Shell.Current.GoToAsync("..");
            }
        }
    }
}