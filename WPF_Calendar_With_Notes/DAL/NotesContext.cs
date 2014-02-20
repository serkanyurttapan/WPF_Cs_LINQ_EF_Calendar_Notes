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

        }

        public DbSet<Model.Note> Notes { get; set; }
    }
}
