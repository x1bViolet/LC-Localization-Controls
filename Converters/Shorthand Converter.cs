using Configuration;
using GeneralResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static LC_Localization_Controls.MainWindow;
using static TexelExtension.ExternalBase;

namespace LC_Localization_Controls.Converters
{
    internal abstract class ShorthandConverter
    {
        internal static string Redirect(string FileText)
        {
            foreach (Match Match in Regex.Matches(FileText, AppConfiguration.StringConfiguration["Shorthand Pattern"]))
            {
                string ID     = Match.Groups["ID"].Value;
                string Export = Match.Groups[0   ].Value;

                if (Internal.KeywordsGlobalDictionary.ContainsKey(ID))
                {
                    string Name  = Match.Groups["Name" ].Value;
                    string Color = Match.Groups["Color"].Value;

                    if  (Color.Equals(""))   Color = Internal.KeywordsGlobalDictionary[ID].Color;
                    else Color = Regex.Match(Color, "(#[a-fA-F0-9]{6})").Value;

                    Export = $"<sprite name=\\\"{ID}\\\"><color={Color}><u><link=\\\"{ID}\\\">{Name}</link></u></color>";

                    FileText = FileText.Replace(Match.Value, Export);
                }
            }

            return FileText;
        }
    }
}
