
using System;
using SQLite;
namespace TermTracker.Models
{
    public class Assessment
    {
        [PrimaryKey, AutoIncrement]
        public int AssessmentId { get; set; }
        public string Ref { get; set; }
        public int AssessmentCourseId { get; set; }
        public string AssessmentTitle { get; set; }
        public DateTime DueDate { get; set; }
        public bool NotifyDueDate { get; set; }
        public string AssessmentType { get; set; }
    }
}