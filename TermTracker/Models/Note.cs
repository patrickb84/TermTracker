using System;
using SQLite;
namespace TermTracker.Models
{
    public class Note
    {
        [PrimaryKey, AutoIncrement]
        public int NoteId { get; set; }
        public int NoteCourseId { get; set; }
        public string Text { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}