using Newtonsoft.Json;
using System;
using System.Windows;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using static TexelExtension.ExternalBase;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using Configuration;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Windows.Media;
using RichText;
using System.Reflection;
using NLog.Time;
using GeneralResources;

namespace LC_Localization_Controls.Converters
{
    internal abstract class KSTFont
    {
        internal protected static Dictionary<string, dynamic> T = new();
        internal protected static void InitT() => T = MainWindow.T;

        internal protected static Dictionary<string, string> SafeEscape = new();

        internal protected static Dictionary<string, Dictionary<string, string>> InternalFontsList = new();

        internal protected static string Specified_FontName = "";
        internal protected static FontFamily Specified_FontObject = null;



        internal protected static void UpdateSelectedFontAction(string SelectedFontName)
        {
            try
            {

                string SelectedFilesConfig = GetExtFiles(@$"⇲ Asset Directory\Font\{SelectedFontName}", ".toml")[0];
                string SelectedReplacementMap = GetExtFiles(@$"⇲ Asset Directory\Font\{SelectedFontName}", ".json")[0];
                string SelectedCustomFontRelPath = GetExtFiles(@$"⇲ Asset Directory\Font\{SelectedFontName}", ".ttf", ".otf")[0];
                var FullFileInfo = new FileInfo(SelectedCustomFontRelPath);

                Specified_FontObject = FileToFontFamily(SelectedCustomFontRelPath);

                LoadSafeEscape();
                UpdateReplacementMap(SelectedReplacementMap);
                GenerateCustomFontsPreview(T["Test font string"].Text);

                AppConfiguration.SaveConfiguration("Selected font", SelectedFontName);


                T["Fonts content list"].Visibility = Visibility.Collapsed;
                T["Selected font display"].Text = SelectedFontName;
                LocalizationFullExport.LoadConfigValues(SelectedFilesConfig);
            }
            catch { }
        }

        internal protected static void UpdateSelectedFont(object sender, MouseButtonEventArgs e)
        {
            Specified_FontName = (sender as TextBlock).Text;
            UpdateSelectedFontAction(Specified_FontName);
        }
        internal protected static void HighlightSelectedFontSet(object sender, MouseEventArgs e)
        {
            (sender as TextBlock).TextDecorations = TextDecorations.Underline;
        }
        internal protected static void HighlightSelectedFontRem(object sender, MouseEventArgs e)
        {
            (sender as TextBlock).TextDecorations = null;
        }

        internal static void UpdateCustomFontsDropdownList(string FileSource)
        {
            T["Fonts dropdown list"].Children.Clear();
            foreach (var FontDir in Directory.GetDirectories(@"⇲ Asset Directory\Font"))
            {
                string FontDirName = FontDir.Split("\\")[^1];

                TextBlock FontSelector = new TextBlock()
                {
                    FontSize = 26,
                    Text = FontDirName,
                    FontFamily = MainWindow.GostTypeBU
                };
                StackPanel GeneralAdd = new StackPanel()
                {
                    Orientation = Orientation.Horizontal,
                    Children =
                    {
                        new TextBlock()
                        {
                            FontSize = 26,
                            Text = "※ ",
                            FontFamily = MainWindow.GostTypeBU
                        },
                        FontSelector,
                        
                    },
                };
                FontSelector.PreviewMouseLeftButtonUp += UpdateSelectedFont;
                FontSelector.MouseEnter += HighlightSelectedFontSet;
                FontSelector.MouseLeave += HighlightSelectedFontRem;

                T["Fonts dropdown list"].Children.Add(GeneralAdd);
            }

            LoadSafeEscape();
            UpdateReplacementMap(FileSource);
            GenerateCustomFontsPreview(T["Test font string"].Text);
        }
        internal static void GenerateCustomFontsPreview(string Text = "Test", bool UseUnicode = false)
        {
            T["Custom fonts display list"].Children.Clear();

            bool FirstAdd = true;
            foreach(var CustomFontItem in InternalFontsList)
            {
                CreateCustomFontPreviewPanel(CustomFontItem.Key, CustomFontItem.Value, Text, FirstAdd, UseUnicode);
                FirstAdd = false;
            }
        }

