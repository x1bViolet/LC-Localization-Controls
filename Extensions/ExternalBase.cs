using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Drawing.Text;

namespace TexelExtension
{
    internal static class ExternalBase
    {
        internal static int LinesCount(this string check) => check.Count(f => f == '\n');
        internal static string RegexRemove(this string TargetString, Regex PartPattern)
        {
            return PartPattern.Replace(TargetString, Match => { return ""; });
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
            List<string> Export = new() { "" };
            foreach (var Check in CheckSource)
            {
                if (Check.Contains(CheckString))
                {
                    if (Export[0].Equals("")) Export.RemoveAt(0);
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
            foreach(var file in CheckSource)
            {
                if (file.Name.Equals(CheckString)) return true;
            }

            return false;
        }

        internal static List<string> RemoveAtIndex(this List<string> Source, int Index)
        {
            Source.RemoveAt(Index);
            return Source;
        }


        internal static string ExTern(this string TargetString, object Replacement)
        {
            return TargetString.Replace("[$]", $"{Replacement}");
        }
        internal static string Exform(this string TargetString, params string[] Replacements)
        {
            Dictionary<string, string> IndexReplacements = new();
            int Indexer = 1;
            foreach (string Replacement in Replacements)
            {
                IndexReplacements[$"[${Indexer}]"] = Replacement;
                Indexer++;
            }

            foreach (KeyValuePair<string, string> Replacement in IndexReplacements)
            {
                TargetString = TargetString.Replace(Replacement.Key, Replacement.Value);
            }

            return TargetString;
        }


        internal static bool IsNull(this object item)
        {
            try { if (item.Equals == null) { } }
            catch { return true; }

            return false;
        }
        internal static void rin(params object[] s) => Console.WriteLine(String.Join(' ', s));
        internal static void rinx(params object[] s) { Console.WriteLine(String.Join(' ', s)); Console.ReadKey(); }
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

        internal static SolidColorBrush? ToColor(string HexString)
        {
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
            else return null;
        }

        internal static SolidColorBrush? ToColor(this string HexString, bool Throw = false)
        {
            if (HexString.IsColor()) return ToColor(HexString);
            else return null;
        }


        internal static bool IsColor(string HexString)
        {
            try
            {
                ToColor(HexString);
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal static bool IsColor(this string HexString, bool Throw = false)
        {
            try
            {
                ToColor(HexString);
                return true;
            }
            catch
            {
                if (Throw) throw new Exception();
                else return false;
            }
        }


        internal static System.Windows.Media.FontFamily FileToFontFamily(string FontPath)
        {
            string FontFullPath = new FileInfo(FontPath).FullName;
            Uri FontUri = new Uri(FontFullPath, UriKind.Absolute);

            return new System.Windows.Media.FontFamily(FontUri, $"./#{GetFontName(FontFullPath)}");
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

        internal static string FontName(this string FontPath)
        {
            using (PrivateFontCollection privateFonts = new PrivateFontCollection())
            {
                privateFonts.AddFontFile(FontPath);
                string fontName = privateFonts.Families[0].Name;
                return fontName;
            }
        }



        internal static List<string> GetExtFiles(string path, params string[] Extensions)
        {
            return Directory.GetFiles(path, "*.*")
                            .Where(file => file.EndsWithOneOf(Extensions))
                            .ToList();
        }



        internal static string GetEscapeSequence(char c)
        {
            return ((int)c).ToString("X4");
        }
        internal static string ToUnicodeSequence(string TargetString, string MaskString = "")
        {
            string Export = "";

            if (MaskString.Equals(""))
            {
                foreach(char c in TargetString)
                {
                    Export += GetEscapeSequence(c) + " ";
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
