using System;
using System.Collections.Generic;
using TermTracker.Views;
using Xamarin.Forms;

namespace TermTracker
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(TermEntryPage), typeof(TermEntryPage));
            Routing.RegisterRoute(nameof(TermDetailPage), typeof(TermDetailPage));
            Routing.RegisterRoute(nameof(CourseEntryPage), typeof(CourseEntryPage));
            Routing.RegisterRoute(nameof(CourseDetailPage), typeof(CourseDetailPage));
            Routing.RegisterRoute(nameof(AssessmentEntryPage), typeof(AssessmentEntryPage));
            Routing.RegisterRoute(nameof(CourseNotesPage), typeof(CourseNotesPage));
            Routing.RegisterRoute(nameof(CourseNotePage), typeof(CourseNotePage));
        }
    }
}