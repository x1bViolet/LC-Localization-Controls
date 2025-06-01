using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using static Translation_Devouring_Siltcurrent.MainWindow;
using static Translation_Devouring_Siltcurrent.Requirements;

namespace Translation_Devouring_Siltcurrent
{
    internal abstract class UI_Language_Loader
    {
        internal protected record UILanguage
        {
            [JsonProperty("UI Text")]
            public List<UIOverrideTextParameters> UIText { get; set; }
        }

        internal protected record UIOverrideTextParameters
        {
            public string ElementID { get; set; }
            public double? Width { get; set; }
            public double? Height { get; set; }
            public double? FontSize { get; set; }
            public string Text { get; set; }
            public string FontFamily { get; set; }
            public string Foreground { get; set; }
            public string SelectionBrush { get; set; }
            public string CaretBrush { get; set; }
        }

        internal protected static void LoadUIOverrideText(List<UIOverrideTextParameters> OverrideParameters)
        {
            foreach(UIOverrideTextParameters Parameter in OverrideParameters)
            {
                if (Parameter.ElementID != null)
                {
                    if (MainControl.FindName(Parameter.ElementID) != null)
                    {
                        FrameworkElement TargetElementCheck = MainControl.FindName(Parameter.ElementID) as FrameworkElement;
                        dynamic TargetElement = MainControl.FindName(Parameter.ElementID);
                        UIOverrideTextParameters t = Parameter;

                        if (TargetElementCheck.HasProperty("SelectionBrush") & t.SelectionBrush != null) TargetElement.Foreground = ToColor(t.SelectionBrush);
                        if (TargetElementCheck.HasProperty    ("CaretBrush") & t.CaretBrush     != null) TargetElement.Foreground = ToColor(t.CaretBrush);
                        if (TargetElementCheck.HasProperty    ("Foreground") & t.Foreground     != null) TargetElement.Foreground = ToColor(t.Foreground);
                        if (TargetElementCheck.HasProperty    ("FontFamily") & t.FontFamily     != null) TargetElement.FontFamily = new FontFamily(t.FontFamily);
                        if (TargetElementCheck.HasProperty      ("FontSize") & t.FontSize       != null) TargetElement.FontSize   = (double)t.FontSize;
                        if (TargetElementCheck.HasProperty        ("Height") & t.Height         != null) TargetElement.Height     = (double)t.Height;
                        if (TargetElementCheck.HasProperty         ("Width") & t.Width          != null) TargetElement.Width      = (double)t.Width;
                        if (TargetElementCheck.HasProperty          ("Text") & t.Text           != null) TargetElement.Text       = t.Text;
                    }
                }
            }
        }
    }
}
