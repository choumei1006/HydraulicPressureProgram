using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CreepRateApp.Core
{
    public class EntityBase
    {
        [BsonId]
        public ObjectId Id { get; set; }
    }
}
