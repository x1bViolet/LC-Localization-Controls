
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Text.RegularExpressions;
using static Translation_Devouring_Siltcurrent.Requirements;
using static Translation_Devouring_Siltcurrent.MainWindow;
using static Translation_Devouring_Siltcurrent.SkillsQuickFormatter;
using System.Diagnostics;
using Translation_Devouring_Siltcurrent;

namespace Siltcurrent
{
    internal abstract class LimbusTranslationExport
    {
        internal protected static OutputReportManager.OutputReport OutputReport;

        internal abstract class OutputReportManager
        {
            internal protected record OutputReport
            {
                [JsonProperty("Date")]
                public string Date { get; set; }

                [JsonProperty("Destination Directory")]
                public string DestinationDirectory { get; set; }

                [JsonProperty("Source Directory")]
                public string SourceDirectory { get; set; }

                [JsonProperty("Translation Fallback Directory")]
                public string TranslationFallbackDirectory { get; set; }

                [JsonProperty("Untranslated Content Report")]
                public MissingContentReport UntranslatedElementsReport { get; set; } = new MissingContentReport();

                [JsonProperty("Special Fonts Conversion Report")]
                public SpecialFontsConversionInfo SpecialFontsConversionReport { get; set; } = new SpecialFontsConversionInfo();

                [JsonProperty("Shorthands Conversion Info")]
                public ShorthandsConversionInfo ShorthandsConversionInfo { get; set; } = new ShorthandsConversionInfo();

                public OutputReport(TranslationBuilder.ExportParameters Parameters)
                {
                    Date = DateTime.Now.ToString();
                    DestinationDirectory = Parameters.OutputDirectory.Replace("\\", "/");
                    SourceDirectory = Parameters.RawFanmade_LocalizationPath.Replace("\\", "/");
                    TranslationFallbackDirectory = Parameters.Reference_LocalizationPath.Replace("\\", "/");
                }
            }
            internal protected record MissingContentReport
            {
                [JsonProperty("Missing IDs Count")]
                public int MissingIDsCount { get; set; }

                [JsonProperty("Untranslated Files Count")]
                public int UntranslatedFilesCount { get; set; }

                [JsonProperty("Untranslated Files")]
                public List<string> UntranslatedFiles { get; set; } = new List<string>();

                [JsonProperty("Missing IDs")]
                public Dictionary<string, List<dynamic>> MissingIDs { get; set; } = new Dictionary<string, List<dynamic>>();
            }
            internal protected record SpecialFontsConversionInfo
            {
                [JsonProperty("Attached Fonts")]
                public Dictionary<string, Dictionary<string, string>> AttachedFonts { get; set; } = new Dictionary<string, Dictionary<string, string>>();

                [JsonProperty("Used Fonts Info")]
                public Dictionary<string, Dictionary<string, string>> UsedFontsInfo { get; set; } = new Dictionary<string, Dictionary<string, string>>();
            }
            internal protected record ShorthandsConversionInfo
            {
                [JsonProperty("Used Pattern")]
                public string UsedPattern { get; set; }

                [JsonProperty("Conversions Count")]
                public int ConvertedCount { get; set; }

                [JsonProperty("Undefined Colors for:")]
                public List<string> KeywordsWithoutColor { get; set; } = new List<string>();
            }
        }





