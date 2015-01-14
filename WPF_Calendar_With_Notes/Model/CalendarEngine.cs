using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Globalization;
using WPF_Calendar_With_Notes.Utilities;
using System.Data.Entity;
using WPF_Calendar_With_Notes.CommonTypes;
using WPF_Calendar_With_Notes.DAL;
using WPF_Calendar_With_Notes.DAL.RealDAL;
using WPF_Calendar_With_Notes.DAL.FakeDAL;

namespace WPF_Calendar_With_Notes
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
                    foreach (var item in m_notesDB.BrokerNotes)
                    {
                        DtList.Add(item.Date);
                    }

                    m_Broker.FireEvent(EventType.SelectedDateChanged, this);

                    UpdateOfPositions();
                }
            }
        }

        public ObservableCollection<PositionOfDay> Positions { get; set; }

        private IEventBroker m_Broker;
        public INotesContext<Note> m_notesDB;
        public CalendarEngine(IEventBroker broker)
        {
            m_Broker = broker;

            m_Broker.RegisterFor(EventType.LanguageChanged, this);

            Positions = new ObservableCollection<PositionOfDay>();

            m_notesDB = DBSingleton<RealNotesContext>.Instancja;

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

            IEnumerable<FieldsOfDataGrid> _list = GetNotesForSelectedDay(m_selected_date);

            if (_list.Count() == 0)
            {
                NumberOfPositionForDataGrid = 0;
            }
            else
            {
                int i = 0;
                foreach (var rowOfDataGrid in _list)
                {
                    Positions.Add(new PositionOfDay()
                    {
                        CurrentHour = rowOfDataGrid.Hour,
                        OldHour = rowOfDataGrid.Hour,
                        CurrentMinute = rowOfDataGrid.Minute,
                        OldMinute = rowOfDataGrid.Minute,
                        CurrentNote = rowOfDataGrid.Note,
                        OldNote = rowOfDataGrid.Note,

                        CurrentUser = rowOfDataGrid.User,
                        OldUser = rowOfDataGrid.User,

                        NumberOfPosition = i++,
                        DateTimeVal = new DateTime(Selected_Date.Year, Selected_Date.Month, Selected_Date.Day,
                            rowOfDataGrid.Hour, rowOfDataGrid.Minute, 0)
                    });
                }

                NumberOfPositionForDataGrid = Positions.Count;
            }
        }


        public IEnumerable<FieldsOfDataGrid> GetNotesForSelectedDay(DateTime date)
        {
            List<FieldsOfDataGrid> retList1 = new List<FieldsOfDataGrid>();

            foreach (var item in m_notesDB.BrokerNotes.Where(item => item.Date.Year == date.Year &&
                                                        item.Date.Month == date.Month &&
                                                        item.Date.Day == date.Day
                                                        ).ToList())
            {
                retList1.Add(new FieldsOfDataGrid()
                {
                    Hour = (short)item.Date.Hour,
                    Minute = (short)item.Date.Minute,
                    Note = item.Message,
                    User = item.User
                });

            }

            var retList2 = retList1.OrderBy(item => item.Hour).ThenBy(item => item.Minute);

            return retList2;
        }


        public int AddNoteToDB(FieldsOfDataGrid fodg)
        {
            int res = DateHourMinute_Busy(Selected_Date, fodg.Hour, fodg.Minute);
            if (res > 0) return -1;

            int res2 = AddNote_ForDB_hlp(Selected_Date, fodg);

            if (res2 == 0) return 0;
            else
                return -2;
        }

        public int DateHourMinute_Busy(DateTime date, short hour, short minute)
        {
            int count = m_notesDB.BrokerNotes.Where(item =>
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


        private int AddNote_ForDB_hlp(DateTime _dt, FieldsOfDataGrid fodg)
        {
            //n.Note_Id = Guid.NewGuid();//baza sdf
            //m_DBNotatki.Database.ExecuteSqlCommand("delete from NotatkaEncja");

            Note n = new Note()
            {
                Message = fodg.Note,
                User = fodg.User,
                Date = new DateTime(_dt.Year, _dt.Month, _dt.Day, fodg.Hour, fodg.Minute, 0)
            };

            m_notesDB.Add(n);

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

            var q = m_notesDB.BrokerNotes.Where(item => item.Date.Year == year && item.Date.Month == month && item.Date.Day == day && item.Date.Hour == hour && item.Date.Minute == minute).ToList();

            foreach (var item in q)
            {
                m_notesDB.Remove(item);
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
