using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace style_radio_group
{
    partial class OneHotButton
    {
        protected override void OnHandlerChanged() 
        {
            if (Handler.PlatformView is Microsoft.UI.Xaml.Controls.Button windowsButton)
            {
                windowsButton.IsTabStop = false;
            }
        }
    }
}
