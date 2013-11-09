using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;
using System.Globalization;

namespace WPF_Calendar_With_Notes.Utilities
{
    public class i18nManager
    {
        public Properties.Resources GetResourceManager()
        {
            return new Properties.Resources();
        }

        private static readonly object LockObject = new object();
        private static ObjectDataProvider m_provider;
        public static ObjectDataProvider ResourceProvider
        {
            get
            {
                if (m_provider==null)
                    lock (LockObject)
                    {
                        if (m_provider == null)
                            m_provider = (ObjectDataProvider)Application.Current.FindResource("Resources");
                    }
                return m_provider;

            }            
        }

        public static void ChangeCulture(CultureInfo culture)
        {
            //set cultrure for Resource
            Properties.Resources.Culture = culture;
            ResourceProvider.Refresh();
        }







    }









}
