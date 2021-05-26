using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TermTracker.Models;

using Xamarin.Forms;

namespace TermTracker.Views
{
    [QueryProperty(nameof(CourseId), nameof(CourseId))]
    public partial class CourseDetailPage : ContentPage
    {
        public Course ActiveCourse;
        public string CourseId
        {
            set { Load(value); }
        }
        public Assessment O_assessment;
        public Assessment P_assessment;

        public CourseDetailPage()
        {
            InitializeComponent();

            ObjButton.IsVisible = O_assessment == null;
            ObjFrame.IsVisible = O_assessment != null;

            PerfButton.IsVisible = P_assessment == null;
            PerfFrame.IsVisible = P_assessment != null;
        }

        async void Load(string courseId)
        {
            try
            {
                int id = Convert.ToInt32(courseId);
                ActiveCourse = await App.Database.GetCourseAsync(id);

                if (!string.IsNullOrEmpty(ActiveCourse.O_AssessmentRef))
                {
                    O_assessment = await App.Database.GetAssessmentAsync(
                        ActiveCourse.O_AssessmentRef);
                    ObjDate.Text = O_assessment.DueDate.ToString("MMMM d, yyyy");
                    ObjName.Text = O_assessment.AssessmentTitle;
                }

                if (!string.IsNullOrEmpty(ActiveCourse.P_AssessmentRef))
                {
                    P_assessment = await App.Database.GetAssessmentAsync(
                        ActiveCourse.P_AssessmentRef);
                    PerfDate.Text = P_assessment.DueDate.ToString("MMMM d, yyyy");
                    PerfName.Text = P_assessment.AssessmentTitle;
                }

                UpdateAssessments();
                BindingContext = ActiveCourse;
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to load course.");
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            O_assessment = await CheckUpdate(O_assessment);
            P_assessment = await CheckUpdate(P_assessment);
            UpdateAssessments();
        }

        async Task<Assessment> CheckUpdate(Assessment a)
        {
            if (a != null)
            {
                var c = await App.Database.GetCourseAsync(a.AssessmentCourseId);
                if (a.AssessmentType == "OBJECTIVE")
                {
                    if (string.IsNullOrEmpty(c.O_AssessmentRef)) return null;
                }
                else
                {
                    if (string.IsNullOrEmpty(c.P_AssessmentRef)) return null;
                }
            }
            return a;
        }

        void UpdateAssessments()
        {
            ObjButton.IsVisible = O_assessment == null;
            ObjFrame.IsVisible = O_assessment != null;

            PerfButton.IsVisible = P_assessment == null;
            PerfFrame.IsVisible = P_assessment != null;
        }

        async void Edit_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(
                $"{nameof(CourseEntryPage)}?{nameof(CourseEntryPage.CourseId)}={ActiveCourse.CourseId}");
        }

        async void EditAssessmentP_Handler(System.Object sender, System.EventArgs e)
        {
            if (P_assessment != null)
            {
                await Shell.Current.GoToAsync(
                    $"{nameof(AssessmentEntryPage)}?{nameof(AssessmentEntryPage.AssessmentId)}={P_assessment.AssessmentId}");
            }
            else
            {
                var ae = new AssessmentEntryPage("PERFORMANCE", ActiveCourse.CourseId);
                await Navigation.PushAsync(ae);
            }
        }

        async void EditAssessmentO_Handler(System.Object sender, System.EventArgs e)
        {
            if (O_assessment != null)
            {
                await Shell.Current.GoToAsync(
                    $"{nameof(AssessmentEntryPage)}?{nameof(AssessmentEntryPage.AssessmentId)}={O_assessment.AssessmentId}");
            }
            else
            {
                var ae = new AssessmentEntryPage("OBJECTIVE", ActiveCourse.CourseId);
                await Navigation.PushAsync(ae);
            }
        }

        //async void InstructorInfo_Clicked(System.Object sender, System.EventArgs e)
        //{
        //    var instructorInfoPage = new InstructorInfoPage(ActiveCourse.CourseId);
        //    await Navigation.PushModalAsync(instructorInfoPage);
        //}
    }
}