        internal abstract class ActionsProvider
        {
            internal protected record UniversalType
            {
                public List<dynamic> dataList { get; set; }
            }
            internal abstract class MissingJsonIDManager
            {
                /// <summary>
                /// Insert missing objects to <c>RawFanmade_LocalizationJson</c> from <c>Reference_LocalizationJson</c>
                /// </summary>
                /// <returns><c>RawFanmade_LocalizationJson</c> with extended <c>dataList</c> if something was actually added, else just <c>RawFanmade_LocalizationJson</c></returns>
                internal protected static string CompareAppend(string RawFanmade_LocalizationJson, string Reference_LocalizationJson, string CheckFileName = "")
                {
                    UniversalType RawFanmade_Parsed = JsonConvert.DeserializeObject<UniversalType>(RawFanmade_LocalizationJson);
                    UniversalType Reference_Parsed  = JsonConvert.DeserializeObject<UniversalType>(Reference_LocalizationJson);

                    bool SomethingWasAdded = false;

                    if (RawFanmade_Parsed.dataList != null & Reference_Parsed.dataList != null)
                    {
                        // Create list with IDs that fanmade localization json file currently has
                        List<string> RawFanmade_Parsed_KnownIDList = RawFanmade_Parsed.dataList.Select(x => $"{x.id}").ToList();


                        // Then, check reference localization json file dataList
                        foreach (dynamic Reference_dataList_Object in Reference_Parsed.dataList)
                        {
                            // If list with fanmande localization IDs does not contain current object ID, append it
                            if (!RawFanmade_Parsed_KnownIDList.Contains($"{Reference_dataList_Object.id}") & Reference_dataList_Object.id != null)
                            {
                                RawFanmade_Parsed.dataList.Add(Reference_dataList_Object);
                                SomethingWasAdded = true;


                                if (!OutputReport.UntranslatedElementsReport.MissingIDs.ContainsKey(CheckFileName))
                                {
                                    OutputReport.UntranslatedElementsReport.MissingIDs[CheckFileName] = new List<dynamic>();
                                }
                                OutputReport.UntranslatedElementsReport.MissingIDsCount++;
                                OutputReport.UntranslatedElementsReport.MissingIDs[CheckFileName].Add(Reference_dataList_Object.id);
                            }
                        }
                    }

                    if (SomethingWasAdded) return JsonConvert.SerializeObject(RawFanmade_Parsed, Formatting.Indented).Replace("\r\n", "\n");
                    else return RawFanmade_LocalizationJson;
                }
            }

            internal abstract class ShorthandsTransform
            {
                internal protected static Dictionary<string, string> GetKeywordColors(string FilePath)
                {
                    Dictionary<string, string> Export = new();
                    if (File.Exists(FilePath))
                    {
                        foreach (string Line in File.ReadAllLines(FilePath))
                        {
                            if (Line.Contains(" ¤ "))
                            {
                                string[] ColorPair = Line.Split(" ¤ ");

                                Export[ColorPair[0].Trim()] = ColorPair[1].Trim();
                            }
                        }
                    }

                    return Export;
                }
                internal protected static Dictionary<string, string> KeywordColors = GetKeywordColors(@"⇲ Assets Directory\Keyword Colors.txt");

                internal protected static string Convert(string Text, Regex ShorthandsPattern, string CheckFileName = "")
                {
                    Text = ShorthandsPattern.Replace(Text, Match =>
                    {
                        if (!Match.Groups["ID"].Value.Equals(""))
                        {
                            string ID = Match.Groups["ID"].Value;
                            string Color = Match.Groups["Color"].Value;
                            string Name = Match.Groups["Name"].Value;
                            string ExportTMPro = $"<sprite name=\\\"{ID}\\\"><color={(!Color.Equals("") ? Color : (KeywordColors.ContainsKey(ID) ? KeywordColors[ID] : "#9f6a3a"))}><u><link=\\\"{ID}\\\">{Name}</link></u></color>";
                            OutputReport.ShorthandsConversionInfo.ConvertedCount++;

                            if (!KeywordColors.ContainsKey(ID) & !OutputReport.ShorthandsConversionInfo.KeywordsWithoutColor.Contains(ID))
                            {
                                OutputReport.ShorthandsConversionInfo.KeywordsWithoutColor.Add(ID);
                            }

                            return ExportTMPro;
                        }
                        else
                        {
                            return Match.Value;
                        }
                    });

                    return Text;
                }
            }

