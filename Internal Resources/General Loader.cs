using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Configuration;
using System.Windows.Input;
using System.Windows.Media;

using Newtonsoft.Json;
using SixLabors.ImageSharp;
using static TexelExtension.ExternalBase;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace GeneralResources
{
    public static class Internal
    {
        public class KeywordComplexInfo
        {
            public string ID    { get; set; }
            public string Name  { get; set; }
            public string Desc  { get; set; }
            public string Color { get; set; }
            public SolidColorBrush ColorBursh { get; set; }
            public BitmapImage  Image { get; set; }
            public List<string> Forms { get; set; }
        }

        private class KeywordImageInfo
        {
            public string KeywordID { get; set; }
            public bool IsWebp { get; set; }
        }

        public static Dictionary<string, KeywordComplexInfo> KeywordsGlobalDictionary = new();

        public static class FontReplace
        {
            public class GlyphReplaceData
            {
                public Dictionary<string, string> mikodacs { get; set; }
                public Dictionary<string, string> bebas { get; set; }
                public Dictionary<string, string> caveat { get; set; }
                public Dictionary<string, string> excelsior { get; set; }
            }

            public static class TypeMatches
            {
                public static List<string> PanicNames = new() { "PanicInfo" };
                public static List<string> Skills = new() { "Skills" };
                public static List<string> NameOnly = new() {
                    "Passive",
                    "EGOgift_",
                    "BattleKeywords",
                    "Bufs",
                    "Egos"
                };
                public static List<string> Personalities = new() { "Personalities" };
            }

            public static Dictionary<char, char> Mikodacs = new();
            public static Dictionary<char, char> BebasKai = new();
            public static Dictionary<char, char> Caveat = new();
            public static Dictionary<char, char> Excelsior = new();

            public static void ReadGRD()
            {
                GlyphReplaceData? Replaces = JsonConvert.DeserializeObject<GlyphReplaceData>(File.ReadAllText(@$"⇲ Asset Directory\Font\{AppConfiguration.StringConfiguration["Selected font"]}\replacement_map.json"));

                foreach (var CharPair in Replaces.mikodacs)  Mikodacs  [char.Parse(CharPair.Key)] = char.Parse(CharPair.Value);
                foreach (var CharPair in Replaces.bebas)     BebasKai  [char.Parse(CharPair.Key)] = char.Parse(CharPair.Value);
                foreach (var CharPair in Replaces.caveat)    Caveat    [char.Parse(CharPair.Key)] = char.Parse(CharPair.Value);
                foreach (var CharPair in Replaces.excelsior) Excelsior [char.Parse(CharPair.Key)] = char.Parse(CharPair.Value);
            }

            public static string Convert(string Input, Dictionary<char, char> ReplaceSource)
            {
                foreach (KeyValuePair<char, char> CharMaps in ReplaceSource)
                {
                    Input = Input.Replace(CharMaps.Key, CharMaps.Value);
                }

                return Input;
            }

            public static void Typematch_Convert(ref string JsonText, string Filename)
            {
                rin(Filename);
                if (Filename.StartsWithOneOf(TypeMatches.NameOnly))
                {
                    JsonText = ConvertDatalistNames(JsonText);
                }
                else if (Filename.StartsWithOneOf(TypeMatches.Skills))
                {
                    JsonText = ConvertSkillNames(JsonText);
                }
                else if (Filename.StartsWithOneOf(TypeMatches.PanicNames))
                {
                    JsonText = ConvertPanicNames(JsonText);
                }
                else if (Filename.StartsWithOneOf(TypeMatches.Personalities))
                {
                    JsonText = ConvertPersonalities(JsonText);
                }
            }

            

            public static string ConvertSkillNames(string JsonText)
            {
                JObject JParser = JObject.Parse(JsonText);
                var SkillNames = JParser.SelectTokens("$.dataList[*].levelList[*].name");
                foreach (var SkillName in SkillNames)
                {
                    SkillName.Replace(Convert($"{SkillName}", Mikodacs));
                }

                if (JsonText.Contains(@"""abName"": """))
                {
                    var SkillABNames = JParser.SelectTokens("$.dataList[*].levelList[*].abName");
                    foreach (var SkillABName in SkillABNames)
                    {
                        SkillABName.Replace(Convert($"{SkillABName}", Mikodacs));
                    }
                }

                return JParser.ToString(Formatting.Indented);
            }

            public static string ConvertDatalistNames(string JsonText)
            {
                JObject JParser = JObject.Parse(JsonText);
                var Names = JParser.SelectTokens("$.dataList[*].name");
                foreach (var Name in Names)
                {
                    Name.Replace(Convert($"{Name}", Mikodacs));
                }

                return JParser.ToString(Formatting.Indented);
            }

            public static string ConvertPanicNames(string JsonText)
            {
                var JParser = JObject.Parse(JsonText);
                var PanicNames = JParser.SelectTokens("$.dataList[*].panicName");
                foreach (var PanicName in PanicNames)
                {
                    PanicName.Replace(Convert($"{PanicName}", Mikodacs));
                }

                return JParser.ToString(Formatting.Indented);
            }

            public static string ConvertPersonalities(string JsonText)
            {
                var JParser = JObject.Parse(JsonText);
                var Personalities = JParser.SelectTokens("$.dataList[*].title");
                foreach (var Personality in Personalities)
                {
                    Personality.Replace(Convert($"{Personality}", Mikodacs));
                }

                return JParser.ToString(Formatting.Indented);
            }
        }

        public static void Initialize_KeywordsGlobalDictionary()
        {
            Dictionary<string, string> KeywordsColorsInfo = GenerateColorInfo(@"⇲ Asset Directory\Keywords\Keywords@DefaultColordata.T[-]");
            Dictionary<string, BitmapImage> KeywordsImageInfo = GetKeywordImages();
            try { FontReplace.ReadGRD(); } catch { }

            KeywordsGlobalDictionary["Unknown"] = new KeywordComplexInfo
            {
                   ID = "Unknown",
                 Name = "Unknown",
                 Desc = "",
                Color = "#9f6a3a",
                Image = KeywordsImageInfo["Unknown"],
                ColorBursh = ToColor("#9f6a3a")
            };

            List<string> KeywordsStringInfo_PriorityLoad = new() {
                @$"⇲ Asset Directory\Keywords\Text Sources\{AppConfiguration.StringConfiguration["Selected Keywords"]}\Bufs",
                @$"⇲ Asset Directory\Keywords\Text Sources\{AppConfiguration.StringConfiguration["Selected Keywords"]}\BattleKeywords"
            };

            int TotalCounter = 0;
            foreach (string KeywordsDirectory in KeywordsStringInfo_PriorityLoad)
            {
                DirectoryInfo ThisKeywordsDirectory = new DirectoryInfo(KeywordsDirectory);
                foreach(FileInfo KeywordsFile in ThisKeywordsDirectory.GetFiles("*.json", searchOption: SearchOption.AllDirectories))
                {
                    KeywordsJson? Keywords_Json = JsonConvert.DeserializeObject<KeywordsJson>(File.ReadAllText(KeywordsFile.FullName));
                    foreach(KeywordsJson.Keyword Keyword in Keywords_Json.dataList)
                    {
                        if (!KeywordsGlobalDictionary.ContainsKey(Keyword.id))
                        {
                            if (!Keyword.desc.IsNull())
                            {
                                if (!Keyword.desc.Equals(""))
                                {
                                    string PreOrderKeywordColor = KeywordsColorsInfo.ContainsKey(Keyword.id) ? KeywordsColorsInfo[Keyword.id] : "#9f6a3a";

                                    BitmapImage PreOrderKeywordImage = KeywordsImageInfo.ContainsKey(Keyword.id) ? KeywordsImageInfo[Keyword.id] : KeywordsImageInfo["Unknown"];

                                    KeywordsGlobalDictionary[Keyword.id] = new KeywordComplexInfo
                                    {
                                           ID =  Keyword.id,
                                         Name = !Keyword.name.IsNull() ? Keyword.name : "",
                                         Desc = !Keyword.desc.IsNull() ? Keyword.desc : "",
                                        Color =  PreOrderKeywordColor,
                                        Image =  PreOrderKeywordImage,
                                        ColorBursh = ToColor(PreOrderKeywordColor),
                                    };
                                    TotalCounter++;
                                }
                            }
                        }
                    }
                }
            }
            rin(@$"{TotalCounter} Keywords loaded from '⇲ Asset Directory\Keywords\Text Sources\{AppConfiguration.KeywordsDirectory}\'");
        }


















        private class KeywordsJson
        {
            public List<Keyword> dataList { get; set; }
            public class Keyword
            {
                public string id { get; set; }
                public string name { get; set; }
                public string desc { get; set; }
            }
        }

        public static Dictionary<string, BitmapImage> GetKeywordImages()
        {
            Dictionary<string, BitmapImage> SpriteFiles = new();

            foreach (string KeywordImage in Directory.EnumerateFiles(@"⇲ Asset Directory\Keywords\Images", searchPattern: "*.*", searchOption: SearchOption.AllDirectories))
            {
                string KeywordID = KeywordImage.Split(@"\")[^1].Split('.')[0]; //   C:\Image.png → Image.png → Image

                SpriteFiles[KeywordID] = GenerateBitmap(KeywordImage);
            }
            return SpriteFiles;
        }

        private static BitmapImage GenerateBitmap(string ImageFilepath)
        {
            bool IsWebp = ImageFilepath.EndsWith(".webp");
            byte[] ImageData = File.ReadAllBytes(ImageFilepath);
            using (MemoryStream stream = new MemoryStream(IsWebp ? ConvertWebPToPng(ImageData) : ImageData))
            {
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = stream;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                return bitmapImage;
            }
        }

        private static byte[] ConvertWebPToPng(byte[] WebpData)
        {
            using (var InputStream = new MemoryStream(WebpData))
            using (var image = Image.Load(InputStream))
            {
                using (var OutputStream = new MemoryStream())
                {
                    image.SaveAsPng(OutputStream);
                    return OutputStream.ToArray();
                }
            }
        }

        private static Dictionary<string, string> GenerateColorInfo(string ColorDataFilepath)
        {
            Dictionary<string, string> KeywordColorsInfo = new();
            foreach (string Line in File.ReadAllLines(ColorDataFilepath))
            {
                string[] Sep = Line.Split(" ¤ ");
                KeywordColorsInfo[Sep[0]] = Sep[1];
            }

            return KeywordColorsInfo;
        }
    }
}
