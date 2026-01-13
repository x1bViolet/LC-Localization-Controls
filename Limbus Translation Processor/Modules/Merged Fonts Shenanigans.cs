using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using static Translation_Devouring_Siltcurrent.LimbusTranslationProcessor;

namespace Translation_Devouring_Siltcurrent
{
    namespace LocalizationProcessingModules
    {
        public static class Merged_Fonts_Shenanigans
        {
            /// <summary>
            /// Key is font name, Value is characters replacement dictionary
            /// </summary>
            public static Dictionary<string, Dictionary<String, String>> LoadedMergedFontReplacementMap = [];

            /// <summary>
            /// Key is file pattern, Value is list of applied font rules (JsonPath of affected properties and Font Name from <see cref="LoadedMergedFontReplacementMap"/>)
            /// </summary>
            public static Dictionary<string, List<MergedFontRule>> LoadedMergedFontMultipleApplyConfig = [];

            public record MergedFontRule
            {
                [JsonProperty("Font")]
                public string FontName { get; set; }

                [JsonProperty("JsonPath")]
                public string JsonPath { get; set; }

                [JsonIgnore] // For report only
                public string FilesPattern { get; set; }
            }


            public static string ReplaceCharacters(this string Text, Dictionary<String, String> ReplacementMap_Font, string LoggingFileName = "", string DetailedInfo = "")
            {
                if (Text.StartsWith("[font=")) Text = Text.RegexRemove(new(@"^\[font=\w+\]"));

                string OriginalText = Text;

                // List of special escape areas that shouldnt be converted (<tags> and {insertions} such as '<size=13>' or '{0}', '{1}')
                //Key: original index, Value: escape area string part itself
                Dictionary<int, string> SavedEscapeAreas = Regex.Matches(Text, Configurazione.CurrentProfile.MergedFonts.MergedFontIgnoreSequencesRegexPattern)
                    .Select(Match => new KeyValuePair<int, string>(key: Match.Index, value: Match.Value)).ToDictionary();
                
                foreach (KeyValuePair<String, String> CharPair in ReplacementMap_Font)
                {
                    Text = Text.Replace(CharPair.Key, CharPair.Value);
                }
            
                // Return back <tags> and {insertions}
                foreach (KeyValuePair<int, string> EscapeArea in SavedEscapeAreas)
                {
                    Text = Text.InsertEscapeAreaBack(EscapeArea);
                }

                #region Logging
                if (LoggingFileName != "")
                {
                    string DictionaryKeyName = LoggingFileName + (DetailedInfo != "" ? $" (Option: {DetailedInfo})" : "");

                    if (!CurrentReport.MergedFontsConversionReport.AttachedFonts_MultipleApplyConfig.ContainsKey(DictionaryKeyName))
                    {
                        CurrentReport.MergedFontsConversionReport.AttachedFonts_MultipleApplyConfig[DictionaryKeyName] = new Dictionary<string, string>();
                    }

                    CurrentReport.MergedFontsConversionReport.AttachedFonts_MultipleApplyConfig[DictionaryKeyName][$"{OriginalText}"] = $"{Text} :: {Text.ToUnicodeSequence()}";
                }
                #endregion

                return Text;
            }

            //                                                              original pos, string itself
            public static string InsertEscapeAreaBack(this string Text, KeyValuePair<int, string> EscapeArea)
            {
                return new StringBuilder(value: Text)
                    .Remove(startIndex: EscapeArea.Key, length: EscapeArea.Value.Length)
                    .Insert(index: EscapeArea.Key, value: EscapeArea.Value).ToString();
            }


            public static MessageBoxResult CheckForInvalidMergedFontFontRules()
            {
                if (LoadedMergedFontReplacementMap.Keys.Count > 0)
                {
                    foreach (KeyValuePair<string, List<MergedFontRule>> AppliedFontRules in LoadedMergedFontMultipleApplyConfig)
                    {
                        foreach (MergedFontRule FontRule in AppliedFontRules.Value)
                        {
                            FontRule.FilesPattern = AppliedFontRules.Key; // Add AppliedFontRules FilesPattern to value

                            if (!LoadedMergedFontReplacementMap.ContainsKey(FontRule.FontName))
                            {
                                MessageBoxResult Question = MessageBox.Show(
                                    messageBoxText: $"Font rule '{AppliedFontRules.Key} -> {FontRule.JsonPath}' from Merged Font Multiple Apply Config uses unknown font name \"{FontRule.FontName}\" that is not defined at the current Merged Font Characters Replacement map.\n\nClick \"Cancel\" to cancel localization files processing, \"OK\" to ignore this.",
                                    caption: "Invalid font name in Merged Font Multiple Apply Config",
                                    button: MessageBoxButton.OKCancel,
                                    icon: MessageBoxImage.Warning
                                );

                                if (Question == MessageBoxResult.Cancel) return MessageBoxResult.Cancel;
                            }
                        }
                    }
                }

                return MessageBoxResult.OK;
            }


