using System.IO;
using System.Text.RegularExpressions;
using static Translation_Devouring_Siltcurrent.Configurazione;
using static Translation_Devouring_Siltcurrent.LimbusTranslationProcessor;

namespace Translation_Devouring_Siltcurrent
{
    namespace LocalizationProcessingModules
    {
        public static class Shorthands
        {
            public static Dictionary<string, string> KeywordColors = [];

            public static void ReadKeywordColors()
            {
                KeywordColors.Clear();
                foreach (string Line in File.ReadAllLines(CurrentProfile.Misc.KeywordShorthandsColorsInfoFile))
                {
                    string[] ColorPair = Line.Split(" ¤ ");
                    if (ColorPair.Length == 2)
                    {
                        string KeywordID = ColorPair[0].Trim();
                        string KeywordColor = ColorPair[1].Trim();
                        KeywordColors[KeywordID] = KeywordColor;
                    }
                }
            }

            public static string ConvertShorthands(string Text, string ShorthandsPattern)
            {
                Text = Regex.Replace(Text, ShorthandsPattern, Match =>
                {
                    if (!Match.Groups["ID"].Value.Equals(""))
                    {
                        string ID = Match.Groups["ID"].Value;
                        string Name = Match.Groups["Name"].Value;
                        string Color = Match.Groups["Color"].Value;
                        string SpriteID = Match.Groups["SpriteID"].Value;

                        string ExportTMPro =
                        @$"<sprite name=\""{(SpriteID != "" ? SpriteID : ID)}\"">" +
                        @$"<color={(!Color.Equals("") ? Color : (KeywordColors.ContainsKey(ID) ? KeywordColors[ID] : "#9f6a3a"))}>" +
                        @$"<u>" +
                        @$"<link=\""{ID}\"">" +
                        Name +
                        @$"</link>" +
                        @$"</u>" +
                        @$"</color>";

                        #region Logging
                        CurrentReport.ShorthandsConversionInfo.ConvertedCount++;
                        if (!KeywordColors.ContainsKey(ID) & !CurrentReport.ShorthandsConversionInfo.ShorthandsConvertedWithoutColor.Contains(ID))
                        {
                            CurrentReport.ShorthandsConversionInfo.ShorthandsConvertedWithoutColor.Add(ID);
                        }
                        #endregion

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
    }
}
