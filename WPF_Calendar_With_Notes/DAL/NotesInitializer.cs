using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Calendar_With_Notes.Model;
using System.Data.Entity;

namespace WPF_Calendar_With_Notes.DAL
{
    public class NotesInitializer : System.Data.Entity.CreateDatabaseIfNotExists<NotesContext>
    {
        protected override void Seed(NotesContext context)
        {
            var notes = new List<Note>
            {
                new Note {Date=DateTime.Now,Message="Plan"}
            };
            notes.ForEach(n=>context.Notes.Add(n));
            context.SaveChanges();
        }
    }
}
