using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace Localizer.Helper
{
    internal class WindowHelper
    {
        public static Window? ParentWindow<T>() where T: Window
        {
            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                return desktop.Windows.First(x => x is T);
            }
            return null;
        }
    }
}
