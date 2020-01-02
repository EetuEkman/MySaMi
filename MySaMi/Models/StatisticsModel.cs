using System.ComponentModel.DataAnnotations.Schema;

namespace MySaMi.Models
{
    /// <summary>
    /// Represents various usage statistics
    /// </summary>
    [Table("Statistics")]
    public class StatisticsModel
    {
        public int Id { get; set; }
        /// <summary>
        /// Represents the query count
        /// </summary>
        public int QueryCount { get; set; } = 0;
        /// <summary>
        /// Represents the count of the returned measurements
        /// </summary>
        public int MeasurementCount { get; set; } = 0;
    }
}
