using Newtonsoft.Json;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TexExtension.ExternalBase;
using System.Text.RegularExpressions;
using LC_Localization_Controls;
using LC_Localization_Controls.Converters;
using GeneralResources;

namespace Configuration
{
    internal abstract class AppConfiguration
    {
        internal protected static Dictionary<string, dynamic> T = new();
        public static void InitT() => T = MainWindow.T;

        internal protected static Regex ParameterIngre = new Regex(@"`(?<parameter>.*?)`\[(?<type>\w+)\] @ (?<value>.*)");

        internal protected static string SelectedLanguage = ".undefined";
        internal protected static string KeywordsDirectory = ".undefined";




        internal protected static Dictionary<string, string> StringConfiguration = new();
        internal protected static Dictionary<string, int> IntegerConfiguration = new();
        internal protected static Dictionary<string, bool> ToggleConfiguration = new();




        public static void SaveConfiguration(string SavingParameterName, object NewValue)
        {
            if (!MainWindow.ConfigurationLoadingEvent)
            {
                string[] Lines = File.ReadAllLines(@"⇲ Asset Directory\Parameters.T[-]");

                int LineIndex = 0;

                foreach (string Line in Lines)
                {
                    GroupCollection Setting = ParameterIngre.Match(Line.Trim()).Groups;
                    string Name = Setting["parameter"].Value;

                    if (Name.Equals(SavingParameterName))
                    {
                        Lines[LineIndex] = ParameterIngre.Replace(Lines[LineIndex], Match =>
                        {
                            string Name = Setting["parameter"].Value;
                            string Type = Setting["type"].Value;
                            string Value = Setting["value"].Value;

                            object ActualValue = Type.Equals("String") ? $"\"{NewValue}\"".Replace("\n", "\\n").Replace("\r", "") : NewValue;

                            return $"`{Name}`[{Type}] @ {ActualValue}";
                        });

                        File.WriteAllLines(@"⇲ Asset Directory\Parameters.T[-]", Lines);
                        LoadConfiguration();
                        break;
                    }

                    LineIndex++;
                }
            }
        }

        public static void LoadConfiguration()
        {
            MainWindow.ConfigurationLoadingEvent = true;

            string[] Lines = File.ReadAllLines(@"⇲ Asset Directory\Parameters.T[-]");

            foreach(string Line in Lines)
            {
                try
                {
                    GroupCollection Setting = ParameterIngre.Match(Line.Trim()).Groups;

                    string Name = Setting["parameter"].Value;
                    string Type = Setting["type"].Value;
                    string Value = Setting["value"].Value;

                    switch(Type)
                    {
                        case "String":
                            StringConfiguration  [Name] = Convert.ToString(Value[1..^1]).Replace("\\n", "\n"); break;

                        case "Integer":
                            IntegerConfiguration [Name] = Convert.ToInt32 (Value); break;

                        case "Toggle":
                            ToggleConfiguration  [Name] = Value.Equals("Yes"); break;
                    }
                }
                catch { }
            }
            try
            {
                SelectedLanguage = StringConfiguration["Selected Language"];
                KeywordsDirectory = StringConfiguration["Selected Keywords"];

                KSTFont.UpdateSelectedFontAction(StringConfiguration["Selected font"]);
                KSTFont.UpdateCustomFontsDropdownList(@$"⇲ Asset Directory\Font\{StringConfiguration["Selected font"]}\replacement_map.json");

                CustomLanguage.LoadCustomLanguage();

                MainWindow.ConfigurationLoadingEvent = false;
            }
            catch { }
        }
    }
}
