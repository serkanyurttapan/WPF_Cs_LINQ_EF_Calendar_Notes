using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Calendar_With_Notes.DAL
{
    public class NotesContext : DbContext
    {
        public NotesContext() : base("NotesContext")
        {
            //Database.SetInitializer<NotesContext>(new CreateDatabaseIfNotExists<NotesContext>());
            //Database.SetInitializer<NotesContext>(new DropCreateDatabaseIfModelChanges<NotesContext>());
            //Database.SetInitializer<NotesContext>(new DropCreateDatabaseAlways<NotesContext>());
            //Database.SetInitializer<NotesContext>(new NotesInitializer());        
        }

        public DbSet<Model.Note> Notes { get; set; }
    }
}
