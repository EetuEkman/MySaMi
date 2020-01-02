using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MySaMi.Models
{
    /// <summary>
    /// Represents a measurement SaMi api query made by an user
    /// </summary>
    [Table("SaMiQueries")]
    public class SaMiQueryModel
    {
        public int Id { get; set; }
        /// <summary>
        /// Represents the user who made the query
        /// </summary>
        public virtual ApplicationUser ApplicationUser { get; set; }
        /// <summary>
        /// Represents the key used in the api query
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// Represents the sensor identifying tag used in the api query
        /// </summary>
        public string Tag { get; set; }
        /// <summary>
        /// Represents the date queried
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// Represents the unit associated with the sensor, e.g. voltage v or milliampere mA
        /// </summary>
        public string Unit { get; set; }
        /// <summary>
        /// Represents the sensor name
        /// </summary>
        public string Name { get; set; }
    }
}
