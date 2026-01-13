using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;

namespace Translation_Devouring_Siltcurrent
{
    // All utility garbage
    public static class Requirements
    {
        public static void rin(params object[] s) => Console.WriteLine(string.Join(' ', s));

        public static Color ToColor(string HexColor)
        {
            if (HexColor == null) return Colors.White;

            HexColor = HexColor.Replace("#", "");
            try
            {
                if (HexColor.Length != 8 & HexColor.Length != 6) return Colors.White;

                static byte AsByte(string Hex) => Convert.ToByte(Hex, 16);

                string RGB = HexColor.Length == 8 ? HexColor[2..8] : HexColor;
                string Alpha = HexColor.Length == 8 ? HexColor[0..2] : "FF";

                return new Color()
                {
                    A = AsByte(Alpha),
                    R = AsByte(RGB[0..2]),
                    G = AsByte(RGB[2..4]),
                    B = AsByte(RGB[4..6]),
                };
            }
            catch
            {
                return Colors.White;
            }
        }

        public static SolidColorBrush ToSolidColorBrush(string HexColor) => new SolidColorBrush(ToColor(HexColor));

        /// <summary>
        /// Remove string parts
        /// </summary>
        public static string Del(this string Target, params string[] FragmentsToRemove)
        {
            if (!string.IsNullOrEmpty(Target))
            {
                foreach (string? Fragment in FragmentsToRemove)
                {
                    if (!string.IsNullOrEmpty(Fragment)) Target = Target.Replace(Fragment, "");
                }
            }

            return Target;
        }

        public static DependencyProperty Register<OwnerType, PropertyType>(string Name, PropertyType DefaultValue, PropertyChangedCallback ValueSetEvent = null)
        {
            return DependencyProperty.Register(
               name: Name, ownerType: typeof(OwnerType), propertyType: typeof(PropertyType),
               typeMetadata: new PropertyMetadata(DefaultValue, ValueSetEvent)
            );
        }

        public static bool HasAttribute<AttributeType>(this PropertyInfo Property, out AttributeType AcquiredAttribute) where AttributeType : Attribute
        {
            AcquiredAttribute = null;
            AttributeType? GettedAttribute = Property.GetCustomAttribute<AttributeType>();

            if (GettedAttribute != null)
            {
                AcquiredAttribute = GettedAttribute;
                return true;
            }
            else return false;
        }

        public static bool HasAttribute<AttributeType>(this PropertyInfo Property) where AttributeType : Attribute => Property.GetCustomAttribute<AttributeType>() != null;


        public static void CreateDirectoryTree(string HeaderDirectory, List<string> SubDirectories)
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


        // Readed file encoding
        public static Encoding GetFileEncoding(this FileInfo TargetFile)
        {
            using (StreamReader Reader = new StreamReader(path: TargetFile.FullName, encoding: Encoding.UTF8, detectEncodingFromByteOrderMarks: true))
            {
                Reader.Peek();
                return Reader.CurrentEncoding;
            }
        }

        public static bool IsUTF8BOM(this FileInfo TargetFile)
        {
            using (StreamReader Reader = new(TargetFile.FullName, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false)))
            {
                Reader.Read(); // Read the first character to trigger encoding detection
                return Reader.CurrentEncoding.GetPreamble().Length != 0;
            }
        }

        public static bool MatchesWithOneOf(this string CheckString, params string[] Patterns) => Patterns.ToList().Any(Pattern => Regex.Match(CheckString, Pattern).Success);

        
        public static bool EqualsOneOf(this char CheckCharacter, params char[] CheckSource) => CheckSource.ToList().Any(x => x.Equals(CheckCharacter));

        public static bool ContainsOneOf(this string CheckString, params string[] CheckSource) => CheckSource.ToList().Any(x => x.Contains(CheckString));

        public static string RegexRemove(this string TargetString, Regex PartPattern) => PartPattern.Replace(TargetString, Match => { return ""; });

