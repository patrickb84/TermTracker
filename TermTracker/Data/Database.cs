using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        public Task<List<Term>> GetTermsAsync()
        {
            return database.Table<Term>().ToListAsync();
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

        public Task<List<Course>> GetCoursesAsync(int termId)
        {
            return database.Table<Course>()
                .Where(i => i.CourseTermId == termId)
                .ToListAsync();
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

        #region Notification

        public Task<Notification> GetNotificationAsync(int id)
        {
            return database.Table<Notification>()
                            .Where(i => i.NotificationId == id)
                            .FirstOrDefaultAsync();
        }

        public Task<List<Notification>> GetNotificationsAsync()
        {
            return database.Table<Notification>().ToListAsync();
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

    }
}