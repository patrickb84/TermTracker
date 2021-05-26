using System;
using System.Collections.Generic;
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
        public string RelationshipType { get; set; }
        public int RelationshipId { get; set; }
    }
}