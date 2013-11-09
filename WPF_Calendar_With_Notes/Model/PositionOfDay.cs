using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace WPF_Calendar_With_Notes.Model
{
    public class PositionOfDay : INotifyPropertyChanged, IDataErrorInfo
    {
        
        public PositionOfDay()
        {

        }

        private int m_NumberOfPosition;
        public int NumberOfPosition
        {
            get { return m_NumberOfPosition; }
            set { m_NumberOfPosition = value; }
        }


        private short m_OldHour;
        public short OldHour
        {
            get { return m_OldHour; }
            set { m_OldHour = value; }
        }
        


        private short m_CurrentHour;
        public short CurrentHour
        {
            get { return m_CurrentHour; }
            set
            {
                m_CurrentHour = value;
                WyslijPowiadomienie("CurrentHour");
            }
        }

        private short m_OldMinute;
        public short OldMinute
        {
            get { return m_OldMinute; }
            set { m_OldMinute = value; }
        }
        

        private short m_CurrentMinute;
        public short CurrentMinute
        {
            get { return m_CurrentMinute; }
            set
            {
                m_CurrentMinute = value;
                WyslijPowiadomienie("CurrentMinute");
            }
        }


        private string m_OldNote="";
        public string OldNote
        {
            get { return m_OldNote; }
            set { m_OldNote = value; }
        }
        

        private string m_CurrentNote="";
        public string CurrentNote
        {
            get { return m_CurrentNote; }
            set
            {
                m_CurrentNote = value;
                WyslijPowiadomienie("CurrentNote");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void WyslijPowiadomienie(string nazwaWlasciwosci)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(nazwaWlasciwosci));
            }
        }


        public string Error
        {
            get { return "Postaraj się chcieć wpisać to dobrze"; }
        }

        public string this[string columnName]
        {
            get {
                switch (columnName)
                {
                    case "CurrentHour":
                        if (CurrentHour < 0 || CurrentHour > 23)
                        {
                            return "Wpisz godzinę pomiędzy 0 i 23";
                        }
                        break;
                    case "CurrentMinute":
                        if (CurrentMinute < 0 || CurrentMinute > 59)
                        {
                            return "Wpisz minute z przedziału 0-59";
                        }
                        break;
                    case "CurrentNote":
                        if (CurrentNote == null) break;
                        if (CurrentNote.Length>498)
                        {
                            return "Notatka za długa. Max długość notatki to 498 znaków";
                        }
                        break;


                }
                return "";
            
            }
        }
    }
}
