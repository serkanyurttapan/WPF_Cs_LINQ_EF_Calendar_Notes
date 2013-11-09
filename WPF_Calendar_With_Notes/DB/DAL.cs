using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPF_Calendar_With_Notes.DB
{
    class DAL
    {
        private static readonly Lazy<Model.NotesEntities1> _instancja = new Lazy<Model.NotesEntities1>(() => new Model.NotesEntities1());

        private DAL() { }

        public static Model.NotesEntities1 Instancja
        {
            get
            {
                return _instancja.Value;

            }
        }


    }
}
