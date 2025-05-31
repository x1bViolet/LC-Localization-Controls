using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Translation_Devouring_Siltcurrent.MainWindow;
using static Translation_Devouring_Siltcurrent.Requirements;

namespace Translation_Devouring_Siltcurrent
{
    internal abstract class UI_Language_Loader
    {
        internal protected record UILanguage
        {
            [JsonProperty("UI Text")]
            public Dictionary<string, UIOverrideTextParameters> UIText { get; set; }
        }

        internal protected record UIOverrideTextParameters
        {
            public int? Width { get; set; }
            public int? Height { get; set; }
            public int? FontSize { get; set; }
            public string Text { get; set; }
            public string FontFamily { get; set; }
            public string Foreground { get; set; }
            public string SelectionBrush { get; set; }
            public string CaretBrush { get; set; }
        }

        internal protected static void LoadUIOverrideText(Dictionary<string, UIOverrideTextParameters> OverrideParameters)
        {
            foreach(KeyValuePair<string, UIOverrideTextParameters> Parameter in OverrideParameters)
            {
                if (MainControl.FindName(Parameter.Key) != null)
                {
                    dynamic TargetElement = MainControl.FindName(Parameter.Key);
                    UIOverrideTextParameters _ = Parameter.Value;

                    if (TargetElement.HasProperty("SelectionBrush") & _.SelectionBrush != null) TargetElement.Foreground = ToColor(_.SelectionBrush);
                    if (TargetElement.HasProperty    ("CaretBrush") & _.CaretBrush     != null) TargetElement.Foreground = ToColor(_.CaretBrush);
                    if (TargetElement.HasProperty    ("Foreground") & _.Foreground     != null) TargetElement.Foreground = ToColor(_.Foreground);
                    if (TargetElement.HasProperty    ("FontFamily") & _.FontFamily     != null) TargetElement.FontFamily = _.FontFamily;
                    if (TargetElement.HasProperty      ("FontSize") & _.FontSize       != null) TargetElement.FontSize   = _.FontSize;
                    if (TargetElement.HasProperty        ("Height") & _.Height         != null) TargetElement.Height     = _.Height;
                    if (TargetElement.HasProperty         ("Width") & _.Width          != null) TargetElement.Width      = _.Width;
                    if (TargetElement.HasProperty          ("Text") & _.Text           != null) TargetElement.Text       = _.Text;
                }
            }
        }
    }
}
