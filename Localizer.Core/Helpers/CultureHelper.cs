using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Localizer.Core.Helpers;

internal static class Culture
{
    private static HashSet<string>? cultures = null;
    public static HashSet<string> AllCultures
    {
        get
        {
            if(cultures!=null)
                return cultures;

            cultures = new HashSet<string>();
            foreach (var culture in CultureInfo.GetCultures(CultureTypes.AllCultures))
            {
                cultures.Add(culture.Name);
            }
            return cultures;
        }
    }
}
