using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace Additional_Codebase
{
    public static class Utils
    {
        public static bool IsNull(this object item)
        {
            try { if (item.Equals == null); }
            catch { return true; }

            return false;
        }
        public static int LinesCount(this string check) => check.Count(f => f == '\n');
        public static bool StartsWithOneOf(this string CheckString, IEnumerable<string> CheckSources)
        {
            foreach (var Check in CheckSources)
            {
                if (CheckString.StartsWith(Check)) return true;
            }

            return false;
        }
        public static void rin(params object[] s) => Console.WriteLine(String.Join(' ', s));

        public static bool IsDirectoryWritable(string dirPath, bool throwIfFails = false)
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

        public static void GenerateRichText(this RichTextBox Target, string RichTextString)
        {
            RichText.RichTextBoxApplicator.GenerateAt(RichTextString, Target);
        }

        public static SolidColorBrush From(string HexString)
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
            else
            {
                return new SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(
                        Convert.ToByte("ff", 16),
                        Convert.ToByte("ff", 16),
                        Convert.ToByte("ff", 16)
                    )
                );
            }
        }


        public static bool IsColor(string HexString)
        {
            try
            {
                From(HexString);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

}
