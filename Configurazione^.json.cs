using Newtonsoft.Json;
using System.ComponentModel;
using System.IO;
using System.Reflection;

namespace Translation_Devouring_Siltcurrent
{
    public static class Configurazione
    {
        public static GeneralOptions CurrentOptions { get; set; } = new();
        public static ConfigurationProfile CurrentProfile { get; set; } = new();
    }

    /// <summary>
    /// ୧((#Φ益Φ#))୨ ୧((#Φ益Φ#))୨ ୧((#Φ益Φ#))୨ ୧((#Φ益Φ#))୨ MVVM ୧((#Φ益Φ#))୨ ୧((#Φ益Φ#))୨ ୧((#Φ益Φ#))୨ CommunityToolkit.Mvvm ୧((#Φ益Φ#))୨ ୧((#Φ益Φ#))୨ ୧((#Φ益Φ#))୨ ୧((#Φ益Φ#))୨<br/>
    /// <br/>
    /// • Allows to use <see cref="INotifyPropertyChanged.PropertyChanged"/> feature without any sub <see langword="private"/> underscore start lowercase name properties to store actual value.<br/>
    /// • <see langword="get"/> and <see langword="set"/> accessors should look like this for it: <c>{ get => LazyGet(nameof(YourProperty)); set => LazySet(nameof(YourProperty), value); }</c><br/>
    /// • Use <see cref="DefaultValueAttribute"/> to set default value for properties with this advanced <see langword="get"/> and <see langword="set"/>.<br/>
    /// • <see cref="ValueFormattingInstructor"/>  delegate at the <c>LazyGet</c> or <c>LazySet</c> methods can change out/in value on <see langword="get"></see>/<see langword="set"/>.
    /// </summary>
    public abstract record LazyINotifyPropertyChanged : INotifyPropertyChanged
    {
        private Dictionary<string, dynamic> INotifyPropertyChangedActualValues = [];
        private Dictionary<string, PropertyInfo> ThisProperties = [];

        public event PropertyChangedEventHandler? PropertyChanged;

        public delegate object ValueFormattingInstructor(string PropertyName, object Value);
        
        protected void LazySet(string PropertyName, dynamic Value, ValueFormattingInstructor ValueFormatting = null)
        {
            if (ValueFormatting != null) Value = ValueFormatting(PropertyName, Value);
            INotifyPropertyChangedActualValues[PropertyName] = Value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }
        protected dynamic LazyGet(string PropertyName, ValueFormattingInstructor ValueFormatting = null) => INotifyPropertyChangedActualValues.ContainsKey(PropertyName)
            ? ValueFormatting != null ? ValueFormatting(PropertyName, INotifyPropertyChangedActualValues[PropertyName]) : INotifyPropertyChangedActualValues[PropertyName]
            : Nullable.GetUnderlyingType(ThisProperties[PropertyName].PropertyType) != null | !ThisProperties[PropertyName].PropertyType.IsValueType
                ? null
                : Activator.CreateInstance(ThisProperties[PropertyName].PropertyType);

        public LazyINotifyPropertyChanged()
        {
            foreach (PropertyInfo Property in this.GetType().GetProperties())
            {
                ThisProperties[Property.Name] = Property;

                if (Property.HasAttribute(out DefaultValueAttribute DefaultValueAttribute))
                {
                    Property.SetValue(this, DefaultValueAttribute.Value);
                }
            }
        }

        public static object CreateDefaultInstanceOf(Type Type)
        {
            return Nullable.GetUnderlyingType(Type) != null | !Type.IsValueType
                ? null
                : Activator.CreateInstance(Type);
        }
    }


    
    public record GeneralOptions
    {
        [JsonProperty("Selected Configuration profile")]
        public string SelectedConfigurationProfile { get; set; } = "";

        [JsonProperty("Show Parameter remarks")]
        public bool ShowParameterRemarks { get; set; } = true;
    }

