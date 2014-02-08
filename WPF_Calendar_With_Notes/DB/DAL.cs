using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPF_Calendar_With_Notes.DB
{
    class DAL
    {
        private static readonly Lazy<Model.NotesEntities> _instancja = new Lazy<Model.NotesEntities>(() => new Model.NotesEntities());

        private DAL() { }

        public static Model.NotesEntities Instancja
        {
            get
            {
                return _instancja.Value;

            }
        }


    }
}
