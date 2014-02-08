using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Globalization;
using WPF_Calendar_With_Notes.CommonTypes;
using WPF_Calendar_With_Notes.Utilities;

namespace WPF_Calendar_With_Notes.Model
{
    public class CalendarEngine : INotifyPropertyChanged, IListener
    {

        private string m_LanguageName = "Language Information: " + CultureInfo.CurrentUICulture.DisplayName;
        public string LanguageName
        {
            get { return m_LanguageName; }
            set
            {
                m_LanguageName = value;
                WyslijPowiadomienie("LanguageName");
            }
        }


        private int m_NumberOfPositionForDataGrid;
        public int NumberOfPositionForDataGrid
        {
            get { return m_NumberOfPositionForDataGrid; }
            set
            {
                m_NumberOfPositionForDataGrid = value;
                WyslijPowiadomienie("NumberOfPositionForDataGrid");
            }
        }

        private DateTime m_selected_date;
        public DateTime Selected_Date
        {
            get { return m_selected_date; }
            set { m_selected_date = value; WyslijPowiadomienie("WybranaData"); }
        }

        public ObservableCollection<PositionOfDay> Positions { get; set; }

        private IEventBroker m_Broker;
        public NotesEntities m_notesDB;
        public CalendarEngine(IEventBroker broker)
        {
            m_Broker = broker;

            Positions = new ObservableCollection<PositionOfDay>();

            DateTime dt_tmp = DateTime.Now;
            m_selected_date = new DateTime(dt_tmp.Year, dt_tmp.Month, dt_tmp.Day);
            
            m_notesDB = DB.DAL.Instancja;

            m_Broker.RegisterFor(EventType.LanguageChanged, this);
        }


        public void NotifyMe(EventType type, object data)
        {
            LanguageName = "Language Information: " + CultureInfo.CurrentUICulture.DisplayName;
        }

        public void UpdateOfPositions()
        {
            Positions.Clear();
            List<Tuple<short, short, string>> lista = GetNotesForSelectedDay(Selected_Date);

            if (lista == null)
            {
                NumberOfPositionForDataGrid = 0;
            }
            else
            {
                int i = 0;
                lista.ForEach(tuple =>
                {
                    Positions.Add(new PositionOfDay() { CurrentHour = tuple.Item1, OldHour = tuple.Item1, CurrentMinute = tuple.Item2, OldMinute = tuple.Item2, CurrentNote = tuple.Item3, OldNote = tuple.Item3, NumberOfPosition = i++ });
                });

                NumberOfPositionForDataGrid = Positions.Count;
            }
        }

        
        public List<Tuple<short, short, string>> GetNotesForSelectedDay(DateTime date)
        {
            List<Tuple<short, short, string>> retList1 = new List<Tuple<short, short, string>>();
            
            foreach (var item in m_notesDB.Notes.Where(item => item.Note_DateTime.Year==date.Year &&
                                                        item.Note_DateTime.Month == date.Month &&
                                                        item.Note_DateTime.Day == date.Day
                                                        ).ToList())
            {
                retList1.Add(new Tuple<short, short, string>((short)item.Note_DateTime.Hour, (short)item.Note_DateTime.Minute,
                    item.Note_String));
            }

            List<Tuple<short, short, string>> retList2
                = retList1.OrderBy(a => a.Item1).ThenBy(a => a.Item2).ToList();
              
            return retList2;
        }


        public int AddNoteToDB(string note, short hour, short minute)
        {
            int res2=-1;

            res2 = AddNote_ForDB_hlp(note, Selected_Date, hour, minute);

            if (res2 == 0) return 0;

            else
                return -1;
        }

        private int AddNote_ForDB_hlp(string _note, DateTime _date, short _hour, short _minute)
        {

            //m_DBNotatki.Database.ExecuteSqlCommand("delete from NotatkaEncja");
            Note n = new Note();
            n.Note_Id = Guid.NewGuid();
            n.Note_String = _note;

            DateTime dt = new DateTime(_date.Year, _date.Month, _date.Day, _hour, _minute, 0);
            n.Note_DateTime = dt;

            m_notesDB.Notes.AddObject(n);
            //m_DBNotatki.NotatkaEncjas.Add(ne);

            m_notesDB.SaveChanges();
            return 0;
        }


        public int DateHourMinute_Busy(DateTime date, short hour, short minute)
        {
            DateTime Date_tmp = new DateTime(date.Year, date.Month, date.Day);

            var q = m_notesDB.Notes.Where(item =>
                item.Note_DateTime.Year==date.Year                
                &&
                item.Note_DateTime.Month==date.Month                
                &&
                item.Note_DateTime.Day==date.Day
                &&
                item.Note_DateTime.Hour==date.Hour
                &&
                item.Note_DateTime.Minute==date.Minute                
                );

            if (q != null) return 1;
            else
                return 0;
        }


        public int RemoveNoteFromDB(short hour, short minute)
        {
            int res1 = RemoveNoteFromDBhlp(Selected_Date, hour, minute);
            if (res1 == 0)
                return 0;
            return -1;
        }


        private int RemoveNoteFromDBhlp(DateTime date, short hour, short minute)
        {

            var year = date.Year;
            var month = date.Month;
            var day = date.Day;

            var q = m_notesDB.Notes.Where(item => item.Note_DateTime.Year == year && item.Note_DateTime.Month == month && item.Note_DateTime.Day == day && item.Note_DateTime.Hour == hour && item.Note_DateTime.Minute == minute).ToList();
                
            foreach (var item in q)
            {
                m_notesDB.DeleteObject(item);
            }

            if (q.Count > 0)
            {
                m_notesDB.SaveChanges();
                return 0;

            }

            return -1;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void WyslijPowiadomienie(string NazwaWlasciwosci)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(NazwaWlasciwosci));
            }
        }


    }
}
