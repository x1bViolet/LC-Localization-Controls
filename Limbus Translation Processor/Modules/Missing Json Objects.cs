using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Translation_Devouring_Siltcurrent.LimbusTranslationProcessor;
using static Translation_Devouring_Siltcurrent.Configurazione;
using static Translation_Devouring_Siltcurrent.Requirements;

namespace Translation_Devouring_Siltcurrent
{
    namespace LocalizationProcessingModules
    {
        public static class MissingJsonObjects
        {
            private record UniversalTypeDataList
            {
                public List<dynamic> dataList { get; set; }
            }
            

            /// <summary>
            /// Insert missing objects to <paramref name="TargetJson"/> dataList from <paramref name="ReferenceJson"/> by their IDs
            /// </summary>
            public static string CompareAppendDataList(string TargetJson, string ReferenceJson, string LoggingFileName = "")
            {
                UniversalTypeDataList TargetJson_Deserialized = JsonConvert.DeserializeObject<UniversalTypeDataList>(TargetJson);
                UniversalTypeDataList ReferenceJson_Deserialized = JsonConvert.DeserializeObject<UniversalTypeDataList>(ReferenceJson);

                bool SomethingWasAdded = false;

                if ((TargetJson_Deserialized.dataList != null & ReferenceJson_Deserialized.dataList != null) && ReferenceJson_Deserialized.dataList.Count>0)
                {
                    // Create list with IDs that TargetJson dataList currently has (string/int)
                    List<dynamic> TargetJson_KnownIDs = [.. TargetJson_Deserialized.dataList.Select(x => x.id)];
                    List<dynamic> ReferenceJson_KnownIDs = [.. ReferenceJson_Deserialized.dataList.Select(x => x.id)];

                    int ReferenceJsonEnumeratorStart = 0;

                    if (CurrentProfile.ReferenceLocalization.MissingContentAppending.CountIDsAsMissedStartingFromLastOneFromSourceFile & TargetJson_Deserialized.dataList.Count > 0)
                    {
                        ReferenceJsonEnumeratorStart = ReferenceJson_KnownIDs.IndexOf(TargetJson_KnownIDs[^1]);
                        if (ReferenceJsonEnumeratorStart == -1) ReferenceJsonEnumeratorStart = 0;
                    }

                    // Then, check ReferenceJson dataList
                    foreach (dynamic ReferenceJson_dataList_Object in ReferenceJson_Deserialized.dataList[ReferenceJsonEnumeratorStart..])
                    {
                        // If list with TargetJson_Deserialized IDs does not contain current object ID, append it
                        if (ReferenceJson_dataList_Object.id != null && !TargetJson_KnownIDs.Contains(ReferenceJson_dataList_Object.id))
                        {
                            TargetJson_Deserialized.dataList.Add(ReferenceJson_dataList_Object);
                            SomethingWasAdded = true;

                            #region Logging
                            if (!CurrentReport.UntranslatedElementsReport.MissingIDs.ContainsKey(LoggingFileName))
                            {
                                CurrentReport.UntranslatedElementsReport.MissingIDs[LoggingFileName] = new List<dynamic>();
                            }
                            CurrentReport.UntranslatedElementsReport.MissingIDsCount++;
                            CurrentReport.UntranslatedElementsReport.MissingIDs[LoggingFileName].Add(ReferenceJson_dataList_Object.id);
                            #endregion
                        }
                    }
                }

                return SomethingWasAdded ? JsonConvert.SerializeObject(TargetJson_Deserialized, Formatting.Indented) : TargetJson;
            }
        }
    }
}