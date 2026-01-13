using Newtonsoft.Json;

namespace Translation_Devouring_Siltcurrent
{
    namespace LocalizationProcessingModules
    {
        public static class Log
        {
            public record ProcessingReport
            {
                [JsonProperty("Date")]
                public DateTime Date { get; set; } = DateTime.Now;

                [JsonProperty("Destination directory")]
                public string DestinationDirectory { get; set; }

                [JsonProperty("Source directory")]
                public string SourceDirectory { get; set; }

                [JsonProperty("Reference translation directory")]
                public string TranslationReferenceDirectory { get; set; }

                [JsonProperty("Untranslated content added")]
                public UntranslatedElementsReport_PROP UntranslatedElementsReport { get; set; } = new();
                public record UntranslatedElementsReport_PROP
                {
                    [JsonProperty("Missing IDs count")]
                    public int MissingIDsCount { get; set; }

                    [JsonProperty("Untranslated files count")]
                    public int UntranslatedFilesCount { get; set; }

                    [JsonProperty("Untranslated files added")]
                    public List<string> UntranslatedFiles { get; set; } = [];

                    [JsonProperty("Missing IDs added")]
                    public Dictionary<string, List<dynamic>> MissingIDs { get; set; } = [];
                }

                [JsonProperty("Merged Fonts conversion")]
                public SpecialFontsConversionReport_PROP MergedFontsConversionReport { get; set; } = new();
                public record SpecialFontsConversionReport_PROP
                {
                    [JsonProperty("Attached fonts (Markers)")]
                    public Dictionary<string, Dictionary<string, string>> AttachedFonts_Markers { get; set; } = [];
                    
                    [JsonProperty("Attached fonts (Multiple Apply Config)")]
                    public Dictionary<string, Dictionary<string, string>> AttachedFonts_MultipleApplyConfig { get; set; } = [];

                    [JsonProperty("Used Characters Replacement Map fonts")]
                    public Dictionary<string, Dictionary<String, String>> UsedReplacementMapFonts { get; set; } = [];
                }

                [JsonProperty("JsonPath Multiple Regex conversions")]
                public Dictionary<string, Dictionary<string, string>> JsonPathMultipleRegexConversions { get; set; } = [];

                [JsonProperty("Shorthands Conversion Info")]
                public ShorthandsConversionInfo_PROP ShorthandsConversionInfo { get; set; } = new();
                public record ShorthandsConversionInfo_PROP
                {
                    [JsonProperty("Used Pattern")]
                    public string UsedPattern { get; set; }

                    [JsonProperty("Conversions Count")]
                    public int ConvertedCount { get; set; }

                    [JsonProperty("Undefined Colors for:")]
                    public List<string> ShorthandsConvertedWithoutColor { get; set; } = [];
                }
            }
        }
    }
}
