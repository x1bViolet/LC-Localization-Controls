using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using static Translation_Devouring_Siltcurrent.Configurazione;
using static Translation_Devouring_Siltcurrent.JsonSerialization;
using static Translation_Devouring_Siltcurrent.LocalizationProcessingModules.JsonPathRegexConversions;
using static Translation_Devouring_Siltcurrent.LocalizationProcessingModules.Log;
using static Translation_Devouring_Siltcurrent.LocalizationProcessingModules.Merged_Fonts_Shenanigans;
using static Translation_Devouring_Siltcurrent.LocalizationProcessingModules.MissingJsonObjects;
using static Translation_Devouring_Siltcurrent.LocalizationProcessingModules.Shorthands;
using static Translation_Devouring_Siltcurrent.LocalizationProcessingModules.StylePlaceholders;
using static Translation_Devouring_Siltcurrent.MainWindow;
using static Translation_Devouring_Siltcurrent.Requirements;


namespace Translation_Devouring_Siltcurrent
{
    public static class LimbusTranslationProcessor
    {
        public static ProcessingReport CurrentReport = new();
        public static async Task DoDirectExport()
        {
            MainControl.TaskBarProgressIcon.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Normal;

            MainControl.FilesCounter.Visibility = Visibility.Visible;
            MainControl.ViewReportButtons.Visibility = Visibility.Collapsed;
            MainControl.StartButton.IsEnabled = false;

            MainControl.ExportedFilesLog.Children.Clear();

            KeywordColors.Clear();
            LoadedMergedFontReplacementMap.Clear();
            LoadedMergedFontMultipleApplyConfig.Clear();

            bool CancelProcessing = false;


            try
            {
                #region Pre-init warnings about something that is wrong
                if (CurrentProfile.MergedFonts.ConvertFontsByMultipleApplyConfig | CurrentProfile.MergedFonts.ConvertFontsByMarkers)
                {
                    if (File.Exists(CurrentProfile.MergedFonts.MergedFontCharactersReplacementMap))
                    {
                        LoadedMergedFontReplacementMap =
                            JsonConvert.DeserializeObject<Dictionary<string, Dictionary<String, String>>>(value: File.ReadAllText(path: CurrentProfile.MergedFonts.MergedFontCharactersReplacementMap));
                    }
                    else
                    {
                        MessageBoxResult MissingReplacementMapResult = MessageBox.Show(
                            caption: "Missing Merged Font Replacement Map",
                            messageBoxText: "Some Merged Font conversions is enabled, but Replacement Map for Merged Font is missing. Merged Fonts cannot work without it.\n\nClick \"Cancel\" to cancel localization files processing, \"OK\" to ignore this.",
                            button: MessageBoxButton.OKCancel,
                            icon: MessageBoxImage.Warning
                        );

                        if (MissingReplacementMapResult == MessageBoxResult.Cancel) CancelProcessing = true;
                    }
                }


                if (CurrentProfile.MergedFonts.ConvertFontsByMultipleApplyConfig)
                {
                    if (File.Exists(CurrentProfile.MergedFonts.MergedFontMultipleApplyConfig))
                    {
                        LoadedMergedFontMultipleApplyConfig =
                            JsonConvert.DeserializeObject<Dictionary<string, List<MergedFontRule>>>(value: File.ReadAllText(path: CurrentProfile.MergedFonts.MergedFontMultipleApplyConfig));

                        if (CheckForInvalidMergedFontFontRules() == MessageBoxResult.Cancel) CancelProcessing = true;
                    }
                    else
                    {
                        MessageBoxResult MissingMergedFontMultipleApplyConfig = MessageBox.Show(
                            caption: "Missing Merged Font Multiple Apply Config",
                            messageBoxText: "Merged Font Multiple Apply Config conversion is enabled, but Config file itself is missing.\n\nClick \"Cancel\" to cancel localization files processing, \"OK\" to ignore this.",
                            button: MessageBoxButton.OKCancel,
                            icon: MessageBoxImage.Warning
                        );

                        if (MissingMergedFontMultipleApplyConfig == MessageBoxResult.Cancel) CancelProcessing = true;
                    }
                }



                if (CurrentProfile.JsonPathMultipleRegexConversions.DoJsonPathMultipleRegexConversions)
                {
                    if (File.Exists(CurrentProfile.JsonPathMultipleRegexConversions.JsonPathMultipleRegexConversionsConfigFile))
                    {
                        LoadJsonPathMultipleRegexConversions(CurrentProfile.JsonPathMultipleRegexConversions.JsonPathMultipleRegexConversionsConfigFile);
                    }
                    else
                    {
                        MessageBoxResult MissingMergedFontMultipleApplyConfig = MessageBox.Show(
                            caption: "Missing JsonPath Multiple Regex Conversions config",
                            messageBoxText: "JsonPath Multiple Regex Conversions is enabled, but Config file itself is missing.\n\nClick \"Cancel\" to cancel localization files processing, \"OK\" to ignore this.",
                            button: MessageBoxButton.OKCancel,
                            icon: MessageBoxImage.Warning
                        );

                        if (MissingMergedFontMultipleApplyConfig == MessageBoxResult.Cancel) CancelProcessing = true;
                    }
                }



                if (CurrentProfile.Misc.ConvertKeywordShorthands)
                {
                    if (File.Exists(CurrentProfile.Misc.KeywordShorthandsColorsInfoFile))
                    {
                        ReadKeywordColors();
                    }
                    else
                    {
                        MessageBoxResult MissingKeywordsShorthandsColorsInfo = MessageBox.Show(
                            caption: "Missing Keyword Colors file",
                            messageBoxText: "Keywords Shorthands conversion is enabled, but colors info file is missing.\n\nClick \"Cancel\" to cancel localization files processing, \"OK\" to ignore this (Then all converted keywords will be brown).",
                            button: MessageBoxButton.OKCancel,
                            icon: MessageBoxImage.Warning
                        );

                        if (MissingKeywordsShorthandsColorsInfo == MessageBoxResult.Cancel) CancelProcessing = true;
                    }
                }



                if (CurrentProfile.FontFiles.AlsoCopyFontFiles)
                {
                    if (!File.Exists(CurrentProfile.FontFiles.ContextFontFile))
                    {
                        MessageBoxResult MissingContextFont = MessageBox.Show(
                            caption: "Missing Context font",
                            messageBoxText: "Fonts copying to Destination directory is enabled, but Context font file is missing.\n\nClick \"Cancel\" to cancel localization files processing, \"OK\" to ignore this (If there are only one of Title/Context fonts, then Limbus will use it as general font. If there are no fonts at all, then custom localization WILL NOT WORK).",
                            button: MessageBoxButton.OKCancel,
                            icon: MessageBoxImage.Warning
                        );

                        if (MissingContextFont == MessageBoxResult.Cancel) CancelProcessing = true;
                    }

                    if (!File.Exists(CurrentProfile.FontFiles.TitleFontFile))
                    {
                        MessageBoxResult MissingTitleFont = MessageBox.Show(
                            caption: "Missing Title font",
                            messageBoxText: "Fonts copying to Destination directory is enabled, but Title font file is missing.\n\nClick \"Cancel\" to cancel localization files processing, \"OK\" to ignore this (If there are only one of Title/Context fonts, then Limbus will use it as general font. If there are no fonts at all, then custom localization WILL NOT WORK).",
                            button: MessageBoxButton.OKCancel,
                            icon: MessageBoxImage.Warning
                        );

                        if (MissingTitleFont == MessageBoxResult.Cancel) CancelProcessing = true;
                    }
                }
                #endregion






                if (Directory.Exists(CurrentProfile.Paths.SourceDirectory) & CancelProcessing == false)
                {
                    LineBreakMode SelectedOutputLineBreakMode = Enum.TryParse(CurrentProfile.JsonFilesFormatting.LineBreakMode, out LineBreakMode ResultLineBreakMode)
                        ? ResultLineBreakMode
                        : LineBreakMode.KeepOriginal;

                    Formatting SelectedOutputFormatting = Enum.TryParse(CurrentProfile.JsonFilesFormatting.OutputJsonFormatting, out Formatting ResultFormatting)
                        ? ResultFormatting
                        : Formatting.Indented;

                    int? SelectedOutputIndentationSize = int.TryParse(CurrentProfile.JsonFilesFormatting.JsonIndentationSize, out int ResultIndentationSize)
                        ? ResultIndentationSize
                        : null;




                    string SourceDirectory = Path.GetFullPath(CurrentProfile.Paths.SourceDirectory.Trim());
                    string DestinationDirectory = Path.GetFullPath(CurrentProfile.Paths.DestinationDirectory.Trim());
                    string ReferenceDirectory = Directory.Exists(CurrentProfile.ReferenceLocalization.Directory.Trim()) ? Path.GetFullPath(CurrentProfile.ReferenceLocalization.Directory.Trim()) : "asdasdasd\0";

                    CurrentReport = new()
                    {
                        SourceDirectory = SourceDirectory.Replace("\\", "/"),
                        DestinationDirectory = DestinationDirectory.Replace("\\", "/"),
                        TranslationReferenceDirectory = ReferenceDirectory.Replace("\\", "/"),
                    };

                    CreateDirectoryTree(
                        HeaderDirectory: DestinationDirectory,
                        SubDirectories: [
                            @"BattleAnnouncerDlg",
                            @"BgmLyrics",
                            @"EGOVoiceDig",
                            @"PersonalityVoiceDlg",
                            @"StoryData",
                            CurrentProfile.FontFiles.AlsoCopyFontFiles ? @"Font"         : @"",
                            CurrentProfile.FontFiles.AlsoCopyFontFiles ? @"Font\Context" : @"",
                            CurrentProfile.FontFiles.AlsoCopyFontFiles ? @"Font\Title"   : @""
                        ]
                    );


                    string[] GeneralFilesWhitelist = GeneratePatternsList(CurrentProfile.GeneralLocalizationFilesWhitelist);

                    IEnumerable<FileInfo> TargetFiles = new DirectoryInfo(SourceDirectory)
                        .GetFiles("*.json", SearchOption.AllDirectories)
                        .Where(LocalizationFile => !LocalizationFile.FullName.Del(SourceDirectory).Contains(@".vs\")) // .vs folder ignore
                        .Where(LocalizationFile => LocalizationFile.FullName.Del(SourceDirectory + "\\").Replace("\\", "/").MatchesWithOneOf(GeneralFilesWhitelist)); // General whitelist

                    if (CurrentProfile.FilesProcessingOrder == "Recently changed first (Recommended)")
                    {
                        TargetFiles = TargetFiles.OrderBy(LocalizationFile => LocalizationFile.LastWriteTime).Reverse();
                    }


                    int ExportedFilesCounter = 0;
                    int TotalFilesCounter = 0;
                    int TotalFilesCount = TargetFiles.Count();

                    MainControl.FilesCounter.Text = $"0 / {TotalFilesCount}";



                    #region General conditions
                    string[] WhiteListOfMissingFilesToAppend
                        = GeneratePatternsList(CurrentProfile.ReferenceLocalization.MissingContentAppending.WhitelistOfFilesToAppend);
                
                    string[] WhiteListOfFilesToAddMissingIDsTo
                        = GeneratePatternsList(CurrentProfile.ReferenceLocalization.MissingContentAppending.WhitelistOfFilesToAddIDsTo);
                

                    string[] KeywordsShorthandsApplyingRange
                        = GeneratePatternsList(CurrentProfile.Misc.KeywordShorthandsFilesWhiteList);
                    #endregion


                    if (CurrentProfile.FontFiles.AlsoCopyFontFiles)
                    {
                        if (File.Exists(CurrentProfile.FontFiles.ContextFontFile))
                        {
                            string Destination = @$"{DestinationDirectory}\Font\Context\{Path.GetFileName(CurrentProfile.FontFiles.ContextFontFile)}";
                            if (File.Exists(Destination)) File.Delete(Destination);
                            File.Copy(CurrentProfile.FontFiles.ContextFontFile, Destination);
                        }
                    
                        if (File.Exists(CurrentProfile.FontFiles.TitleFontFile))
                        {
                            string Destination = @$"{DestinationDirectory}\Font\Title\{Path.GetFileName(CurrentProfile.FontFiles.TitleFontFile)}";
                            if (File.Exists(Destination)) File.Delete(Destination);
                            File.Copy(CurrentProfile.FontFiles.TitleFontFile, @$"{DestinationDirectory}\Font\Title\{Path.GetFileName(CurrentProfile.FontFiles.TitleFontFile)}");
                        }
                    }

                
                    /// async     await Task.Delay(1)
                    await Task.Run(() =>
                    {
                        List<string> PrimaryExportedFiles = [];
                        List<string> PrimaryCheckedFiles = [];
                    
                        foreach (FileInfo LocalizationFile in TargetFiles)
                        {
                            TotalFilesCounter++;

                            MainControl.TaskBarProgressIcon.Dispatcher.Invoke(() =>
                            {
                                MainControl.TaskBarProgressIcon.ProgressValue = (double)TotalFilesCounter / (double)TotalFilesCount;
                            });

                            MainControl.FilesCounter.Dispatcher.Invoke(() =>
                            {
                                MainControl.FilesCounter.Text = $"{TotalFilesCounter} / {TotalFilesCount}";
                            });

                            PrimaryCheckedFiles.Add(LocalizationFile.Name);
                        
                            string SourceFile_RelativePath = LocalizationFile.FullName.Del(SourceDirectory + "\\");
                            string OutputFile_DestinationFullPath = DestinationDirectory + "\\" + SourceFile_RelativePath;

                            try
                            {
                                // EN_ KR_ JP_ Prefix insertion to the right place
                                string ReferenceFile_RelativePath = SourceFile_RelativePath.Contains('\\')
                                    ? SourceFile_RelativePath.Insert(SourceFile_RelativePath.LastIndexOf('\\') + 1, CurrentProfile.ReferenceLocalization.Prefix)
                                    : $"{CurrentProfile.ReferenceLocalization.Prefix}{SourceFile_RelativePath}";
                                string ReferenceFile_FullPath = ReferenceDirectory + "\\" + ReferenceFile_RelativePath;


                                // AbDlg_DonQuixote.json, PersonalityVoiceDlg/Voice_DonQuixote_Bloodfiend_10310.json, ... (With regular slash instead of backslash)
                                string LoggingRelativeName = SourceFile_RelativePath.Replace("\\", "/");
                                

                                    // Currently not processed at this point
                                string ProcessedSourceFileText = File.ReadAllText(LocalizationFile.FullName);
                                string OriginalFileText = ProcessedSourceFileText;
                                UTF8Encoding OuputEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: CurrentProfile.JsonFilesFormatting.UTF8ByteOrderMarks switch
                                {
                                    "Keep BOM if its inside source file" => LocalizationFile.IsUTF8BOM(),
                                    "Save files with UTF-8 BOM encoding" => true,
                                    "Save files with regular UTF-8 encoding" => false,
                                    _ => LocalizationFile.IsUTF8BOM(),
                                });


                                {
                                    JToken JsonSyntaxTest = JToken.Parse(ProcessedSourceFileText); // Primary check for synatax errors (Throws exception if something is wrong -> `catch`)
                                }
                                
                                int OriginalJsonIndentationSize = ProcessedSourceFileText.GetJsonIndentationSize();
                                LineBreakMode OriginalLineBreakMode = ProcessedSourceFileText.DetermineLineBreakType();




                                

                                // Append missing IDs
                                if (CurrentProfile.ReferenceLocalization.MissingContentAppending.AppendMissingIDs &&
                                    LoggingRelativeName.MatchesWithOneOf(WhiteListOfFilesToAddMissingIDsTo)
                                ) {
                                    if (File.Exists(ReferenceFile_FullPath))
                                    {
                                        ProcessedSourceFileText = CompareAppendDataList(
                                            TargetJson: ProcessedSourceFileText,
                                            ReferenceJson: File.ReadAllText(ReferenceFile_FullPath),
                                            LoggingFileName: LoggingRelativeName
                                        );
                                    }
                                }



                                // Convert Shorthands
                                if (CurrentProfile.Misc.ConvertKeywordShorthands &&
                                    CurrentProfile.Misc.KeywordShorthandsRegexPattern != @"" &&
                                    LoggingRelativeName.MatchesWithOneOf(KeywordsShorthandsApplyingRange)
                                ) {
                                    ProcessedSourceFileText = ConvertShorthands(
                                        Text: ProcessedSourceFileText,
                                        ShorthandsPattern: CurrentProfile.Misc.KeywordShorthandsRegexPattern
                                    );
                                }



                                // Place <style> placeholders
                                if (CurrentProfile.Misc.AddStylePlaceholders &&
                                    LocalizationFile.Name.ToLower().ContainsOneOf("skills.json", "skills_ego.json", "_personality-")
                                ) {
                                    ProcessedSourceFileText = PlaceStyleHighlightPlaceholders(SkillsJsonText: ProcessedSourceFileText);
                                }



                                // Place merged fonts by markers
                                if (CurrentProfile.MergedFonts.ConvertFontsByMarkers &&
                                    ProcessedSourceFileText.Contains("\"[font=")
                                ) {
                                    ProcessedSourceFileText = PlaceMergedFontByMarkers(
                                        JsonText: ProcessedSourceFileText,
                                        LoggingFileName: LoggingRelativeName
                                    );
                                }


                                // Place merged fonts by multiple apply config
                                if (CurrentProfile.MergedFonts.ConvertFontsByMultipleApplyConfig &&
                                    LoggingRelativeName.MatchesWithWildcards(LoadedMergedFontMultipleApplyConfig.Keys, out List<string> MatchedFileNamePatterns1)
                                ) {
                                    foreach (string MatchedFileNamePattern in MatchedFileNamePatterns1)
                                    {
                                        ProcessedSourceFileText = PlaceMergedFontsByMultipleApplyConfig(
                                            JsonText: ProcessedSourceFileText,
                                            FontRules: LoadedMergedFontMultipleApplyConfig[MatchedFileNamePattern],
                                            LoggingFileName: LoggingRelativeName
                                        );
                                    }
                                }



                                // Do Regex Multiple Conversions
                                if (CurrentProfile.JsonPathMultipleRegexConversions.DoJsonPathMultipleRegexConversions &&
                                    LoggingRelativeName.MatchesWithWildcards(LoadedJsonPathMultipleRegexConversions.Keys, out List<string> MatchedFileNamePatterns2)
                                ) {
                                    foreach (string MatchedFileNamePattern in MatchedFileNamePatterns2)
                                    {
                                        ProcessedSourceFileText = DoMultipleRegexConversions(
                                            JsonText: ProcessedSourceFileText,
                                            ReplacementRules: LoadedJsonPathMultipleRegexConversions[MatchedFileNamePattern],
                                            LoggingFileName: LoggingRelativeName
                                        );
                                    }
                                }






                                string FinalFileText = JsonConvert.DeserializeObject(ProcessedSourceFileText).SerializeToFormattedText
                                (
                                    Formatting: SelectedOutputFormatting,

                                    IndentationSize: SelectedOutputIndentationSize != null
                                        ? (int)SelectedOutputIndentationSize
                                        : OriginalJsonIndentationSize,

                                    LineBreakMode: SelectedOutputLineBreakMode != LineBreakMode.KeepOriginal // Strictly selected (Not "Keep original" option)
                                        ? SelectedOutputLineBreakMode
                                        : OriginalLineBreakMode // Keep original
                                );

                                if (CurrentProfile.JsonFilesFormatting.EmptyLastLine == "Keep empty last line if its inside source file" && OriginalFileText[^1].EqualsOneOf('\r', '\n'))
                                {
                                    FinalFileText += (SelectedOutputLineBreakMode != LineBreakMode.KeepOriginal
                                        ? SelectedOutputLineBreakMode
                                        : OriginalLineBreakMode).ToActualString();
                                }


                                if (File.Exists(OutputFile_DestinationFullPath) && !CanOverwriteExistingFile(OutputFile_DestinationFullPath, FinalFileText))
                                {
                                    continue;
                                }
                                else // save file
                                {
                                    File.WriteAllText(path: OutputFile_DestinationFullPath, contents: FinalFileText, encoding: OuputEncoding);

                                    PrimaryExportedFiles.Add(LocalizationFile.Name);
                                    ExportedFilesCounter++;
                                    
                                    MainControl.ExportedFilesLog.Dispatcher.Invoke(() =>
                                    {
                                        MainControl.ExportedFilesLog.Children.Add(new TextBlock { Text = LoggingRelativeName });
                                    });
                                    MainControl.ExportedFilesLog_ParentScrollViewer.Dispatcher.Invoke(() =>
                                    {
                                        MainControl.ExportedFilesLog_ParentScrollViewer.ScrollToBottom();
                                    });

                                }
                            }
                            catch (Exception Exception)
                            {
                                ShowFileProcessingError(Exception, SourceFile_RelativePath);
                            }
                        }


                        #region Missing files appending
                        if (CurrentProfile.ReferenceLocalization.MissingContentAppending.AppendMissingFiles)
                        {
                            IEnumerable<FileInfo> TargetFiles_Missing = new DirectoryInfo(ReferenceDirectory)
                                .GetFiles("*.json", SearchOption.AllDirectories)
                                .Where(LocalizationFile => !LocalizationFile.FullName.Del(ReferenceDirectory).Contains(@".vs\")) // .vs\ folder ignore
                                .Where(LocalizationFile => LocalizationFile.FullName.Del(SourceDirectory + "\\").Replace("\\", "/").MatchesWithOneOf(WhiteListOfMissingFilesToAppend)) // General whitelist
                                .Where(LocalizationFile => !PrimaryCheckedFiles.Contains(LocalizationFile.Name.Del(CurrentProfile.ReferenceLocalization.Prefix))); // File wasn't in source


                            foreach (FileInfo MissingReferenceFile in TargetFiles_Missing)
                            {
                                string MissingReferenceFile_RelativePath = MissingReferenceFile.FullName.Del(ReferenceDirectory + "\\").Del(CurrentProfile.ReferenceLocalization.Prefix);
                                string OutputMissingReferenceFile_DestinationFullPath = DestinationDirectory + "\\" + MissingReferenceFile_RelativePath;
                                
                                try
                                {
                                    if (!PrimaryExportedFiles.Contains(MissingReferenceFile.Name.RemovePrefix(CurrentProfile.ReferenceLocalization.Prefix)))
                                    {
                                        string ReadedFileText = File.ReadAllText(MissingReferenceFile.FullName);
                                        string OriginalFileText = ReadedFileText;
                                        UTF8Encoding OuputEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: CurrentProfile.JsonFilesFormatting.UTF8ByteOrderMarks switch
                                        {
                                            "Keep BOM if its inside source file" => MissingReferenceFile.IsUTF8BOM(),
                                            "Save files with UTF-8 BOM encoding" => true,
                                            "Save files with regular UTF-8 encoding" => false,
                                            _ => MissingReferenceFile.IsUTF8BOM(),
                                        });

                                        int OriginalJsonIndentationSize = ReadedFileText.GetJsonIndentationSize();
                                        LineBreakMode OriginalLineBreakMode = ReadedFileText.DetermineLineBreakType();


                                        if (CurrentProfile.JsonPathMultipleRegexConversions.DoJsonPathMultipleRegexConversions &&
                                            MissingReferenceFile.Name.RemovePrefix(CurrentProfile.ReferenceLocalization.Prefix)
                                                .MatchesWithWildcards(LoadedJsonPathMultipleRegexConversions.Keys, out List<string> MatchedFileNamePatterns)
                                        ) {
                                            foreach (string MatchedFileNamePattern in MatchedFileNamePatterns)
                                            {
                                                ReadedFileText = DoMultipleRegexConversions(
                                                    JsonText: ReadedFileText,
                                                    ReplacementRules: LoadedJsonPathMultipleRegexConversions[MatchedFileNamePattern],
                                                    LoggingFileName: "[Reference] " + MissingReferenceFile_RelativePath
                                                );
                                            }
                                        }


                                        string FinalFileText = JsonConvert.DeserializeObject(ReadedFileText).SerializeToFormattedText
                                        (
                                            Formatting: SelectedOutputFormatting,

                                            IndentationSize: SelectedOutputIndentationSize != null
                                                ? (int)SelectedOutputIndentationSize
                                                : OriginalJsonIndentationSize,

                                            LineBreakMode: SelectedOutputLineBreakMode != LineBreakMode.KeepOriginal // Strictly selected (Not "Keep original" option)
                                                ? SelectedOutputLineBreakMode
                                                : OriginalLineBreakMode // Keep original
                                        );

                                        if (CurrentProfile.JsonFilesFormatting.EmptyLastLine == "Keep empty last line if its inside source file" && OriginalFileText[^1].EqualsOneOf('\r', '\n'))
                                        {
                                            FinalFileText += (SelectedOutputLineBreakMode != LineBreakMode.KeepOriginal
                                                ? SelectedOutputLineBreakMode
                                                : OriginalLineBreakMode).ToActualString();
                                        }
                                        
                                        
                                        if (File.Exists(OutputMissingReferenceFile_DestinationFullPath) && !CanOverwriteExistingFile(OutputMissingReferenceFile_DestinationFullPath, FinalFileText))
                                        {
                                            continue;
                                        }
                                        else // save file
                                        {
                                            File.WriteAllText(OutputMissingReferenceFile_DestinationFullPath, FinalFileText, OuputEncoding);
                                        

                                            CurrentReport.UntranslatedElementsReport.UntranslatedFilesCount++;
                                            CurrentReport.UntranslatedElementsReport.UntranslatedFiles.Add(MissingReferenceFile_RelativePath);

                                            MainControl.ExportedFilesLog.Dispatcher.Invoke(() =>
                                            {
                                                MainControl.ExportedFilesLog.Children.Add(new TextBlock
                                                {
                                                    Text = $"[Reference] {MissingReferenceFile.Name.Replace(CurrentProfile.ReferenceLocalization.Prefix, $"({CurrentProfile.ReferenceLocalization.Prefix})")}"
                                                });
                                            });
                                            MainControl.ExportedFilesLog_ParentScrollViewer.Dispatcher.Invoke(() =>
                                            {
                                                MainControl.ExportedFilesLog_ParentScrollViewer.ScrollToBottom();
                                            });
                                        }
                                    }
                                }
                                catch (Exception Exception)
                                {
                                    ShowFileProcessingError(Exception, MissingReferenceFile_RelativePath, IsReferenceFile: true);
                                }
                            }
                        }
                        #endregion
                    });
                }

                // Transform "" values in used merged fonts report to "\ue260" after serialization to clearly show characters unicode numbers
                Dictionary<string, Dictionary<string, string>> UsedMergedFontsInfo = CurrentReport.MergedFontsConversionReport.UsedReplacementMapFonts;
                foreach (KeyValuePair<string, Dictionary<string, string>> ReplacementMap_Font in UsedMergedFontsInfo)
                {
                    try
                    {
                        UsedMergedFontsInfo[ReplacementMap_Font.Key] = new Dictionary<string, string>(UsedMergedFontsInfo[ReplacementMap_Font.Key])
                            .Select(CharReplacement => new KeyValuePair<string, string>(CharReplacement.Key, CharReplacement.Value.ToUnicodeSequence()))
                            .ToDictionary();
                    }
                    catch { }
                }


                Dictionary<string, Dictionary<string, string>> AttachedFonts_Markers = CurrentReport.MergedFontsConversionReport.AttachedFonts_Markers;
                CurrentReport.MergedFontsConversionReport.AttachedFonts_Markers = AttachedFonts_Markers.OrderBy(x => x.Key).ToDictionary();

                Dictionary<string, Dictionary<string, string>> AttachedFonts_MultipleApplyConfig = CurrentReport.MergedFontsConversionReport.AttachedFonts_MultipleApplyConfig;
                CurrentReport.MergedFontsConversionReport.AttachedFonts_MultipleApplyConfig = AttachedFonts_MultipleApplyConfig.OrderBy(x => x.Key).ToDictionary();


                CurrentReport.SerializeToFormattedFile("Export Records.json", ReplaceUnicodeDoubleBackslashes: true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), @"Cant process files (Something fatal)", MessageBoxButton.OK, MessageBoxImage.Error);
            }


            MainControl.TaskBarProgressIcon.ProgressState = System.Windows.Shell.TaskbarItemProgressState.None;
            if (!CancelProcessing) MainControl.ViewReportButtons.Visibility = Visibility.Visible;

            if (!CancelProcessing) await Task.Delay(2100);
            MainControl.FilesCounter.Visibility = Visibility.Collapsed;
            MainControl.StartButton.IsEnabled = true;
        }




        public static void ShowFileProcessingError(Exception Exception, string FileRelativePath, bool IsReferenceFile = false)
        {
            string Info = $"Error occurred while processing file \"{FileRelativePath}\"{(IsReferenceFile ? " (Added missing file from Reference directory)" : "")}:";

            if (Exception is JsonReaderException)
            {
                Info += $"\n{Exception.Message}\n\n(This means you have incorrect json syntax in this file, fix it first)";
            }
            else
            {
                Info += $"\n{Exception}";
            }

            MessageBox.Show(Info, @"Cant process or save file", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public static bool CanOverwriteExistingFile(string Filepath, string CurrentText)
        {
            if (File.Exists(Filepath)) // Check for skip if file is already exists and equals to final version
            {
                switch (CurrentProfile.JsonFilesFormatting.ExistingFilesOverwritingRule)
                {
                    case "Only if Json content itself is not the same":
                        try
                        {
                            JToken ExistingFileJToken = JToken.Parse(File.ReadAllText(Filepath /* Already existing file by this path */));

                            if (JToken.DeepEquals(JToken.Parse(CurrentText), ExistingFileJToken))  /**/ return false /**/;
                        }
                        catch (JsonReaderException) { return true; /* Json reading error = wrong syntax = can */ }

                        break;


                    case "Only if just text of both files is not the same":
                        string ExistingFileText = File.ReadAllText(Filepath);

                        if (CurrentText == ExistingFileText)  /**/ return false /**/;

                        break;


                    case "Overwrite existing files anyway": /** :P **/ break;

                            
                    default: goto case "Only if Json content itself is not the same";
                }
            }

            return true;
        }
    }
}