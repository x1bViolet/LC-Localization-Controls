using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using static Translation_Devouring_Siltcurrent.Requirements;
using static Translation_Devouring_Siltcurrent.MainWindow;
using static Translation_Devouring_Siltcurrent.NullableControl;
using Siltcurrent;
using static Siltcurrent.LimbusTranslationExport.ActionsProvider;

namespace Translation_Devouring_Siltcurrent
{
    internal abstract class Configurazione
    {
        internal protected record Config
        {
            public ExportSettings Export { get; set; }
            public InternalSettings Internal { get; set; }
        }

        internal protected record ExportSettings
        {
            [JsonProperty("Raw Custom Localization Source")]
            public string RawCustomLocalizationSource { get; set; }

            [JsonProperty("Exported Custom Localization Destination")]
            public string ExportedCustomLocalizationDestination { get; set; }

            [JsonProperty("Reference Localization Files")]
            public string ReferenceLocalizationFiles { get; set; }

            [JsonProperty("Reference Localization Files Prefix")]
            public string ReferenceLocalizationFilesPrefix { get; set; }

            [JsonProperty("Shorthands Pattern")]
            public string ShorthandsPattern { get; set; }

            [JsonIgnore]
            public Regex LoadedShorthandsPattern { get; set; } = new Regex(@"NOTHING THERE");

            [JsonProperty("Convert Shorthands")]
            public bool ConvertShorthands { get; set; }

            [JsonProperty("Add <style> placeholders")]
            public bool AddStyleTagPlaceholders { get; set; }

            [JsonProperty("Add Missing Files")]
            public bool AddMissingFiles { get; set; }

            [JsonProperty("Add Missing IDs")]
            public bool AddMissingIDs { get; set; }

            [JsonProperty("Copy font files")]
            public bool CopyFontFiles { get; set; }

            [JsonProperty("Apply special fonts from [font=*]")]
            public bool ApplySpecialFontsByMarks { get; set; }

            [JsonProperty("Apply special fonts from Toml Config")]
            public bool ApplySpecialFontsFromToml { get; set; }

            [JsonProperty("Merged Font Parameters")]
            public string MergedFontParameters { get; set; } = "";

            [JsonProperty("Merged Font Config")]
            public string MergedFontConfig { get; set; } = "";

            [JsonProperty("Font Files")]
            public FontFiles FontFiles { get; set; }

            [OnDeserialized]
            internal protected void OnInit(StreamingContext context)
            {
                MainControl.ReferenceLocalizationFilePathInput.Text = ReferenceLocalizationFiles;
                MainControl.ShorthandsPatternInput.Text = ShorthandsPattern;
                MainControl.MergedFontParametersInput.Text = MergedFontParameters;

                
                LimbusTranslationExport.ActionsProvider.MergedFont.LoadedParameters = new MergedFont.FontParameters(MergedFontParameters);
                LimbusTranslationExport.ActionsProvider.MergedFont.FontsTomlConfig.Load(
                    MergedFontConfig,
                    LimbusTranslationExport.ActionsProvider.MergedFont.LoadedParameters.ReplacementMapsList
                );



                MainControl.TranslationExport_RawFanmade_SourceDirectory.Text = RawCustomLocalizationSource;
                MainControl.TranslationExport_RawFanmadeOutputDirectory.Text = ExportedCustomLocalizationDestination;

                try
                {
                    LoadedShorthandsPattern = new Regex(ShorthandsPattern);
                }
                catch
                {
                    LoadedShorthandsPattern = new Regex(@"NOTHING THERE");
                }
            }
        }

        internal protected record FontFiles
        {
            public string Context { get; set; }
            public string Title { get; set; }
        }

        internal protected record InternalSettings
        {
            [JsonProperty("Language (⇲ Assets Directory/Languages)")]
            public string Language { get; set; }

            [JsonProperty("Automatically open Destination directory after completion")]
            public bool AutomaticallyOpenDestinationDirectoryAfterCompletion { get; set; }

            [JsonProperty("Automatically open Report File after completion")]
            public bool AutomaticallyOpenReportFileAfterCompletion { get; set; }

            [JsonProperty("Window Scaling")]
            public double? WindowScaling { get; set; }

            [OnDeserialized]
            internal protected void OnInit(StreamingContext context)
            {
                MainControl.WindwScaling.ScaleX = (double)WindowScaling;
                MainControl.WindwScaling.ScaleY = (double)WindowScaling;
                MainControl.MinWidth = MainWindow.DefaultMinWidth * (double)WindowScaling;
                MainControl.MinHeight = MainWindow.DefaultMinHeight * (double)WindowScaling;
            }
        }

        internal protected static Config Settings = new();

        internal protected static bool LoadingEvent = true;
        internal protected static void PullLoad()
        {
            LoadingEvent = true;


            Settings = JsonConvert.DeserializeObject<Config>(File.ReadAllText(@"⇲ Assets Directory\Configurazione.json"));

            if (Settings.Internal.Language != null)
            {
                if (File.Exists(Settings.Internal.Language))
                {
                    UI_Language_Loader.LoadUIOverrideText(JsonConvert.DeserializeObject<UI_Language_Loader.UILanguage>(File.ReadAllText(Configurazione.Settings.Internal.Language)).UIText);
                }
            }


            LoadingEvent = false;
        }
    }
}
