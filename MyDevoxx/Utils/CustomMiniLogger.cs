using FFImageLoading.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDevoxx.Utils
{
    public class CustomMiniLogger : IMiniLogger
    {
        public void Debug(string message)
        {
        }

        public void Error(string errorMessage)
        {
            System.Diagnostics.Debug.WriteLine(errorMessage);
        }

        public void Error(string errorMessage, Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(errorMessage);
            System.Diagnostics.Debug.WriteLine(ex);
        }
    }
}
