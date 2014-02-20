using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace WPF_Calendar_With_Notes.Model
{
    //Kontrolka Calendar posiada cechę,która jest tu przeszkodą.
    //Kiedy użytkownik klika na dany dzien, wtedy przestają byc zaznaczone inne, wczesniej zaznaczone dni.
    public class Timer_CalendarPatch
    {

        private Menu m_menu;
        private Button m_addButton;
        private Calendar m_GUI_cal;
        private CalendarEngine m_model;
        private Sem m_sem;

        public DispatcherTimer Timer = new DispatcherTimer();

        public Timer_CalendarPatch(Menu _menu, Button _button, Calendar _calendar, CalendarEngine _model, Sem _sem)
        {
            m_sem = _sem;
            m_menu = _menu;
            m_addButton = _button;
            m_GUI_cal = _calendar;
            m_model = _model;

            Timer.Tick += new EventHandler(Timer_Tick);
            Timer.Interval = new TimeSpan(0, 0, 0, 0, 100);//200ms
            Timer.Start();

        }



        public void Timer_Tick(object sender, EventArgs e)
        {
            //m_GUI_cal.SelectedDates.Count == 1 to znaczy, ze uzytkownik właśnie wybrał date w kalendarzu, 
            //czyli ilosc wybranych dat==1 czyli trzeba zrobic update w kalendarzu
            if (m_GUI_cal.SelectedDates.Count == 1)
                if (m_sem.zrobione == 0)
                {
                    m_sem.uzywany = true;

                    m_GUI_cal.SelectedDates.Clear();

                    m_GUI_cal.SelectedDates.Add(m_model.Selected_Date);

                    foreach (var item in m_model.m_notesDB.Notes)
                    {
                        DateTime dt = new DateTime(item.Date.Year, item.Date.Month, item.Date.Day);
                        m_GUI_cal.SelectedDates.Add(dt);
                    }

                    m_sem.zrobione = 1;
                    m_sem.uzywany = false;

                    m_menu.Focus();
                    m_addButton.Focus();

                }
        }

    }

}
