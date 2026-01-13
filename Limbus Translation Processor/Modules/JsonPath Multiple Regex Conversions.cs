using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text.RegularExpressions;
using static Translation_Devouring_Siltcurrent.LimbusTranslationProcessor;
using static Translation_Devouring_Siltcurrent.Requirements;

namespace Translation_Devouring_Siltcurrent
{
    namespace LocalizationProcessingModules
    {
        public static class JsonPathRegexConversions
        {
            public record RegexConversion
            {
                public string Pattern { get; set; }

                public string Replace { get; set; }
            }

            /// <summary>
            /// Key is files pattern regex, Value is Dictionary{ Key is JsonPath, Value is list of regex conversions }
            /// </summary>
            public static Dictionary<string, Dictionary<string, List<RegexConversion>>> LoadedJsonPathMultipleRegexConversions = [];

            public static void LoadJsonPathMultipleRegexConversions(string FilePath)
            {
                LoadedJsonPathMultipleRegexConversions.Clear();

                string[] Lines = File.ReadAllLines(FilePath);
                int TotalIndex = Lines.Length - 1;
                int LineIndex = 0;
                string[]? LatestFilePatterns = null;
                string? LatestJsonPath = null;

                foreach (string Line in Lines)
                {
                    try
                    {
                        if (Line == "{Regex Option}")
                        {
                            if (LineIndex + 1 <= TotalIndex & LineIndex + 2 <= TotalIndex)
                            {
                                string Files = Lines[LineIndex + 1][9..];
                                string JsonPath = Lines[LineIndex + 2][12..];

                                string[] FilePatterns = Files.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(FilePattern => FilePattern.Trim()).Where(FilePattern => FilePattern != "").ToArray();
                                LatestFilePatterns = FilePatterns;
                                LatestJsonPath = JsonPath;
                                foreach (string Pattern in FilePatterns)
                                {
                                    if (!LoadedJsonPathMultipleRegexConversions.ContainsKey(Pattern)) LoadedJsonPathMultipleRegexConversions[Pattern] = new Dictionary<string, List<RegexConversion>>();
                                    LoadedJsonPathMultipleRegexConversions[Pattern][JsonPath] = new List<RegexConversion>();
                                }
                            }
                        }
                        else if (LatestFilePatterns != null & LatestJsonPath != null & Line.StartsWith("* Pattern: ") & (LineIndex < TotalIndex && Lines[LineIndex + 1].StartsWith("  Replace: ")))
                        {
                            foreach (string Pattern in LatestFilePatterns)
                            {
                                LoadedJsonPathMultipleRegexConversions[Pattern][LatestJsonPath].Add(new RegexConversion()
                                {
                                    Pattern = Line[11..],
                                    Replace = Lines[LineIndex + 1][11..]
                                });
                            }
                        }
                    }
                    catch (Exception ex) { rin(ex.ToString()); }

                    LineIndex++;
                }

                //foreach (var i in LoadedJsonPathMultipleRegexConversions)
                //{
                //    rin($"Files Pattern: {i.Key}");
                //    foreach (var j in i.Value)
                //    {
                //        rin($"- JsonPath: {j.Key}");
                //        foreach (var k in j.Value)
                //        {
                //            rin($"  * Pattern: {k.Pattern}");
                //            rin($"    Replace: {k.Replace}");
                //            rin($"");
                //        }
                //    }
                //}
            }
            public static string DoMultipleRegexConversions(string JsonText, Dictionary<string, List<RegexConversion>> ReplacementRules, string LoggingFileName = "")
            {
                JToken JParser = JToken.Parse(JsonText);
                bool SomethingWasReplaced = false;
                
                foreach (KeyValuePair<string, List<RegexConversion>> JsonPathAndConversions in ReplacementRules)
                {
                    foreach (JToken StringItem in JParser.SelectTokens(JsonPathAndConversions.Key))
                    {
                        string StringItem_OriginalValue = $"{StringItem}";
                        string StringItem_WithReplacements = StringItem_OriginalValue;

                        foreach (RegexConversion Conversion in JsonPathAndConversions.Value)
                        {
                            StringItem_WithReplacements = Regex.Replace(StringItem_WithReplacements, Conversion.Pattern, Conversion.Replace);
                        }

                        if (StringItem_OriginalValue != StringItem_WithReplacements)
                        {
                            StringItem.Replace(StringItem_WithReplacements);
                            SomethingWasReplaced = true;

                            if (LoggingFileName != "")
                            {
                                string SignName = $"{LoggingFileName} ({JsonPathAndConversions.Key})";

                                if (!CurrentReport.JsonPathMultipleRegexConversions.ContainsKey(SignName))
                                {
                                    CurrentReport.JsonPathMultipleRegexConversions[SignName] = new Dictionary<string, string>();
                                }
                                CurrentReport.JsonPathMultipleRegexConversions[SignName][StringItem_OriginalValue] = StringItem_WithReplacements;
                            }
                        }
                    }
                }

                return SomethingWasReplaced ? JParser.ToString(Formatting.Indented) : JsonText;
            }
        }
    }
}