        internal static string GetEscapeSequence(char Character) => ((int)Character).ToString("X4");
        public static string ToUnicodeSequence(this string TargetString)
        {
            string Export = "";
            foreach (char c in TargetString) Export += @$"\u{GetEscapeSequence(c).ToLower()}";

            return Export;
        }

        public static int CountLines(this string check) => check.Count(f => f == '\n');

        public static SaveFileDialog NewSaveFileDialog(string FilesHint, IEnumerable<string> Extensions, string FileDefaultName = "", string DefaultDirectory = "")
        {
            List<string> FileFilters_DefaultExt = [];
            List<string> FileFilters_Filter = [];

            foreach (string Filter in Extensions)
            {
                FileFilters_DefaultExt.Add($".{Filter}");
                FileFilters_Filter.Add($"*.{Filter}");
            }

            SaveFileDialog FileSaving = new()
            {
                DefaultExt = string.Join("|", FileFilters_DefaultExt), // .png|.jpg
                Filter = $"{FilesHint}|{string.Join(";", FileFilters_Filter)}",  // *.png;*.jpg
                FileName = FileDefaultName,
                DefaultDirectory = DefaultDirectory
            };

            return FileSaving;
        }


        public static OpenFileDialog NewOpenFileDialog(string FilesHint, IEnumerable<string> Extensions, string DefaultDirectory = "")
        {
            List<string> FileFilters_DefaultExt = [];
            List<string> FileFilters_Filter = [];

            foreach (string Filter in Extensions)
            {
                FileFilters_DefaultExt.Add($".{Filter}");
                FileFilters_Filter.Add($"*.{Filter}");
            }

            OpenFileDialog FileSelection = new()
            {
                DefaultExt = string.Join("|", FileFilters_DefaultExt), // .png|.jpg
                Filter = $"{FilesHint}|{string.Join(";", FileFilters_Filter)}",  // *.png;*.jpg,
                DefaultDirectory = DefaultDirectory
            };

            return FileSelection;
        }


        public static string RemovePrefix(this string Target, params string[] Prefixes)
        {
            foreach (string Prefix in Prefixes)
            {
                if (Target.StartsWith(Prefix)) return Target[Prefix.Length..];
            }
            
            return Target;
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

            Process.Start("explorer.exe", argument);
        }


        public static string WildcardToRegex(string WildcardPattern)
        {
            WildcardPattern = WildcardPattern.Trim().Replace("*", "\0WILDCARD:STAR").Replace("?", "\0WILDCARD:QUESTION");
            WildcardPattern = Regex.Escape(WildcardPattern).Replace("\0WILDCARD:STAR", ".*?").Replace("\0WILDCARD:QUESTION", ".");
            return WildcardPattern;
        }

        public static bool MatchesWithWildcards(this string CheckString, IEnumerable<string> WildcardPatterns, out List<string> MatchedPatterns)
        {
            MatchedPatterns = [];
            foreach (string Pattern in WildcardPatterns)
            {
                if (Regex.Match(CheckString, WildcardToRegex(Pattern)).Success) MatchedPatterns.Add(Pattern);
            }
            return MatchedPatterns.Count != 0;
        }


        /// <summary>
        /// Wildcard patterns to regex patterns for strings
        /// </summary>
        public static string[] GeneratePatternsList(string FilePatterns)
        {
            List<string> Output = [];

            List<string> PatternsList = FilePatterns.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(FilePattern => FilePattern.Trim().Replace("*", "\0WILDCARD:STAR").Replace("?", "\0WILDCARD:QUESTION"))
                .Where(FilePattern => FilePattern != "")
                .ToList();

            foreach (string FilePattern in PatternsList)
            {
                Output.Add(Regex.Escape(FilePattern).Replace("\0WILDCARD:STAR", ".*?").Replace("\0WILDCARD:QUESTION", "."));
            }

            return Output.ToArray();
        }

    }
}
