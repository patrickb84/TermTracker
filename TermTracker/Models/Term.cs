using System;
using SQLite;
namespace TermTracker.Models
{
    public class Term
    {
        [PrimaryKey, AutoIncrement]
        public int TermId { get; set; }
        public string TermTitle { get; set; }
        public DateTime TermStartDate { get; set; }
        public DateTime TermEndDate { get; set; }

        public string TermDates
        {
            get { return $"{TermStartDate:MMMM d, yyyy} - {TermEndDate:MMMM d, yyyy}"; }
        }
    }
}