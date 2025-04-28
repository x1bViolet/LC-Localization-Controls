using System;
using System.Windows;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using static TexelExtension.ExternalBase;
using System.Windows.Threading;
using System.Windows.Controls;
using Configuration;
using GeneralResources;
using TexelExtension;

namespace LC_Localization_Controls.Converters
{
    internal abstract class LocalizationFullExport
    {
        internal protected static Dictionary<string, dynamic> T = new();
        internal protected static void InitT() => T = MainWindow.T;

        internal protected static Dictionary<string, List<FontRule>> InternalFont = new();
        internal protected record FontRule
        {
            public FontRule(string FontName)
            {
                if (KSTFont.InternalFontsList.ContainsKey(FontName))
                {
                    FontMask = KSTFont.InternalFontsList[FontName];
                }
            }
            public Dictionary<string, string> FontMask { get; set; }
            public string FontName { get; set; }
            public string JsonPath { get; set; }
        }
        internal protected record ActionRules
        {
            public bool AppendMissingID { get; set; }
            public bool IgnoreStoryData { get; set; }
            public bool OnlyKeywords    { get; set; }
        }

        internal protected static void LoadConfigValues(string From)
        {
            string[] TomlDaxt = File.ReadAllLines(From);
            InternalFont.Clear();
            for (int LineIndex = 0; LineIndex <= (TomlDaxt.Count() - 1); LineIndex++)
            {
                string Line = TomlDaxt[LineIndex];
                string FontFilenamePattern = Regex.Match(Line, @"\[\[font_rules\.""(?<NamePattern>.*?)""\]\]").Groups["NamePattern"].Value;
                if (!FontFilenamePattern.Equals(""))
                {
                    string FontName = Regex.Match(TomlDaxt[LineIndex + 1], @"font = ""(?<FontName>\w+)""").Groups["FontName"].Value;
                    string JsonPath =             TomlDaxt[LineIndex + 2][8..^1].Replace(".[*]", "[*]");
                    
                    if (!InternalFont.ContainsKey(FontFilenamePattern)) InternalFont[FontFilenamePattern] = new();
                    
                    InternalFont[FontFilenamePattern].Add( new FontRule(FontName){JsonPath = JsonPath} );
                }
            }
            T["Selected font info Rules count"].Text = CustomLanguage.Dynamic["Selected font info Rules count"].ExTern(InternalFont.Keys.Count);
        }

        internal protected static string JsonPathConvert(string TargetString, string JsonPath, Dictionary<string, string> ReplaceSource)
        {
            JObject JParser = JObject.Parse(TargetString);
            foreach (var StringItem in JParser.SelectTokens(JsonPath))
            {
                StringItem.Replace(KSTFont.Convert($"{StringItem}", ReplaceSource));
            }
            TargetString = JParser.ToString(Formatting.Indented);
            
            return TargetString;
        }


        internal protected record UniversalType
        {
            public List<dynamic> dataList { get; set; }
        }

        internal protected static string AppendMissing(string JsonText, string OriginalJsonText)
        {
            UniversalType? JsonTextData = JsonConvert.DeserializeObject<UniversalType>(JsonText);
            UniversalType? OriginalJsonTextData = JsonConvert.DeserializeObject<UniversalType>(OriginalJsonText);
            try
            {
                Dictionary<int, string> IDSet_JsonText = new();

                int Indexer = 0;
                foreach (var i in JsonTextData.dataList)
                {
                    try
                    {
                        if (!$"{i.id}".Equals(""))
                        {
                            IDSet_JsonText[Indexer] = $"{i.id}";
                            Indexer++;
                        }
                    }
                    catch { }
                }
                Indexer = 0;
                //rin("-------------------------------");
                foreach (var i in OriginalJsonTextData.dataList)
                {
                    try
                    {
                        if (!IDSet_JsonText.ContainsValue($"{i.id}") & !$"{i.id}".Equals(""))
                        {
                            //rin($"Вставлен пропущенный ID: {i.id}");
                            JsonTextData.dataList.Add(i);
                        }
                    }
                    catch { }
                }

                string s = JsonConvert.SerializeObject(JsonTextData, Formatting.Indented).Replace("\r", "");
                return s + "\n";
            }
            catch
            {
                return JsonText;
            }
        }

