using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using static Additional_Codebase.Utils;
using static GeneralResources.Internal;

namespace RichText 
{
    class RichTextBoxApplicator
    {
        #region Методы добавления текста и спрайтов на предпросмотр
        private static void RichText_AppendText(InlineTextConstructor TextData, RichTextBox Target)
        {
            try
            {
                var document = Target.Document;
                if (document.Blocks.LastBlock is not Paragraph lastParagraph)
                {
                    lastParagraph = new Paragraph();
                    document.Blocks.Add(lastParagraph);
                }

                Run PreviewLayout_AppendRun = new Run(TextData.Text);

                TagManager.ApplyTags(ref PreviewLayout_AppendRun, TextData.InnerTags);

                #region Sub/Sup
                if (TextData.InnerTags.Contains("TextStyle@Subscript") | TextData.InnerTags.Contains("TextStyle@Superscript"))
                {
                    PreviewLayout_AppendRun.FontSize = 12;
                    StackPanel AlterHeight = new()
                    {
                        Height = 16,
                        Children = { new TextBlock(PreviewLayout_AppendRun) }
                    };

                    if (TextData.InnerTags.Contains("TextStyle@Subscript"))
                    {
                        AlterHeight.Margin = new Thickness(0, 0, 0, 0);
                    }
                    else if (TextData.InnerTags.Contains("TextStyle@Superscript"))
                    {
                        AlterHeight.Margin = new Thickness(0, -40, 0, 0);
                    }
                    AlterHeight.RenderTransform = new TranslateTransform(0, 5);

                    lastParagraph.Inlines.Add(new InlineUIContainer(AlterHeight));
                }
                #endregion

                else
                {
                    lastParagraph.Inlines.Add(PreviewLayout_AppendRun);
                }
            }
            catch { }
        }

