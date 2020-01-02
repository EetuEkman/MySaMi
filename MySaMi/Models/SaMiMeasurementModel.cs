using System;
using System.Collections.Generic;

namespace MySaMi.Models
{
    public class SaMiMeasurementModel
    {
        public List<SaMiDataModel> Data { get; set; }
        public object Location { get; set; }
        public object Note { get; set; }
        public string Object { get; set; }
        public object Tag { get; set; }
        public DateTime TimestampISO8601 { get; set; }
    }
}