            internal abstract class MergedFont
            {
                // Template safe area for tags or format insertions at special fonts conversion
                private protected static Dictionary<string, string> SafeEscape = new Dictionary<string, string>()
                {
                    ["\u0410"] = "\uD800",
                    ["\u0411"] = "\uD801",
                    ["\u0412"] = "\uD802",
                    ["\u0413"] = "\uD803",
                    ["\u0414"] = "\uD804",
                    ["\u0415"] = "\uD805",
                    ["\u0416"] = "\uD806",
                    ["\u0417"] = "\uD807",
                    ["\u0418"] = "\uD808",
                    ["\u0419"] = "\uD809",
                    ["\u041a"] = "\uD80A",
                    ["\u041b"] = "\uD80B",
                    ["\u041c"] = "\uD80C",
                    ["\u041d"] = "\uD80D",
                    ["\u041e"] = "\uD80E",
                    ["\u041f"] = "\uD80F",
                    ["\u0420"] = "\uD810",
                    ["\u0421"] = "\uD811",
                    ["\u0422"] = "\uD812",
                    ["\u0423"] = "\uD813",
                    ["\u0424"] = "\uD814",
                    ["\u0425"] = "\uD815",
                    ["\u0426"] = "\uD816",
                    ["\u0427"] = "\uD817",
                    ["\u0428"] = "\uD818",
                    ["\u0429"] = "\uD819",
                    ["\u042a"] = "\uD81A",
                    ["\u042b"] = "\uD81B",
                    ["\u042c"] = "\uD81C",
                    ["\u042d"] = "\uD81D",
                    ["\u042e"] = "\uD81E",
                    ["\u042f"] = "\uD81F",
                    ["\u0430"] = "\uD820",
                    ["\u0431"] = "\uD821",
                    ["\u0432"] = "\uD822",
                    ["\u0433"] = "\uD823",
                    ["\u0434"] = "\uD824",
                    ["\u0435"] = "\uD825",
                    ["\u0436"] = "\uD826",
                    ["\u0437"] = "\uD827",
                    ["\u0438"] = "\uD828",
                    ["\u0439"] = "\uD829",
                    ["\u043a"] = "\uD82A",
                    ["\u043b"] = "\uD82B",
                    ["\u043c"] = "\uD82C",
                    ["\u043d"] = "\uD82D",
                    ["\u043e"] = "\uD82E",
                    ["\u043f"] = "\uD82F",
                    ["\u0440"] = "\uD830",
                    ["\u0441"] = "\uD831",
                    ["\u0442"] = "\uD832",
                    ["\u0443"] = "\uD833",
                    ["\u0444"] = "\uD834",
                    ["\u0445"] = "\uD835",
                    ["\u0446"] = "\uD836",
                    ["\u0447"] = "\uD837",
                    ["\u0448"] = "\uD838",
                    ["\u0449"] = "\uD839",
                    ["\u044a"] = "\uD83A",
                    ["\u044b"] = "\uD83B",
                    ["\u044c"] = "\uD83C",
                    ["\u044d"] = "\uD83D",
                    ["\u044e"] = "\uD83E",
                    ["\u044f"] = "\uD83F",
                    ["0"] = "\uD840",
                    ["1"] = "\uD841",
                    ["2"] = "\uD842",
                    ["3"] = "\uD843",
                    ["4"] = "\uD844",
                    ["5"] = "\uD845",
                    ["6"] = "\uD846",
                    ["7"] = "\uD847",
                    ["8"] = "\uD848",
                    ["9"] = "\uD849",
                    ["a"] = "\uD84A",
                    ["b"] = "\uD84B",
                    ["c"] = "\uD84C",
                    ["d"] = "\uD84D",
                    ["e"] = "\uD84E",
                    ["f"] = "\uD84F",
                    ["g"] = "\uD850",
                    ["h"] = "\uD851",
                    ["i"] = "\uD852",
                    ["j"] = "\uD853",
                    ["k"] = "\uD854",
                    ["l"] = "\uD855",
                    ["m"] = "\uD856",
                    ["n"] = "\uD857",
                    ["o"] = "\uD858",
                    ["p"] = "\uD859",
                    ["q"] = "\uD85A",
                    ["r"] = "\uD85B",
                    ["s"] = "\uD85C",
                    ["t"] = "\uD85D",
                    ["u"] = "\uD85E",
                    ["v"] = "\uD85F",
                    ["w"] = "\uD860",
                    ["x"] = "\uD861",
                    ["y"] = "\uD862",
                    ["z"] = "\uD863",
                    ["A"] = "\uD864",
                    ["B"] = "\uD865",
                    ["C"] = "\uD866",
                    ["D"] = "\uD867",
                    ["E"] = "\uD868",
                    ["F"] = "\uD869",
                    ["G"] = "\uD86A",
                    ["H"] = "\uD86B",
                    ["I"] = "\uD86C",
                    ["J"] = "\uD86D",
                    ["K"] = "\uD86E",
                    ["L"] = "\uD86F",
                    ["M"] = "\uD870",
                    ["N"] = "\uD871",
                    ["O"] = "\uD872",
                    ["P"] = "\uD873",
                    ["Q"] = "\uD874",
                    ["R"] = "\uD875",
                    ["S"] = "\uD876",
                    ["T"] = "\uD877",
                    ["U"] = "\uD878",
                    ["V"] = "\uD879",
                    ["W"] = "\uD87A",
                    ["X"] = "\uD87B",
                    ["Y"] = "\uD87C",
                    ["Z"] = "\uD87D",
                    ["!"] = "\uD87E",
                    ["\""] = "\uD87F",
                    ["#"] = "\uD880",
                    ["$"] = "\uD881",
                    ["%"] = "\uD882",
                    ["&"] = "\uD883",
                    ["\\"] = "\uD884",
                    ["'"] = "\uD885",
                    ["("] = "\uD886",
                    [")"] = "\uD887",
                    ["*"] = "\uD888",
                    ["+"] = "\uD889",
                    [","] = "\uD88A",
                    ["-"] = "\uD88B",
                    ["."] = "\uD88C",
                    ["/"] = "\uD88D",
                    [":"] = "\uD88E",
                    [";"] = "\uD88F",
                    ["<"] = "\uD890",
                    ["="] = "\uD891",
                    [">"] = "\uD892",
                    ["?"] = "\uD893",
                    ["@"] = "\uD894",
                    ["["] = "\uD895",
                    ["]"] = "\uD896",
                    ["^"] = "\uD897",
                    ["_"] = "\uD898",
                    ["`"] = "\uD899",
                    ["{"] = "\uD89A",
                    ["|"] = "\uD89B",
                    ["}"] = "\uD89C",
                    ["~"] = "\uD89D"
                };
                private protected static Regex PlacementRegex = new Regex(@"""(?<PropertyValue>\[font=(?<FontName>\w+)\](.*?))(?<Afterward>""(,)?(\r)?\n)");

                internal protected record FontParameters
                {
                    public Dictionary<string, Dictionary<string, string>> ReplacementMapsList { get; private set; } = new Dictionary<string, Dictionary<string, string>>();

                    public FontParameters(string ReplacementMapPath)
                    {
                        if (File.Exists(ReplacementMapPath))
                        {
                            JObject ReplacementMapJson = JObject.Parse(File.ReadAllText(ReplacementMapPath));
                            var DefinedReplacementMaps = ReplacementMapJson.SelectTokens("$.*");
                            foreach (JProperty ReplacementMapHeader in ReplacementMapJson.Properties())
                            {
                                ReplacementMapsList[ReplacementMapHeader.Name] = ReplacementMapHeader.Value.ToObject<Dictionary<string, string>>();
                            }
                        }
                    }
                }


                internal protected static string PlaceMergedFonts(string Text, FontParameters FontParameters, string CheckFileName = "")
                {
                    bool SuccessReplace = false;
                    string PlacedFont = "";

                    if (!OutputReport.SpecialFontsConversionReport.AttachedFonts.ContainsKey(CheckFileName)) OutputReport.SpecialFontsConversionReport.AttachedFonts[CheckFileName] = new Dictionary<string, string>();
                    Text = PlacementRegex.Replace(Text, Match =>
                    {
                        GroupCollection Groups = Match.Groups;
                        string AttachedFontName = Groups["FontName"].Value;
                        if (FontParameters.ReplacementMapsList.ContainsKey(AttachedFontName))
                        {
                            PlacedFont = AttachedFontName;
                            SuccessReplace = true;

                            string Replacement = Groups["PropertyValue"].Value;

                            Replacement = RegexRemove(Replacement, new Regex(@"\[font=\w+\]"));


                            Replacement = SafeEscapeConvert(Replacement, Reversal: false);
                            foreach (KeyValuePair<string, string> CharacterReplacement in FontParameters.ReplacementMapsList[AttachedFontName])
                            {
                                Replacement = Replacement.Replace(CharacterReplacement.Key, CharacterReplacement.Value);
                            }
                            Replacement = SafeEscapeConvert(Replacement, Reversal: true);


                            int LineNumber = Text[0..Match.Index].LinesCount() + 1;
                            OutputReport.SpecialFontsConversionReport.AttachedFonts[CheckFileName][$"LINE NO.{LineNumber} :: {Groups["PropertyValue"].Value}"] = $"{Replacement} :: {Replacement.ToUnicodeSequence()}";
                            
                            Replacement = @$"""{Replacement}{Groups["Afterward"]}";
                            return Replacement;
                        }
                        else return Groups[0].Value;
                    });

