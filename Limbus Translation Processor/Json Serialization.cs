using Newtonsoft.Json;
using System.IO;
using System.Text.RegularExpressions;

namespace Translation_Devouring_Siltcurrent
{
    public static class JsonSerialization
    {
        public enum LineBreakMode { LF, CR, CRLF, KeepOriginal /* Needed locally by configuration profiles */ }
        public static string ToActualString(this LineBreakMode LineBreakMode)
        {
            return LineBreakMode switch { LineBreakMode.LF => "\n", LineBreakMode.CR => "\r", LineBreakMode.CRLF => "\r\n" };
        }
        public static string SerializeToFormattedText(this object Target, int IndentationSize = 2, LineBreakMode LineBreakMode = LineBreakMode.LF, Formatting Formatting = Formatting.Indented)
        {
            using (StringWriter StringWriter = new StringWriter())
            {
                using (JsonTextWriter JsonWriter = new JsonTextWriter(StringWriter))
                {
                    JsonWriter.Formatting = Formatting;
                    JsonWriter.Indentation = IndentationSize;

                    new JsonSerializer().Serialize(JsonWriter, Target);
                }

                string Serialized = StringWriter.ToString();
                if (LineBreakMode == LineBreakMode.LF) Serialized = Serialized.Replace("\r\n", "\n");
                if (LineBreakMode == LineBreakMode.CR) Serialized = Serialized.Replace("\r\n", "\r");

                return Serialized;
            }
        }

        public static void SerializeToFormattedFile(this object Source, string Filename, bool ReplaceUnicodeDoubleBackslashes = false)
        {
            string Serialized = Source.SerializeToFormattedText();
            if (ReplaceUnicodeDoubleBackslashes) Serialized = Serialized.Replace(@"\\u", @"\u");
            File.WriteAllText(Filename, Serialized);
        }

        public static LineBreakMode DetermineLineBreakType(this string Text, LineBreakMode Fallback = LineBreakMode.CRLF)
        {
            if (Text.Contains("\r\n"))
            {
                return LineBreakMode.CRLF;
            }
            else if (Text.Contains('\n'))
            {
                return LineBreakMode.LF;
            }
            else if (Text.Contains('\r'))
            {
                return LineBreakMode.CR;
            }
            else
            {
                return Fallback;
            }
        }

        public static int GetJsonIndentationSize(this string JsonText, int FailedMatchFallback = 2)
        {
            Match IndentationMatch = Regex.Match(JsonText.Trim(), @"^{(\r)?\n(?<Indentation> +)""");
            return IndentationMatch.Success ? IndentationMatch.Groups["Indentation"].Length : FailedMatchFallback;
        }
    }
}