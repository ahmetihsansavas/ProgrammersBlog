using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Shared.Utilities.Extensions
{
   public static class DateTimeExtensions //resim upload islemleri icin gün ay ve saati _ ayırma islemi
    {
        public static string FullDateAndTimeStringwithUnderscore(this DateTime dateTime)
        {
            return $"{dateTime.Millisecond}_{dateTime.Second}_{dateTime.Minute}_{dateTime.Hour}_{dateTime.Day}_{dateTime.Month}_{dateTime.Year}";
        /*
         * AhmetSavas_587_5_38_12_3_10_2021_userAhmetSavas.png
         */
        }

    }
}
