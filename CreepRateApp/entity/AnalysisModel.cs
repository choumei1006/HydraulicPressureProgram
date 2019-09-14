using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CreepRateApp.Core;
using Newtonsoft.Json;

namespace CreepRateApp.entity
{
    public class AnalysisModel : EntityBase
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string FileName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Time { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string TAL { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string TSEF { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string TEU { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string TEM { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string TER { get; set; } 

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Ter_Teu { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Tal_Teu { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string RuHuaLv { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string IsHuiZhuTie { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<FileInfo> FileInfoList { get; set; }
    }
}
