using Microsoft.Win32;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using static Translation_Devouring_Siltcurrent.Requirements;

namespace Translation_Devouring_Siltcurrent
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow MainControl;

        public ref struct @ProfileSelection
        {
            public static FileStream ConfigurationProfileLockerStream;

            public static FileSystemWatcher ProfilesWatcher;
            public static ObservableCollection<string> ProfilesObservableList = new();
            public static string ProfilesPath = @"[⇲] Assets Directory\Configuration profiles";

            public static void SetFielSystemWatcher()
            {
                ProfilesWatcher = new FileSystemWatcher(ProfilesPath, "*.json")
                {
                    NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite
                };

                ProfilesWatcher.Created += OnChanged;
                ProfilesWatcher.Deleted += OnChanged;
                ProfilesWatcher.Renamed += OnRenamed;
                ProfilesWatcher.EnableRaisingEvents = true;
            }

            private static void OnChanged(object RequestSender, FileSystemEventArgs EventArgs)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var filename = Path.GetFileName(EventArgs.FullPath);
                    if (EventArgs.ChangeType == WatcherChangeTypes.Created)
                    {
                        ProfilesObservableList.Add(filename);
                    }
                    else if (EventArgs.ChangeType == WatcherChangeTypes.Deleted) ProfilesObservableList.Remove(filename);
                });
            }

            private static void OnRenamed(object RequestSender, RenamedEventArgs EventArgs)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var oldName = Path.GetFileName(EventArgs.OldFullPath);
                    var newName = Path.GetFileName(EventArgs.FullPath);
                    if (ProfilesObservableList.Contains(oldName))
                    {
                        int index = ProfilesObservableList.IndexOf(oldName);
                        ProfilesObservableList[index] = newName;
                    }
                });
            }
        }

        public MainWindow()
        {
            try { Console.OutputEncoding = Encoding.UTF8; }
            catch { }

            InitializeComponent();
            MainControl = this;

            SourceDirectoryInput.Text = "dummy to trigger TextChanged \0";

            Configurazione.CurrentOptions = JsonConvert.DeserializeObject<GeneralOptions>(File.ReadAllText(@"[⇲] Assets Directory\General Options.json"));
            ShowHintsCheckBox.IsChecked = Configurazione.CurrentOptions.ShowParameterRemarks;

            #region File watcher
            ConfigurationProfileSelector.ItemsSource = @ProfileSelection.ProfilesObservableList;
            foreach (FileInfo ConfigurationProfileJson in new DirectoryInfo(@ProfileSelection.ProfilesPath).GetFiles())
            {
                @ProfileSelection.ProfilesObservableList.Add(ConfigurationProfileJson.Name);
                if (ConfigurationProfileJson.Name == Configurazione.CurrentOptions.SelectedConfigurationProfile)
                {
                    ConfigurationProfileSelector.SelectedItem = ConfigurationProfileJson.Name;
                }
            }
            @ProfileSelection.SetFielSystemWatcher();
            #endregion

            try { ReloadThemeFile(null, null); }
            catch { }
        }

        private void ReloadThemeFile(object RequestSender, RoutedEventArgs EventArgs)
        {
            if (File.Exists(@"[⇲] Assets Directory\UI Colors.xaml"))
            {
                using (FileStream Stream = new FileStream(@"[⇲] Assets Directory\UI Colors.xaml", FileMode.Open))
                {
                    this.Resources.MergedDictionaries[0] = (ResourceDictionary)XamlReader.Load(Stream);
                }
            }
        }

        private void CheckForStartButtonAvailability(object RequestSender, TextChangedEventArgs EventArgs)
        {
            StartButton.IsEnabled = Directory.Exists(SourceDirectoryInput.Text);
        }

        private async void SaveCurrentConfigurationProfile(object RequestSender, RoutedEventArgs EventArgs)
        {
            SaveFileDialog Dialog = NewSaveFileDialog(".json files", ["json"], "NEW Configuration profile", @$"{Path.GetDirectoryName(Environment.ProcessPath)}\[⇲] Assets Directory\Configuration profiles");
            if (Dialog.ShowDialog() == true)
            {
                @ProfileSelection.ConfigurationProfileLockerStream.Dispose();
                Configurazione.CurrentProfile.SerializeToFormattedFile(Dialog.FileName);
                await Task.Delay(500);
                ConfigurationProfileSelector.SelectedItem = Dialog.SafeFileName;
            }
        }


        private void ConfigurationProfileSelector_SelectionChanged(object RequestSender, SelectionChangedEventArgs EventArgs)
        {
            if (ConfigurationProfileSelector.SelectedItem != null)
            {
                LoadCurrentConfigurationProfile(ConfigurationProfileSelector.SelectedItem as string);
            }
        }
        private void LoadCurrentConfigurationProfile(string FileName)
        {
            @ProfileSelection.ConfigurationProfileLockerStream?.Dispose();
            Configurazione.CurrentProfile = JsonConvert.DeserializeObject<ConfigurationProfile>(File.ReadAllText(@$"[⇲] Assets Directory\Configuration profiles\{FileName}"));
            Configurazione.CurrentOptions.SelectedConfigurationProfile = FileName;
            Configurazione.CurrentOptions.SerializeToFormattedFile(@"[⇲] Assets Directory\General Options.json");
            this.DataContext = Configurazione.CurrentProfile;
            @ProfileSelection.ConfigurationProfileLockerStream = new FileStream(@$"[⇲] Assets Directory\Configuration profiles\{FileName}", FileMode.Open, FileAccess.Read, FileShare.Read);
        }





        private bool AlmostMaximized = false;
        private bool LockedOnlyNow = false;
        private double PreviousWidth = double.NaN;
        private double PreviousHeight = double.NaN;

        private void Window_SizeChanged(object RequestSender, SizeChangedEventArgs EventArgs)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
                SwitchToAlmostMaximized();
            }
        }

        private void SwitchToAlmostMaximized()
        {
            this.Left = -1.5;
            this.Top = -1.5;
            PreviousWidth = this.Width;
            PreviousHeight = this.Height;
            this.Width = SystemParameters.WorkArea.Width + 2.5;
            this.Height = SystemParameters.WorkArea.Height + 2.5;
            this.CurrentWindowChrome.ResizeBorderThickness = UIBorder.BorderThickness = new Thickness(0);

            AlmostMaximized = true;
            LockedOnlyNow = true;
        }

        private void ExitFromAlmostMaximized()
        {
            this.Width = PreviousWidth;
            this.Height = PreviousHeight;

            UIBorder.BorderThickness = new Thickness(2);
            this.CurrentWindowChrome.ResizeBorderThickness = new Thickness(5, 10, 5, 10);

            AlmostMaximized = false;
        }

        private void TitleBarButtons_MouseEnter(object sender, MouseEventArgs e)
        {
            this.CurrentWindowChrome.ResizeBorderThickness = new Thickness(0);
        }

        private void TitleBarButtons_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!AlmostMaximized) this.CurrentWindowChrome.ResizeBorderThickness = new Thickness(5, 10, 5, 10);
        }





        private void Window_DragMove(object RequestSender, MouseButtonEventArgs EventArgs)
        {
            this.DragMove();

            if (AlmostMaximized)
            {
                if (LockedOnlyNow)
                {
                    LockedOnlyNow = false;
                }
                else
                {
                    ExitFromAlmostMaximized();
                }
            }
        }

        private void Minimize(object RequestSender, RoutedEventArgs EventArgs) => WindowState = WindowState.Minimized;

        private void Maximize(object RequestSender, RoutedEventArgs EventArgs)
        {
            if (!AlmostMaximized)
            {
                SwitchToAlmostMaximized();
            }
            else
            {
                ExitFromAlmostMaximized();
                this.Left = (SystemParameters.WorkArea.Width - this.Width) / 2;
                this.Top = (SystemParameters.WorkArea.Height - this.Height) / 2;
            }
        }
        private void Shutdown(object RequestSender, RoutedEventArgs EventArgs) => Application.Current.Shutdown();





        private void SwitchSection(object RequestSender, MouseButtonEventArgs EventArgs)
        {
            _ = SectionSwitch_Parameters.IsSelected
              = SectionSwitch_TranslationExport.IsSelected
              = false;

            (RequestSender as TextBlockSelectable).IsSelected = true;

            Sections.SelectedIndex = int.Parse((RequestSender as TextBlock).DataContext.ToString());
        }

        private void ToggleSettingsHints(object RequestSender, RoutedEventArgs EventArgs)
        {
            bool IsChecked = (bool)(RequestSender as CheckBox).IsChecked;

            foreach (HintTextBlock Hint in HintTextBlock.CreatedHints)
            {
                Hint.Visibility = IsChecked ? Visibility.Visible : Visibility.Collapsed;
            }

            Configurazione.CurrentOptions.ShowParameterRemarks = IsChecked;
            Configurazione.CurrentOptions.SerializeToFormattedFile(@"[⇲] Assets Directory\General Options.json");
        }

        private void Hint_10_MouseLeftButtonUp(object RequestSender, MouseButtonEventArgs EventArgs)
        {
            Process.Start(new ProcessStartInfo()
            {
                FileName = "https://github.com/kimght/LimbusFonts", /*URL*/
                UseShellExecute = true
            });
        }





        private async void RunTranslationProcessing(object RequestSender, RoutedEventArgs EventArgs) => await LimbusTranslationProcessor.DoDirectExport();

        private void OpenDestinationDirectory(object RequestSender, MouseButtonEventArgs EventArgs)
        {
            if (Directory.Exists(Configurazione.CurrentProfile.Paths.DestinationDirectory))
            {
                Process.Start("explorer.exe", Configurazione.CurrentProfile.Paths.DestinationDirectory.Replace("/", "\\"));
            }
        }

        private void OpenReportLocation(object RequestSender, MouseButtonEventArgs EventArgs)
        {
            if (File.Exists(@"Export Records.json")) OpenDirectoryAndSelect(@"Export Records.json");
        }

        private void OpenReportFile(object RequestSender, RoutedEventArgs EventArgs)
        {
            if (File.Exists(@"Export Records.json"))
            {
                OpenWithDefaultProgram(@"Export Records.json");
            }
        }





        private void TextBoxShadowable_CheckPathExistance(object RequestSender, TextChangedEventArgs EventArgs)
        {
            TextBoxShadowable ActualSender = (RequestSender as TextBoxShadowable);
            if (ActualSender.PathExistanceCheck != TextBoxShadowable.PathExistanceCheckTypeEnum.None)
            {
                string Path = ActualSender.Text.Trim();
                bool Exists = ActualSender.PathExistanceCheck == TextBoxShadowable.PathExistanceCheckTypeEnum.File
                    ? File.Exists(Path)
                    : Directory.Exists(Path);

                bool RedHighlightDecision = Exists | (Path == "" & ActualSender.AnyPathRequired == false); // "" = no file or path
                ActualSender.BorderBrush = ToSolidColorBrush(RedHighlightDecision ? "#34333D" : "#782F2F");
                ActualSender.ToolTip = RedHighlightDecision ? null : $"This {ActualSender.PathExistanceCheck.ToString().ToLower()} doesn't exists";
            }
        }
        private void TextBoxShadowable_SelectDirectory(object RequestSender, RoutedEventArgs EventArgs)
        {
            OpenFolderDialog Dialog = new OpenFolderDialog();
            if (Dialog.ShowDialog() == true)
            {
                ((RequestSender as Button).DataContext as TextBoxShadowable).Text = Dialog.FolderName.Del(Path.GetDirectoryName(Environment.ProcessPath) + "\\").Replace("\\", "/");
            }
        }
        private void TextBoxShadowable_SelectFile(object RequestSender, RoutedEventArgs EventArgs)
        {
            TextBoxShadowable Sender = (RequestSender as Button).DataContext as TextBoxShadowable;
            OpenFileDialog Dialog = NewOpenFileDialog(
                Sender.FileSelectAttributes.FilesHint,
                Extensions: Sender.FileSelectAttributes.Extensions.Split(", ")
            );
            if (Dialog.ShowDialog() == true)
            {
                Sender.Text = Dialog.FileName.Del(Path.GetDirectoryName(Environment.ProcessPath) + "\\").Replace("\\", "/");
            }
        }
    }





    public class HintTextBlock : TextBlock
    {
        public static List<HintTextBlock> CreatedHints = [];
        public HintTextBlock() => CreatedHints.Add(this);
    }

    public class TextBlockSelectable : TextBlock
    {
        public static readonly DependencyProperty IsSelectedProperty = Register<TextBlockSelectable, bool>(Name: nameof(IsSelected), DefaultValue: false);
        public bool IsSelected { get => (bool)GetValue(IsSelectedProperty); set => SetValue(IsSelectedProperty, value); }
    }

    public class TextBoxShadowable : TextBox
    {
        public static readonly DependencyProperty ShadowTextProperty = Register<TextBoxShadowable, string>(Name: nameof(ShadowText), DefaultValue: "");
        public string ShadowText { get => (string)GetValue(ShadowTextProperty); set => SetValue(ShadowTextProperty, value); }
        

        public static readonly DependencyProperty ShadowTextForegroundProperty = Register<TextBoxShadowable, SolidColorBrush>(Name: nameof(ShadowTextForeground), DefaultValue: Brushes.Black);
        public SolidColorBrush ShadowTextForeground { get => (SolidColorBrush)GetValue(ShadowTextForegroundProperty); set => SetValue(ShadowTextForegroundProperty, value); }
        
        
        public static readonly DependencyProperty CornerRadiusProperty = Register<TextBoxShadowable, CornerRadius>(Name: nameof(CornerRadius), DefaultValue: new CornerRadius(0.0));
        public CornerRadius CornerRadius { get => (CornerRadius)GetValue(CornerRadiusProperty); set => SetValue(CornerRadiusProperty, value); }


        public bool AnyPathRequired { get; set; }


        public PathExistanceCheckTypeEnum PathExistanceCheck { get; set; } = PathExistanceCheckTypeEnum.None;
        public enum PathExistanceCheckTypeEnum
        {
            File,
            Directory,
            None
        }


        public static readonly DependencyProperty SelectionButtonTypeProperty = Register<TextBoxShadowable, SelectionButtonTypeEnum>(Name: nameof(SelectionButtonType), DefaultValue: SelectionButtonTypeEnum.None);
        public SelectionButtonTypeEnum SelectionButtonType { get => (SelectionButtonTypeEnum)GetValue(SelectionButtonTypeProperty); set => SetValue(SelectionButtonTypeProperty, value); }
        public enum SelectionButtonTypeEnum
        {
            File,
            Directory,
            None
        }

        public FileSelectAttributes_PROP FileSelectAttributes { get; set; } = new();

        public TextBoxShadowable()
        {
            this.KeyDown += (Sender, Args) => { if (Args.Key == Key.Escape) Keyboard.ClearFocus(); };
        }
    }

    public class FileSelectAttributes_PROP
    {
        public string FilesHint { get; set; } = "";
        public string Extensions { get; set; } = "";
    }



    public class TwoColumned : Grid
    {
        public double Length1 { set { this.ColumnDefinitions[0].Width = new GridLength(value, GridUnitType.Star); } }
        public double Length2 { set { this.ColumnDefinitions[1].Width = new GridLength(value, GridUnitType.Star); } }
        public double Width1 { set { this.ColumnDefinitions[0].Width = new GridLength(value); } }
        public double Width2 { set { this.ColumnDefinitions[1].Width = new GridLength(value); } }
        public TwoColumned()
        {
            this.ColumnDefinitions.Add(new ColumnDefinition() { MaxWidth = 368.5 });
            this.ColumnDefinitions.Add(new ColumnDefinition());
        }
    }
}