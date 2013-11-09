using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;

namespace WPF_Calendar_With_Notes.Utilities.Commands
{
    public class ApplicationHelpCommand : ICommand
    {

        public bool CanExecute(object parameter)
        {
            return true;
        }
        public void Execute(object parameter)
        {
            MessageBox.Show(Properties.Resources.HelpInformation,Properties.Resources.HelpInformationTitle , MessageBoxButton.OK, MessageBoxImage.Information);

        }

        public event EventHandler CanExecuteChanged;

    }
}
