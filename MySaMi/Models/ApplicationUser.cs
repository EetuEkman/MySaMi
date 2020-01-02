using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace MySaMi.Models
{
    /// <summary>
    /// Represents an application user.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        public virtual List<SaMiKeyModel> Keys { get; set; }
        public virtual List<SaMiQueryModel> Queries { get; set; }
    }
}