            public static string PlaceMergedFontsByMultipleApplyConfig(string JsonText, List<MergedFontRule> FontRules, string LoggingFileName = "")
            {
                JToken JParser = JToken.Parse(JsonText);
                bool SomethingWasReplaced = false;

                foreach (MergedFontRule FontRule in FontRules)
                {
                    if (LoadedMergedFontReplacementMap.ContainsKey(FontRule.FontName))
                    {
                        foreach (JToken StringItem in JParser.SelectTokens(FontRule.JsonPath))
                        {
                            string StringItem_OriginalValue = $"{StringItem}";
                            if (StringItem_OriginalValue != "")
                            {
                                string StringItem_WithConvertedFont = StringItem_OriginalValue.ReplaceCharacters(
                                    ReplacementMap_Font: LoadedMergedFontReplacementMap[FontRule.FontName],
                                    LoggingFileName: LoggingFileName,
                                    DetailedInfo: $"'{FontRule.FilesPattern}' → '{FontRule.JsonPath}', Font: {FontRule.FontName}"
                                );

                                if (StringItem_WithConvertedFont != StringItem_OriginalValue)
                                {
                                    StringItem.Replace(StringItem_WithConvertedFont);
                                    SomethingWasReplaced = true;

                                    #region Logging
                                    if (!CurrentReport.MergedFontsConversionReport.UsedReplacementMapFonts.ContainsKey(FontRule.FontName))
                                    {
                                        CurrentReport.MergedFontsConversionReport.UsedReplacementMapFonts[FontRule.FontName] = LoadedMergedFontReplacementMap[FontRule.FontName];
                                    }
                                    #endregion
                                }
                            }
                        }
                    }
                }

                return SomethingWasReplaced ? JParser.ToString(Formatting.Indented) : JsonText;
            }


            private static readonly Regex StandaloneFontMarkersPlacementRegex = new(@"""(?<PropertyValue>\[font=(?<FontNameMarker>\w+)\](.*?))(?<Afterward>""(,)?(\r)?\n)", RegexOptions.Compiled);
            public static string PlaceMergedFontByMarkers(string JsonText, string LoggingFileName = "")
            {
                JsonText = StandaloneFontMarkersPlacementRegex.Replace(JsonText, Match =>
                {
                    GroupCollection Groups = Match.Groups;
                    string FontName = Groups["FontNameMarker"].Value;

                    if (LoadedMergedFontReplacementMap.ContainsKey(FontName))
                    {
                        string ReplacementString = Groups["PropertyValue"].Value; // Current property text
                        ReplacementString = ReplacementString.ReplaceCharacters(ReplacementMap_Font: LoadedMergedFontReplacementMap[FontName]);

                        #region Logging
                        if (!CurrentReport.MergedFontsConversionReport.AttachedFonts_Markers.ContainsKey(LoggingFileName))
                        {
                            CurrentReport.MergedFontsConversionReport.AttachedFonts_Markers[LoggingFileName] = new Dictionary<string, string>();
                        }
                        int LineNumber = JsonText[0..Match.Index].CountLines() + 1;
                        CurrentReport.MergedFontsConversionReport.AttachedFonts_Markers[LoggingFileName][$"LINE NO.{LineNumber} :: {Groups["PropertyValue"].Value}"] = $"{ReplacementString} :: {ReplacementString.ToUnicodeSequence()}";

                        if (!CurrentReport.MergedFontsConversionReport.UsedReplacementMapFonts.ContainsKey(FontName))
                        {
                            CurrentReport.MergedFontsConversionReport.UsedReplacementMapFonts[FontName] = LoadedMergedFontReplacementMap[FontName];
                        }
                        #endregion

                        return $"\"{ReplacementString}{Groups["Afterward"]}";
                    }
                    else
                    {
                        MessageBoxResult Question = MessageBox.Show(
                            messageBoxText: $"{LoggingFileName} file contains \"[font={FontName}]\" merged font marker with a font name that is not defined at the current Merged Font Characters Replacement map.",
                            caption: "Invalid font name in \"[font=name]\" marker",
                            button: MessageBoxButton.OK,
                            icon: MessageBoxImage.Warning
                        );

                        return Match.Value;
                    }
                });

                return JsonText;
            }   
        }
    }
}