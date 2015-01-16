using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Controls;
using WPF_Calendar_With_Notes.CommonTypes;
using System.Globalization;
using System.Threading;
using WPF_Calendar_With_Notes.Utilities;
using System.Windows;


namespace WPF_Calendar_With_Notes.ViewModel
{
    public class ApplicationViewModel
    {
        public CalendarEngine engine { get; set; }
        private DataGrid m_DataGrid;
        private IEventBroker m_Broker;

        public Func<object, bool> m_true = new Func<object, bool>(o => { return true; });
        public ICommand LanguageChangeCommand { get; set; }
        public ICommand HelpCommand { get; set; }
        public ICommand DeleteSelectedNoteCommand { get; set; }
        public ICommand NewNoteCommand { get; set; }
        public ICommand EditSelectedNoteCommand { get; set; }

        public ApplicationViewModel(CalendarEngine _engin, IEventBroker _broker, DataGrid _dataGrid)
        {
            engine = _engin;
            m_DataGrid = _dataGrid;
            m_Broker = _broker;
            this.LanguageChangeCommand = new Utilities.CommandBase(LanguageChangeAction, m_true);
            this.HelpCommand = new Utilities.CommandBase(HelpAction, m_true);
            this.DeleteSelectedNoteCommand = new Utilities.CommandBase(DeleteAction, m_true);
            this.NewNoteCommand = new Utilities.CommandBase(NewNoteAction, m_true);
            this.EditSelectedNoteCommand = new Utilities.CommandBase(EditSelectedNoteAction, m_true);
        }

        void LanguageChangeAction(object parameter)
        {
            var languageToSwitch = parameter.ToString();

            var currentUiCulture = new CultureInfo(languageToSwitch);

            Thread.CurrentThread.CurrentUICulture = currentUiCulture;
            Thread.CurrentThread.CurrentCulture = currentUiCulture;

            i18nManager.ChangeCulture(currentUiCulture);

            m_Broker.FireEvent(EventType.LanguageChanged, new object());
        }

        void HelpAction(object parameter)
        {
            MessageBox.Show(Properties.Resources.HelpInformation, Properties.Resources.HelpInformationTitle, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        void DeleteAction(object parameter)
        {
            PositionOfDay selectedPosition = m_DataGrid.SelectedItem as PositionOfDay;
            if (selectedPosition != null)
            {
                if (MessageBox.Show(
                    Properties.Resources.NoteWillBeDeleted, Properties.Resources.NoteWillBeDeletedTitle,
                    MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    ActionResult saveRes = engine.RemoveNoteFromDB(selectedPosition.CurrentHour, selectedPosition.CurrentMinute);
                    if (!saveRes.IsSuccess)
                    {
                        MessageBox.Show(saveRes.ErrorMsg, "Error");
                    }
                }
            }
            engine.UpdateOfPositions();
        }

        void NewNoteAction(object parameter)
        {
            PositionOfDay PosOfDay = new PositionOfDay()
            {
                CurrentHour = 0,
                CurrentMinute = 0,
                CurrentNote = String.Empty,
                CurrentUser = String.Empty,
                //1,1,1 bo PositionOfDay korzysta tylko z hh:mm
                DateTimeVal = new DateTime(1, 1, 1, 0, 0, 0)
            };

            WindowOfPositions okno = new WindowOfPositions(PosOfDay, engine);
            var x = okno.ShowDialog().Value;
            if (x)
            {
                if (PosOfDay.CurrentNote.Length >= 498) PosOfDay.CurrentNote = PosOfDay.CurrentNote.Remove(498);
                if (PosOfDay.CurrentUser.Length >= 498) PosOfDay.CurrentUser = PosOfDay.CurrentUser.Remove(498);

                var fodg = new FieldsOfDataGrid()
                {
                    Hour = PosOfDay.CurrentHour,
                    Minute = PosOfDay.CurrentMinute,
                    Note = PosOfDay.CurrentNote,
                    User = PosOfDay.CurrentUser
                };

                ActionResult addNoteResult = engine.AddNoteToDB(fodg);

                if (!addNoteResult.IsSuccess)
                {
                    if (addNoteResult.ErrorType == ErrorType.DataAlreadyPresent)
                    {
                        if (MessageBox.Show(Properties.Resources.CurrentNoteBusy, Properties.Resources.ReplaceNote, MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                        {
                            ActionResult saveRes = engine.RemoveNoteFromDB(PosOfDay.CurrentHour, PosOfDay.CurrentMinute);
                            if (!saveRes.IsSuccess)
                            {
                                MessageBox.Show(saveRes.ErrorMsg, "Error");
                            }

                            ActionResult addNoteAgainResult = engine.AddNoteToDB(fodg);
                            if (!addNoteAgainResult.IsSuccess)
                            {
                                MessageBox.Show(addNoteAgainResult.ErrorMsg, "Error");
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show(addNoteResult.ErrorMsg,"Error");
                    }
                }

                engine.UpdateOfPositions();
            }
        }

        void EditSelectedNoteAction(object parameter)
        {
            foreach (var row in m_DataGrid.SelectedItems)
            {
                PositionOfDay selectedPosition = row as PositionOfDay;
                PositionOfDay primary = new PositionOfDay()
                {
                    CurrentHour = selectedPosition.CurrentHour,
                    CurrentMinute = selectedPosition.CurrentMinute,
                    CurrentNote = selectedPosition.CurrentNote,
                    CurrentUser = selectedPosition.CurrentUser
                };

                WindowOfPositions okno = new WindowOfPositions(selectedPosition, engine);
                var x = okno.ShowDialog().Value;
                if (x)
                {
                    ActionResult saveRes = engine.RemoveNoteFromDB(primary.CurrentHour, primary.CurrentMinute);
                    if (!saveRes.IsSuccess)
                    {
                        MessageBox.Show(saveRes.ErrorMsg, "Error");
                    }

                    if (selectedPosition.CurrentNote.Length >= 498)
                        selectedPosition.CurrentNote = selectedPosition.CurrentNote.Remove(498);

                    if (selectedPosition.CurrentUser.Length >= 498)
                        selectedPosition.CurrentUser = selectedPosition.CurrentUser.Remove(498);

                    var fodg = new FieldsOfDataGrid()
                    {
                        Hour = selectedPosition.CurrentHour,
                        Minute = selectedPosition.CurrentMinute,
                        Note = selectedPosition.CurrentNote,
                        User = selectedPosition.CurrentUser
                    };
                    engine.AddNoteToDB(fodg);
                }
                //else//selectedPosition == null
                //{
                //    PositionOfDay pozycja = new PositionOfDay() { CurrentHour = 0, CurrentMinute = 0, CurrentNote = String.Empty };

                //    WindowOfPositions okno = new WindowOfPositions(pozycja, engine);
                //    var x = okno.ShowDialog().Value;
                //    if (x)
                //    {
                //        if (pozycja.CurrentNote.Length >= 498) pozycja.CurrentNote = pozycja.CurrentNote.Remove(498);

                //        int res = engine.AddNoteToDB(pozycja.CurrentNote, pozycja.CurrentHour, pozycja.CurrentMinute);

                //        if (res == -1)
                //            if (MessageBox.Show(Properties.Resources.CurrentNoteBusy, Properties.Resources.ReplaceNote, MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                //            {
                //                engine.RemoveNoteFromDB(pozycja.CurrentHour, pozycja.CurrentMinute);
                //                engine.AddNoteToDB(pozycja.CurrentNote, pozycja.CurrentHour, pozycja.CurrentMinute);

                //            }
                //    }
                //}
            }
            engine.UpdateOfPositions();
        }

    }

}
