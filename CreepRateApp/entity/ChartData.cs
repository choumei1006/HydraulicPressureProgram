using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace CreepRateApp.entity
{
    public class ChartData
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int Number { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public double DataValue { get; set; }
    }
}