        private static void RichText_AppendImage(InlineImageConstructor ImageData, RichTextBox Target)
        {
            try
            {
                var document = Target.Document;
                if (document.Blocks.LastBlock is not Paragraph lastParagraph)
                {
                    lastParagraph = new Paragraph();
                    document.Blocks.Add(lastParagraph);
                }
                BitmapImage source = new BitmapImage();
                if (KeywordsGlobalDictionary.ContainsKey(ImageData.ImageID))
                {
                    source = KeywordsGlobalDictionary[ImageData.ImageID].Image;
                }
                else
                {
                    source = KeywordsGlobalDictionary["Unknown"].Image;
                }
                
                Image SpriteImage = new()
                {
                    Source = source,
                    Width = Target.FontSize * 1.0285,
                    Height = Target.FontSize,
                    Margin = new Thickness(-2, -1.9, 0, 0)
                };

                StackPanel SpritePlusEffectname = new() { Orientation = Orientation.Horizontal };
                SpritePlusEffectname.Children.Add(new TextBlock(new InlineUIContainer(SpriteImage)));

                Run KeywordName = new Run(ImageData.TextBase.Text);
                TagManager.ApplyTags(ref KeywordName, ImageData.TextBase.InnerTags);

                SpritePlusEffectname.Children.Add(new TextBlock(KeywordName));

                SpritePlusEffectname.RenderTransform = new TranslateTransform(0, Target.FontSize/2.1);
                SpritePlusEffectname.Margin = new Thickness(0, -Target.FontSize/2.1, 0, 0);

                SpritePlusEffectname.VerticalAlignment = VerticalAlignment.Bottom;
                lastParagraph.Inlines.Add(new InlineUIContainer(SpritePlusEffectname));
            }
            catch { }
        }
        #endregion

        
        public static RichTextBox ApplyOn(RichTextBox Target, string RichTextString)
        {
            GenerateAt(RichTextString, Target);

            return Target;
        }

        
        public static void GenerateAt(string RichTextString, RichTextBox Target, bool ImagesLineBreak = false)
        {
            List<string> TagList = new()
            {
                "/color", 
                "sub",
                "sup",
                "/sub",
                "/sup",
                "i",
                "/i",
                "u",
                "/u",
                "b",
                "/b",
                "/",
                "style=\"upgradeHighlight\"",
                "/style",
                "strikethrough",
                "/strikethrough",
                "/font",
                "/size",

                "\0",

                "EMPTY¤",
            };
            foreach (var Tag in TagList)
            {
                RichTextString = RichTextString.Replace($">{Tag}<", $">\0{Tag}<");
            }

            bool ICm(string Range_TextItem)
            {
                return !TagList.Contains(Range_TextItem) & !Range_TextItem.StartsWith("color=#") & !Range_TextItem.StartsWith("sprite name=\"") & !Range_TextItem.StartsWith("font=\"") & !Range_TextItem.StartsWith("pfont=\"") & !Range_TextItem.StartsWith("size=");
            }

            #region Базовое форматирование текста
            RichTextString = RichTextString.Replace("color=#None", "color=#ffffff")
                               .Replace("<style=\"highlight\">", "<style=\"upgradeHighlight\">") // Подсветка улучшения (Без разницы как)
                               .Replace("</link>", "") // Ссылки вырезать (тултипы не работают (Возможно))
                               .Replace("[TabExplain]", "");

            RichTextString = Regex.Replace(RichTextString, @"<link=""\w+"">", ""); // убрать все link (Тултип не рабоатет)

            // Сепарированые обычных '<' '>' от тегов
            RichTextString = Regex.Replace(RichTextString, @"<color=#([0-9a-fA-F]{6})>", @"⇱color=#$1⇲");
            RichTextString = Regex.Replace(RichTextString, @"<sprite name=""(\w+)"">", @"⇱sprite name=""$1""⇲");
            RichTextString = Regex.Replace(RichTextString, @"<style=""(\w+)"">", @"⇱style=""$1""⇲");
            RichTextString = Regex.Replace(RichTextString, @"<font=""(.*?)"">", @"⇱font=""$1""⇲");
            RichTextString = Regex.Replace(RichTextString, @"<pfont=""(.*?)"">", @"⇱font=""$1""⇲");
            RichTextString = Regex.Replace(RichTextString, @"<size=(\d+)%>", @"⇱size=$1%⇲");
            foreach (var Tag in TagList) RichTextString = RichTextString.Replace($"<{Tag}>", $"⇱{Tag}⇲");
            #endregion

            // Главное разбивание текста на список с обычным текстом и тегами
            string[] TextSegmented = $"⇱EMPTY¤⇲\0⇱EMPTY¤⇲\0⇱EMPTY¤⇲\0⇱EMPTY¤⇲\0{RichTextString.Replace("\\n", "\n")}\0⇱EMPTY¤⇲\0⇱EMPTY¤⇲\0⇱EMPTY¤⇲\0⇱EMPTY¤⇲\0".Split(new char[] { '⇱', '⇲' }, StringSplitOptions.RemoveEmptyEntries);

            List<string> __TextSegmented__ = TextSegmented.ToList();
            __TextSegmented__.RemoveAll(TextItem => TextItem.Equals("\0"));

            #region ¤ Форматирование тегов ¤
            int TextSegmented_Count = __TextSegmented__.Count();

            #region Обычный текст
            for (int TextItem_Index = 0; TextItem_Index < __TextSegmented__.Count; TextItem_Index++)
            {
                string TextItem = __TextSegmented__[TextItem_Index];

                #region ⟦InnerTag/UptieHighlight⟧
                if (TextItem.Equals("style=\"upgradeHighlight\""))
                {
                    for (int RangeIndex = TextItem_Index + 1; RangeIndex < TextSegmented_Count; RangeIndex++)
                    {
                        string Range_TextItem = __TextSegmented__[RangeIndex];

                        if (Range_TextItem.Equals("/style")) break;
                        else if (!TagList.Contains(Range_TextItem))
                        {
                            if (Range_TextItem.Equals("/color"))
                            {
                                __TextSegmented__[RangeIndex] = "";
                            }
                            try
                            {
                                if (!__TextSegmented__[RangeIndex - 1].StartsWith("sprite name=\"") &
                                    !__TextSegmented__[RangeIndex - 2].StartsWith("sprite name=\"") &
                                    !__TextSegmented__[RangeIndex].StartsWith("size=") &
                                    !__TextSegmented__[RangeIndex - 1].StartsWith("size=") &
                                    !__TextSegmented__[RangeIndex].StartsWith("sprite name=\"")) // Сохранять цвета для статусных эффектов

                                {
                                    if (__TextSegmented__[RangeIndex].StartsWith("color=#"))
                                    {
                                        __TextSegmented__[RangeIndex] = "color=#f8c200";
                                    }
                                    else if (!Range_TextItem.Contains("⟦InnerTag/UptieHighlight⟧"))
                                    {
                                        __TextSegmented__[RangeIndex] += "⟦InnerTag/UptieHighlight⟧";
                                    }
                                }
                            }
                            catch { }
                        }

                    }
                }
                #endregion

                #region ⟦InnerTag/FontFamily@FontFamilyName⟧
                if (TextItem.StartsWith("font=\""))
                {
                    string FontFamily = Regex.Match(TextItem, @"font=""(.*)""").Groups[1].ToString();

                    for (int RangeIndex = TextItem_Index + 1; RangeIndex < TextSegmented_Count; RangeIndex++)
                    {
                        string Range_TextItem = __TextSegmented__[RangeIndex];

                        if (Range_TextItem.Equals("/font") | Range_TextItem.StartsWith("font=\"") | Range_TextItem.StartsWith("pfont=\"")) break;
                        if (ICm(Range_TextItem) & !Range_TextItem.Contains("⟦InnerTag/FontFamily@"))
                        {
                            __TextSegmented__[RangeIndex] += $"⟦InnerTag/FontFamily@{FontFamily}⟧";
                        }
                    }
                }
                #endregion

                #region ⟦InnerTag/FontFamily@PackFontFamilyName⟧
                // Needed for taking fontfamily from application resources
                if (TextItem.StartsWith("pfont=\""))
                {
                    string FontFamily = Regex.Match(TextItem, @"pfont=""(.*)""").Groups[1].ToString();

                    for (int RangeIndex = TextItem_Index + 1; RangeIndex < TextSegmented_Count; RangeIndex++)
                    {
                        string Range_TextItem = __TextSegmented__[RangeIndex];

                        if (Range_TextItem.Equals("/font") | Range_TextItem.StartsWith("font=\"") | Range_TextItem.StartsWith("pfont=\"")) break;
                        if (ICm(Range_TextItem) & !Range_TextItem.Contains("⟦InnerTag/FontFamily@") & !Range_TextItem.Contains("⟦InnerTag/PackFontFamily@"))
                        {
                            __TextSegmented__[RangeIndex] += $"⟦InnerTag/PackFontFamily@{FontFamily}⟧";
                        }
                    }
                }
                #endregion

                #region ⟦InnerTag/FontSize@Percent⟧
                if (TextItem.StartsWith("size=") & TextItem.EndsWith('%'))
                {
                    string FontSize = Regex.Match(TextItem, @"size=(\d+)%").Groups[1].ToString();

                    for (int RangeIndex = TextItem_Index + 1; RangeIndex < TextSegmented_Count; RangeIndex++)
                    {
                        string Range_TextItem = __TextSegmented__[RangeIndex];

                        if (Range_TextItem.Equals("/size") | Range_TextItem.StartsWith("size=") & Range_TextItem.EndsWith('%')) break;
                        if (ICm(Range_TextItem) & !Range_TextItem.Contains("⟦InnerTag/FontSize@"))
                        {
                            __TextSegmented__[RangeIndex] += $"⟦InnerTag/FontSize@{FontSize}%⟧";
                        }
                    }
                }
                #endregion

                #region ⟦InnerTag/TextColor@HexRGB⟧
                if (TextItem.StartsWith("color=#") & TextItem.Length == 13)
                {
                    string ColorCode = Regex.Match(TextItem, @"([0-9a-fA-F]{6})").Groups[1].ToString();
                    if (IsColor(ColorCode))
                    {
                        for (int RangeIndex = TextItem_Index + 1; RangeIndex < TextSegmented_Count; RangeIndex++)
                        {
                            string Range_TextItem = __TextSegmented__[RangeIndex];

                            if (Range_TextItem.Equals("/color") | Range_TextItem.StartsWith("color=#")) break;

                            else
                            {
                                if (ICm(Range_TextItem))
                                {
                                    __TextSegmented__[RangeIndex] += $"⟦InnerTag/TextColor@{ColorCode}⟧";
                                }
                            }
                        }
                    }
                }
                #endregion

                #region ⟦InnerTag/TextStyle@Underline⟧
                if (TextItem.Equals("u"))
                {
                    for (int RangeIndex = TextItem_Index + 1; RangeIndex < TextSegmented_Count; RangeIndex++)
                    {
                        string Range_TextItem = __TextSegmented__[RangeIndex];

                        if (Range_TextItem.Equals("/u")) break;
                        if (ICm(Range_TextItem) & !Range_TextItem.Contains("⟦InnerTag/TextStyle@Underline⟧") & !Range_TextItem.Contains("⟦InnerTag/TextStyle@Strikethrough⟧"))
                        {
                            __TextSegmented__[RangeIndex] += "⟦InnerTag/TextStyle@Underline⟧";
                        }
                    }
                }
                #endregion

                #region ⟦InnerTag/TextStyle@Italic⟧
                if (TextItem.Equals("i"))
                {
                    for (int RangeIndex = TextItem_Index + 1; RangeIndex < TextSegmented_Count; RangeIndex++)
                    {
                        string Range_TextItem = __TextSegmented__[RangeIndex];

                        if (Range_TextItem.Equals("/i")) break;
                        if (ICm(Range_TextItem) & !Range_TextItem.Contains("⟦InnerTag/TextStyle@Italic⟧"))
                        {
                            __TextSegmented__[RangeIndex] += $"⟦InnerTag/TextStyle@Italic⟧";
                        }
                    }
                }
                #endregion

                #region ⟦InnerTag/TextStyle@Bold⟧
                if (TextItem.Equals("b"))
                {
                    for (int RangeIndex = TextItem_Index + 1; RangeIndex < TextSegmented_Count; RangeIndex++)
                    {
                        string Range_TextItem = __TextSegmented__[RangeIndex];

                        if (Range_TextItem.Equals("/b")) break;
                        if (ICm(Range_TextItem) & !Range_TextItem.Contains("⟦InnerTag/TextStyle@Bold⟧"))
                        {
                            __TextSegmented__[RangeIndex] += $"⟦InnerTag/TextStyle@Bold⟧";
                        }
                    }
                }
                #endregion

                #region ⟦InnerTag/TextStyle@Strikethrough⟧
                if (TextItem.Equals("strikethrough"))
                {
                    for (int RangeIndex = TextItem_Index + 1; RangeIndex < TextSegmented_Count; RangeIndex++)
                    {
                        string Range_TextItem = __TextSegmented__[RangeIndex];

                        if (Range_TextItem.Equals("/strikethrough")) break;
                        if (ICm(Range_TextItem) & !Range_TextItem.Contains("⟦InnerTag/TextStyle@Strikethrough⟧") & !Range_TextItem.Contains("⟦InnerTag/TextStyle@Underline⟧"))
                        {
                            __TextSegmented__[RangeIndex] += $"⟦InnerTag/TextStyle@Strikethrough⟧";
                        }
                    }
                }
                #endregion

                #region ⟦InnerTag/TextStyle@Subscript⟧
                if (TextItem.Equals("sub"))
                {
                    for (int RangeIndex = TextItem_Index + 1; RangeIndex < TextSegmented_Count; RangeIndex++)
                    {
                        string Range_TextItem = __TextSegmented__[RangeIndex];

                        if (Range_TextItem.Equals("/sub") | Range_TextItem.Equals("sup")) break;
                        if (ICm(Range_TextItem) & !Range_TextItem.Contains("⟦InnerTag/TextStyle@Subscript⟧"))
                        {
                            __TextSegmented__[RangeIndex] += $"⟦InnerTag/TextStyle@Subscript⟧";
                        }
                    }
                }
                #endregion

                #region ⟦InnerTag/TextStyle@Superscript⟧
                if (TextItem.Equals("sup"))
                {
                    for (int RangeIndex = TextItem_Index + 1; RangeIndex < TextSegmented_Count; RangeIndex++)
                    {
                        string Range_TextItem = __TextSegmented__[RangeIndex];

                        if (Range_TextItem.Equals("/sup") | Range_TextItem.Equals("sub")) break;
                        if (ICm(Range_TextItem) & !Range_TextItem.Contains("⟦InnerTag/TextStyle@Superscript⟧"))
                        {
                            __TextSegmented__[RangeIndex] += $"⟦InnerTag/TextStyle@Superscript⟧";
                        }
                    }
                }
                #endregion
            }
            #endregion

            // Очистить сегментированный список текстовых фрагметов от тегов
            __TextSegmented__.RemoveAll(TextItem => TagList.Contains(TextItem) | TextItem.StartsWith("color=#") | TextItem.StartsWith("font=\"") | TextItem.StartsWith("pfont=\"") | TextItem.StartsWith("size="));

            #region Спрайты
            for (int TextItem_Index = 0; TextItem_Index < __TextSegmented__.Count; TextItem_Index++)
            {
                string TextItem = __TextSegmented__[TextItem_Index];

                #region ⟦LevelTag/SpriteLink⟧
                if (TextItem.StartsWith("sprite name=\""))
                {
                    string SpriteLink = TextItem.Split("sprite name=\"")[^1][0..^1];

                    string SpriteKeyword = ":«»";
                    if (ImagesLineBreak)
                    {
                        try
                        {
                            if (TextItem_Index + 1 != __TextSegmented__.Count)
                            {
                                string SpriteKeywordAppend = __TextSegmented__[TextItem_Index + 1].Split(' ')[0];
                                if (!__TextSegmented__[TextItem_Index + 1].StartsWith("sprite name=\""))
                                {
                                    if (!__TextSegmented__[TextItem_Index + 1][0].Equals(" "))
                                    {
                                        bool SpaceAdd = false;
                                        if (__TextSegmented__[TextItem_Index + 1].Split(' ').Count() > 1) SpaceAdd = true;
                                        __TextSegmented__[TextItem_Index + 1] = (SpaceAdd ? " " : "") + string.Join(' ', __TextSegmented__[TextItem_Index + 1].Split(' ')[1..]);

                                        string NextTextItem_InnerTags = "";
                                        Regex InnerTags = new Regex(@"⟦InnerTag/(.*?)⟧");

                                        foreach (Match InnerTagMatch in InnerTags.Matches(__TextSegmented__[TextItem_Index + 1]))
                                        {
                                            NextTextItem_InnerTags += InnerTagMatch;
                                        }

                                        SpriteKeyword = $":«{(!SpriteKeywordAppend.Contains("\n") ? SpriteKeywordAppend : "") + NextTextItem_InnerTags}»";
                                    }
                                }
                            }
                        }
                        catch { }
                    }
                    __TextSegmented__[TextItem_Index] = $"⟦LevelTag/SpriteLink@{SpriteLink}{SpriteKeyword}⟧";
                }
                #endregion
            }
            #endregion

            #endregion

            //rin($"Name: {Target.Name}  —  Font: {Target.FontFamily}");

            #region Debug Preview
            {
                //string ApplySyntax(string s)
                //{
                //    s = s.Replace("⟦", "\x1b[38;5;62m⟦\x1b[0m");
                //    s = s.Replace("⟧", "\x1b[38;5;62m⟧\x1b[0m");
                //    s = s.Replace("@", "\x1b[38;5;62m@\x1b[0m");
                //    s = s.Replace("«", "\x1b[38;5;72m«");
                //    s = s.Replace("»", "\x1b[38;5;72m»\x1b[0m");
                //    s = s.Replace("InnerTag", "\x1b[38;5;203mInnerTag\x1b[0m");
                //    s = s.Replace("LevelTag", "\x1b[38;5;204mLevelTag\x1b[0m");
                //    s = s.Replace("FontFamily", "\x1b[38;5;202mFontFamily\x1b[0m");
                //    s = s.Replace("TextColor", "\x1b[38;5;202mTextColor\x1b[0m");
                //    s = s.Replace("TextStyle", "\x1b[38;5;202mTextStyle\x1b[0m");
                //    s = s.Replace("SpriteLink", "\x1b[38;5;202mSpriteLink\x1b[0m");
                //    return s;
                //}

                //Console.Clear();
                //Console.WriteLine($"Limbus Company TextMeshPro WPF-Mimicry  (\x1b[38;5;62m{Target.Name}\x1b[0m):");
                //int index = 0;
                //List<string> PreviewDebug = new();
                //foreach (var TextItem in __TextSegmented__)
                //{
                //    if (TextItem.StartsWith("⟦LevelTag/SpriteLink"))
                //    {
                //        string Content = Regex.Match(TextItem, @"«(.*?)»").Groups[1].Value;
                //        rin(ApplySyntax($"\n({index}) LevelTag ^\n - Link: «{TextItem.Split("⟦LevelTag/SpriteLink@")[1].Split(":«")[0]}»\n - Content: «`{ClearText(TextItem)}`»\n - Properties: {String.Join("\n               ", InnerTags(TextItem))}").Replace("»", "").Replace("«", ""));
                //    }
                //    else
                //    {
                //        rin(ApplySyntax($"\n({index}) InnerTag ^\n - Content: «`{ClearText(TextItem)}`»\n - Properties: {String.Join("\n               ", InnerTags(TextItem))}").Replace("«", "").Replace("»", ""));
                //    }
                //    index++;
                //}
            }
            #endregion
            
            try
            {
                Target.Document.Blocks.Clear();
            }
            catch { }
            #region Вывод на предпросмотр
            foreach (string TextItem in __TextSegmented__)
            {
                #region Спрайты
                if (TextItem.StartsWith("⟦LevelTag/SpriteLink@") & TextItem.EndsWith('⟧'))
                {
                    var SpriteSet = TextItem.Split(":«");
                    string This_SpriteKeyword = SpriteSet[0].Split("@")[^1].Replace("⟧", "");

                    string This_StickedWord = "";
                    if (SpriteSet.Count() == 2) This_StickedWord = SpriteSet[1][0..^2];

                    if (!KeywordsGlobalDictionary.ContainsKey(This_SpriteKeyword))
                    {
                        This_SpriteKeyword = "Unknown";
                    }

                    InlineImageConstructor Current_SpriteConstructor = new InlineImageConstructor
                    {
                        ImageID = This_SpriteKeyword,
                        TextBase = new InlineTextConstructor
                        {
                            InnerTags = TagManager.InnerTags(This_StickedWord),
                            Text = TagManager.ClearText(This_StickedWord),
                        }
                    };
                    RichText_AppendImage(Current_SpriteConstructor, Target);
                }
                #endregion
                #region Обычный текст
                else
                {
                    InlineTextConstructor Current_TextConstructor = new InlineTextConstructor
                    {
                        InnerTags = TagManager.InnerTags(TextItem),
                        Text = TagManager.ClearText(TextItem)
                    };

                    RichText_AppendText(Current_TextConstructor, Target);
                }
                #endregion
            }
            #endregion
        }
    }
}
