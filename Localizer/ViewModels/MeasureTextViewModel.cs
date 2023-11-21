using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using Localizer.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Localizer.ViewModels
{
    internal partial class MeasureTextViewModel:ObservableObject
    {

        [NotifyPropertyChangedFor(nameof(Path))]
        [NotifyPropertyChangedFor(nameof(Key))]
        [ObservableProperty]
        private ResxEntityViewModel entity;

        public string Key=>Entity.Key;
        public string Path => Entity.ResxEntity.Node.NodePathPartsFromParent.JoinBy("\\");

        [ObservableProperty]
        private FontFamily family;

        [ObservableProperty]
        private ObservableCollection<MesaureResultItem> items = new ObservableCollection<MesaureResultItem>();

        public MeasureTextViewModel(ResxEntityViewModel entity,FontFamily family)
        {
            Entity = entity;
            Family = family;
            CalculateLength();
        }

        void CalculateLength()
        {
            foreach (var (culture,value) in Entity.CultureValues)
            {
                var cult = ResxEntityViewModel.GetKeyNameFromVm(culture);
                var val = value ?? string.Empty;

                int i = 0;
                foreach(var line in val.Split('\n'))
                {
                    if(i++==0)
                        Items.Add(new(cult, line, MeasureText(line, cult)));
                    else
                        Items.Add(new(string.Empty, string.Empty, MeasureText(line, cult)));
                }
            }
        }

        double MeasureText(string text,string culture)
        {
            var formatText = new FormattedText(text, new CultureInfo(culture), FlowDirection.LeftToRight,new Typeface(Family),1/96.0,null);
            return formatText.WidthIncludingTrailingWhitespace * 1472;
        }
    }

    public record MesaureResultItem(string Culture,string Text,double Length);
}
