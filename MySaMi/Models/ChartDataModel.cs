using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MySaMi.Models
{
    public class ChartDataModel
    {
        [JsonPropertyName("labels")]
        public List<string> Labels { get; set; } = new List<string>();
        [JsonPropertyName("datasets")]
        public List<ChartDatasetModel> Datasets { get; set; } = new List<ChartDatasetModel>();
    }
}
