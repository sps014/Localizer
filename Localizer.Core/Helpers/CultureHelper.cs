using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Localizer.Core.Helpers;

public static class Culture
{
    private static HashSet<string>? cultures = null;
    private static Dictionary<string, string> OverrrideCultures = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["en"] = "en-US",
        ["zh"] = "zh-CN",
        ["zh-CHT"] = "zh-CN",
        ["zh-HANT"] = "zh-CN",
    };
    public static HashSet<string> AllCultures
    {
        get
        {
            if (cultures != null)
                return cultures;

            cultures = new HashSet<string>();

            foreach (var culture in CultureInfo.GetCultures(CultureTypes.AllCultures))
            {
                var name = culture.Name;

                if (OverrrideCultures.ContainsKey(name))
                    name = OverrrideCultures[name];

                cultures.Add(name);
            }
            return cultures;
        }
    }
    public static string? GetLanguageName(string cultureCode)
    {
        if(OverrrideCultures.Values.Contains(cultureCode))
            cultureCode = OverrrideCultures.FirstOrDefault(x=>x.Value == cultureCode).Key;

        return new CultureInfo(cultureCode).DisplayName;
    }
}
