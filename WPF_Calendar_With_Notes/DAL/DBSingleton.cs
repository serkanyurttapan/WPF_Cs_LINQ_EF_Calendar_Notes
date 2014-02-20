using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Calendar_With_Notes.DAL
{
    public class DBSingleton
    {
        private static readonly Lazy<NotesContext> _instancja = new Lazy<NotesContext>(() => new NotesContext());

        private DBSingleton() { }

        public static NotesContext Instancja
        {
            get
            {
                return _instancja.Value;

            }
        }



    }
}