    public record ConfigurationProfile : LazyINotifyPropertyChanged
    {
        // Change backslashes to regular slashes to unify them for matching patterns during localization files processing
        private static string BackslashFormatInstructor(string PropertyName, object Value) => (Value as string).Replace("\\", "/");
        private static string PathStringFormatInstructor(string PropertyName, object Value)
        {
            string PathString = Value as string;
            return Path.Exists(PathString) ? BackslashFormatInstructor(null, PathString) : PathString;
        }



        [JsonProperty("Paths")]
        public Paths_PROP Paths { get; set; } = new();
        public record Paths_PROP : LazyINotifyPropertyChanged
        {
            [JsonProperty("Source path of localization files")] [DefaultValue("")]
            public string SourceDirectory { get => LazyGet(nameof(SourceDirectory)); set => LazySet(nameof(SourceDirectory), value, PathStringFormatInstructor); }



            [JsonProperty("Destination path of processed files")] [DefaultValue("")]
            public string DestinationDirectory { get => LazyGet(nameof(DestinationDirectory)); set => LazySet(nameof(DestinationDirectory), value, PathStringFormatInstructor); }
        }



        [JsonProperty("General localization files whitelist")] [DefaultValue("*.json")]        
        public string GeneralLocalizationFilesWhitelist { get => LazyGet(nameof(GeneralLocalizationFilesWhitelist)); set => LazySet(nameof(GeneralLocalizationFilesWhitelist), value, BackslashFormatInstructor); }
        
        
        
        [JsonProperty("Files processing order")] [DefaultValue("Recently changed first (Recommended)")]        
        public string FilesProcessingOrder { get => LazyGet(nameof(FilesProcessingOrder)); set => LazySet(nameof(FilesProcessingOrder), value); }



        [JsonProperty("Json files formatting")]
        public JsonFilesFormatting_PROP JsonFilesFormatting { get; set; } = new();
        public record JsonFilesFormatting_PROP : LazyINotifyPropertyChanged
        {
            [JsonProperty("Existing files overwriting rule")] [DefaultValue("Only if Json content itself is not the same")]
            public string ExistingFilesOverwritingRule { get => LazyGet(nameof(ExistingFilesOverwritingRule)); set => LazySet(nameof(ExistingFilesOverwritingRule), value); }



            [JsonProperty("Output json formatting")] [DefaultValue("Indented")]
            public string OutputJsonFormatting { get => LazyGet(nameof(OutputJsonFormatting)); set => LazySet(nameof(OutputJsonFormatting), value); }



            [JsonProperty("Json Indentation size")] [DefaultValue("Keep original")]
            public string JsonIndentationSize { get => LazyGet(nameof(JsonIndentationSize)); set => LazySet(nameof(JsonIndentationSize), value); }



            [JsonProperty("Line break mode")] [DefaultValue("Keep original")]
            public string LineBreakMode { get => LazyGet(nameof(LineBreakMode)); set => LazySet(nameof(LineBreakMode), value); }



            [JsonProperty("UTF-8 Byte Order Marks")] [DefaultValue("Keep BOM if they are inside source file")]
            public string UTF8ByteOrderMarks { get => LazyGet(nameof(UTF8ByteOrderMarks)); set => LazySet(nameof(UTF8ByteOrderMarks), value); }



            [JsonProperty("Empty last line")] [DefaultValue("Keep empty last line if its inside source file")]
            public string EmptyLastLine { get => LazyGet(nameof(EmptyLastLine)); set => LazySet(nameof(EmptyLastLine), value); }
        }



        [JsonProperty("Reference localization")]
        public ReferenceLocalization_PROP ReferenceLocalization { get; set; } = new();
        public record ReferenceLocalization_PROP : LazyINotifyPropertyChanged
        {
            [JsonProperty("Directory of reference localization")] [DefaultValue(@"C:\Program Files (x86)\Steam\steamapps\common\Limbus Company\LimbusCompany_Data\Assets\Resources_moved\Localize\en")]            
            public string Directory { get => LazyGet(nameof(Directory)); set => LazySet(nameof(Directory), value, PathStringFormatInstructor); }



