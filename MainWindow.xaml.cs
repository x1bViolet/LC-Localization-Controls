using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TexelExtension;
using Configuration;
using GeneralResources;
using RichText;
using LC_Localization_Controls.Converters;

using static TexelExtension.ExternalBase;

namespace LC_Localization_Controls;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public class ReplacePanelData
    {
        public string Shorthand { get; set; }
        public string TMPro { get; set; }
        public bool FirstMatch { get; set; }
        public int MatchIndex { get; set; }
        public string Filename { get; set; }
        public int LineNumber { get; set; }
    }
    public static bool ConfigurationLoadingEvent = true;
    public static Dictionary<string, dynamic> T = new();
    private static bool IsProcessingDirectory = false;

    public static FontFamily GostTypeAU = new FontFamily(new Uri("pack://application:,,,/"), "./Resources/#GOST Type AU");
    public static FontFamily GostTypeBU = new FontFamily(new Uri("pack://application:,,,/"), "./Resources/#GOST Type BU");
    public static FontFamily Unispace = new FontFamily(new Uri("pack://application:,,,/"), "./Resources/#Unispace");
    public static FontFamily UnispaceRUS = new FontFamily(new Uri("pack://application:,,,/"), "./Resources/#Unispace [RUS by Daymarius]");
    public static FontFamily PretendardLight = new FontFamily(new Uri("pack://application:,,,/"), "./Resources/#Pretendard Light");
    public static FontFamily CorbelLight = new FontFamily(new Uri("pack://application:,,,/"), "./Resources/#Corbel Light");
    public static FontFamily MicrosoftJhengHeiLight = new FontFamily(new Uri("pack://application:,,,/"), "./Resources/#Microsoft JhengHei Light");

    public static FontFamily SegoeFluentIcons = new FontFamily(new Uri("pack://application:,,,/"), "./Resources/#Segoe Fluent Icons");

    //public static void SetupFonts()
    //{
    //    TitleButton_Minimize_Text.FontFamily = "";
    //}
    public static Dictionary<string, List<dynamic>> FontConfiguration = new();
    public static Dictionary<string, dynamic> StaticElementLocalization = new();

    public void InitializeSharedDictionaries()
    {
        T = new Dictionary<string, dynamic>
        {
            ["Shorthand pattern text"] = Parameters_SelectedDeltaKeywordsPattern,
            ["Shorthand pattern test string TB"] = Parameters_SelectedDeltaKeywordsPattern_TestString,
            ["Shorthand Converter Selected Original directory"] = DirectorySelect_First_ShorthandConverter,
            ["Shorthand Converter Selected Output directory"] = DirectorySelect_Second_ShorthandConverter,

            ["Full Converter Selected Original directory"] = DirectorySelect_First_FullConverter,
            ["Full Converter Selected Output directory"] = DirectorySelect_Second_FullConverter,
            ["Fonts dropdown list"] = FontsList_DropdownContent,
            ["Fonts content list"] = FontsList_Dropdown,
            ["Selected font display"] = SelectedFontDisplay,
            ["Custom fonts display list"] = CustomFontsPreview,
            ["Custom fonts display list"] = CustomFontsPreview,
            ["Test font string"] = TestFontString,
            ["Selected font info Rules count"] = XD550487,

            ["Full export log"] = FullExport_Log,
            ["Full export scrollviewer"] = XFD9014,

            ["Last export info"] = LastExport_date,
        };

        FontConfiguration = new Dictionary<string, List<dynamic>>
        {
            ["GOST Type BU"] = new List<dynamic>
            {
                XF9045,
                XF9046,
                XF9047,
                XF9048,
                XF9049,
                XF90412,
                XF904,
                XF904122,
                XF904123,
                XF904332,
                XF904213,
                DirectorySelect_First_ShorthandConverter,
                DirectorySelect_Second_ShorthandConverter,
                XF90454,
                XC9123,
                XF189123,
                XF189124,
                XF189654,
                XF189765,
                XF189453,
                XF1203,
                XF11902,
                XF1902,
                SelectedFontDisplay,
                LastExport_date,

                DirectorySelect_First_FullConverter,
                DirectorySelect_Second_FullConverter,
                XDR98,
                XF12318,
                XFD9204,
                XFD9205,
                XFD9206,
                XFD9207,
                XD550485,
                XD550487,

            },
            ["GOST Type AU"] = new List<dynamic>
            {
                XF1083,
                XF1656,
                JsonFilesCounter,
            },
            ["Unispace [RUS by Daymarius]"] = new List<dynamic>
            {
                Parameters_SelectedDeltaKeywordsPattern_TestString_Background,
                TestFontString_bg,
            },
            ["Unispace"] = new List<dynamic>
            {
                Parameters_SelectedDeltaKeywordsPattern_TestString,
                TestFontString,
            },
            ["Pretendard Light"] = new(),
            ["Corbel Light"] = new List<dynamic>
            {
                XFC1001,
                XFC1002,
                XFC1003,
            },
            ["Microsoft JhengHei Light"] = new List<dynamic>
            {
                XCD1020,
                XCD1021,
            }
        };

        StaticElementLocalization = new Dictionary<string, dynamic>
        {
            ["Converters section Header"] = XFC1002,
            ["Shorthand"] = XF9045,
            ["Keyword Links"] = XF9047,
            ["Localization export"] = XF9048,
            ["Parameters section Header"] = XFC1003,
            ["Regex (Par. List)"] = XF9049,
            ["Font (Par. List)"] = XF90412,
            ["Keywords (Par. List)"] = XF904,
            ["Header (Converter 1)"] = XF904122,
            ["Source Localization directory"] = XF904332,
            ["Output Localization directory"] = XF904213,
            ["Template header (Shorthand)"] = XF90454,
            ["Template header (TextMeshPro)"] = XC9123,
            ["Start Button"] = XF0918,
            ["Start Button (Use Font option)"] = XF0912,
            ["Start Button (Use Missing ID Append option)"] = XCFA1290,
            ["Header (Regex)"] = XF189123,
            ["Pattern textfield Header"] = XCD1020,
            ["Pattern Test textfield Header"] = XF189124,
            ["Pattern matches list Header"] = XF189654,
            ["Header (Font)"] = XF189765,
            ["Selection dropdown Header"] = XCD1021,
            ["Font Test textfield Header"] = XF189453,
            ["Font replacement_map.json fonts list Header"] = XF1203,
            ["Font replacement_map.json fonts list Header (Show unicode option)"] = XF1902,
            ["Parameters title (none)"] = XF904123,

            ["Header (Converter 2)"] = XDR98,
            ["Start Export Button"] = XF12318,
            ["Start Export Button (Use Font option)"] = XFD9204,
            ["Start Export Button (Use Shorthand option)"] = XFD9205,
            ["Start Export Button (Do not ignore StoryData option)"] = XFD9206,
            ["Start Export Button (Use Missing ID Append option)"] = XFD9207,
            ["Selected font info Header"] = XD550485,
            ["Selected font info Rules count"] = XD550487,
        };
    }
    public static List<string> KeywordFiles = new()
    {
        "Skills",
        "Passive",
        "EGOgift_",
        "PanicInfo",
        "MentalCondition",
        "BattleKeywords",
        "Bufs",
        "Personalities",
        "Egos"
    };
    public MainWindow()
    {
        InitializeComponent();
        
        StartupInits();
    }

    internal async void ts()
    {
        //double i = 0;
        //while (true)
        //{
        //    XD9044123.RenderTransform = new RotateTransform()
        //    {
        //        Angle = i,
        //    };

        //    i += 0.3;
        //    await Task.Delay(1);

        //}
    }

    private async void StartupInits()
    {
        try { Console.OutputEncoding = Encoding.UTF8; }
        catch { }
        ts();
        DecoTransformLoop();
        InitializeSharedDictionaries();
        AppConfiguration.InitT();
        KSTFont.InitT();
        LocalizationFullExport.InitT();
        //InitSurfaceScroll(DeltaKeywordsMatchesPreview);
        GetDpiAndScale();
        SelectionMenu_Item1Highlight.Background = ToColor("#C9BBB3");

        AppConfiguration.LoadConfiguration();

        foreach(var LangItem in CustomLanguage.Static)
        {
            if (StaticElementLocalization[LangItem.Key] is TextBlock)
            {
                StaticElementLocalization[LangItem.Key].Text = LangItem.Value;
            }
            else
            {
                StaticElementLocalization[LangItem.Key].Content = LangItem.Value;
            }
        }

        Internal.Initialize_KeywordsGlobalDictionary();
        UpdateDeltaKeywordsMatchList();
        X4908.Text = AppConfiguration.ToggleConfiguration["Apply font with shorthands"] ? ""  : "";
        X49908.Text = AppConfiguration.ToggleConfiguration["Enable Unicode on font preview"] ? "" : "";
        Parameters_SelectedDeltaKeywordsPattern_TestString.Text = AppConfiguration.StringConfiguration["Shorthand test string"];

        LastExport_date.Text = AppConfiguration.StringConfiguration["Last Export info"];

        T["Shorthand pattern test string TB"].Text = AppConfiguration.StringConfiguration["Shorthand test string"];

        T["Shorthand Converter Selected Original directory"].Text = AppConfiguration.StringConfiguration["Shorthand Converter Selected Original directory"];
        T["Shorthand Converter Selected Output directory"].Text = AppConfiguration.StringConfiguration["Shorthand Converter Selected Output directory"];

        T["Full Converter Selected Original directory"].Text = AppConfiguration.StringConfiguration["Full Converter Selected Original directory"];
        T["Full Converter Selected Output directory"].Text = AppConfiguration.StringConfiguration["Full Converter Selected Output directory"];

        T["Test font string"].Text = AppConfiguration.StringConfiguration["Selected font test string"];

        XSD1001.Text = AppConfiguration.ToggleConfiguration["Apply Font on Full Export"] switch
        {
            false => "",
            true => "",
        };
        XSD1003.Text = AppConfiguration.ToggleConfiguration["Convert Shorthands on Full Export"] switch
        {
            false => "",
            true => "",
        };
        XSD1002.Text = AppConfiguration.ToggleConfiguration["Don't Ignore StoryData on Full Export"] switch
        {
            false => "",
            true => "",
        };
        XSD1004.Text = AppConfiguration.ToggleConfiguration["Insert missing ID on Full Export"] switch
        {
            false => "",
            true => "",
        };

        //KFont.CopyConvert("По цепи", KFont.KFonts["excelsior"]);

        //ts();
        //await DirectoryFontConverter.DirectExport(@"C:\0x7FE9A.Rijnadel.44980Џ\General\@Localization\Localize Data @ External source", @"C:\Users\javas\OneDrive\Документы\XK\Новая папка", false, true);


        //string s = File.ReadAllText("MainWindow.xaml");
        //string ex = "";
        //Dictionary<string, List<string>> fontss = new();
        //int LineNumber = 1;
        //foreach(var i in s.Split('\n'))
        //{
        //    if (i.Contains("FontFamily=\"Resources\\#"))
        //    {
        //        string fontfamily = Regex.Match(i, @"FontFamily=""Resources\\#(.*?)""").Groups[1].Value;

        //        if (!fontfamily.Equals("Segoe Fluent Icons"))
        //        {
        //            string name = Regex.Match(i, @"x:Name=""(\w+)""").Groups[1].Value;
        //            if (name.Equals("")) rin($"Пусто на {LineNumber}");

        //            if (!fontss.ContainsKey(fontfamily)) fontss[fontfamily] = new();
        //            if (!fontss[fontfamily].Contains(name)) fontss[fontfamily].Add(name);
        //        }
        //    }
        //    LineNumber++;
        //}
        //foreach(var i in fontss)
        //{
        //    ex += $"[\"{i.Key}\"] = new List<dynamic>\n{{\n";
        //    foreach(var e in i.Value)
        //    {
        //        ex += $"    {e},\n";
        //    }
        //    ex += "},\n";
        //}
        //Clipboard.SetText(ex);
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        this.Width = SystemParameters.PrimaryScreenWidth;
        this.Height = SystemParameters.PrimaryScreenHeight - 48;
    }

    private void TitleButton_Close_MouseEnter(object sender, MouseEventArgs e) => (sender as Button).Background = ToColor("#932E2E");
    private void TitleButton_Close_MouseLeave(object sender, MouseEventArgs e) => (sender as Button).Background = ToColor("#1F1E23");
    private void TitleButton_Minimize_MouseEnter(object sender, MouseEventArgs e) => (sender as Button).Background = ToColor("#34323A");
    private void TitleButton_Minimize_MouseLeave(object sender, MouseEventArgs e) => (sender as Button).Background = ToColor("#1F1E23");
    
    private void TitleButton_Close_Click(object sender, RoutedEventArgs e) => Close();
    private void TitleButton_Minimize_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

    internal double scaleX = 100;
    internal double scaleY = 100;
    private void GetDpiAndScale()
    {
        var dpi = VisualTreeHelper.GetDpi(this);

        // Вычисляем масштаб в процентах
        scaleX = (dpi.DpiScaleX * 100);
        scaleY = (dpi.DpiScaleY * 100);


        if (scaleX == 125 & scaleY == 125)
        {
            ConvertGridScale1.ScaleX = 1;
            ConvertGridScale1.ScaleY = 1;

            ConvertGridScale2.ScaleX = 1;
            ConvertGridScale2.ScaleY = 1;
            DeltaKeywordsMatchesPreview.Height = 310;
            Родя.Width = 400;
            
            FonsPreviewSCW.Height = 327;
        }
        else if (scaleX == 100 & scaleY == 100)
        {
            ConvertGridScale1.ScaleX = 1.265;
            ConvertGridScale1.ScaleY = 1.265;

            ConvertGridScale2.ScaleX = 1.265;
            ConvertGridScale2.ScaleY = 1.265;



            DeltaKeywordsMatchesPreview.Height = 470;
            Родя.Width = 500;
            FonsPreviewSCW.Height = 516;
        }
    }

    private void InitSurfaceScroll(ScrollViewer Target)
    {
        Target.Resources.Add(SystemParameters.VerticalScrollBarWidthKey, 0.0);
        Target.PreviewMouseLeftButtonDown += SurfaceScroll_MouseLeftButtonDown;
        Target.PreviewMouseMove           += SurfaceScroll_MouseMove;
        Target.PreviewMouseLeftButtonUp   += SurfaceScroll_MouseLeftButtonUp;
    }
    private bool SurfaceScroll_isDragging = false;
    private Point SurfaceScroll_lastMousePosition;
    private void SurfaceScroll_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        SurfaceScroll_isDragging = true;
        SurfaceScroll_lastMousePosition = e.GetPosition(sender as ScrollViewer);
        (sender as ScrollViewer).CaptureMouse();
    }
    private void SurfaceScroll_MouseMove(object sender, MouseEventArgs e)
    {
        if (SurfaceScroll_isDragging)
        {
            Point currentPosition = e.GetPosition(sender as ScrollViewer);
            Vector diff = SurfaceScroll_lastMousePosition - currentPosition;
            (sender as ScrollViewer).ScrollToVerticalOffset((sender as ScrollViewer).VerticalOffset + diff.Y);
            (sender as ScrollViewer).ScrollToHorizontalOffset((sender as ScrollViewer).HorizontalOffset + diff.X);
            SurfaceScroll_lastMousePosition = currentPosition;
        }
    }
    private void SurfaceScroll_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        SurfaceScroll_isDragging = false;
        (sender as ScrollViewer).ReleaseMouseCapture();
    }

    private void SelectionMenu_Item1Grid_MouseEnter(object sender, MouseEventArgs e) => SelectionMenu_Item1Highlight.Background = ToColor(!SelectedMenu.Equals(1) ? "#5F5C5A" : "#C9BBB3");
    private void SelectionMenu_Item1Grid_MouseLeave(object sender, MouseEventArgs e) => SelectionMenu_Item1Highlight.Background = ToColor(!SelectedMenu.Equals(1) ? "#312B28" : "#C9BBB3");
    
    private void SelectionMenu_Item2Grid_MouseEnter(object sender, MouseEventArgs e) => SelectionMenu_Item2Highlight.Background = ToColor("#312B28");
    //private void SelectionMenu_Item2Grid_MouseEnter(object sender, MouseEventArgs e) => SelectionMenu_Item2Highlight.Background = ToColor(!SelectedMenu.Equals(2) ? "#5F5C5A" : "#C9BBB3");
    private void SelectionMenu_Item2Grid_MouseLeave(object sender, MouseEventArgs e) => SelectionMenu_Item2Highlight.Background = ToColor("#312B28");
    //private void SelectionMenu_Item2Grid_MouseLeave(object sender, MouseEventArgs e) => SelectionMenu_Item2Highlight.Background = ToColor(!SelectedMenu.Equals(2) ? "#312B28" : "#C9BBB3");
    
    private void SelectionMenu_Item3Grid_MouseEnter(object sender, MouseEventArgs e) => SelectionMenu_Item3Highlight.Background = ToColor("#312B28");
    //private void SelectionMenu_Item3Grid_MouseEnter(object sender, MouseEventArgs e) => SelectionMenu_Item3Highlight.Background = ToColor(!SelectedMenu.Equals(3) ? "#5F5C5A" : "#C9BBB3");
    private void SelectionMenu_Item3Grid_MouseLeave(object sender, MouseEventArgs e) => SelectionMenu_Item3Highlight.Background = ToColor("#312B28");
    //private void SelectionMenu_Item3Grid_MouseLeave(object sender, MouseEventArgs e) => SelectionMenu_Item3Highlight.Background = ToColor(!SelectedMenu.Equals(3) ? "#312B28" : "#C9BBB3");
    
    //private void SelectionMenu_Item4Grid_MouseEnter(object sender, MouseEventArgs e) => SelectionMenu_Item4Highlight.Background = ToColor("#312B28");
    private void SelectionMenu_Item4Grid_MouseEnter(object sender, MouseEventArgs e) => SelectionMenu_Item4Highlight.Background = ToColor(!SelectedMenu.Equals(4) ? "#5F5C5A" : "#C9BBB3");
    //private void SelectionMenu_Item4Grid_MouseLeave(object sender, MouseEventArgs e) => SelectionMenu_Item4Highlight.Background = ToColor("#312B28");
    private void SelectionMenu_Item4Grid_MouseLeave(object sender, MouseEventArgs e) => SelectionMenu_Item4Highlight.Background = ToColor(!SelectedMenu.Equals(4) ? "#312B28" : "#C9BBB3");
    
    private void SelectionMenu_Item5Grid_MouseEnter(object sender, MouseEventArgs e) => SelectionMenu_Item5Highlight.Background = ToColor(!SelectedMenu.Equals(5) ? "#5F5C5A" : "#C9BBB3");
    private void SelectionMenu_Item5Grid_MouseLeave(object sender, MouseEventArgs e) => SelectionMenu_Item5Highlight.Background = ToColor(!SelectedMenu.Equals(5) ? "#312B28" : "#C9BBB3");
    
    //private void SelectionMenu_Item6Grid_MouseEnter(object sender, MouseEventArgs e) => SelectionMenu_Item6Highlight.Background = ToColor("#312B28");
    private void SelectionMenu_Item6Grid_MouseEnter(object sender, MouseEventArgs e) => SelectionMenu_Item6Highlight.Background = ToColor(!SelectedMenu.Equals(6) ? "#5F5C5A" : "#C9BBB3");
    //private void SelectionMenu_Item6Grid_MouseLeave(object sender, MouseEventArgs e) => SelectionMenu_Item6Highlight.Background = ToColor("#312B28");
    private void SelectionMenu_Item6Grid_MouseLeave(object sender, MouseEventArgs e) => SelectionMenu_Item6Highlight.Background = ToColor(!SelectedMenu.Equals(6) ? "#312B28" : "#C9BBB3");
    
    private void SelectionMenu_Item7Grid_MouseEnter(object sender, MouseEventArgs e) => SelectionMenu_Item7Highlight.Background = ToColor("#312B28");
    //private void SelectionMenu_Item7Grid_MouseEnter(object sender, MouseEventArgs e) => SelectionMenu_Item7Highlight.Background = ToColor(!SelectedMenu.Equals(7) ? "#5F5C5A" : "#C9BBB3");
    private void SelectionMenu_Item7Grid_MouseLeave(object sender, MouseEventArgs e) => SelectionMenu_Item7Highlight.Background = ToColor("#312B28");
    //private void SelectionMenu_Item7Grid_MouseLeave(object sender, MouseEventArgs e) => SelectionMenu_Item7Highlight.Background = ToColor(!SelectedMenu.Equals(7) ? "#312B28" : "#C9BBB3");


    private async void DecoTransformLoop()
    {
        //#404040
        //#BDBDBD
        while (true)
        {
            if (!IsProcessingDirectory)
            {
                X1043.Foreground = ToColor("#404040");
                await Task.Delay(160);
                X1044.Foreground = ToColor("#404040");

                await Task.Delay(2000);

                X1043.Foreground = ToColor("#BDBDBD");
                await Task.Delay(160);
                X1044.Foreground = ToColor("#BDBDBD");
            }
            else
            {
                X1043.Foreground = ToColor("#BDBDBD");
                X1044.Foreground = ToColor("#BDBDBD");
                await Task.Delay(1000);
            }
        }
    }



    private void Parameters_SelectedDeltaKeywordsPattern_SubFocus1(object sender, MouseButtonEventArgs e)
    {
        Parameters_SelectedDeltaKeywordsPattern.Focus();
        Parameters_SelectedDeltaKeywordsPattern.CaretIndex = 0;
    }

    private void Parameters_SelectedDeltaKeywordsPattern_SubFocus2(object sender, MouseButtonEventArgs e)
    {
        Parameters_SelectedDeltaKeywordsPattern.Focus();
        Parameters_SelectedDeltaKeywordsPattern.CaretIndex = Parameters_SelectedDeltaKeywordsPattern.Text.Length;
    }

    private static string GenerateSyntax_TMPro(string TextMeshPro_NullManaged)
    {
        TextMeshPro_NullManaged = Regex.Replace(
            TextMeshPro_NullManaged,
            @"<(?<sprite_name>sprite name)=(?<id>""\w+"")><(?<color>color)=(?<hexcolor>#[a-fA-F0-9]{6})><(?<u>u)><(?<link>link)=(?<id>""\w+"")>(?<name>.*?)<(?<close_link>/link)><(?<close_u>/u)><(?<close_color>/color)>",
            Match => {
                string sprite_name = $"<color=#05c3ba>{Match.Groups["sprite_name"].Value}</color>";
                string id = $"<color=#98c379>{Match.Groups["id"].Value}</color>";

                string color = $"<color=#05c3ba>{Match.Groups["color"].Value}</color>";
                string close_color = $"<color=#05c3ba>{Match.Groups["close_color"].Value}</color>";
                string hexcolor = Match.Groups["hexcolor"].Value;
                hexcolor = $"<color={hexcolor}>{hexcolor}</color>";

                string u = $"<color=#05c3ba>{Match.Groups["u"].Value}</color>";
                string close_u = $"<color=#05c3ba>{Match.Groups["close_u"].Value}</color>";

                string link = $"<color=#05c3ba>{Match.Groups["link"].Value}</color>";
                string close_link = $"<color=#05c3ba>{Match.Groups["close_link"].Value}</color>";

                string name = $"<color=#abcdef>{Match.Groups["name"].Value}</color>";

                string Export = $"<\0{sprite_name}={id}>" +
                                $"<\0{color}={hexcolor}>" +
                                $"<\0{u}>" +
                                $"<\0{link}={id}>" +
                                $"{name}" +
                                $"<\0{close_link}>" +
                                $"<\0{close_u}>" +
                                $"<\0{close_color}>";

                return Export;
            }
        );

        return TextMeshPro_NullManaged;
    }

    private static string To_TextMeshPro(string DeltaKeyword, string DeltaKeywordsPattern)
    {
        DeltaKeyword = Regex.Replace(DeltaKeyword, DeltaKeywordsPattern, Match =>
        {
            string Export = "";

            string ID = Match.Groups["ID"].Value;
            if (Internal.KeywordsGlobalDictionary.ContainsKey(ID))
            {
                string Name = Match.Groups["Name"].Value;
                string Color = Match.Groups["Color"].Value;
                if (Color.Equals(""))
                {
                    Color = Internal.KeywordsGlobalDictionary[ID].Color;
                }
                else
                {
                    Color = Regex.Match(Color, "(#[a-fA-F0-9]{6})").Value;
                }

                Export = $"<sprite name=\"{ID}\"><color={Color}><u><link=\"{ID}\">{Name}</link></u></color>";
            }
            return Export;
        });

        return DeltaKeyword;
    }

    




    
    private Dictionary<string, List<ReplacePanelData>> WholeReplacementInfo = new();

    internal async Task ConvertFileShorthand(FileInfo ThisFile, string OutDir, bool FirstFile)
    {
        string FileText = File.ReadAllText(ThisFile.FullName);
        int MatchesCount = Regex.Matches(FileText, AppConfiguration.StringConfiguration["Shorthand Pattern"]).Count();

        string CurrentConvertigFile = ThisFile.Name;

        string Repalced = "";
        int Counter = 0;
        bool FileSaved = false;

        List<ReplacePanelData> FileReplacementsData = new();

        Repalced = ConvertSync(FileText, CurrentConvertigFile, ref Counter, ref FileReplacementsData);


        if (AppConfiguration.ToggleConfiguration["Apply font with shorthands"]) Internal.FontReplace.Typematch_Convert(ref Repalced, CurrentConvertigFile);



        string OutName = $@"{OutDir}\{ThisFile.Name}";

        if (!Directory.Exists(OutDir)) Directory.CreateDirectory(OutDir);

        if (File.Exists(OutName))
        {
            if (!File.ReadAllText(OutName).Equals(Repalced))
            {
                try
                {
                    File.WriteAllText(OutName, Repalced);

                    FileSaved = true;
                }
                catch { }
            }
        }
        else
        {
            File.WriteAllText(OutName, Repalced);
            FileSaved = true;
        }

        if (FileSaved)
        {
            WholeReplacementInfo[CurrentConvertigFile] = FileReplacementsData;
            
            StackPanel Re = await GenerateFileHeaderPanel(ThisFile.Name, MatchesCount, FirstFile, true, Counter);

            ShorthandFileReplacesList.Dispatcher.Invoke(() =>
            {
                ShorthandFileReplacesList.Children.Add(Re);
            });
            
        }
        await Task.Delay(1);
    }

    private string ConvertSync(string FileText, string Filename, ref int Counter, ref List<ReplacePanelData> FileReplacementsData)
    {
        bool FirstMatch = true;
        var Matches = Regex.Matches(FileText, AppConfiguration.StringConfiguration["Shorthand Pattern"]);
        List<Match> AlreadlyConverted =  new();
        foreach (Match Match in Matches)
        {
            if (!AlreadlyConverted.Contains(Match))
            {
                string ID = Match.Groups["ID"].Value;
                string Export = Match.Groups[0].Value;

                if (Internal.KeywordsGlobalDictionary.ContainsKey(ID))
                {
                    string Name = Match.Groups["Name"].Value;
                    string Color = Match.Groups["Color"].Value;

                    if (Color.Equals("")) Color = Internal.KeywordsGlobalDictionary[ID].Color;
                    else Color = Regex.Match(Color, "(#[a-fA-F0-9]{6})").Value;

                    Export = $"<sprite name=\\\"{ID}\\\"><color={Color}><u><link=\\\"{ID}\\\">{Name}</link></u></color>";

                    int LineNumber = FileText[0..Match.Index].LinesCount() + 1;

                    ReplacePanelData ReplacementInfoItem = new ReplacePanelData()
                    {
                        Shorthand = Match.Groups[0].Value,
                        TMPro = Export.Replace("\\\"", "\""),
                        FirstMatch = FirstMatch,
                        Filename = Filename,
                        LineNumber = LineNumber
                    };
                    FileReplacementsData.Add(ReplacementInfoItem);


                    FileText = FileText.Replace(Match.Value, Export);
                    Counter++;
                    AlreadlyConverted.Add(Match);
                }
                FirstMatch = false;
            }
        }
        return FileText;
    }


    async Task<StackPanel> GenerateFileHeaderPanel(string Filename, int MatchesCount, bool FirstFile, bool IsOnlyNameDisplay = true, int SuccessfulyReplaced = 0)
    {
        int PanelIndex = ShorthandFileReplacesList.Children.Count;
        StackPanel FileDetailedInfoShowClick = new StackPanel()
        {
            Name = $"Dropdown___{PanelIndex}",
            VerticalAlignment = VerticalAlignment.Bottom,
            Children =
            {
                new TextBlock()
                {
                    FontFamily = SegoeFluentIcons,
                    Foreground = ToColor("#5B6097"),
                    FontSize = 21,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Text = "",
                },
                new Border()
                {
                    Background = ToColor("#5B6097"),
                    Width = IsOnlyNameDisplay ? 0 : 1,
                    Height = 2,
                }
            }
        };
        
        FileDetailedInfoShowClick.PreviewMouseLeftButtonUp += ToggleDropdownConvertInfo;
        FileDetailedInfoShowClick.MouseEnter += ShowDropdownConvertInfo_HighlightEnter;
        FileDetailedInfoShowClick.MouseLeave += ShowDropdownConvertInfo_HighlightLeave;

        var Re = new StackPanel()
        {
            Orientation = Orientation.Horizontal,
            Margin = new Thickness(0, FirstFile ? -1 : 8, 0, 0),
            Children =
            {
                FileDetailedInfoShowClick,
                new TextBlock()
                {
                    Margin = new Thickness(5, 0, 0, 0),
                    Foreground = ToColor("#BDBDBD"),
                    FontSize = 23,
                    FontFamily = GostTypeAU,
                    Text = $"{Filename} :: "
                },
                new TextBlock()
                {
                    Foreground = ToColor("#9B8FD1"),
                    FontSize = 23,
                    FontFamily = GostTypeAU,
                    Text = $"{MatchesCount}"
                },
                new TextBlock()
                {
                    Foreground = ToColor("#BDBDBD"),
                    FontSize = 23,
                    FontFamily = GostTypeAU,
                    Text = CustomLanguage.Dynamic["Converted file panel (Matches count)"]
                },
                new TextBlock()
                {
                    Foreground = ToColor("#9B8FD1"),
                    FontSize = 23,
                    FontFamily = GostTypeAU,
                    Text = SuccessfulyReplaced != 0 ? $", {SuccessfulyReplaced}" : ""
                },
                new TextBlock()
                {
                    Foreground = ToColor("#BDBDBD"),
                    FontSize = 23,
                    FontFamily = GostTypeAU,
                    Text = SuccessfulyReplaced != 0 ? CustomLanguage.Dynamic["Converted file panel (Converted count)"] : ""
                },
            },
        };
        await Task.Delay(1);
        ShorthandConverterFilesPresenter.ScrollToVerticalOffset(ShorthandConverterFilesPresenter.ScrollableHeight);
        return Re;
    }








    



    /// <summary>
    /// /////////////////     НЕ ТРОГАЙ
    /// </summary>
    private void CreateMatchItemPanel(Match MatchInfo, string MatchNumber)
    {
        string OutputTextMeshPro = To_TextMeshPro(MatchInfo.Groups[0].Value, Parameters_SelectedDeltaKeywordsPattern.Text);
        MatchesList.Children.Add(new Border()
        {
            BorderBrush = ToColor("#4C4959"),
            BorderThickness = new Thickness(2),
            Margin = new Thickness(0, MatchNumber.Equals("1") ? 0 : 17, 0, 5),
            CornerRadius = new CornerRadius(10, 1, 1, 1),
            Child = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(-2, 0, 0, 0),
                Children =
                {
                    new StackPanel()
                    {
                        Children =
                        {
                            new StackPanel()
                            {
                                Orientation = Orientation.Horizontal,
                                Children =
                                {
                                    new Border()
                                    {
                                        HorizontalAlignment = HorizontalAlignment.Left,

                                        Background = ToColor("#4C4959"),
                                        CornerRadius = new CornerRadius(10, 0, 5, 0),
                                        Child = new TextBlock()
                                        {
                                            Foreground = ToColor("#C4C4C4"),
                                            FontWeight = FontWeights.Bold,
                                            FontSize = 22,
                                            Margin = new Thickness(8, 0, 5, 0),
                                            FontFamily = GostTypeAU,
                                            Text = CustomLanguage.Dynamic["Match panel Header"].ExTern(MatchNumber)
                                        }
                                    },
                                    new TextBlock()
                                    {
                                        Margin = new Thickness(42, 5, 0, 0),
                                        FontFamily = Unispace,
                                        Foreground = ToColor("#6F6D6D"),
                                        Width = 1290,
                                        Text = MatchInfo.Groups[0].Value
                                    }
                                }
                            },
                            new StackPanel()
                            {
                                Orientation = Orientation.Horizontal,
                                Margin = new Thickness(5, 0, 0, 0),
                                Children =
                                {
                                    new TextBlock()
                                    {
                                        FontFamily = GostTypeBU,
                                        FontSize = 24,
                                        Foreground = ToColor("#BDBDBD"),
                                        Width = 137,
                                        Text = CustomLanguage.Dynamic["TextMeshPro info text"]
                                    },
                                    new Border()
                                    {
                                        Margin = new Thickness(8, 0, 0, 0),
                                        CornerRadius = new CornerRadius(5),
                                        Background = ToColor("#2C2B31"),
                                        Child = RichTextBoxApplicator.GetWithApplied(new RichTextBox()
                                        {

                                            Margin = new Thickness(0, 1.4, 0, 0),
                                            Background = ToColor("#00FFFFFF"),
                                            Width = 1149,
                                            FontSize = 20,
                                            BorderThickness = new Thickness(0),
                                            FontFamily = PretendardLight,
                                            Foreground = ToColor("#BDBDBD"),
                                            Focusable = false,
                                        }, GenerateSyntax_TMPro(OutputTextMeshPro))
                                    }
                                }
                            },
                            new StackPanel()
                            {
                                Orientation = Orientation.Horizontal,
                                Margin = new Thickness(5, 5, 0, 5),
                                Children =
                                {
                                    new TextBlock()
                                    {
                                        FontFamily = GostTypeBU,
                                        FontSize = 24,
                                        Foreground = ToColor("#BDBDBD"),
                                        Width = 137,
                                        Text = CustomLanguage.Dynamic["Game-like preview info text"]
                                    },
                                    new Border()
                                    {
                                        Margin = new Thickness(8, 0, 0, 0),
                                        CornerRadius = new CornerRadius(5),
                                        Background = ToColor("#2C2B31"),
                                        Child = RichTextBoxApplicator.GetWithApplied(new RichTextBox()
                                        {
                                            Margin = new Thickness(0, 1.3, 0, 0),
                                            Background = ToColor("#00FFFFFF"),
                                            Width = 1149,
                                            FontSize = 20,
                                            BorderThickness = new Thickness(0),
                                            FontFamily = PretendardLight,
                                            Foreground = ToColor("#BDBDBD"),
                                            Focusable = false,
                                        }, OutputTextMeshPro, ImageInlineOffset: 7.5)
                                    }
                                }
                            }
                        }
                    }
                }
            }
        });
        //(((((MatchesList.Children[int.Parse(MatchNumber) - 1] as Border).Child
        //    as StackPanel).Children[0]
        //        as StackPanel).Children[1]
        //            as StackPanel).Children[1] as Border).MouseLeftButtonDown += (s, e) => StackPanel_MouseLeftButtonDown(s, e);
    }

    private void UpdateDeltaKeywordsMatchList()
    {
        string TestStringInput = Parameters_SelectedDeltaKeywordsPattern_TestString.Text;
        string Pattern = Parameters_SelectedDeltaKeywordsPattern.Text;
        MatchesList.Children.Clear();

        int Index = 1;
        foreach (Match Item in Regex.Matches(TestStringInput, Pattern))
        {
            string ItemID = Regex.Match(Item.Groups[0].Value, Pattern).Groups["ID"].Value;
            if (Internal.KeywordsGlobalDictionary.ContainsKey(ItemID))
            {
                CreateMatchItemPanel(Item, $"{Index}");
                Index++;
            }
        }
    }

    private void General_UpdateDeltaKeywordsMatchList(object sender, TextChangedEventArgs e)
    {
        try
        {
            if ((sender as TextBox).Name.Equals("Parameters_SelectedDeltaKeywordsPattern_TestString"))
            {
                if (!Parameters_SelectedDeltaKeywordsPattern_TestString.Text.Equals(""))
                {
                    Parameters_SelectedDeltaKeywordsPattern_TestString_Background.Text = "";
                }
                else
                {
                    Parameters_SelectedDeltaKeywordsPattern_TestString_Background.Text = "Строка для проверки";
                }

                AppConfiguration.SaveConfiguration("Shorthand test string", Parameters_SelectedDeltaKeywordsPattern_TestString.Text);
            }
            else
            {
                AppConfiguration.SaveConfiguration("Shorthand Pattern", Parameters_SelectedDeltaKeywordsPattern.Text);
            }

            UpdateDeltaKeywordsMatchList();
        }
        catch { }
    }
    private int SelectedMenu = 1;
    private void General_SwitchMenu(object sender, MouseButtonEventArgs e)
    {
        List<Grid> MenuList = new()
        {
            new Grid(),
            MenuGrid_1,
            MenuGrid_2,
            MenuGrid_3,
            MenuGrid_4,
            MenuGrid_5,
            MenuGrid_6,
            MenuGrid_7,
        };
        List<Grid> MenuButtonsList = new()
        {
            new Grid(),
            SelectionMenu_Item1Grid,
            SelectionMenu_Item2Grid,
            SelectionMenu_Item3Grid,
            SelectionMenu_Item4Grid,
            SelectionMenu_Item5Grid,
            SelectionMenu_Item6Grid,
            SelectionMenu_Item7Grid,
        };
        List<Border> MenuButtonsHighlight = new()
        {
            new Border(),
            SelectionMenu_Item1Highlight,
            SelectionMenu_Item2Highlight,
            SelectionMenu_Item3Highlight,
            SelectionMenu_Item4Highlight,
            SelectionMenu_Item5Highlight,
            SelectionMenu_Item6Highlight,
            SelectionMenu_Item7Highlight,
        };

        string s = (sender as Grid).Name;
        int MenuNumber =  int.Parse($"{s[18]}");

        if (MenuNumber == 1 | MenuNumber == 4 | MenuNumber == 5 | MenuNumber == 6)
        {
            SelectedMenu = MenuNumber;

            for (int i = 1; i <= 7; i++)
            {
                MenuButtonsHighlight[i].Background = ToColor("#312B28");
            }

            foreach(Grid Menu in MenuList)
            {
                Menu.Visibility = Visibility.Collapsed;
            }
            MenuButtonsHighlight[MenuNumber].Background = ToColor("#C9BBB3");
        
            MenuList[MenuNumber].Visibility = Visibility.Visible;
        }
        
    }

    private static bool DirectoryReflectsType(string SearchTarget, List<string> Type)
    {
        DirectoryInfo ThisDirectory = new DirectoryInfo(SearchTarget);
        foreach (var Fileinfo in ThisDirectory.GetFiles("*.json", SearchOption.AllDirectories))
        {
            if (Fileinfo.Name.StartsWithOneOf(Type))
            {
                return true;
            }
        }

        return false;
    }

    private void DirectorySelect_Second_ShorthandConverter_TextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            string SelectedDir = DirectorySelect_Second_ShorthandConverter.Text;
            if (Directory.Exists(SelectedDir))
            {
                if (IsDirectoryWritable(SelectedDir))
                {
                    DirectorySelect_Second_ShorthandConverter_State.Foreground = ToColor("#3072AC");
                    X2039.Width = 0; X2039.Height = 0;
                }
                else
                {
                    DirectorySelect_Second_ShorthandConverter_State.Foreground = ToColor("#932E2E");
                    X2039.Width = double.NaN; X2039.Height = double.NaN;
                    X2039_1.Text = CustomLanguage.Dynamic["Directory Tooltip (Unaccessible)"];
                }
            }
            else
            {
                DirectorySelect_Second_ShorthandConverter_State.Foreground = ToColor("#308DAC");
                X2039.Width = double.NaN; X2039.Height = double.NaN;
                X2039_1.Text = CustomLanguage.Dynamic["Directory Tooltip (Will be created)"];
            }

            AppConfiguration.SaveConfiguration("Shorthand Converter Selected Output directory", SelectedDir);
        }
        catch { }
    }

    private void DirectorySelect_First_ShorthandConverter_TextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            string SelectedDir = DirectorySelect_First_ShorthandConverter.Text;
            if (Directory.Exists(SelectedDir))
            {
                if (Directory.GetDirectories(SelectedDir).Count() <= AppConfiguration.IntegerConfiguration["Original directory Max subdirs"] & 
                    Directory.GetFiles(SelectedDir, "*.*", SearchOption.AllDirectories).Count() <= AppConfiguration.IntegerConfiguration["Original directory Max files"]
                ) {
                    if (DirectoryReflectsType(SelectedDir, KeywordFiles))
                    {
                        //Directory.GetFiles(SelectedDir, "*.*", SearchOption.AllDirectories).Count()//JsonFilesCounter

                        int JsonFilesCount = new DirectoryInfo(SelectedDir).GetFiles("*.json", SearchOption.AllDirectories).Where(file => file.Name.StartsWithOneOf(LocalizationFullExport.KeywordFilesMatch)).Count();
                        JsonFilesCounter.Text = CustomLanguage.Dynamic["Json file count in Source Localization Directory"].ExTern(JsonFilesCount);

                        DirectorySelect_First_ShorthandConverter_State.Foreground = ToColor("#3072AC");
                        X1039.Width = 0; X1039.Height = 0;
                    }
                    else
                    {
                        DirectorySelect_First_ShorthandConverter_State.Foreground = ToColor("#932E2E");
                        X1039.Width = double.NaN; X1039.Height = double.NaN;
                        X1039_1.Text = CustomLanguage.Dynamic["Directory Tooltip (No suggested files found)"];
                        JsonFilesCounter.Text = CustomLanguage.Dynamic["Json file count in Source Localization Directory"].ExTern("-");
                    }
                }
                else
                {
                    DirectorySelect_First_ShorthandConverter_State.Foreground = ToColor("#932E2E");
                    X1039.Width = double.NaN; X1039.Height = double.NaN;
                    X1039_1.Text = CustomLanguage.Dynamic["Directory Tooltip (Too much files or subdirs)"];
                    JsonFilesCounter.Text = CustomLanguage.Dynamic["Json file count in Source Localization Directory"].ExTern("-");
                }
            }
            else
            {
                DirectorySelect_First_ShorthandConverter_State.Foreground = ToColor("#932E2E");
                X1039.Width = double.NaN; X1039.Height = double.NaN;
                X1039_1.Text = CustomLanguage.Dynamic["Directory Tooltip (Not found)"];
                JsonFilesCounter.Text = CustomLanguage.Dynamic["Json file count in Source Localization Directory"].ExTern("-");
            }
            AppConfiguration.SaveConfiguration("Shorthand Converter Selected Original directory", SelectedDir);
        }
        catch { }
    }


    private void DirectorySelect_Second_FullConverter_TextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            string SelectedDir = DirectorySelect_Second_FullConverter.Text;
            if (Directory.Exists(SelectedDir))
            {
                if (IsDirectoryWritable(SelectedDir))
                {
                    DirectorySelect_Second_FullConverter_State.Foreground = ToColor("#3072AC");
                    X6039.Width = 0; X6039.Height = 0;
                }
                else
                {
                    DirectorySelect_Second_FullConverter_State.Foreground = ToColor("#932E2E");
                    X6039.Width = double.NaN; X6039.Height = double.NaN;
                    X6039_1.Text = CustomLanguage.Dynamic["Directory Tooltip (Unaccessible)"];
                }
            }
            else
            {
                DirectorySelect_Second_FullConverter_State.Foreground = ToColor("#308DAC");
                X6039.Width = double.NaN; X6039.Height = double.NaN;
                X6039_1.Text = CustomLanguage.Dynamic["Directory Tooltip (Will be created)"];
            }

            AppConfiguration.SaveConfiguration("Full Converter Selected Output directory", SelectedDir);
        }
        catch { }
    }

    private void DirectorySelect_First_FullConverter_TextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            string SelectedDir = DirectorySelect_First_FullConverter.Text;
            if (Directory.Exists(SelectedDir))
            {
                if (Directory.GetDirectories(SelectedDir).Count() <= AppConfiguration.IntegerConfiguration["Original directory Max subdirs"] &
                    Directory.GetFiles(SelectedDir, "*.*", SearchOption.AllDirectories).Count() <= AppConfiguration.IntegerConfiguration["Original directory Max files"]
                )
                {
                    if (DirectoryReflectsType(SelectedDir, KeywordFiles))
                    {
                        int JsonFilesCount = new DirectoryInfo(SelectedDir).GetFiles("*.json", SearchOption.AllDirectories).Count();
                        JsonFilesCounter_FullExport.Text = CustomLanguage.Dynamic["Json file count in Source Localization Directory"].ExTern(JsonFilesCount);

                        DirectorySelect_First_FullConverter_State.Foreground = ToColor("#3072AC");
                        X5039.Width = 0; X5039.Height = 0;
                    }
                    else
                    {
                        DirectorySelect_First_FullConverter_State.Foreground = ToColor("#932E2E");
                        X5039.Width = double.NaN; X5039.Height = double.NaN;
                        X5039_1.Text = CustomLanguage.Dynamic["Directory Tooltip (No suggested files found)"];
                        JsonFilesCounter_FullExport.Text = CustomLanguage.Dynamic["Json file count in Source Localization Directory"].ExTern("-");
                    }
                }
                else
                {
                    DirectorySelect_First_FullConverter_State.Foreground = ToColor("#932E2E");
                    X5039.Width = double.NaN; X5039.Height = double.NaN;
                    X5039_1.Text = CustomLanguage.Dynamic["Directory Tooltip (Too much files or subdirs)"];
                    JsonFilesCounter_FullExport.Text = CustomLanguage.Dynamic["Json file count in Source Localization Directory"].ExTern("-");
                }
            }
            else
            {
                DirectorySelect_First_FullConverter_State.Foreground = ToColor("#932E2E");
                X5039.Width = double.NaN; X5039.Height = double.NaN;
                X5039_1.Text = CustomLanguage.Dynamic["Directory Tooltip (Not found)"];
                JsonFilesCounter_FullExport.Text = CustomLanguage.Dynamic["Json file count in Source Localization Directory"].ExTern("-");
            }
            AppConfiguration.SaveConfiguration("Full Converter Selected Original directory", SelectedDir);
        }
        catch { }
    }




    private void ShorthandConverter_AddMissingID_Toggle(object sender, MouseButtonEventArgs e)
    {
        AppConfiguration.ToggleConfiguration["Apply missing id with shorthands"] = !AppConfiguration.ToggleConfiguration["Apply missing id with shorthands"];
        AppConfiguration.SaveConfiguration("Apply missing id with shorthands", AppConfiguration.ToggleConfiguration["Apply missing id with shorthands"] ? "Yes" : "No");
        X6908.Text = !AppConfiguration.ToggleConfiguration["Apply missing id with shorthands"] ? "" : "";
    }
    private void ShorthandConverter_FontApply_Toggle(object sender, MouseButtonEventArgs e)
    {
        AppConfiguration.ToggleConfiguration["Apply font with shorthands"] = !AppConfiguration.ToggleConfiguration["Apply font with shorthands"];
        AppConfiguration.SaveConfiguration("Apply font with shorthands", AppConfiguration.ToggleConfiguration["Apply font with shorthands"]? "Yes" : "No");
        X4908.Text = !AppConfiguration.ToggleConfiguration["Apply font with shorthands"] ? "" : "";
    }

    private async void StartConvertShorthands(object sender, MouseButtonEventArgs e)
    {
        StartConvertingButton1.Visibility = Visibility.Visible;
        StartConvertingButton2.Visibility = Visibility.Visible;
        IsProcessingDirectory = true;

        ShorthandFileReplacesList.Children.Clear();
        WholeReplacementInfo.Clear();
        RegisteredExpandDetailInfo.Clear();

        DirectoryInfo OriginalDirectory = new DirectoryInfo(AppConfiguration.StringConfiguration["Shorthand Converter Selected Original directory"]);
        string OutputDirectory = AppConfiguration.StringConfiguration["Shorthand Converter Selected Output directory"];
       
        bool FirstFile = true;
        List<string> AlreadyConverted = new();
        ShorthandFileReplacesList.Children.Clear();
        foreach (var ConvertingFile in OriginalDirectory.GetFiles("*.json", SearchOption.AllDirectories))
        {
            if (ConvertingFile.Name.StartsWithOneOf(KeywordFiles) & !AlreadyConverted.Contains(ConvertingFile.FullName))
            {
                await ConvertFileShorthand(ConvertingFile, OutputDirectory, FirstFile);
                FirstFile = false;
                AlreadyConverted.Add(ConvertingFile.FullName);
            }
        }

        StartConvertingButton1.Visibility = Visibility.Collapsed;
        StartConvertingButton2.Visibility = Visibility.Collapsed;
        IsProcessingDirectory = false;

        try
        {
            LastExport_date.Text = CustomLanguage.Dynamic["Last export bottom info"].Exform(DateTime.Now.ToString(CustomLanguage.Dynamic["Last export bottom info (Date format)"]), OutputDirectory);

            AppConfiguration.SaveConfiguration("Last Export info", LastExport_date.Text);
        }
        catch { }
    }




    internal Dictionary<string, int> RegisteredExpandDetailInfo = new();

    async Task<StackPanel> Generate(List<ReplacePanelData> ConvertedInfoList)
    {
        StackPanel OutAdd = new();
        foreach (var ConvertedInfo in ConvertedInfoList)
        {
            OutAdd.Children.Add(new StackPanel()
            {
                Visibility = Visibility.Visible,
                Height = 35.4,
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, ConvertedInfo.FirstMatch ? 0 : 15, 0, 0),
                Children =
                {
                    new Border()
                    {
                        Width = 10,
                        Height =  ConvertedInfo.FirstMatch ? 20 : 60,
                        Margin = new Thickness(10, ConvertedInfo.FirstMatch ? -2 : -41, 0, 0),
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        BorderBrush = ToColor("#5B6097"),
                        BorderThickness = new Thickness(1, 0, 0, 1),
                        CornerRadius = new CornerRadius(0, 0, 0, 4.1),
                    },
                    new Border()
                    {
                        Width = 1,
                        Margin = new Thickness(0, 2, 0, 2),
                        CornerRadius = new CornerRadius(0.5),
                        Background = ToColor("#5B6097")
                    },
                    new StackPanel()
                    {
                        Children =
                        {
                            new StackPanel()
                            {
                                Orientation = Orientation.Horizontal,
                                Height = 16.5,
                                Children =
                                {
                                    new TextBlock()
                                    {
                                        Margin = new Thickness(2, 0, 0, 0),
                                        FontFamily = SegoeFluentIcons,
                                        FontSize = 14,
                                        Height = 11.37,
                                        Foreground = ToColor("#ACB0D4"),
                                        Text = "",
                                    },

                                    RichTextBoxApplicator.GetWithApplied(
                                        new RichTextBox()
                                        {
                                            Focusable = false,
                                            Background = ToColor("#00FFFFFF"),
                                            FontSize = 12.7,
                                            BorderThickness = new Thickness(0),
                                            Width = 890,
                                            Foreground = ToColor("#BDBDBD"),
                                            FontFamily = GostTypeBU,
                                            Height = 16,
                                    },  $"{ConvertedInfo.Shorthand} <size=74%><pfont=\"Segoe Fluent Icons\"></font></size> <pfont=\"Pretendard Light\">{ConvertedInfo.TMPro}</font> {CustomLanguage.Dynamic["Converted keyword panel (Line number)"].ExTern(ConvertedInfo.LineNumber)}", ImageInlineOffset: 6.5)
                                }
                            },
                            new StackPanel()
                            {
                                Orientation = Orientation.Horizontal,
                                Height = 16.5,
                                Margin = new Thickness(0, 2.5, 0, 0),
                                Children =
                                {
                                    new TextBlock()
                                    {
                                        Margin = new Thickness(2, 0, 0, 0),
                                        FontFamily = SegoeFluentIcons,
                                        FontSize = 14,
                                        Height = 11.37,
                                        Foreground = ToColor("#ACB0D4"),
                                        Text = "",
                                    },
                                    RichTextBoxApplicator.GetWithApplied(
                                        new RichTextBox()
                                        {
                                            Focusable = false,
                                            Background = ToColor("#00FFFFFF"),
                                            FontSize = 12.7,
                                            BorderThickness = new Thickness(0),
                                            Width = 890,
                                            Foreground = ToColor("#BDBDBD"),
                                            FontFamily = GostTypeBU,
                                            Height = 15,
                                    },  GenerateSyntax_TMPro(ConvertedInfo.TMPro))
                                }
                            }
                        }
                    }
                }
            });
        }
        await Task.Delay(1);
        return OutAdd;
    }

    internal void IncreaseRegisteredIndexes(int InsertedIndex)
    {
        foreach(var Index in RegisteredExpandDetailInfo)
        {
            if (Index.Value > InsertedIndex) RegisteredExpandDetailInfo[Index.Key] = Index.Value + 1;
        }
    }

    internal async Task влфыьвйзщцу(StackPanel? Sender)
    {
        string SelectedFilename = ((Sender.Parent as StackPanel).Children[1] as TextBlock).Text.Split(" ::")[0];

        if (!RegisteredExpandDetailInfo.ContainsKey(SelectedFilename))
        {
            int AppendIndex = ShorthandFileReplacesList.Children.IndexOf(Sender.Parent as StackPanel) + 1;
            var Stp = await Generate(WholeReplacementInfo[SelectedFilename]);
            if (AppendIndex == -1)
            {
                AppendIndex = ShorthandFileReplacesList.Children.Count - 1;
            }

            RegisteredExpandDetailInfo[WholeReplacementInfo[SelectedFilename][0].Filename] = AppendIndex;
            ShorthandFileReplacesList.Dispatcher.Invoke(() =>
            {
                ShorthandFileReplacesList.Children.Insert(AppendIndex, Stp);
            });
            IncreaseRegisteredIndexes(AppendIndex);
        }
        else
        {
            if (ShorthandFileReplacesList.Children[RegisteredExpandDetailInfo[SelectedFilename]].Visibility.Equals(Visibility.Visible))
            {
                ShorthandFileReplacesList.Children[RegisteredExpandDetailInfo[SelectedFilename]].Visibility = Visibility.Collapsed;
            }
            else
            {
                ShorthandFileReplacesList.Children[RegisteredExpandDetailInfo[SelectedFilename]].Visibility = Visibility.Visible;
            }
        }
        await Task.Delay(1);
    }

    internal async void ToggleDropdownConvertInfo(object sender, MouseButtonEventArgs e)
    {
        if (!IsProcessingDirectory)
        {
            StackPanel? Sender = sender as StackPanel;

            await влфыьвйзщцу(Sender);
        }
    }

    private void ShowDropdownConvertInfo_HighlightEnter(object sender, MouseEventArgs e)
    {
        if (!IsProcessingDirectory)
        {
            try
            {
                ((sender as StackPanel).Children[0] as TextBlock).Foreground = ToColor("#868BC1");
                ((sender as StackPanel).Children[0] as TextBlock).Margin = new Thickness(1.24, 0, 0, 0);

                (((sender as StackPanel).Parent as StackPanel).Children[1] as TextBlock).Margin = new Thickness(3.76, 0, 0, 0);

                ((sender as StackPanel).Children[0] as TextBlock).Text = "";
            }
            catch { }
        }
    }
    private void ShowDropdownConvertInfo_HighlightLeave(object sender, MouseEventArgs e)
    {
        if (!IsProcessingDirectory)
        {
            try
            {
                ((sender as StackPanel).Children[0] as TextBlock).Foreground = ToColor("#5B6097");
                ((sender as StackPanel).Children[0] as TextBlock).Margin = new Thickness(0);

                (((sender as StackPanel).Parent as StackPanel).Children[1] as TextBlock).Margin = new Thickness(5, 0, 0, 0);

                ((sender as StackPanel).Children[0] as TextBlock).Text = "";
            }
            catch { }
        }
    }

    private void FontsList_DropdownButton_MouseEnter(object sender, MouseEventArgs e)
    {
        (sender as TextBlock).Foreground = ToColor("#8786FF");
    }

    private void FontsList_DropdownButton_MouseLeave(object sender, MouseEventArgs e)
    {
        (sender as TextBlock).Foreground = ToColor("#B4B4BE");
    }

    private void FontsList_DropdownButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        FontsList_Dropdown.Visibility = FontsList_Dropdown.Visibility switch
        {
            Visibility.Collapsed => Visibility.Visible,
            Visibility.Visible   => Visibility.Collapsed,
            _ => Visibility.Collapsed
        };
    }

    private void UpdateFontsPreview(object sender, TextChangedEventArgs e)
    {
        TestFontString_bg.Text = TestFontString.Text switch
        {
            "" => "Строка для проверки",
            _  => ""
        };

        KSTFont.GenerateCustomFontsPreview(TestFontString.Text, AppConfiguration.ToggleConfiguration["Enable Unicode on font preview"]);
        AppConfiguration.SaveConfiguration("Selected font test string", TestFontString.Text);
    }

    private void ToggleCustomFontUnicode(object sender, MouseButtonEventArgs e)
    {
        AppConfiguration.ToggleConfiguration["Enable Unicode on font preview"] = !AppConfiguration.ToggleConfiguration["Enable Unicode on font preview"];
        AppConfiguration.SaveConfiguration("Enable Unicode on font preview", AppConfiguration.ToggleConfiguration["Enable Unicode on font preview"] ? "Yes" : "No");

        KSTFont.GenerateCustomFontsPreview(TestFontString.Text, AppConfiguration.ToggleConfiguration["Enable Unicode on font preview"]);

        X49908.Text = AppConfiguration.ToggleConfiguration["Enable Unicode on font preview"] switch
        {
            false => "",
            true  => "",
        };
    }

    private async void ExportLocalization(object sender, MouseButtonEventArgs e)
    {
        await LocalizationFullExport.DirectExport(
            AppConfiguration.StringConfiguration["Full Converter Selected Original directory"],
            AppConfiguration.StringConfiguration["Full Converter Selected Output directory"],
            AppConfiguration.ToggleConfiguration["Apply Font on Full Export"],
            AppConfiguration.ToggleConfiguration["Convert Shorthands on Full Export"],
            AppConfiguration.ToggleConfiguration["Don't Ignore StoryData on Full Export"],
            AppConfiguration.ToggleConfiguration["Insert missing ID on Full Export"]
        );
    }

    private void FullExportToggle_ApplyFont(object sender, MouseButtonEventArgs e)
    {
        AppConfiguration.ToggleConfiguration["Apply Font on Full Export"] = !AppConfiguration.ToggleConfiguration["Apply Font on Full Export"];
        AppConfiguration.SaveConfiguration("Apply Font on Full Export", AppConfiguration.ToggleConfiguration["Apply Font on Full Export"] ? "Yes" : "No");
        XSD1001.Text = AppConfiguration.ToggleConfiguration["Apply Font on Full Export"] switch
        {
            false => "",
            true => "",
        };
    }

    private void FullExportToggle_ApplyShorthand(object sender, MouseButtonEventArgs e)
    {
        AppConfiguration.ToggleConfiguration["Convert Shorthands on Full Export"] = !AppConfiguration.ToggleConfiguration["Convert Shorthands on Full Export"];
        AppConfiguration.SaveConfiguration("Convert Shorthands on Full Export", AppConfiguration.ToggleConfiguration["Convert Shorthands on Full Export"] ? "Yes" : "No");
        XSD1003.Text = AppConfiguration.ToggleConfiguration["Convert Shorthands on Full Export"] switch
        {
            false => "",
            true => "",
        };
    }

    private void FullExportToggle_DoNotIgnoreStoryData(object sender, MouseButtonEventArgs e)
    {
        AppConfiguration.ToggleConfiguration["Don't Ignore StoryData on Full Export"] = !AppConfiguration.ToggleConfiguration["Don't Ignore StoryData on Full Export"];
        AppConfiguration.SaveConfiguration("Don't Ignore StoryData on Full Export", AppConfiguration.ToggleConfiguration["Don't Ignore StoryData on Full Export"] ? "Yes" : "No");
        XSD1002.Text = AppConfiguration.ToggleConfiguration["Don't Ignore StoryData on Full Export"] switch
        {
            false => "",
            true => "",
        };
    }

    private void FullExportToggle_InsertMissingID(object sender, MouseButtonEventArgs e)
    {
        AppConfiguration.ToggleConfiguration["Insert missing ID on Full Export"] = !AppConfiguration.ToggleConfiguration["Insert missing ID on Full Export"];
        AppConfiguration.SaveConfiguration("Insert missing ID on Full Export", AppConfiguration.ToggleConfiguration["Insert missing ID on Full Export"] ? "Yes" : "No");
        XSD1004.Text = AppConfiguration.ToggleConfiguration["Insert missing ID on Full Export"] switch
        {
            false => "",
            true => "",
        };
    }
}