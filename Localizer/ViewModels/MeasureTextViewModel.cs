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

        private static FontFamily? family = new FontFamily("Segoe UI");
        public FontFamily? Family
        {
            get
            {
                return family;
            }
            set
            {
                SetProperty(ref family, value);
                CalculateLength();

            }
        }

        private static int  size=16;
        public int Size
        {
            get
            {
                return size;
            }
            set
            {
                SetProperty(ref size, value);
                CalculateLength();

            }
        }


        [ObservableProperty]
        private ObservableCollection<MesaureResultItem> items = new ObservableCollection<MesaureResultItem>();


        public ObservableCollection<FontFamily> AllFonts
        {
            get
            {
                var fonts = FontManager.Current.SystemFonts;
                var result = new ObservableCollection<FontFamily>();
                foreach(var font in fonts)
                {
                    result.Add(new FontFamily(font.Name));
                }
                return result;
            }
        }
        public MeasureTextViewModel(ResxEntityViewModel entity)
        {
            Entity = entity;
            CalculateLength();
        }

        void CalculateLength()
        {
            Items.Clear();

            foreach (var (culture,value) in Entity.CultureValues)
            {
                var cult = ResxEntityViewModel.GetKeyNameFromVm(culture);
                var val = value ?? string.Empty;

                int i = 0;
                foreach(var line in val.Split('\n'))
                {
                    if(i++==0)
                        Items.Add(new(cult, line, MeasureText(line, cult), MeasureTextHeight(line,cult).ToString()));
                    else
                        Items.Add(new(string.Empty, line, MeasureText(line, cult)));
                }
            }
        }

        double MeasureText(string text,string culture)
        {
            var formatText = new FormattedText(text, new CultureInfo(culture), FlowDirection.LeftToRight,new Typeface(Family!),(1/96.0)*(Size/16.0),null);
            return formatText.WidthIncludingTrailingWhitespace * 1472;
        }
        double MeasureTextHeight(string text, string culture)
        {
            var formatText = new FormattedText(text, new CultureInfo(culture), FlowDirection.LeftToRight, new Typeface(Family!), (1/96.0) * (Size/16.0), null);
            return formatText.Height * 1472;
        }
    }

    public record MesaureResultItem(string Culture,string Text,double Width,string? Height=null);
}
