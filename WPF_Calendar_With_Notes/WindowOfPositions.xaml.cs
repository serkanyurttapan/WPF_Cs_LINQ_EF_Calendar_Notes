using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPF_Calendar_With_Notes.Model;

namespace WPF_Calendar_With_Notes
{

    /// <summary>
    /// Interaction logic for OknoWidokuPozycjiDnia.xaml
    /// </summary>
    public partial class WindowOfPositions : Window
    {

        private PositionOfDay m_pozycja;
        private DateTime m_date;
        private CalendarEngine m_cal_engine;

        public WindowOfPositions()
        {
            InitializeComponent();
        }

        public WindowOfPositions(PositionOfDay _pozycja, CalendarEngine _c_engine)
            : this()
        {
            Loaded += WindowOfPositions_Loaded;
            m_pozycja = _pozycja;
            DataContext = m_pozycja;

            m_cal_engine = _c_engine;
            m_date = m_cal_engine.Selected_Date;

            textBlock1.Text = m_date.ToString().Remove(10) + m_date.ToString(" (dddd)");
        }

        void WindowOfPositions_Loaded(object sender, RoutedEventArgs e)
        {
            this.timePicker1.Value= new DateTime(1,1,1,m_pozycja.CurrentHour,m_pozycja.CurrentMinute,0);
        }

        private void bSave_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrEmpty(m_pozycja.CurrentNote))
            {
                MessageBoxResult result = MessageBox.Show(Properties.Resources.NoteEmpty, Properties.Resources.NoteEmptyTitle, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    this.DialogResult = true;
                    this.Close();
                }
                else
                    if (result == MessageBoxResult.No)
                    {
                        this.DialogResult = false;
                        this.Close();
                    }
                    else
                        return;

            }
            else
            {
                this.DialogResult = true;
                this.Close();

            }

        }

        private void bCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }


}
