using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WPF_Calendar_With_Notes.Model;
using System.Windows.Input;
using System.Windows.Controls;
using WPF_Calendar_With_Notes.Utilities.Commands;
using WPF_Calendar_With_Notes.CommonTypes;


namespace WPF_Calendar_With_Notes.ViewModel
{
    class ApplicationViewModel
    {
        public CalendarEngine engine {get; set;}
        private DataGrid m_DataGrid;
        private IEventBroker m_Broker;

        public ApplicationViewModel(CalendarEngine _engin, IEventBroker broker, DataGrid _dataGrid )
        {
            engine = _engin;
            m_DataGrid = _dataGrid ;
            m_Broker = broker;
        }


        public ICommand LanguageChangeCommand
        {
            get
            {
                return new ApplicationLanguageChangeCommand(m_Broker);
            }
        }
        
        public ICommand HelpCommand
        {
            get
            {
                return new ApplicationHelpCommand();
            }
        }


        public ICommand NewNoteCommand
        {
            get
            {
                return new ApplicationNewNoteCommand(engine);
            }
        }


        public ICommand EditSelectedNoteCommand
        {
            get
            {
                return new ApplicationEditSelectedNoteCommand(engine, m_DataGrid);
            }
        }


        public ICommand DeleteSelectedNoteCommand
        {
            get
            {
                return new ApplicationDeleteSelectedNoteCommand(engine, m_DataGrid);
            }
        }
                

    }
}
