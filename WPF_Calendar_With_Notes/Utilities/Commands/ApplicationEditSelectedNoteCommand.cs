using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Controls;

using WPF_Calendar_With_Notes.Model;
using System.Windows;

namespace WPF_Calendar_With_Notes.Utilities.Commands
{
    public class ApplicationEditSelectedNoteCommand : ICommand
    {

        private CalendarEngine m_cal_engine;
        private DataGrid m_DataGrid;

        public ApplicationEditSelectedNoteCommand(CalendarEngine _cal_engine, DataGrid _DataGrid)
        {
            m_cal_engine = _cal_engine;
            m_DataGrid = _DataGrid;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {

            foreach(var row in m_DataGrid.SelectedItems)
            {
                PositionOfDay selectedPosition = row as PositionOfDay;
                if (selectedPosition != null)
                {
                    PositionOfDay primary = new PositionOfDay()
                    {
                        CurrentHour = selectedPosition.CurrentHour,
                        CurrentMinute = selectedPosition.CurrentMinute,
                        CurrentNote = selectedPosition.CurrentNote
                    };

                    WindowOfPositions okno = new WindowOfPositions(selectedPosition, m_cal_engine);
                    var x = okno.ShowDialog().Value;
                    if (x)
                    {
                        m_cal_engine.RemoveNoteFromDB(primary.CurrentHour, primary.CurrentMinute);

                        if (selectedPosition.CurrentNote.Length >= 498) selectedPosition.CurrentNote = selectedPosition.CurrentNote.Remove(498);

                        m_cal_engine.AddNoteToDB(selectedPosition.CurrentNote, selectedPosition.CurrentHour, selectedPosition.CurrentMinute);

                    }
                }
                else//selectedPosition == null
                {
                    PositionOfDay pozycja = new PositionOfDay() { CurrentHour = 0, CurrentMinute = 0, CurrentNote = String.Empty };

                    WindowOfPositions okno = new WindowOfPositions(pozycja, m_cal_engine);
                    var x = okno.ShowDialog().Value;
                    if (x)
                    {
                        if (pozycja.CurrentNote.Length >= 498) pozycja.CurrentNote = pozycja.CurrentNote.Remove(498);

                        int res = m_cal_engine.AddNoteToDB(pozycja.CurrentNote, pozycja.CurrentHour, pozycja.CurrentMinute);

                        if (res == -1)
                            if (MessageBox.Show("Podana godzina i minuta jest już zajęta.\nPoprzednia notatka zostanie usunięta.\nKontynuować?", "Zapisać nową notatkę na miejscu starej?", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                            {
                                m_cal_engine.RemoveNoteFromDB(pozycja.CurrentHour, pozycja.CurrentMinute);
                                m_cal_engine.AddNoteToDB(pozycja.CurrentNote, pozycja.CurrentHour, pozycja.CurrentMinute);

                            }
                    }
                }
            }
            m_cal_engine.UpdateOfPositions();

        }

        public event EventHandler CanExecuteChanged;


    }
}
