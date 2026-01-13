using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Translation_Devouring_Siltcurrent
{
    namespace LocalizationProcessingModules
    {
        public static class StylePlaceholders
        {
            public static string PlaceStyleHighlightPlaceholders(string SkillsJsonText)
            {
                int Replacements = 0;
                JToken JParser = JToken.Parse(SkillsJsonText);
                foreach (JToken StringItem in JParser.SelectTokens("$.dataList[*].levelList[*].coinlist[*].coindescs[*].desc"))
                {
                    if (!$"{StringItem}".Contains("<style=\"highlight\">") & !$"{StringItem}".Equals(""))
                    {
                        StringItem.Replace($"{StringItem}<style=\"highlight\"></style>");
                        Replacements++;
                    }
                }
                foreach (JToken StringItem in JParser.SelectTokens("$.dataList[*].levelList[*].desc"))
                {
                    if (!$"{StringItem}".Contains("<style=\"highlight\">") & !$"{StringItem}".Equals(""))
                    {
                        StringItem.Replace($"{StringItem}<style=\"highlight\"></style>");
                        Replacements++;
                    }
                }

                if (Replacements > 0)
                {
                    return JParser.ToString(Formatting.Indented);
                }
                else
                {
                    return SkillsJsonText;
                }
            }
        }
    }
}
