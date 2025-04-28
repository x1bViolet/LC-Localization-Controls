using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using static TexelExtension.ExternalBase;

namespace RichText
{
    public class InlineTextConstructor
    {
        public string Text;
        public List<string> InnerTags;
    }

    public class InlineImageConstructor
    {
        public string ImageID;
        public InlineTextConstructor TextBase;
    };

    public static class TagManager
    {
        /// <summary>
        /// Path where all pack fonts placed
        /// counts as 'new FontFamily(new Uri("pack://application:,,,/"), $"./{FontResourcesDirectory}/#Some Font")'
        /// </summary>
        public static string FontResourcesDirectory = "Resources";

        public static List<string> InnerTags(string Source)
        {
            List<string> OutputTags = new();
            try
            {
                Regex InnerTags = new Regex(@"⟦InnerTag/(.*?)⟧");
                foreach (Match InnerTagMatch in InnerTags.Matches(Source))
                {
                    OutputTags.Add(InnerTagMatch.Groups[1].Value.Replace("InnerTag/", ""));
                }
            }
            catch { }
            return OutputTags;
        }

        public static string ClearText(string Source)
        {
            try
            {
                if (!Source.StartsWith("⟦LevelTag/"))
                {
                    Source = Regex.Replace(Source, @"⟦InnerTag/(.*)⟧", Match => { return ""; });
                }
                else
                {
                    Source = Regex.Replace(Source, @"⟦LevelTag/SpriteLink@(\w+):«(.*)»⟧", Match => { return ClearText(Match.Groups[2].Value); });
                }
            }
            catch { }

            return Source;
        }



        public static void ApplyTags(ref Run TargetRun, List<string> Tags)
        {
            try
            {
                foreach (var Tag in Tags)
                {
                    string[] TagBody = Tag.Split('@');
                    switch (TagBody[0])
                    {
                        case "TextColor":
                            TargetRun.Foreground = ToColor($"#{TagBody[1]}");
                            break;

                        case "FontFamily":
                            try   { TargetRun.FontFamily = new FontFamily(TagBody[1]); }
                            catch { }
                            break;

                        case "PackFontFamily":
                            try { TargetRun.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), $"./{FontResourcesDirectory}/#{TagBody[1]}"); }
                            catch { }
                            break;

                        case "FontSize":
                            try
                            {
                                int TargetFontSize = Convert.ToInt32(TagBody[1][..^1]);
                                if (TargetFontSize == 0) TargetFontSize = 1;

                                TargetRun.FontSize *= 0.01 * TargetFontSize;
                            }
                            catch (Exception EX) { Console.WriteLine(EX.ToString()); }
                            break;

                        case "UptieHighlight":
                            TargetRun.Foreground = ToColor($"#fff8c200");
                            break;

                        case "TextStyle":
                            switch (TagBody[1])
                            {
                                case "Underline":
                                    TargetRun.TextDecorations = TextDecorations.Underline;
                                    break;

                                case "Strikethrough":
                                    TargetRun.TextDecorations = TextDecorations.Strikethrough;
                                    break;

                                case "Italic":
                                    TargetRun.FontStyle = FontStyles.Italic;
                                    break;

                                case "Bold":
                                    TargetRun.FontWeight = FontWeights.SemiBold;
                                    break;

                            }

                            break;
                    }
                }
            }
            catch { }
        }
    }
}