            [JsonProperty("Prefix of reference localization")] [DefaultValue("EN_")]            
            public string Prefix { get => LazyGet(nameof(Prefix)); set => LazySet(nameof(Prefix), value); }



            [JsonProperty("Missing content appending")]
            public MissingContentAppending_PROP MissingContentAppending { get; set; } = new();
            public record MissingContentAppending_PROP : LazyINotifyPropertyChanged
            {
                [JsonProperty("Append missing files")] [DefaultValue(false)]                
                public bool AppendMissingFiles { get => LazyGet(nameof(AppendMissingFiles)); set => LazySet(nameof(AppendMissingFiles), value); }



                [JsonProperty("Append missing IDs")] [DefaultValue(false)]                
                public bool AppendMissingIDs { get => LazyGet(nameof(AppendMissingIDs)); set => LazySet(nameof(AppendMissingIDs), value); }



                [JsonProperty("Whitelist of missing files to append")] [DefaultValue("")]                
                public string WhitelistOfFilesToAppend { get => LazyGet(nameof(WhitelistOfFilesToAppend)); set => LazySet(nameof(WhitelistOfFilesToAppend), value, BackslashFormatInstructor); }



                //Skills_personality-*.json, Skills_Ego_Personality-*.json, Passives.json, Personalities.json, GachaTitle*.json, IntroductionPreset.json, Personality_Get_Condition.json
                [JsonProperty("Whitelist of existing files to add IDs to")] [DefaultValue("")]                
                public string WhitelistOfFilesToAddIDsTo { get => LazyGet(nameof(WhitelistOfFilesToAddIDsTo)); set => LazySet(nameof(WhitelistOfFilesToAddIDsTo), value, BackslashFormatInstructor); }



                [JsonProperty("Count IDs as missed starting from the last one from source file")] [DefaultValue(false)]                
                public bool CountIDsAsMissedStartingFromLastOneFromSourceFile { get => LazyGet(nameof(CountIDsAsMissedStartingFromLastOneFromSourceFile)); set => LazySet(nameof(CountIDsAsMissedStartingFromLastOneFromSourceFile), value); }
            }
        }



        [JsonProperty("Font files")]
        public FontFiles_PROP FontFiles { get; set; } = new();
        public record FontFiles_PROP : LazyINotifyPropertyChanged
        {
            [JsonProperty("Also copy font files")] [DefaultValue(false)]            
            public bool AlsoCopyFontFiles { get => LazyGet(nameof(AlsoCopyFontFiles)); set => LazySet(nameof(AlsoCopyFontFiles), value); }



            [JsonProperty("Title font file")] [DefaultValue(@"")]            
            public string TitleFontFile { get => LazyGet(nameof(TitleFontFile)); set => LazySet(nameof(TitleFontFile), value, PathStringFormatInstructor); }



            [JsonProperty("Context font file")] [DefaultValue(@"")]            
            public string ContextFontFile { get => LazyGet(nameof(ContextFontFile)); set => LazySet(nameof(ContextFontFile), value, PathStringFormatInstructor); }
        }



        [JsonProperty("JsonPath regex conversions")]
        public JsonPathMultipleRegexConversions_PROP JsonPathMultipleRegexConversions { get; set; } = new();
        public record JsonPathMultipleRegexConversions_PROP : LazyINotifyPropertyChanged
        {
            [JsonProperty("Do JsonPath Multiple Regex Conversions")] [DefaultValue(false)]            
            public bool DoJsonPathMultipleRegexConversions { get => LazyGet(nameof(DoJsonPathMultipleRegexConversions)); set => LazySet(nameof(DoJsonPathMultipleRegexConversions), value); }



            [JsonProperty("JsonPath Multiple Regex Conversions config")] [DefaultValue(@"")]            
            public string JsonPathMultipleRegexConversionsConfigFile { get => LazyGet(nameof(JsonPathMultipleRegexConversionsConfigFile)); set => LazySet(nameof(JsonPathMultipleRegexConversionsConfigFile), value, PathStringFormatInstructor); }
        }



