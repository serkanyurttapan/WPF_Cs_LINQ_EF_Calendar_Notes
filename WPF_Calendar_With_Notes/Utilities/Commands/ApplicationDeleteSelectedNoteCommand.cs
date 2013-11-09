using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;

using WPF_Calendar_With_Notes.Model;

namespace WPF_Calendar_With_Notes.Utilities.Commands
{
    public class ApplicationDeleteSelectedNoteCommand : ICommand
    {
        
        private CalendarEngine m_cal_engine;
        private DataGrid m_DataGrid;

        public ApplicationDeleteSelectedNoteCommand(CalendarEngine _cal_engine, DataGrid _dataGrid)
        {
            m_cal_engine = _cal_engine;
            m_DataGrid = _dataGrid;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            PositionOfDay selectedPosition = m_DataGrid.SelectedItem as PositionOfDay;
            if (selectedPosition != null)
            {
                

                if (MessageBox.Show(
                    Properties.Resources.NoteWillBeDeleted, Properties.Resources.NoteWillBeDeletedTitle, 
                    MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    m_cal_engine.RemoveNoteFromDB( selectedPosition.CurrentHour, selectedPosition.CurrentMinute);
                    m_cal_engine.UpdateOfPositions();

                }
            }

            m_cal_engine.UpdateOfPositions();

        }



        public event EventHandler CanExecuteChanged;

        
    }
}
