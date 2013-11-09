using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using WPF_Calendar_With_Notes.Model;
using System.Windows.Controls;
using System.Windows;

namespace WPF_Calendar_With_Notes.Utilities.Commands
{
    public class ApplicationNewNoteCommand : ICommand
    {

        private CalendarEngine m_cal_engine;

        public ApplicationNewNoteCommand(CalendarEngine _cal_engine)
        {
            m_cal_engine = _cal_engine;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }


        public void Execute(object parameter)
        {
            PositionOfDay pozycja = new PositionOfDay() { CurrentHour = 0, CurrentMinute = 0, CurrentNote = String.Empty };

            WindowOfPositions okno = new WindowOfPositions(pozycja, m_cal_engine);
            var x = okno.ShowDialog().Value;
            if (x)
            {
                if (pozycja.CurrentNote.Length>=498) pozycja.CurrentNote = pozycja.CurrentNote.Remove(498);

                int res = m_cal_engine.AddNoteToDB(pozycja.CurrentNote,  pozycja.CurrentHour, pozycja.CurrentMinute);

                if (res == -1)
                    if (MessageBox.Show("Podana godzina i minuta jest już zajęta.\nPoprzednia notatka zostanie usunięta.\nKontynuować?", "Zapisać nową notatkę na miejscu starej?", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                    {
                        m_cal_engine.RemoveNoteFromDB( pozycja.CurrentHour, pozycja.CurrentMinute);
                        m_cal_engine.AddNoteToDB( pozycja.CurrentNote, pozycja.CurrentHour, pozycja.CurrentMinute);

                    }

                m_cal_engine.UpdateOfPositions();

            }

        }

        public event EventHandler CanExecuteChanged;
    }
}
