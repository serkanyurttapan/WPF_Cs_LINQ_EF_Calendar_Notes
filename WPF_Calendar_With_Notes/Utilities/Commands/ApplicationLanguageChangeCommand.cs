using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Threading;
using System.Globalization;
using WPF_Calendar_With_Notes.CommonTypes;

namespace WPF_Calendar_With_Notes.Utilities.Commands
{
    class ApplicationLanguageChangeCommand : ICommand
    {

        private IEventBroker m_Broker;
        public ApplicationLanguageChangeCommand(IEventBroker broker)
        {
            m_Broker = broker;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            var languageToSwitch = parameter.ToString();


            var currentUiCulture = new CultureInfo(languageToSwitch);

            Thread.CurrentThread.CurrentUICulture = currentUiCulture;
            Thread.CurrentThread.CurrentCulture = currentUiCulture;

            i18nManager.ChangeCulture(currentUiCulture);

            m_Broker.FireEvent(EventType.LanguageChanged, new object());
        
        }
    }
}