        internal static void UpdateReplacementMap(string FileSource)
        {
            try
            {
                InternalFontsList.Clear();
                JObject JParser = JObject.Parse(File.ReadAllText(FileSource));
                var DefinedReplacements = JParser.SelectTokens("$.*");
                foreach (var jProperty in JParser.Properties())
                {
                    InternalFontsList[jProperty.Name] = jProperty.Value.ToObject<Dictionary<string, string>>();
                }
            }
            catch
            {
                T["Selected font display"].Text = "";
            }
        }

        internal static void LoadSafeEscape()
        {
            JObject JParser = JObject.Parse(File.ReadAllText(@"⇲ Asset Directory\Font\Safe escape.json"));
            var DefinedReplacements = JParser.SelectTokens("$.*");
            foreach (var jProperty in JParser.Values())
            {
                SafeEscape = jProperty.ToObject<Dictionary<string, string>>();
            }
        }

        private static void CreateCustomFontPreviewPanel(string CustomFontPartName, Dictionary<string, string> CustomFontParameter, string TextString, bool FirstAdd = true, bool UseUnicode = false)
        {
            string KFontConverted = KSTFont.Convert(TextString, CustomFontParameter);

            Border CustomFontPreview = new Border()
            {
                Margin = new Thickness(8, 0, 0, 0),
                CornerRadius = new CornerRadius(5),
                Background = ToColor("#2C2B31"),
                Child = new TextBlock
                {
                    Margin = new Thickness(0, 3, 0, 4),
                    Width = 1149,
                    TextWrapping = TextWrapping.Wrap,
                    FontSize = 20,
                    FontFamily = Specified_FontObject,
                    Text = KFontConverted,
                }
            };
            CustomFontPreview.MouseLeftButtonUp += CopyConvertedFontText;
            CustomFontPreview.ToolTip = new ToolTip()
            {
                Background = Brushes.Transparent,
                Foreground = Brushes.White,
                HasDropShadow = true,
                Placement = System.Windows.Controls.Primitives.PlacementMode.Relative,
                BorderThickness = new Thickness(0),
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalOffset = 19,

                Content = new Border
                {
                    CornerRadius = new CornerRadius(3),
                    BorderThickness = new Thickness(0.8),
                    BorderBrush = ToColor("#646464"),
                    Background = ToColor("#895B5B5B"),
                    Child = new Grid()
                    {
                        Children =
                        {
                            new TextBlock
                            {
                                Margin = new Thickness(3),
                                TextAlignment = TextAlignment.Center,
                                FontFamily = MainWindow.GostTypeBU,
                                Text = "Copy to clipboard",
                                FontSize = 14
                            }
                        }
                    }
                }
            };
            CustomFontPreview.SetValue(ToolTipService.InitialShowDelayProperty, 1000);

            T["Custom fonts display list"].Children.Add(new Border()
            {
                BorderBrush = ToColor("#4C4959"),
                BorderThickness = new Thickness(2),
                Margin = new Thickness(0, FirstAdd ? 0 : 17, 0, 5),
                CornerRadius = new CornerRadius(10, 1, 1, 1),
                Child = new StackPanel()
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(-2, 0, 0, 0),
                    Children =
                {
                    new StackPanel()
                    {
                        Children =
                        {
                            new StackPanel()
                            {
                                Orientation = Orientation.Horizontal,
                                Children =
                                {
                                    new Border()
                                    {
                                        HorizontalAlignment = HorizontalAlignment.Left,

                                        Background = ToColor("#4C4959"),
                                        CornerRadius = new CornerRadius(10, 0, 5, 0),
                                        Child = new TextBlock()
                                        {
                                            Foreground = ToColor("#C4C4C4"),
                                            FontWeight = FontWeights.Bold,
                                            FontSize = 22,
                                            Margin = new Thickness(8, 0, 5, 0),
                                            FontFamily = Specified_FontObject,
                                            Text = KSTFont.Convert(CustomFontPartName, CustomFontParameter)
                                        }
                                    }
                                }
                            },
                            new StackPanel()
                            {
                                Orientation = Orientation.Horizontal,
                                Margin = new Thickness(5, 0, 0, !UseUnicode ? 4 : 0),
                                Children =
                                {
                                    new TextBlock()
                                    {
                                        FontFamily = MainWindow.GostTypeBU,
                                        FontSize = 24,
                                        Foreground = ToColor("#BDBDBD"),
                                        Width = 137,
                                        Text = "Custom Font:"
                                    },
                                    new Border()
                                    {
                                        Margin = new Thickness(8, 0, 0, 0),
                                        CornerRadius = new CornerRadius(5),
                                        Background = ToColor("#2C2B31"),
                                        Child = CustomFontPreview
                                    }
                                }
                            },

                            UseUnicode ?
                            new StackPanel()
                            {
                                Orientation = Orientation.Horizontal,
                                Margin = new Thickness(5, 5, 0, 5),
                                Children =
                                {
                                    new TextBlock()
                                    {
                                        FontFamily = MainWindow.GostTypeBU,
                                        FontSize = 24,
                                        Foreground = ToColor("#BDBDBD"),
                                        Width = 137,
                                        Text = "Unicode:"
                                    },
                                    new Border()
                                    {
                                        Margin = new Thickness(8, 0, 0, 0),
                                        CornerRadius = new CornerRadius(5),
                                        Background = ToColor("#2C2B31"),
                                        Child = new TextBlock
                                        {
                                            Margin = new Thickness(4, 3, 0, 4),
                                            Width = 1149,
                                            TextWrapping = TextWrapping.Wrap,
                                            FontSize = 20,
                                            FontFamily = Specified_FontObject,
                                            Text = ToUnicodeSequence(KFontConverted, TextString),
                                        }
                                    }
                                }
                            } : new StackPanel() { Visibility = Visibility.Collapsed }
                        }
                    }
                }
                }
            });
            //(((((MatchesList.Children[int.Parse(MatchNumber) - 1] as Border).Child
            //    as StackPanel).Children[0]
            //        as StackPanel).Children[1]
            //            as StackPanel).Children[1] as Border).MouseLeftButtonDown += (s, e) => StackPanel_MouseLeftButtonDown(s, e);
        }