                    if (SuccessReplace)
                    {
                        if (!OutputReport.SpecialFontsConversionReport.UsedFontsInfo.ContainsKey(PlacedFont))
                        {
                            OutputReport.SpecialFontsConversionReport.UsedFontsInfo[PlacedFont] = FontParameters.ReplacementMapsList[PlacedFont].SingleCharValuesToUnicode();
                        }
                    }

                    return Text;
                }


                internal protected static string SafeEscapeConvert(string Text, bool Reversal = false)
                {
                    Dictionary<string, string> CurrentSafeEscape = SafeEscape;
                
                    if (!Reversal)
                    {
                        Text = Regex.Replace(Text, @"(<.*?>)|({\d+}){1}|(\\n)", Match =>
                        {
                            string TagValue = Match.Value;

                            foreach(KeyValuePair<string, string> CharacterReplacement in SafeEscape)
                            {
                                TagValue = TagValue.Replace(CharacterReplacement.Key, CharacterReplacement.Value);
                            }

                            return TagValue;
                        });
                    }
                    else
                    {
                        foreach (var i in SafeEscape)
                        {
                            Text = Text.Replace(i.Value, i.Key);
                        }
                    }
                    return Text;
                }
            }
        }
        internal abstract class Neccessary
        {
            internal abstract class Shorthands
            {
                internal protected static Regex MTL = new Regex(@"\[(?<ID>\w+):`(?<Name>.*?)`\](\((?<Color>#[a-fA-F0-9]{6})\))?");
                internal protected static Regex Crescent_Corporation = new Regex(@"\[(?<ID>\w+):\*(?<Name>.*?)\*\](\((?<Color>#[a-fA-F0-9]{6})\))?");
            }
        }



        internal abstract class TranslationBuilder
        {
            internal protected record ExportParameters
            {
                public string RawFanmade_LocalizationPath { get; set; }
                public string Reference_LocalizationPath { get; set; }
                public string ReferenceFilesPrefix { get; set; }
                public string KeywordColorsFile { get; set; }
                public string OutputDirectory { get; set; }
                public ActionsProvider.MergedFont.FontParameters MergedFontParameters { get; set; }
                public Regex ShorthandsPattern { get; set; }
                public FontFiles Fonts { get; set; }
            }
            internal protected record FontFiles
            {
                public string ContextFontPath { get; set; }
                public string TitleFontPath { get; set; }
            }

            internal protected static async Task DirectExport(ExportParameters Parameters)
            {
                await Task.Run(async () =>
                {
                    await Task.Delay(1);
                    Parameters.RawFanmade_LocalizationPath = Path.GetFullPath(Parameters.RawFanmade_LocalizationPath);
                    Parameters.Reference_LocalizationPath  = Path.GetFullPath(Parameters.Reference_LocalizationPath);
                    Parameters.OutputDirectory             = Path.GetFullPath(Parameters.OutputDirectory);
                    //rin(Parameters.RawFanmade_LocalizationPath);
                    //ActionsProvider.ShorthandsTransform.KeywordColors = ActionsProvider.ShorthandsTransform.GetKeywordColors(Parameters.KeywordColorsFile);

                    if (Directory.Exists(Parameters.RawFanmade_LocalizationPath) & Directory.Exists(Parameters.Reference_LocalizationPath))
                    {
                        OutputReport = new OutputReportManager.OutputReport(Parameters);

                        List<string> PrimaryExportedFiles = new List<string>();

                        CreateDirectoryTree(
                            HeaderDirectory: Parameters.OutputDirectory,
                            SubDirectories: [
                                @"BattleAnnouncerDlg",
                                @"BgmLyrics",
                                @"EGOVoiceDig",
                                @"PersonalityVoiceDlg",
                                @"StoryData",
                                @"Font",
                                @"Font\Context",
                                @"Font\Title"
                            ]
                        );

                        MainControl.Dispatcher.Invoke(() =>
                        {
                            MainControl.ExportedFilesList.Children.Clear();
                            MainControl.OpenReportFolder_Button.Visibility = Visibility.Collapsed;
                            MainControl.OpenReportFile_Button.Visibility = Visibility.Collapsed;
                            MainControl.Border_PreviewMouseLeftButtonUp_DisableCover.Visibility = Visibility.Visible;
                        });

                        IEnumerable<FileInfo> TargetFiles_RawFanmadeLocalization = new DirectoryInfo(Parameters.RawFanmade_LocalizationPath)
                            .GetFiles("*.json", SearchOption.AllDirectories)
                            .Where(file => !file.FullName.Contains(@"\.vs\"))
                            .OrderBy(x => x.LastWriteTime)
                            .Reverse();

                        int ExportedCounter = 0;
                        int ProgressCounter = 0;
                        int FilesTotalCount = TargetFiles_RawFanmadeLocalization.Count();

                        MainControl.FilesCounter.Dispatcher.Invoke(() =>
                        {
                            MainControl.FilesCounter.Visibility = Visibility.Visible;
                            MainControl.FilesCounter.Text = $"{ExportedCounter}/{FilesTotalCount}";
                        });

                        foreach (FileInfo LocalizeFile in TargetFiles_RawFanmadeLocalization)
                        {
                            ProgressCounter++;
                            MainControl.FilesCounter.Dispatcher.Invoke(() =>
                            {
                                MainControl.FilesCounter.Text = $"{ProgressCounter}/{FilesTotalCount}";
                            });

                            string LocalizeFile_Text = File.ReadAllText(LocalizeFile.FullName);
                            string LocalizeFile_RelativePath = LocalizeFile.FullName[(Parameters.RawFanmade_LocalizationPath.Length + 1)..];

                            // Append missing content
                            string ReferenceFilePath = @$"{Parameters.Reference_LocalizationPath}\{Parameters.ReferenceFilesPrefix}{LocalizeFile_RelativePath}";
                            if (File.Exists(ReferenceFilePath))
                            {
                                LocalizeFile_Text = ActionsProvider.MissingJsonIDManager.CompareAppend(LocalizeFile_Text, File.ReadAllText(ReferenceFilePath), LocalizeFile_RelativePath.Replace("\\", "/"));
                            }

                            // Convert shorthands
                            if (Parameters.ShorthandsPattern != null & LocalizeFile.Name.StartsWithOneOf(["Skills", "Passive", "EGOgift", "PanicInfo", "MentalCondition", "BattleKeywords", "Bufs"]))
                            {
                                LocalizeFile_Text = ActionsProvider.ShorthandsTransform.Convert(LocalizeFile_Text, Parameters.ShorthandsPattern);

                                if (OutputReport.ShorthandsConversionInfo.UsedPattern == null)
                                {
                                    OutputReport.ShorthandsConversionInfo.UsedPattern = Parameters.ShorthandsPattern.ToString();
                                }
                            }

                            // Place <style="highlight"></style> placeholders to descs and coins to avoid incorrect highlighting on uptie (e.g. if coin on some uptie level has <style> and at the next uptie this coin desc is same but without <style>, game automatically highlights whole coin desc by some logic)
                            if (LocalizeFile.Name.ToLower().ContainsOneOf(["skills.json", "_personality-"]))
                            {
                                LocalizeFile_Text = PlaceStyleHighlightPlaceholders(LocalizeFile_Text);
                            }

                            // Apply special fonts if possible
                            if (Parameters.MergedFontParameters != null & LocalizeFile_Text.Contains("[font="))
                            {
                                LocalizeFile_Text = ActionsProvider.MergedFont.PlaceMergedFonts(LocalizeFile_Text, Parameters.MergedFontParameters, LocalizeFile_RelativePath.Replace("\\", "/"));
                            }

                            // Write file
                            if (!File.Exists(@$"{Parameters.OutputDirectory}\{LocalizeFile_RelativePath}"))
                            {
                                //rin($"Export: {LocalizeFile_RelativePath}");
                                File.WriteAllText(@$"{Parameters.OutputDirectory}\{LocalizeFile_RelativePath}", LocalizeFile_Text);
                                MainControl.ExportedFilesList.Dispatcher.Invoke(() =>
                                {
                                    MainControl.ExportedFilesList.Children.Add(new TextBlock() { Text = LocalizeFile_RelativePath, Foreground = ToColor("#B7B7B7"), FontSize = 19 });
                                });

                                ExportedCounter++;
                            }
                            else
                            {
                                if (!File.ReadAllText(@$"{Parameters.OutputDirectory}\{LocalizeFile_RelativePath}").Equals(LocalizeFile_Text))
                                {
                                    File.WriteAllText(@$"{Parameters.OutputDirectory}\{LocalizeFile_RelativePath}", LocalizeFile_Text);

                                    MainControl.ExportedFilesList.Dispatcher.Invoke(() =>
                                    {
                                        MainControl.ExportedFilesList.Children.Add(new TextBlock() { Text = LocalizeFile_RelativePath, Foreground = ToColor("#B7B7B7"), FontSize = 19 });
                                    });

                                    ExportedCounter++;

                                    //rin($"Export: {LocalizeFile_RelativePath}");
                                }
                            }
                            MainControl.ExportLogScrollv.Dispatcher.Invoke(() =>
                            {
                                MainControl.ExportLogScrollv.ScrollToVerticalOffset(MainControl.ExportLogScrollv.ScrollableHeight + 100);
                            });

                            PrimaryExportedFiles.Add(LocalizeFile.Name);
                        }


                        // Copy font files
                        if (File.Exists(Parameters.Fonts.ContextFontPath) & File.Exists(Parameters.Fonts.TitleFontPath))
                        {
                            try { File.Copy(Parameters.Fonts.ContextFontPath, @$"{Parameters.OutputDirectory}\Font\Context\{Parameters.Fonts.ContextFontPath.GetFilename()}"); } catch { }
                            try { File.Copy(Parameters.Fonts.TitleFontPath, @$"{Parameters.OutputDirectory}\Font\Title\{Parameters.Fonts.TitleFontPath.GetFilename()}");       } catch { }
                        }


                        // Add missing files 
                        IEnumerable<FileInfo> TargetFiles_ReferenceLocalization = new DirectoryInfo(Parameters.Reference_LocalizationPath)
                            .GetFiles("*.json", SearchOption.AllDirectories)
                            .Where(x => !PrimaryExportedFiles.Contains(x.Name.RemovePrefix(Parameters.ReferenceFilesPrefix)));

                        FilesTotalCount += TargetFiles_ReferenceLocalization.Count();

                        foreach (FileInfo MissingFileToAdd in TargetFiles_ReferenceLocalization)
                        {
                            OutputReport.UntranslatedElementsReport.UntranslatedFilesCount++;
                            string MissingFileToAdd_RelativePath = MissingFileToAdd.FullName[(Parameters.Reference_LocalizationPath.Length + 1)..].Replace(Parameters.ReferenceFilesPrefix, "");
                            try
                            {
                                OutputReport.UntranslatedElementsReport.UntranslatedFiles.Add(MissingFileToAdd_RelativePath.Replace("\\", "/"));

                                ProgressCounter++;

                                MainControl.FilesCounter.Dispatcher.Invoke(() =>
                                {
                                    MainControl.FilesCounter.Text = $"{ProgressCounter}/{FilesTotalCount} [REF]";
                                });

                                if (!File.Exists(@$"{Parameters.OutputDirectory}\{MissingFileToAdd_RelativePath}"))
                                {
                                    MissingFileToAdd.CopyTo(@$"{Parameters.OutputDirectory}\{MissingFileToAdd_RelativePath}");
                                    ExportedCounter++;

                                    MainControl.Dispatcher.Invoke(() =>
                                    {
                                        MainControl.ExportedFilesList.Children.Add(new TextBlock() { Text = "[REF] " + MissingFileToAdd_RelativePath, Foreground = ToColor("#B7B7B7"), FontSize = 19 });
                                        MainControl.ExportLogScrollv.ScrollToVerticalOffset(MainControl.ExportLogScrollv.ScrollableHeight + 100);
                                    });
                                }
                            }
                            catch (Exception ex) { rin(ex.ToString()); }
                        }




                        try
                        {
                            OutputReport.MarkSerialize(
                                "Export Records.json",
                                ManualCorruptParameter: OutputReport.ShorthandsConversionInfo.KeywordsWithoutColor.Count > 0 ? "Undefined Colors for:" : ""
                            );
                        }
                        catch { }


                        if (Configurazione.Settings.Internal.AutomaticallyOpenDestinationDirectoryAfterCompletion)
                        {
                            Process.Start("explorer.exe", Parameters.OutputDirectory.Replace("/", "\\"));
                        }
                        if (Configurazione.Settings.Internal.AutomaticallyOpenReportFileAfterCompletion)
                        {
                            OpenWithDefaultProgram(@"Export Records.json");
                        }

                        MainControl.Dispatcher.Invoke(() =>
                        {
                            MainControl.OpenReportFolder_Button.Visibility = Visibility.Visible;
                            MainControl.OpenReportFile_Button.Visibility = Visibility.Visible;
                            
                            MainControl.ExportedFilesList.Children.Add(new TextBlock()
                            {
                                Margin = new Thickness(0, 5, 0, 0),
                                Text =
                                $"(View report for detailed information)" +
                                $"\nFiles exported: {ExportedCounter}" +
                                $"\nUntranslated files: {OutputReport.UntranslatedElementsReport.UntranslatedFilesCount}" +
                                $"\nMissing IDs: {OutputReport.UntranslatedElementsReport.MissingIDsCount}" +
                                (OutputReport.ShorthandsConversionInfo.ConvertedCount > 0 ? $"\nShorthands converted: {OutputReport.ShorthandsConversionInfo.ConvertedCount}" : ""),
                                Foreground = ToColor("#B7B7B7"),
                                FontSize = 23
                            });

                            MainControl.ExportLogScrollv.ScrollToVerticalOffset(MainControl.ExportLogScrollv.ScrollableHeight + 1000);
                        });

                        MainControl.Dispatcher.Invoke(() =>
                        {
                            MainControl.Border_PreviewMouseLeftButtonUp_DisableCover.Visibility = Visibility.Collapsed;
                        });

                        await Task.Delay(2100);
                    }
                });
                MainControl.FilesCounter.Visibility = Visibility.Collapsed;
            }
        }
    }
}