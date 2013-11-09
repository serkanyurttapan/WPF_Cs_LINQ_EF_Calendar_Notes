using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPF_Calendar_With_Notes
{
    class AppHelper
    {

        public static event Action OnLanguageChanged;

        public static void SentNotification_CultureChange()
        {
            if (OnLanguageChanged != null) OnLanguageChanged();
        }

    }
}