        private static void CopyConvertedFontText(object sender, MouseButtonEventArgs e)
        {
            Border Sender = sender as Border;
            Clipboard.SetText((Sender.Child as TextBlock).Text);
        }

        internal protected static string SafeEscapeConvert(string TargetString)
        {
            TargetString = Regex.Replace(TargetString, @"(<.*?>)|({\d+}){1}|(\\n)", Match =>
            {
                string value = Match.Groups[0].Value;
                return Convert(value, SafeEscape, PreventSelf: true);
            });

            return TargetString;
        }
        internal protected static string SafeEscapeDeconvert(string TargetString)
        {
            TargetString = ReverseConvert(TargetString, SafeEscape);

            return TargetString;
        }
        internal protected static string StealAttachedFontrule(ref string TargetString)
        {
            string ExtractedFontName = "";
            TargetString = Regex.Replace(TargetString, @"\[font=(\w+)\]", Match =>
            {
                if (InternalFontsList.ContainsKey(Match.Groups[1].Value))
                {
                    ExtractedFontName = Match.Groups[1].Value;
                    return "";
                }
                else
                {
                    return Match.Groups[0].Value;
                }
            });
            return ExtractedFontName;
        }

        internal protected static string Convert(string TargetString, Dictionary<string, string> ReplaceSource, bool PreventSelf = false)
        {
            string AttachedFontRule = StealAttachedFontrule(ref TargetString);

            if (!AttachedFontRule.Equals("") & InternalFontsList.ContainsKey(AttachedFontRule))
            {
                ReplaceSource = InternalFontsList[AttachedFontRule];
            }

            if (!PreventSelf) TargetString = SafeEscapeConvert(TargetString);

            foreach (KeyValuePair<string, string> CharMaps in ReplaceSource)
            {
                TargetString = TargetString.Replace(CharMaps.Key, CharMaps.Value);
            }

            if (!PreventSelf) TargetString = SafeEscapeDeconvert(TargetString);

            return TargetString;
        }
        internal protected static string ReverseConvert(string TargetString, Dictionary<string, string> ReplaceSource)
        {
            ReplaceSource = ReplaceSource.ToDictionary(x => x.Value, x => x.Key);

            foreach (KeyValuePair<string, string> CharMap in ReplaceSource)
            {
                TargetString = TargetString.Replace(CharMap.Key, CharMap.Value);
            }

            return TargetString;
        }

        internal protected static void CopyConvert(string OriginalText, Dictionary<string, string> ReplaceSource)
        {
            Clipboard.SetText(Convert(OriginalText, ReplaceSource));
        }
    }
}
