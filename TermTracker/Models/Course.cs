using System;
using System.Collections.Generic;
using SQLite;
namespace TermTracker.Models
{
    public static class CourseStatuses
    {
        public static string[] Statuses
        {
            get
            {
                return new string[]
                {
                    "In Progress",
                    "Completed",
                    "Dropped",
                    "Plan To Take"
                };
            }
        }
    }


    public class Course
    {
        [PrimaryKey, AutoIncrement]
        public int CourseId { get; set; }
        public int CourseTermId { get; set; }

        public string CourseTitle { get; set; }
        public string Status { get; set; }

        public DateTime CourseStartDate { get; set; }
        public DateTime CourseEndDate { get; set; }

        public string NotificationStartId { get; set; }
        public string NotificationEndId { get; set; }

        public bool NfStartOn
        {
            get { return !string.IsNullOrEmpty(NotificationStartId); }
        }
        public bool NfEndOn
        {
            get { return !string.IsNullOrEmpty(NotificationEndId); }
        }

        public string O_AssessmentRef { get; set; }
        public string P_AssessmentRef { get; set; }

        public string InstructorName { get; set; }
        public string InstructorPhone { get; set; }
        public string InstructorEmail { get; set; }

        public string CourseDates
        {
            get { return $"{CourseStartDate:MMMM d, yyyy} - {CourseEndDate:MMMM d, yyyy}"; }
        }

        public string StatusColor
        {
            get { return GetStatusColor(Status); }
        }

        public string GetStatusColor(string status)
        {
            switch (status)
            {
                case "In Progress":
                    return "#28A744";
                case "Completed":
                    return "#037BFE";
                case "Plan To Take":
                    return "#FFC106";
                case "Dropped":
                    return "#6C757D";
                default:
                    return "Transparent";
            }
        }
    }
}