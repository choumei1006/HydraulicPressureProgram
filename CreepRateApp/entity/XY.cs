using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace CreepRateApp.entity
{
    public class XY
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public double x { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public double y { get; set; }
    }
}
