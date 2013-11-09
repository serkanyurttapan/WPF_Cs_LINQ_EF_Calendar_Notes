using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPF_Calendar_With_Notes.Model;
using WPF_Calendar_With_Notes.ViewModel;
using System.Globalization;
using WPF_Calendar_With_Notes.CommonTypes;


namespace WPF_Calendar_With_Notes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IListener
    {

        private ApplicationViewModel viewModel;
        public CalendarEngine engine { get; set; }
        private Timer_CalendarPatch patch;
        public Sem m_Sem = new Sem();

        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);

            this.Closing += new System.ComponentModel.CancelEventHandler(MainWindow_Closing);
            this.Closed += new EventHandler(MainWindow_Closed);
        }


        private IEventBroker m_Broker;
        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //var kultura = new System.Globalization.CultureInfo("pl-PL");
            //System.Threading.Thread.CurrentThread.CurrentUICulture = kultura;
            //System.Threading.Thread.CurrentThread.CurrentCulture = kultura;      

            this.Top = 125.0;

            var culture = CultureInfo.InstalledUICulture;
            string lang = culture.DisplayName;

            m_Broker = new EventBroker();
            engine = new CalendarEngine(m_Broker);

            viewModel = new ApplicationViewModel(engine, m_Broker, dataGrid1);
            this.DataContext = viewModel;

            bEditSelected.IsEnabled = false;
            bDeleteSelectedNote.IsEnabled = false;

            m_Broker.RegisterFor(EventType.LanguageChanged, this);

            string cultureInformation = CultureInfo.CurrentCulture.Name;
            if (!cultureInformation.Equals("pl-PL"))
                new Utilities.Commands.ApplicationLanguageChangeCommand(m_Broker).Execute("en-GB");

            //Łatka,bo
            //gdy klikamy na kalendarz wtedy zaznaczone daty ulegają odznaczeniu,
            //trzeba więc ponownie zaznaczyć w kalendarzu te daty,które były zaznaczone przed kliknięciem w kalendarz.
            patch = new Timer_CalendarPatch(menu1, bAddNote, MainCalendar, engine, m_Sem);
        }

        public void NotifyMe(EventType type, object data)
        {
            var lDGridBindingExpr = lDataGridDescription.GetBindingExpression(Label.ContentProperty);
            if (lDGridBindingExpr != null) lDGridBindingExpr.UpdateTarget();

            var tBlockBindingExpr = DateTextBlock.GetBindingExpression(TextBlock.TextProperty);
            if (tBlockBindingExpr != null) tBlockBindingExpr.UpdateTarget();

            var tBlock1BindingExpr = textBlock1.GetBindingExpression(TextBlock.TextProperty);
            if (tBlock1BindingExpr != null) tBlock1BindingExpr.UpdateTarget();
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show(Properties.Resources.Quit,
                Properties.Resources.QuitTitle, MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

        void MainWindow_Closed(object sender, EventArgs e)
        {
            Loaded -= MainWindow_Loaded;
            Closing -= MainWindow_Closing;
            Closed -= MainWindow_Closed;

            patch.Timer.Tick -= patch.Timer_Tick;
        }


        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        //wpf bringintoview - dodać procedure
        private void MainCalendar_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {

            if (m_Sem.uzywany == false)
            {
                if (MainCalendar.SelectedDate != null)
                {
                    engine.Selected_Date = (DateTime)MainCalendar.SelectedDate;
                    engine.UpdateOfPositions();
                    m_Sem.zrobione = 0;
                }
            }
        }

        private void dataGrid1_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {

            //uzyć Binding do zmiany "IsEnabled"
            if (engine.Positions.Count == 0)
            {
                bEditSelected.IsEnabled = false;
                bDeleteSelectedNote.IsEnabled = false;
            }
            else
            {
                bEditSelected.IsEnabled = true;
                bDeleteSelectedNote.IsEnabled = true;
            }

        }

        private void dataGrid1_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            var PosOfDay = e.Row.Item as PositionOfDay;

            if (PosOfDay.CurrentHour != PosOfDay.OldHour ||
                PosOfDay.CurrentMinute != PosOfDay.OldMinute ||
                !PosOfDay.CurrentNote.Equals(PosOfDay.OldNote))
            {
                engine.RemoveNoteFromDB(PosOfDay.OldHour, PosOfDay.OldMinute);
                engine.AddNoteToDB(PosOfDay.CurrentNote, PosOfDay.CurrentHour, PosOfDay.CurrentMinute);
                engine.UpdateOfPositions();
            }
        }


        private void dataGrid1_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var Grid = (DataGrid)sender;
            if (e.Key == Key.Delete)
            {
                foreach (var row in Grid.SelectedItems)
                {
                    PositionOfDay PosOfDay = row as PositionOfDay;
                    if (PosOfDay != null)
                    {
                        engine.RemoveNoteFromDB(PosOfDay.CurrentHour, PosOfDay.CurrentMinute);
                    }
                }
            }
        }

        private void bQuit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }


    public class Sem
    {
        public bool uzywany { get; set; }
        public int zrobione { get; set; }


        public Sem()
        {
            uzywany = false;
            zrobione = 0;
        }
    }

}
