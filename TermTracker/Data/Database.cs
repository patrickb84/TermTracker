using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plugin.LocalNotifications;
using SQLite;
using TermTracker.Models;

namespace TermTracker.Data
{
    public class Database
    {
        readonly SQLiteAsyncConnection database;

        public Database(string dbPath)
        {
            database = new SQLiteAsyncConnection(dbPath);

            database.CreateTableAsync<Term>().Wait();
            database.CreateTableAsync<Course>().Wait();
            database.CreateTableAsync<Notification>().Wait();
            database.CreateTableAsync<Assessment>().Wait();
            database.CreateTableAsync<Note>().Wait();
        }

        #region Terms
        public async Task<List<Term>> GetTermsAsync()
        {
            var list = await database.Table<Term>().ToListAsync();
            list.Sort((x, y) => x.TermStartDate.CompareTo(y.TermStartDate));
            return list;
        }

        public Task<Term> GetTermAsync(int id)
        {
            return database.Table<Term>()
                            .Where(i => i.TermId == id)
                            .FirstOrDefaultAsync();
        }

        public Task<int> SaveTermAsync(Term term)
        {
            if (term.TermId != 0)
            {
                return database.UpdateAsync(term);
            }
            else
            {
                return database.InsertAsync(term);
            }
        }

        public Task<int> DeleteTermAsync(Term term)
        {
            return database.DeleteAsync(term);
        }
        #endregion

        #region Courses
        public Task<List<Course>> GetCoursesAsync()
        {
            return database.Table<Course>().ToListAsync();
        }

        public async Task<List<Course>> GetCoursesAsync(int termId)
        {
            var list = await database.Table<Course>()
                .Where(i => i.CourseTermId == termId)
                .ToListAsync();
            list.Sort((x, y) => x.CourseStartDate.CompareTo(y.CourseStartDate));
            return list;
        }

        public Task<Course> GetCourseAsync(int id)
        {
            return database.Table<Course>()
                            .Where(i => i.CourseId == id)
                            .FirstOrDefaultAsync();
        }

        public Task<int> SaveCourseAsync(Course course)
        {
            if (course.CourseId != 0)
            {
                return database.UpdateAsync(course);
            }
            else
            {
                return database.InsertAsync(course);
            }
        }

        public Task<int> DeleteCourseAsync(Course course)
        {
            return database.DeleteAsync(course);
        }
        #endregion

        #region Notifications
        public Task<Notification> GetNotificationAsync(int id)
        {
            return database.Table<Notification>()
                            .Where(i => i.NotificationId == id)
                            .FirstOrDefaultAsync();
        }

        public Task<Notification> GetNotificationAsync(string id)
        {
            return database.Table<Notification>()
                            .Where(i => i.RelationshipId == id)
                            .FirstOrDefaultAsync();
        }

        public async Task<List<Notification>> GetNotificationsAsync()
        {
            var list = await database.Table<Notification>().ToListAsync();
            list.Sort((x, y) => x.Schedule.CompareTo(y.Schedule));
            return list;
        }

        public Task<int> SaveNotificationAsync(Notification n)
        {
            if (n.NotificationId != 0)
            {
                return database.UpdateAsync(n);
            }
            else
            {
                return database.InsertAsync(n);
            }
        }

        public Task<int> DeleteNotificationAsync(Notification n)
        {
            return database.DeleteAsync(n);
        }
        #endregion

        #region Assessments
        public Task<Assessment> GetAssessmentAsync(int assessmentId)
        {
            return database.Table<Assessment>()
                            .Where(i => i.AssessmentId == assessmentId)
                            .FirstOrDefaultAsync();
        }

        public Task<Assessment> GetAssessmentAsync(string assessmentRef)
        {
            return database.Table<Assessment>()
                            .Where(i => i.Ref == assessmentRef)
                            .FirstOrDefaultAsync();
        }

        public Task<int> SaveAssessmentAsync(Assessment assessment)
        {
            if (assessment.AssessmentId != 0)
            {
                return database.UpdateAsync(assessment);
            }
            else
            {
                return database.InsertAsync(assessment);
            }
        }

        public Task<int> DeleteAssessmentAsync(Assessment assessment)
        {
            return database.DeleteAsync(assessment);
        }
        #endregion

        #region Notes
        public Task<Note> GetNoteAsync(int id)
        {
            return database.Table<Note>()
                            .Where(i => i.NoteId == id)
                            .FirstOrDefaultAsync();
        }

        public async Task<List<Note>> GetNotesAsync(int courseId)
        {
            var list = await database.Table<Note>()
                .Where(i => i.NoteCourseId == courseId).ToListAsync();
            list.Sort((x, y) => x.CreatedDate.CompareTo(y.CreatedDate));
            return list;
        }

        public Task<int> SaveNoteAsync(Note n)
        {
            if (n.NoteId != 0)
            {
                return database.UpdateAsync(n);
            }
            else
            {
                return database.InsertAsync(n);
            }
        }

        public Task<int> DeleteNoteAsync(Note n)
        {
            return database.DeleteAsync(n);
        }
        #endregion

        #region Cascading Deletes
        public async Task<int> FullDeleteTermAsync(Term t)
        {
            var courses = await App.Database.GetCoursesAsync(t.TermId);
            if (courses.Count > 0)
            {
                foreach (Course c in courses)
                {
                    await App.Database.FullDeleteCourseAsync(c);
                }
            }
            return await App.Database.DeleteTermAsync(t);
        }

        public async Task<int> FullDeleteCourseAsync(Course c)
        {
            // Delete notifications
            if (!string.IsNullOrEmpty(c.NotificationStartId))
            {
                var n = await App.Database.GetNotificationAsync(c.NotificationStartId);
                await App.Database.FullDeleteNotificationAsync(n);
            }
            if (!string.IsNullOrEmpty(c.NotificationEndId))
            {
                var n = await App.Database.GetNotificationAsync(c.NotificationEndId);
                await App.Database.FullDeleteNotificationAsync(n);
            }
            // Delete assessments
            if (!string.IsNullOrEmpty(c.O_AssessmentRef))
            {
                var a = await App.Database.GetAssessmentAsync(c.O_AssessmentRef);
                await App.Database.FullDeleteAssessmentAsync(a);
            }
            if (!string.IsNullOrEmpty(c.P_AssessmentRef))
            {
                var a = await App.Database.GetAssessmentAsync(c.P_AssessmentRef);
                await App.Database.FullDeleteAssessmentAsync(a);
            }
            // Delete Notes
            var notes = await App.Database.GetNotesAsync(c.CourseId);
            if (notes.Count > 0)
            {
                foreach (Note n in notes)
                {
                    await App.Database.DeleteNoteAsync(n);
                }
            }
            // Delete Course
            return await App.Database.DeleteCourseAsync(c);
        }

        public async Task<int> FullDeleteAssessmentAsync(Assessment a)
        {
            if (!string.IsNullOrEmpty(a.NotificationDueId))
            {
                var n = await App.Database.GetNotificationAsync(a.NotificationDueId);
                await App.Database.FullDeleteNotificationAsync(n);
            }
            return await App.Database.DeleteAssessmentAsync(a);
        }

        public async Task<int> FullDeleteNotificationAsync(Notification n)
        {
            try
            {
                CrossLocalNotifications.Current.Cancel(n.NotificationId);
            }
            catch
            {
                // log: no cross local notification
            }
            return await App.Database.DeleteNotificationAsync(n);
        }
        #endregion
    }
}