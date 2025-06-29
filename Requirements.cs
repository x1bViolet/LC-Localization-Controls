using Newtonsoft.Json;
using System.Reflection;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Diagnostics;
using Siltcurrent;
using static Siltcurrent.LimbusTranslationExport.ActionsProvider;
using static Siltcurrent.LimbusTranslationExport.ActionsProvider.MergedFont.FontsTomlConfig;

namespace Translation_Devouring_Siltcurrent
{
    internal abstract class NullableControl
    {
        internal abstract class Settings
        {
            internal protected static bool WriteInfomationExternal = false;
            internal protected static bool PlaceStringNullMarker = true;
            internal protected static string StringNullMarker = "<Null>";
        }

        // All null strings to "<Null>" strings
        internal protected static void NullExterminate(object Target, bool WriteInfo = false)
        {
            string NullValueInformation = "\x1b[0m\x1b[38;5;197mNull\x1b[0m";
            string PropertyItemInformation = "\x1b[38;5;245m[$]\x1b[0m";

            PropertyInfo[] TargetPropertiesInfo = Target.GetType().GetProperties();
            if (NullableControl.Settings.WriteInfomationExternal | WriteInfo) Console.WriteLine($"Properties of '\x1b[38;5;248m{Target.GetType().Name}\x1b[0m'");
            foreach (PropertyInfo ClassProperty in TargetPropertiesInfo)
            {
                string PropertyName = ClassProperty.Name;
                object PropertyValue = ClassProperty.GetValue(Target);

                if (NullableControl.Settings.WriteInfomationExternal | WriteInfo) Console.Write($"  [\x1b[38;5;63mProperty Name\x1b[0m] '\x1b[38;5;248m{PropertyName}\x1b[0m' ::\x1b[0m [\x1b[38;5;63mValue\x1b[0m]\x1b[38;5;62m<\x1b[38;5;203m{ClassProperty.PropertyType.ToString().Replace(".", "\x1b[38;5;62m.\x1b[38;5;203m")}\x1b[38;5;62m>\x1b[0m {(PropertyValue.IsNull() ? NullValueInformation : PropertyItemInformation.Extern(PropertyValue))}");

                if (PropertyValue.IsNull())
                {
                    switch (ClassProperty.PropertyType.ToString())
                    {
                        case "System.String": ClassProperty.SetValue(Target, NullableControl.Settings.StringNullMarker);
                            if (NullableControl.Settings.WriteInfomationExternal | WriteInfo) Console.Write($"   \x1b[38;5;62m{{\x1b[38;5;203mReset\x1b[38;5;62m}}\x1b[0m -> \"\x1b[38;5;245m{NullableControl.Settings.StringNullMarker}\x1b[0m\"");
                            break;

                        default: break;
                    }
                }
                if (NullableControl.Settings.WriteInfomationExternal | WriteInfo) Console.WriteLine();
            }
            if (NullableControl.Settings.WriteInfomationExternal | WriteInfo) Console.WriteLine();
        }
    }

    internal static class Requirements
    {
        internal static int LinesCount(this string check) => check.Count(f => f == '\n');
        internal static void MarkSerialize(this object Target, string Filename, string ManualCorruptParameter = "")
        {
            string Output = JsonConvert.SerializeObject(
                value: Target,
                formatting: Formatting.Indented,
                settings: new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }
            ).Replace("\r", "").Replace(@"\\u", @"\u");

            if (!ManualCorruptParameter.Equals(""))
            {
                Output = Output.Replace($"\"{ManualCorruptParameter}\":", $"!!!\"{ManualCorruptParameter}\":");
            }

            File.WriteAllText(Filename, Output, new UTF8Encoding());
        }
        internal static Encoding GetFileEncoding(this FileInfo TargetFile)
        {
            using (var reader = new StreamReader(TargetFile.FullName, Encoding.Default, true))
            {
                reader.Peek(); // you need this!
                return reader.CurrentEncoding;
            }
        }

        internal static Dictionary<string, string> RemoveItemWithValue(this Dictionary<string, string> TargetDictionary, string RemoveValue)
        {
            foreach (KeyValuePair<string, string> StringItem in TargetDictionary.Where(KeyValuePair => KeyValuePair.Value == RemoveValue).ToList())
            {
                TargetDictionary.Remove(StringItem.Key);
            }

            return TargetDictionary;
        }

        internal static string RemovePrefix(this string Target, params string[] Prefixes)
        {
            if (Target.StartsWithOneOf(Prefixes))
            {
                foreach (string SinglePrefix in Prefixes)
                {
                    if (Target.StartsWith(SinglePrefix))
                    {
                        return Target[SinglePrefix.Length..];
                    }
                }
            }

            return Target;
        }

        internal static DictionaryWithDefault<dynamic, string> ExtractGroupValues(Match Target)
        {
            DictionaryWithDefault<dynamic, string> Export = new DictionaryWithDefault<dynamic, string>("");

            foreach (Group i in Target.Groups)
            {
                try   { Export[int.Parse(i.Name)] = i.Value; }
                catch { Export[i.Name] = i.Value; }
            }

            return Export;
        }
        public class DictionaryWithDefault<TKey, TValue> : Dictionary<TKey, TValue>
        {
            TValue _default;
            public TValue DefaultValue
            {
                get { return _default; }
                set { _default = value; }
            }
            public DictionaryWithDefault() : base() { }
            public DictionaryWithDefault(TValue defaultValue) : base()
            {
                _default = defaultValue;
            }
            public new TValue this[TKey key]
            {
                get
                {
                    TValue t;
                    return base.TryGetValue(key, out t) ? t : _default;
                }
                set { base[key] = value; }
            }
        }










        internal static void RegexTransform(ref string Target, string Pattern, MatchEvaluator Evaluator)
        {
            Target = Regex.Replace(Target, Pattern, Evaluator);
        }

        internal static string RegexRemove(string TargetString, Regex PartPattern)
        {
            TargetString = PartPattern.Replace(TargetString, Match =>
            {
                return "";
            });
            return TargetString;
        }

        internal static void RegexRemove(this string TargetString, Regex PartPattern, bool s = false)
        {
            TargetString = PartPattern.Replace(TargetString, Match =>
            {
                return "";
            });
        }
        internal static string RemoveMany(this string TargetString, params string[] RemoveItems)
        {
            foreach(string RemoveItem in RemoveItems)
            {
                TargetString = TargetString.Replace(RemoveItem, "");
            }
            return TargetString;
        }

        internal static bool EqualsOneOf(this string CheckString, IEnumerable<string> CheckSource)
        {
            foreach (var Check in CheckSource)
            {
                if (CheckString.Equals(Check)) return true;
            }

            return false;
        }
        internal static bool StartsWithOneOf(this string CheckString, IEnumerable<string> CheckSource)
        {
            foreach (var Check in CheckSource)
            {
                if (CheckString.StartsWith(Check)) return true;
            }

            return false;
        }
        internal static bool EndsWithOneOf(this string CheckString, IEnumerable<string> CheckSource)
        {
            foreach (var Check in CheckSource)
            {
                if (CheckString.EndsWith(Check)) return true;
            }

            return false;
        }
        internal static bool ContainsOneOf(this string CheckString, IEnumerable<string> CheckSource)
        {
            foreach (var Check in CheckSource)
            {
                if (CheckString.Contains(Check)) return true;
            }

            return false;
        }
        internal static List<string> ItemsThatContain(this IEnumerable<string> CheckSource, string CheckString)
        {
            List<string> Export = new() { };
            foreach (var Check in CheckSource)
            {
                if (Check.Contains(CheckString))
                {
                    Export.Add(Check);
                }
            }

            return Export;
        }
        internal static List<string> ItemsThatStartsWith(this IEnumerable<string> CheckSource, string CheckString)
        {
            List<string> Export = new() { "" };
            foreach (var Check in CheckSource)
            {
                if (Check.StartsWith(CheckString))
                {
                    if (Export[0].Equals("")) Export.RemoveAt(0);
                    Export.Add(Check);
                }
            }

            return Export;
        }
        internal static List<string> ItemsThatEndsWith(this IEnumerable<string> CheckSource, string CheckString)
        {
            List<string> Export = new() { "" };
            foreach (var Check in CheckSource)
            {
                if (Check.EndsWith(CheckString))
                {
                    if (Export[0].Equals("")) Export.RemoveAt(0);
                    Export.Add(Check);
                }
            }

            return Export;
        }
        internal static bool ContainsFileInfoWithName(this IEnumerable<FileInfo> CheckSource, string CheckString)
        {
            foreach (var file in CheckSource)
            {
                if (file.Name.Equals(CheckString)) return true;
            }

            return false;
        }
        internal static FileInfo? SelectWithName(this IEnumerable<FileInfo> Source, string Name)
        {
            Source = Source.Where(file => file.Name.Equals(Name));
            if (Source.Count() > 0)
            {
                return Source.ToList()[0];
            }
            else
            {
                return null;
            }
        }
        internal static string GetText(this FileInfo file)
        {
            return File.ReadAllText(file.FullName);
        }
        internal static string[] GetLines(this FileInfo file)
        {
            return File.ReadAllLines(file.FullName);
        }
        internal static byte[] GetBytes(this FileInfo file)
        {
            return File.ReadAllBytes(file.FullName);
        }
        public static void OpenWithDefaultProgram(string path)
        {
            using Process fileopener = new Process();

            fileopener.StartInfo.FileName = "explorer";
            fileopener.StartInfo.Arguments = "\"" + path + "\"";
            fileopener.Start();
        }

        public static void OpenDirectoryAndSelect(string path)
        {
            if (!File.Exists(path))
            {
                return;
            }
            string argument = "/select, \"" + path + "\"";

            System.Diagnostics.Process.Start("explorer.exe", argument);
        }

        internal static List<string> RemoveAtIndex(this List<string> Source, int Index)
        {
            Source.RemoveAt(Index);
            return Source;
        }

        /// <summary>
        /// Formatter for [$] insertions
        /// </summary>
        internal static string Extern(this string TargetString, object Replacement)
        {
            return TargetString.Replace("[$]", $"{Replacement}");
        }
        /// <summary>
        /// Formatter for enumerated [$n] insertions
        /// </summary>
        internal static string Exform(this string TargetString, params object[] Replacements)
        {
            Dictionary<string, string> IndexReplacements = new();
            int ReplacementsIndexer = 1;
            foreach (object Replacement in Replacements)
            {
                IndexReplacements[$"[${ReplacementsIndexer}]"] = $"{Replacement}";
                ReplacementsIndexer++;
            }

            foreach (KeyValuePair<string, string> Replacement in IndexReplacements)
            {
                TargetString = TargetString.Replace(Replacement.Key, Replacement.Value);
            }

            return TargetString;
        }


        internal static bool IsNull(this object? item)
        {
            if (item == null)
            {
                return true;
            }
            else if (item.Equals("<Null>"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        internal static void rin(params object[] s) => Console.WriteLine(String.Join(' ', s));
        internal static void rinx(params object[] s) { Console.WriteLine(String.Join(' ', s)); rinx(); }
        internal static void rinx() => Console.ReadKey();

        internal static bool IsDirectoryWritable(string dirPath, bool throwIfFails = false)
        {
            try
            {
                using (FileStream fs = File.Create(
                    Path.Combine(
                        dirPath,
                        Path.GetRandomFileName()
                    ),
                    1,
                    FileOptions.DeleteOnClose)
                )
                { }
                return true;
            }
            catch
            {
                if (throwIfFails)
                    throw;
                else
                    return false;
            }
        }



        internal static void CreateDirectoryTree(string HeaderDirectory, List<string> SubDirectories)
        {
            if (!Directory.Exists(HeaderDirectory))
            {
                Directory.CreateDirectory(HeaderDirectory);
            }
            foreach (string RelativeSubDir in SubDirectories)
            {
                if (!Directory.Exists(@$"{HeaderDirectory}\{RelativeSubDir}") & !RelativeSubDir.Equals("")) Directory.CreateDirectory(@$"{HeaderDirectory}\{RelativeSubDir}");
            }
        }

        internal static List<string> GetFilesWithExtensions(string path, params string[] Extensions)
        {
            return Directory.GetFiles(path, "*.*")
                            .Where(file => file.EndsWithOneOf(Extensions))
                            .ToList();
        }

        internal static string GetFilename(this string s)
        {
            return s.Split("\\")[^1].Split("/")[^1];
        }


        internal static Dictionary<string, string> SingleCharValuesToUnicode(this Dictionary<string, string> Target)
        {
            Dictionary<string, string> Export = Target.Clone();

            foreach (var i in Target)
            {
                Export[i.Key] = @$"\u{GetEscapeSequence(char.Parse(i.Value)).ToLower()}";
            }

            return Export;
        }

        internal static Dictionary<string, string> Clone(this Dictionary<string, string> Target)
        {
            Dictionary<string, string> Cloned = new Dictionary<string, string>();
            foreach (var i in Target)
            {
                Cloned[i.Key] = i.Value;
            }

            return Cloned;
        }

        internal static bool MatchesWithSearchPattern(this string CheckString, string Pattern)
        {
            return Regex.Match(CheckString, "^" + Pattern.Replace("*", ".*") + "$").Success;
        }
        internal static bool MatchesWithAnySearchPatterns(this string CheckString, IEnumerable<string> Patterns)
        {
            foreach(string Pattern in Patterns)
            {
                if (CheckString.MatchesWithSearchPattern(Pattern)) return true;
            }
            return false;
        }
        internal static List<string> FindMatchesWithSearchPatternsFrom(this string CheckString, IEnumerable<string> Patterns)
        {
            List<string> Founded = new List<string>();

            foreach (string Pattern in Patterns)
            {
                if (CheckString.MatchesWithSearchPattern(Pattern)) Founded.Add(Pattern);
            }
            return Founded;
        }
        internal static List<MergedFont.FontsTomlConfig.FontRule> FindMatchingFontRules(this string CheckString)
        {
            List<MergedFont.FontsTomlConfig.FontRule> Founded = new List<MergedFont.FontsTomlConfig.FontRule>();

            foreach (var Pattern in MergedFont.FontsTomlConfig.Parameters)
            {
                if (CheckString.MatchesWithSearchPattern(Pattern.Key))
                {
                    foreach(var FontRule in Pattern.Value)
                    {
                        Founded.Add(FontRule);
                    }
                }
            }
            return Founded;
        }

        internal static bool HasProperty(this object ObjectItem, string SearchProperty)
        {
            return ObjectItem.GetType().GetProperties().Select(property => property.Name.ToString()).ToList().Contains(SearchProperty);
        }
        /// <summary>
        /// Accepts RGB or ARGB hex sequence (#abcdef / #ffabcdef)<br/>
        /// Returns transperent if HexString is null or equals "", White if HexString is not color
        /// </summary>
        internal static SolidColorBrush ToColor(string HexString)
        {
            if (HexString.IsNull()) return System.Windows.Media.Brushes.White;

            if (HexString.Length == 7)
            {
                return new SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(
                        Convert.ToByte(HexString.Substring(1, 2), 16),
                        Convert.ToByte(HexString.Substring(3, 2), 16),
                        Convert.ToByte(HexString.Substring(5, 2), 16)
                    )
                );
            }
            else if (HexString.Length == 9)
            {
                return new SolidColorBrush(
                    System.Windows.Media.Color.FromArgb(
                        Convert.ToByte(HexString.Substring(1, 2), 16),
                        Convert.ToByte(HexString.Substring(3, 2), 16),
                        Convert.ToByte(HexString.Substring(5, 2), 16),
                        Convert.ToByte(HexString.Substring(7, 2), 16)
                    )
                );
            }
            else
            {
                return System.Windows.Media.Brushes.White;
            }
        }


        internal static System.Windows.Media.FontFamily FileToFontFamily(string FontPath, string OverrideFontInternalName = "")
        {
            if (File.Exists(FontPath))
            {
                string FontFullPath = new FileInfo(FontPath).FullName;
                Uri FontUri = new Uri(FontFullPath, UriKind.Absolute);
                //rin($"  Successful font loading from file \"{FontPath}\"");
                string FontInternalName = OverrideFontInternalName.Equals("") ? GetFontName(FontFullPath) : OverrideFontInternalName;
                return new System.Windows.Media.FontFamily(FontUri, $"./#{FontInternalName}");
            }
            else
            {
                //rin($"  Font file \"{FontPath}\" not found, returning Arial");
                return new System.Windows.Media.FontFamily("Arial");
            }
        }

        internal static string GetFontName(string FontPath)
        {
            using (System.Drawing.Text.PrivateFontCollection PrivateFonts = new())
            {
                PrivateFonts.AddFontFile(FontPath);
                string FontName = PrivateFonts.Families[0].Name;

                return FontName;
            }
        }
        internal static string GetEscapeSequence(char c)
        {
            return ((int)c).ToString("X4");
        }
        internal static string ToUnicodeSequence(this string TargetString, string MaskString = "")
        {
            string Export = "";

            if (MaskString.Equals(""))
            {
                foreach (char c in TargetString)
                {
                    Export += @$"\u{GetEscapeSequence(c).ToLower()}";
                }
            }
            else if (TargetString.Length == TargetString.Length)
            {
                int Indexer = 0;
                foreach (char c in TargetString)
                {
                    Export += GetEscapeSequence(c) + $"[{MaskString[Indexer]}] ";
                    Indexer++;
                }
            }

            return Export;
        }
    }
}
