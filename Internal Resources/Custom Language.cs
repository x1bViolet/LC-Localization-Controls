using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;

using Configuration;
using System.Text.RegularExpressions;
using static TexExtension.ExternalBase;

using LC_Localization_Controls;

namespace GeneralResources
{
    internal abstract class CustomLanguage
    {
        internal protected static Dictionary<string, string> Static = new();
        internal protected static Dictionary<string, string> Dynamic = new();

        internal static void LoadCustomLanguage()
        {
            try
            {

                string LanguageFolder = @$"⇲ Asset Directory\Languages\{AppConfiguration.SelectedLanguage}";
                if (Directory.Exists(LanguageFolder))
                {
                    string[] LanguageConfig = File.ReadAllLines(GetExtFiles(LanguageFolder, ".L[-]")[0]);

                    string Sector = "Internal";
                    string[] sss = File.ReadAllLines(@"C:\Users\javas\source\repos\LC Localization Controls\MainWindow.xaml");
                    foreach (var Line in LanguageConfig)
                    {
                        string TrimParameter = Line.Trim();
                        Sector = TrimParameter switch
                        {
                            "Internal -> ()" => "Internal",
                            "Static -> ()"   => "Static",
                            "Dynamic -> ()"  => "Dynamic",
                            _ => Sector
                        };

                        switch (Sector)
                        {
                            case "Internal":

                                Match CustomFontLine = Regex.Match(TrimParameter, @"`Custom font : (.*?) override` @ ""(.*?)""");
                                string FontPath = @$"{LanguageFolder}\Internal\{CustomFontLine.Groups[2].Value}";

                                if (File.Exists(FontPath))
                                {
                                    string ReplacingFontName = CustomFontLine.Groups[1].Value;
                                    switch (ReplacingFontName)
                                    {
                                        case "GOST Type BU":
                                            MainWindow.GostTypeBU = FileToFontFamily(FontPath);
                                            foreach(var UIElem in MainWindow.FontConfiguration[ReplacingFontName])
                                            {
                                                UIElem.FontFamily = MainWindow.GostTypeBU;
                                            }
                                            break;

                                        case "GOST Type AU":
                                            MainWindow.GostTypeAU = FileToFontFamily(FontPath);
                                            foreach (var UIElem in MainWindow.FontConfiguration[ReplacingFontName])
                                            {
                                                UIElem.FontFamily = MainWindow.GostTypeAU;
                                            }
                                            break;

                                        case "Unispace":
                                            MainWindow.Unispace = FileToFontFamily(FontPath);
                                            foreach (var UIElem in MainWindow.FontConfiguration[ReplacingFontName])
                                            {
                                                UIElem.FontFamily = MainWindow.Unispace;
                                            }
                                            break;

                                        case "Unispace [RUS by Daymarius]":
                                            MainWindow.UnispaceRUS = FileToFontFamily(FontPath);
                                            foreach (var UIElem in MainWindow.FontConfiguration[ReplacingFontName])
                                            {
                                                UIElem.FontFamily = MainWindow.UnispaceRUS;
                                            }
                                            break;

                                        case "Pretendard Light":
                                            MainWindow.PretendardLight = FileToFontFamily(FontPath);
                                            foreach (var UIElem in MainWindow.FontConfiguration[ReplacingFontName])
                                            {
                                                UIElem.FontFamily = MainWindow.PretendardLight;
                                            }
                                            break;

                                        case "Corbel Light":
                                            MainWindow.CorbelLight = FileToFontFamily(FontPath);
                                            foreach (var UIElem in MainWindow.FontConfiguration[ReplacingFontName])
                                            {
                                                UIElem.FontFamily = MainWindow.CorbelLight;
                                            }
                                            break;

                                        case "Microsoft JhengHei Light":
                                            MainWindow.MicrosoftJhengHeiLight = FileToFontFamily(FontPath);
                                            foreach (var UIElem in MainWindow.FontConfiguration[ReplacingFontName])
                                            {
                                                UIElem.FontFamily = MainWindow.MicrosoftJhengHeiLight;
                                            }
                                            break;

                                        default:
                                            break;
                                    };
                                }

                                break;


                            case "Static":
                                if (TrimParameter.Contains("` @ \""))
                                {
                                    string ParameterName = Regex.Match(TrimParameter, @"`(.*?)` @ """).Groups[1].Value;
                                    string ParameterValue = TrimParameter.Split("` @ \"")[1][..^1];

                                    //var ea = sss.ItemsThatContain(ParameterValue)[0].Trim().Replace("\\n", "\n");
                                    //var xname = Regex.Match(ea, @"x:Name=""(\w+)""").Groups[1].Value;
                                    //if (ea.Equals("")) rin($"  >>>>> NULL VALUE FOR {ParameterValue}");
                                    //else
                                    //{
                                    //    ex += $"[\"{ParameterName}\"] = {xname},\n";
                                    //    rin($"{xname}: {ParameterName}");
                                    //}

                                    Static[ParameterName] = ParameterValue.Replace("\\n", "\n");
                                }
                                break;


                            case "Dynamic":
                                if (TrimParameter.Contains("` @ \""))
                                {
                                    string ParameterName = Regex.Match(TrimParameter, @"`(.*?)` @ """).Groups[1].Value;
                                    string ParameterValue = TrimParameter.Split("` @ \"")[1][..^1];
                                    //rin($"Dynamic {ParameterName}: \"{ParameterValue}\"");
                                    Dynamic[ParameterName] = ParameterValue.Replace("\\n", "\n");
                                }
                                break;
                        }
                    }
                    //Clipboard.SetText(ex);
                    //Console.ReadLine();
                }
            }
            catch { }
        }
    }
}
