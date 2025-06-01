using Siltcurrent;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using System.Text.RegularExpressions;
using static Siltcurrent.LimbusTranslationExport.ActionsProvider;
using static Siltcurrent.LimbusTranslationExport.Neccessary;
using static Siltcurrent.LimbusTranslationExport.TranslationBuilder;
using static Translation_Devouring_Siltcurrent.NullableControl;
using static Translation_Devouring_Siltcurrent.Requirements;

namespace Translation_Devouring_Siltcurrent;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public static MainWindow MainControl;

    internal protected static double DefaultMinWidth = 0;
    internal protected static double DefaultMinHeight = 0;

    public MainWindow()
    {
        InitializeComponent();
        MainControl = this;

        DefaultMinWidth = this.MinWidth;
        DefaultMinHeight = this.MinHeight;

        Configurazione.PullLoad();

        //taskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Normal;
        //taskbarItemInfo.ProgressValue = 0.5;
        //Process.Start("explorer.exe", @"⇲ Assets Directory");
        //OpenWithDefaultProgram(@"⇲ Assets Directory\Configurazione.json");
        //OpenDirectoryAndSelect(@"file.tx");
    }


    private async void RunExport()
    {
        await LimbusTranslationExport.TranslationBuilder.DirectExport(new ExportParameters
        {
            RawFanmade_LocalizationPath = Configurazione.Settings.Export.RawCustomLocalizationSource,
            Reference_LocalizationPath = Configurazione.Settings.Export.ReferenceLocalizationFiles,
            ReferenceFilesPrefix = Configurazione.Settings.Export.ReferenceLocalizationFilesPrefix,
            OutputDirectory = Configurazione.Settings.Export.ExportedCustomLocalizationDestination,
            ShorthandsPattern = Configurazione.Settings.Export.LoadedShorthandsPattern,
            MergedFontParameters = new MergedFont.FontParameters(Configurazione.Settings.Export.MergedFontParameters),
            Fonts = new FontFiles
            {
                ContextFontPath = Configurazione.Settings.Export.FontFiles.Context,
                TitleFontPath = Configurazione.Settings.Export.FontFiles.Title,
            }
        });
    }

    private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        NewWindowSizes.Rect = new Rect(0, 0, this.Width, this.Height);
    }

    

    private void Window_DragMove(object sender, MouseButtonEventArgs e) => this.DragMove();
    private void Minimize(object sender, MouseButtonEventArgs e) => WindowState = WindowState.Minimized;
    private void Shutdown(object sender, MouseButtonEventArgs e) => Application.Current.Shutdown();

    private void OpenOutputReport_Location(object sender, MouseButtonEventArgs e)
    {
        if (File.Exists(@"Export Records.json"))
        {
            OpenDirectoryAndSelect(@"Export Records.json");
        }
    }
    private void OpenOutputReport_File(object sender, MouseButtonEventArgs e)
    {
        if (File.Exists(@"Export Records.json"))
        {
            OpenWithDefaultProgram(@"Export Records.json");
        }
    }

    private void OpenDestinationDirectory(object sender, MouseButtonEventArgs e)
    {
        if (Directory.Exists(Configurazione.Settings.Export.ExportedCustomLocalizationDestination))
        {
            Process.Start("explorer.exe", Configurazione.Settings.Export.ExportedCustomLocalizationDestination.Replace("/", "\\"));
        }
    }

    private void StringSettings_Toggle_ReferenceLocalizationPath(object sender, MouseButtonEventArgs e)
    {
        StringSettings_ReferenceLocalizationPath.Visibility = StringSettings_ReferenceLocalizationPath.Visibility switch
        {
            Visibility.Hidden => Visibility.Visible,
            _ => Visibility.Hidden
        };
    }
    private void StringSettings_Toggle_ShorthandsPattern(object sender, MouseButtonEventArgs e)
    {
        StringSettings_ShorthandsPattern.Visibility = StringSettings_ShorthandsPattern.Visibility switch
        {
            Visibility.Hidden => Visibility.Visible,
            _ => Visibility.Hidden
        };
    }
    private void StringSettings_Toggle_FontOptions(object sender, MouseButtonEventArgs e)
    {
        StringSettings_FontOptions.Visibility = StringSettings_FontOptions.Visibility switch
        {
            Visibility.Hidden => Visibility.Visible,
            _ => Visibility.Hidden
        };
    }

    private void TextChanged_BGTextAttr(object sender, TextChangedEventArgs e)
    {
        ((sender as TextBox).Parent as Grid).Children[0].Visibility = (sender as TextBox).Text.Equals("") ? Visibility.Visible : Visibility.Collapsed;

        if (!Configurazione.LoadingEvent)
        {
            switch ((sender as TextBox).Name)
            {
                case "TranslationExport_RawFanmade_SourceDirectory":
                    Configurazione.Settings.Export.RawCustomLocalizationSource = TranslationExport_RawFanmade_SourceDirectory.Text;
                    Configurazione.Settings.MarkSerialize(@"⇲ Assets Directory\Configurazione.json");

                    break;

                case "TranslationExport_RawFanmadeOutputDirectory":
                    Configurazione.Settings.Export.ExportedCustomLocalizationDestination = TranslationExport_RawFanmadeOutputDirectory.Text;
                    Configurazione.Settings.MarkSerialize(@"⇲ Assets Directory\Configurazione.json");
                    break;
            }
        }

        if (sender != null & Border_PreviewMouseLeftButtonUp_DisableCover != null)
        {
            if ((sender as TextBox).Name.Equals("TranslationExport_RawFanmade_SourceDirectory"))
            {
                switch (Directory.Exists(TranslationExport_RawFanmade_SourceDirectory.Text))
                {
                    case true:
                        Border_PreviewMouseLeftButtonUp_DisableCover.Visibility = Visibility.Collapsed;
                        break;

                    case false:
                        Border_PreviewMouseLeftButtonUp_DisableCover.Visibility = Visibility.Visible;
                        break;
                }
            }
        }
    }

    private void Border_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        RunExport();
    }

    private void RightMenuSettingsQickChange(object sender, TextChangedEventArgs e)
    {
        if (!Configurazione.LoadingEvent)
        {
            if ((sender as TextBox) != null)
            {
                if ((sender as TextBox).Name != null)
                {
                    switch ((sender as TextBox).Name)
                    {
                        case "ReferenceLocalizationFilePathInput":
                            Configurazione.Settings.Export.ReferenceLocalizationFiles = ReferenceLocalizationFilePathInput.Text;
                            Configurazione.Settings.MarkSerialize(@"⇲ Assets Directory\Configurazione.json");
                            break;


                        case "ShorthandsPatternInput":
                            Configurazione.Settings.Export.ShorthandsPattern = ShorthandsPatternInput.Text;
                            try
                            {
                                Configurazione.Settings.Export.LoadedShorthandsPattern = new Regex(ShorthandsPatternInput.Text);
                            }
                            catch
                            {
                                Configurazione.Settings.Export.LoadedShorthandsPattern = new Regex("");
                            }
                            Configurazione.Settings.MarkSerialize(@"⇲ Assets Directory\Configurazione.json");
                            break;


                        case "MergedFontParametersInput":
                            Configurazione.Settings.Export.MergedFontParameters = MergedFontParametersInput.Text;
                            Configurazione.Settings.MarkSerialize(@"⇲ Assets Directory\Configurazione.json");
                            break;
                    }
                }
            }
        }
    }

    private void ReloadSettings(object sender, MouseButtonEventArgs e)
    {
        Configurazione.PullLoad();
    }
}