        internal protected static List<string> KeywordFilesMatch = new List<string>
        {
            "Skills",
            "Passive",
            "EGOgift_",
            "PanicInfo",
            "MentalCondition",
            "BattleKeywords",
            "Bufs",
        };

        internal protected static StackPanel GenerateRuleInfoPanel(string S1 = "", string S2 = "", string S3 = "", string S4 = "")
        {
            return new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 5, 0, 0),
                Children =
                {
                    new TextBlock() // Ind. 0 : Icon
                    {
                        FontFamily = MainWindow.SegoeFluentIcons,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(0, -2, 0, 0),
                        Foreground = ToColor("#5B6097"),
                        FontSize = 25,
                        Text = ""
                    },
                    new TextBlock() // Ind. 1 : Processing rule string
                    {
                        FontFamily = MainWindow.GostTypeBU,
                        VerticalAlignment = VerticalAlignment.Center,
                        FontSize = 21,
                        Text = S1.Equals("") ? CustomLanguage.Dynamic["Current rule panel - Processing rule name"] : S1
                    },
                    new TextBlock() // Ind. 2 : Fontrule
                    {
                        FontFamily = MainWindow.GostTypeBU,
                        VerticalAlignment = VerticalAlignment.Center,
                        Foreground = ToColor("#5B6097"),
                        FontSize = 21,
                        Text = S2,
                    },
                    new TextBlock() // Ind. 3 : Current progress
                    {
                        FontFamily = MainWindow.GostTypeBU,
                        VerticalAlignment = VerticalAlignment.Center,
                        FontSize = 21,
                        Text = S3.Equals("") ? CustomLanguage.Dynamic["Current rule panel - Processing rule file"] : S3
                    },
                    new TextBlock() // Ind. 4 Filename
                    {
                        FontFamily = MainWindow.GostTypeBU,
                        VerticalAlignment = VerticalAlignment.Center,
                        Foreground = ToColor("#8883A0"),
                        FontSize = 21,
                        Text = S4
                    },
                }
            };
        }

        internal protected async static Task DirectExport(string LocalizationDirectory, string DestinationDirectory, bool ApplyFont, bool ConvertShorthands, bool DoNotIgnoreStoryData, bool InsertMissingID)
        {
            await Task.Run(() =>
            {
                StackPanel InfoLog = T["Full export log"] as StackPanel;
                InfoLog.Dispatcher.Invoke(() =>
                {
                    InfoLog.Children.Clear();
                });

                List<string> ExportedFiles = new List<string>();

                int Indexer = 0;
                foreach (KeyValuePair<string, List<FontRule>> CastFontRule in InternalFont)
                {
                    string AcquirePattern = CastFontRule.Key;
                    List<FontRule> TexlsFontRules = CastFontRule.Value;
                    try
                    {
                        InfoLog.Dispatcher.Invoke(() => {
                            InfoLog.Children.Add(GenerateRuleInfoPanel(S2: AcquirePattern));
                            T["Full export scrollviewer"].ScrollToVerticalOffset(T["Full export scrollviewer"].ScrollableHeight + 30);
                        });

                        List<FileInfo> ProcessingTargets = new DirectoryInfo(LocalizationDirectory).GetFiles(AcquirePattern)
                            .Where(File => !DoNotIgnoreStoryData ? !File.FullName.Contains(@"StoryData\") : true).ToList();

                        List<string> AppendedFiles = new List<string>();
                        string LocalizationMaskDirectory = @$"⇲ Asset Directory\Missing ID sources\{AppConfiguration.StringConfiguration["Missing ID source"]}";
                        if (Directory.Exists(LocalizationMaskDirectory) & InsertMissingID)
                        {
                            List<FileInfo> ProcessingTargets_Missing = new DirectoryInfo(LocalizationMaskDirectory).GetFiles(AcquirePattern)
                                .Where(File => !DoNotIgnoreStoryData ? !File.FullName.Contains(@"StoryData\") : true)
                                    .Where(file => !ProcessingTargets.ContainsFileInfoWithName(file.Name)).ToList();
                            foreach(var AppendedFile in ProcessingTargets_Missing)
                            {
                                //rin($"Вставлен пропущенный файл {AppendedFile.Name}");
                                AppendedFiles.Add(AppendedFile.Name);
                            }
                            ProcessingTargets = ProcessingTargets.Concat(ProcessingTargets_Missing).ToList();
                        }

                        int FileCounter = 0;
                        int FilesTotal = ProcessingTargets.Count();
                        if (TexlsFontRules.Count > 1)
                        {
                            FilesTotal = FilesTotal * TexlsFontRules.Count;
                        }

                        foreach (FileInfo LocalizeFile in ProcessingTargets)
                        {
                            string LocalizeValue = File.ReadAllText(LocalizeFile.FullName);

                            if (LocalizeFile.Name.StartsWithOneOf(KeywordFilesMatch) & ConvertShorthands)
                            {
                                LocalizeValue = ShorthandConverter.Redirect(LocalizeValue);
                            }

                            if (InsertMissingID)
                            {
                                string Filename_Strip = LocalizeFile.FullName[LocalizationDirectory.Length..];
                                string MissingIDSourceFile = $"{LocalizationMaskDirectory}{Filename_Strip}";

                                if (File.Exists(MissingIDSourceFile))
                                {
                                    //string old = LocalizeValue;
                                    LocalizeValue = AppendMissing(LocalizeValue, File.ReadAllText(MissingIDSourceFile));
                                    //if (!LocalizeValue.Equals(old)) rin($"Вставлены пропущенные ID в {LocalizeFile.Name}");
                                }
                            }

                            foreach (var FontConvertData in TexlsFontRules)
                            {
                                if (ApplyFont) LocalizeValue = JsonPathConvert(LocalizeValue, FontConvertData.JsonPath, FontConvertData.FontMask);
                                FileCounter++;
                            }

                            InfoLog.Dispatcher.Invoke(() =>
                            {
                                ((InfoLog.Children[Indexer] as StackPanel).Children[3] as TextBlock).Text = CustomLanguage.Dynamic["Current rule panel - Processing rule file"].Exform($"{FileCounter}", $"{FilesTotal}");
                                ((InfoLog.Children[Indexer] as StackPanel).Children[4] as TextBlock).Text = " " + LocalizeFile.Name;
                            });

                            string DestFullname = DestinationDirectory + LocalizeFile.FullName[LocalizationDirectory.Length..];
                            if (DestFullname.Contains(@$"{LocalizationMaskDirectory}\"))
                            {
                                DestFullname = DestFullname.Replace(@$"{LocalizationMaskDirectory}\", "");
                            }

                            List<string> TC1 = DestFullname.Split('\\').ToList();
                            string ExportDirectory = String.Join('\\', TC1.RemoveAtIndex(TC1.Count - 1));

                            if (File.Exists(DestFullname))
                            {
                                if (!File.ReadAllText(DestFullname).Equals(LocalizeValue))
                                {
                                    File.WriteAllText(DestFullname, LocalizeValue);
                                }
                            }
                            else
                            {
                                if (!Directory.Exists(ExportDirectory))
                                {
                                    Directory.CreateDirectory(ExportDirectory);
                                }

                                File.WriteAllText(DestFullname, LocalizeValue);
                            }

                            ExportedFiles.Add(LocalizeFile.Name);
                        }
                        Indexer++;
                    }
                    catch { }
                }



                InfoLog.Dispatcher.Invoke(() => {
                    InfoLog.Children.Add(GenerateRuleInfoPanel(
                        S2: CustomLanguage.Dynamic["Current rule panel - Appending other files rule"])
                    );
                    T["Full export scrollviewer"].ScrollToVerticalOffset(T["Full export scrollviewer"].ScrollableHeight + 30);
                });

                IEnumerable<FileInfo> ProcessingTargets_MissingFiles = new DirectoryInfo(LocalizationDirectory).GetFiles("*.json", SearchOption.AllDirectories)
                    .Where(file => !ExportedFiles.Contains(file.Name) & !file.FullName.Contains(@"\.vs\"))
                        .Where(file => !DoNotIgnoreStoryData ? !file.FullName.Contains(@"StoryData\") : true);

                int FileCounter_MF = 1;
                int FileCounter_Total_MF = ProcessingTargets_MissingFiles.Count();

                foreach (FileInfo OtherLocalizeFile in ProcessingTargets_MissingFiles
                ) {
                    InfoLog.Dispatcher.Invoke(() =>
                    {
                        ((InfoLog.Children[Indexer] as StackPanel).Children[3] as TextBlock).Text = CustomLanguage.Dynamic["Current rule panel - Processing rule file"].Exform($"{FileCounter_MF}", $"{FileCounter_Total_MF}");
                        ((InfoLog.Children[Indexer] as StackPanel).Children[4] as TextBlock).Text = " " + OtherLocalizeFile.Name;
                    });
                    FileCounter_MF++;

                    string WriteValue = File.ReadAllText(OtherLocalizeFile.FullName);

                    if (InsertMissingID)
                    {
                        string Filename_Strip = OtherLocalizeFile.FullName[LocalizationDirectory.Length..];
                        string MissingIDSourceFile = @$"⇲ Asset Directory\Missing ID sources\{AppConfiguration.StringConfiguration["Missing ID source"]}{Filename_Strip}";

                        if (File.Exists(MissingIDSourceFile))
                        {
                            //string old = WriteValue;
                            WriteValue = AppendMissing(WriteValue, File.ReadAllText(MissingIDSourceFile));
                            //if (!WriteValue.Equals(old)) rin($"Вставлены пропущенные ID в {OtherLocalizeFile.Name}");
                        }
                    }
                    if (ConvertShorthands & OtherLocalizeFile.Name.StartsWithOneOf(KeywordFilesMatch))
                    {
                        WriteValue = ShorthandConverter.Redirect(WriteValue);
                    }


                    string DestFullname = DestinationDirectory + OtherLocalizeFile.FullName[LocalizationDirectory.Length..];

                    List<string> TC1 = DestFullname.Split('\\').ToList();
                    string ExportDirectory = String.Join('\\', TC1.RemoveAtIndex(TC1.Count - 1));

                    if (File.Exists(DestFullname))
                    {
                        if (!File.ReadAllText(DestFullname).Equals(WriteValue))
                        {
                            File.WriteAllText(DestFullname, WriteValue);
                            ExportedFiles.Add(OtherLocalizeFile.Name);
                        }
                    }
                    else
                    {
                        if (!Directory.Exists(ExportDirectory))
                        {
                            Directory.CreateDirectory(ExportDirectory);
                        }

                        File.WriteAllText(DestFullname, WriteValue);
                        ExportedFiles.Add(OtherLocalizeFile.Name);
                    }
                }
            });

            try
            {
                T["Last export info"].Text = CustomLanguage.Dynamic["Last export bottom info"].Exform(DateTime.Now.ToString(CustomLanguage.Dynamic["Last export bottom info (Date format)"]), DestinationDirectory);
                AppConfiguration.SaveConfiguration("Last Export info", T["Last export info"].Text);;
            }
            catch { }
        }
    }
}
