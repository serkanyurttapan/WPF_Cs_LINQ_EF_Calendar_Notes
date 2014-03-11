using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Globalization;
using WPF_Calendar_With_Notes.CommonTypes;
using WPF_Calendar_With_Notes.Utilities;

namespace WPF_Calendar_With_Notes.Model
{
    public class CalendarEngine : INotifyPropertyChanged, IListener, IData
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

        private List<DateTime> m_DtList = new List<DateTime>();
        public List<DateTime> DtList
        {
            get
            {
                return m_DtList;
            }
            set
            {
                m_DtList = value;
            }
        }


        private DateTime m_selected_date;
        public DateTime Selected_Date
        {
            get
            {
                return m_selected_date;
            }

            set
            {
                if (value != m_selected_date)
                {
                    m_selected_date = value;

                    //W kontrolce Calendar, po kliknięciu
                    //kolekcja dat przestaje byc zaznaczona
                    //rozwiazanie: 1. Watek uaktualniajacy lub 2. powiadomienie MainWindow o potrzebie zaznaczenia
                    DtList.Clear();
                    DtList.Add(m_selected_date);
                    foreach (var item in m_notesDB.Notes)
                    {
                        DtList.Add(item.Date);
                    }

                    m_Broker.FireEvent(EventType.SelectedDateChanged, this);

                    UpdateOfPositions();

                }
            }
        }

        public ObservableCollection<PositionOfDay> Positions { get; set; }

        MainWindow GUI;

        private IEventBroker m_Broker;
        public DAL.NotesContext m_notesDB;
        public CalendarEngine(IEventBroker broker)
        {
            m_Broker = broker;

            m_Broker.RegisterFor(EventType.LanguageChanged, this);

            Positions = new ObservableCollection<PositionOfDay>();

            m_notesDB = DAL.DBSingleton.Instancja;

            DateTime dt_tmp = DateTime.Now;
            Selected_Date = new DateTime(dt_tmp.Year, dt_tmp.Month, dt_tmp.Day);
        }


        public void NotifyMe(EventType type, object data)
        {
            if (type == EventType.LanguageChanged)
            {
                LanguageName = "Language Information: " + CultureInfo.CurrentUICulture.DisplayName;
            }
        }

        public void UpdateOfPositions()
        {

            if (Positions.Count != 0)
                Positions.Clear();
            List<Tuple<short, short, string>> lista = GetNotesForSelectedDay(m_selected_date);

            if (lista.Count == 0)
            {
                NumberOfPositionForDataGrid = 0;
            }
            else
            {
                int i = 0;
                lista.ForEach(tuple =>
                {
                    Positions.Add(new PositionOfDay()
                    {
                        CurrentHour = tuple.Item1,
                        OldHour = tuple.Item1,
                        CurrentMinute = tuple.Item2,
                        OldMinute = tuple.Item2,
                        CurrentNote = tuple.Item3,
                        OldNote = tuple.Item3,
                        NumberOfPosition = i++,
                        DateTimeVal = new DateTime(Selected_Date.Year, Selected_Date.Month, Selected_Date.Day,
                            tuple.Item1, tuple.Item2, 0)
                    });
                });

                NumberOfPositionForDataGrid = Positions.Count;
            }
        }


        public List<Tuple<short, short, string>> GetNotesForSelectedDay(DateTime date)
        {
            List<Tuple<short, short, string>> retList1 = new List<Tuple<short, short, string>>();

            foreach (var item in m_notesDB.Notes.Where(item => item.Date.Year == date.Year &&
                                                        item.Date.Month == date.Month &&
                                                        item.Date.Day == date.Day
                                                        ).ToList())
            {
                retList1.Add(new Tuple<short, short, string>((short)item.Date.Hour, (short)item.Date.Minute,
                    item.Message));
            }

            List<Tuple<short, short, string>> retList2
                = retList1.OrderBy(a => a.Item1).ThenBy(a => a.Item2).ToList();

            return retList2;
        }


        public int AddNoteToDB(string note, short hour, short minute)
        {
            int res = DateHourMinute_Busy(Selected_Date, hour, minute);
            if (res > 0) return -1;

            int res2 = AddNote_ForDB_hlp(note, Selected_Date, hour, minute);

            if (res2 == 0) return 0;
            else
                return -2;
        }

        public int DateHourMinute_Busy(DateTime date, short hour, short minute)
        {
            int count = m_notesDB.Notes.Where(item =>
                item.Date.Year == date.Year
                &&
                item.Date.Month == date.Month
                &&
                item.Date.Day == date.Day
                &&
                item.Date.Hour == hour
                &&
                item.Date.Minute == minute
                ).ToList().Count;

            if (count == 0) return 0;
            else
                return count;
        }


        private int AddNote_ForDB_hlp(string _note, DateTime _date, short _hour, short _minute)
        {
            //m_DBNotatki.Database.ExecuteSqlCommand("delete from NotatkaEncja");
            Note n = new Note();

            //n.Note_Id = Guid.NewGuid();//baza sdf
            n.Message = _note;

            DateTime dt = new DateTime(_date.Year, _date.Month, _date.Day, _hour, _minute, 0);
            n.Date = dt;

            m_notesDB.Notes.Add(n);

            var i = m_notesDB.SaveChanges();

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

            var q = m_notesDB.Notes.Where(item => item.Date.Year == year && item.Date.Month == month && item.Date.Day == day && item.Date.Hour == hour && item.Date.Minute == minute).ToList();

            foreach (var item in q)
            {
                m_notesDB.Notes.Remove(item);
            }

            if (q.Count > 0)
            {
                m_notesDB.SaveChanges();
                return 0;
            }

            return 0;
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

        ~CalendarEngine()
        {
            m_Broker.UnregisterFromAll(this);
        }


    }

}
