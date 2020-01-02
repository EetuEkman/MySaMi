using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MySaMi.Models
{
    public class ChartDatasetModel
    {
        [JsonPropertyName("label")]
        public string Label { get; set; }
        [JsonPropertyName("data")]
        public List<double?> Data { get; set; } = new List<double?>();
    }
}
