using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Calendar_With_Notes
{
    public class FieldsOfDataGrid
    {
        private string m_User = "";
        public string User
        {
            get { return m_User; }
            set { m_User = value; }
        }

        private string m_Note = "";
        public string Note
        {
            get { return m_Note; }
            set { m_Note = value; }
        }

        private short m_Hour = 0;
        public short Hour
        {
            get { return m_Hour; }
            set { m_Hour = value; }
        }

        private short m_Minute = 0;
        public short Minute
        {
            get { return m_Minute; }
            set { m_Minute = value; }
        }
    }
}