        [JsonProperty("Miscellaneous")]
        public Misc_PROP Misc { get; set; } = new();
        public record Misc_PROP : LazyINotifyPropertyChanged
        {
            [JsonProperty("Add <style> placeholders")] [DefaultValue(false)]            
            public bool AddStylePlaceholders { get => LazyGet(nameof(AddStylePlaceholders)); set => LazySet(nameof(AddStylePlaceholders), value); }



            [JsonProperty("Convert Keyword Shorthands")] [DefaultValue(false)]            
            public bool ConvertKeywordShorthands { get => LazyGet(nameof(ConvertKeywordShorthands)); set => LazySet(nameof(ConvertKeywordShorthands), value); }



            //lang=regex
            [JsonProperty("Keyword Shorthands regex pattern")] [DefaultValue(@"\[(?<ID>\w+):`(?<Name>.*?)`\](\((?<Color>#[a-fA-F0-9]{6})\))?")]            
            public string KeywordShorthandsRegexPattern { get => LazyGet(nameof(KeywordShorthandsRegexPattern)); set => LazySet(nameof(KeywordShorthandsRegexPattern), value); }



            //lang=regex (Not really)
            [JsonProperty("Keyword Shorthands files whitelist")] [DefaultValue("Skills*.json, Passive*.json, EGOgift_*.json, PanicInfo*.json, BuffAbilities.json, MentalCondition*.json, BattleKeywords*.json, Bufs*.json")]            
            public string KeywordShorthandsFilesWhiteList { get => LazyGet(nameof(KeywordShorthandsFilesWhiteList)); set => LazySet(nameof(KeywordShorthandsFilesWhiteList), value, BackslashFormatInstructor); }



            [JsonProperty("Keyword Shorthands colors info file")] [DefaultValue(@"[⇲] Assets Directory\Keyword Colors.cd.txt")]            
            public string KeywordShorthandsColorsInfoFile { get => LazyGet(nameof(KeywordShorthandsColorsInfoFile)); set => LazySet(nameof(KeywordShorthandsColorsInfoFile), value, PathStringFormatInstructor); }
        }



        [JsonProperty("Merged fonts")]
        public MergedFonts_PROP MergedFonts { get; set; } = new();
        public record MergedFonts_PROP : LazyINotifyPropertyChanged
        {
            [JsonProperty("Merged font Characters Replacement Map")] [DefaultValue(@"")]            
            public string MergedFontCharactersReplacementMap { get => LazyGet(nameof(MergedFontCharactersReplacementMap)); set => LazySet(nameof(MergedFontCharactersReplacementMap), value, PathStringFormatInstructor); }



            [JsonProperty("Merged font Multiple Apply config")] [DefaultValue(@"")]            
            public string MergedFontMultipleApplyConfig { get => LazyGet(nameof(MergedFontMultipleApplyConfig)); set => LazySet(nameof(MergedFontMultipleApplyConfig), value, PathStringFormatInstructor); }



            //lang=regex
            [JsonProperty("Merged font Ignore Sequences regex pattern")] [DefaultValue(@"{\d+}|<.*?>|\\n")]            
            public string MergedFontIgnoreSequencesRegexPattern { get => LazyGet(nameof(MergedFontIgnoreSequencesRegexPattern)); set => LazySet(nameof(MergedFontIgnoreSequencesRegexPattern), value); }



            [JsonProperty("Convert fonts by Multiple Apply config")] [DefaultValue(false)]            
            public bool ConvertFontsByMultipleApplyConfig { get => LazyGet(nameof(ConvertFontsByMultipleApplyConfig)); set => LazySet(nameof(ConvertFontsByMultipleApplyConfig), value); }



            [JsonProperty("Convert fonts by '[font=name]' markers")] [DefaultValue(false)]            
            public bool ConvertFontsByMarkers { get => LazyGet(nameof(ConvertFontsByMarkers)); set => LazySet(nameof(ConvertFontsByMarkers), value); }
        }
    }
}