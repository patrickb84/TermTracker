using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;

namespace TermTracker.Models
{
    // https://github.com/edsnider/LocalNotificationsPlugin

    public class Notification
    {
        [PrimaryKey, AutoIncrement]
        public int NotificationId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime Schedule { get; set; }
        public string ScheduleText {
            get {
                return Schedule.ToString("MMMM d, yyyy");
            }
        }
        public string RelationshipId { get; set; }

        public Notification() { }

        public Notification(string title, string body, DateTime schedule)
        {
            Title = title;
            Body = body;
            Schedule = schedule;
            RelationshipId = DateTime.Now.ToString("yyyyMMddHHmmss");
        }
    }
}