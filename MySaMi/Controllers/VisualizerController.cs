using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySaMi.Models;

namespace MySaMi.Controllers
{
    [Authorize]
    public class VisualizerController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IDataProtectionProvider protectionProvider;
        private string[] purposes;

        public VisualizerController(UserManager<ApplicationUser> userManager, 
            IConfiguration configuration,
            IDataProtectionProvider protectionProvider)
        {
            this.userManager = userManager;
            this.protectionProvider = protectionProvider;

            purposes = new string[]
            {
                configuration.GetSection("DataProtection").GetSection("SaMiKey").GetSection("Purpose").Value,
                configuration.GetSection("DataProtection").GetSection("SaMiKey").GetSection("Version").Value
            };
        }
        // GET: Visualizer
        public async Task<IActionResult> Index()
        {
            var user = await userManager.GetUserAsync(HttpContext.User);

            var saMiKeys = user.Keys;

            if (saMiKeys != null && saMiKeys.Count > 0)
            {
                var protector = protectionProvider.CreateProtector(purposes);

                foreach (var saMiKey in saMiKeys)
                {
                    saMiKey.Key = protector.Unprotect(saMiKey.Key);
                }
            }

            var visualizerViewModel = new VisualizerViewModel
            {
                SaMiKeys = saMiKeys,
                SaMiQueries = user.Queries
            };

            return View(visualizerViewModel);
        }
    }
}