using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MySaMi.Models
{
    /// <summary>
    /// Represents a saved api key used in an api query
    /// </summary>
    [Table("SaMiKeys")]
    public class SaMiKeyModel
    {
        public int Id { get; set; }
        /// <summary>
        /// Represents the user who saved the key
        /// </summary>
        public virtual ApplicationUser ApplicationUser { get; set; }
        /// <summary>
        /// Represents the saved key
        /// </summary>
        [Required]
        public string Key { get; set; }
        /// <summary>
        /// Represents the name given to the saved key
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// Represents the description given to the saved key
        /// </summary>
        public string Description { get; set; }
    }
}
