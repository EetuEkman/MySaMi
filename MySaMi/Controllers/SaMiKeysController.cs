using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MySaMi.Data;
using MySaMi.Models;

namespace MySaMi.Controllers
{
    [Authorize]
    public class SaMiKeysController : Controller
    {
        private readonly MySaMiDbContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IConfiguration configuration;
        private readonly IHttpClientFactory clientFactory;
        private readonly IDataProtectionProvider protectionProvider;
        private string[] purposes;

        public SaMiKeysController
        (
            MySaMiDbContext context,
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            IDataProtectionProvider protectionProvider,
            IHttpClientFactory clientFactory
        )
        {
            this.context = context;
            this.userManager = userManager;
            this.configuration = configuration;
            this.protectionProvider = protectionProvider;
            this.clientFactory = clientFactory;

            purposes = new string[] 
            { 
                configuration.GetSection("DataProtection").GetSection("SaMiKey").GetSection("Purpose").Value, 
                configuration.GetSection("DataProtection").GetSection("SaMiKey").GetSection("Version").Value 
            };
        }

        // GET: SaMiKeys
        public async Task<IActionResult> Index()
        {
            var user = await userManager.GetUserAsync(HttpContext.User);

            var saMiKeys = user.Keys;

            if(saMiKeys != null && saMiKeys.Count > 0)
            {
                var dataProtector = protectionProvider.CreateProtector(purposes);

                foreach (var saMiKey in saMiKeys)
                {
                    saMiKey.Key = dataProtector.Unprotect(saMiKey.Key);
                }
            }

            var saMiKeysViewModel = new SaMiKeysViewModel { SaMiKeys = saMiKeys };

            return View(saMiKeysViewModel);
        }

        // GET: SaMiKeys/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var saMiKeyModel = await context.SaMiKeyModel
                .FirstOrDefaultAsync(m => m.Id == id);

            if (saMiKeyModel == null)
            {
                return NotFound();
            }

            var protector = protectionProvider.CreateProtector(purposes);

            saMiKeyModel.Key = protector.Unprotect(saMiKeyModel.Key);

            return View(saMiKeyModel);
        }

        // GET: SaMiKeys/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SaMiKeys/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Key,Name,Description")] SaMiKeyModel saMiKeyModel)
        {
            if(!ModelState.IsValid)
            {
                return View(saMiKeyModel);
            }

            // Query SaMi for key validity

            var uri = configuration.GetSection("Urls").GetSection("Sensors").Value + saMiKeyModel.Key;

            var request = new HttpRequestMessage(HttpMethod.Get, uri);

            var client = clientFactory.CreateClient();

            var response = await client.SendAsync(request);

            if(!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("Key", "Invalid key.");

                return View(saMiKeyModel);
            }

            var protector = protectionProvider.CreateProtector(purposes);

            saMiKeyModel.Key = protector.Protect(saMiKeyModel.Key);

            saMiKeyModel.ApplicationUser = await userManager.GetUserAsync(HttpContext.User);

            context.Add(saMiKeyModel);

            await context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: SaMiKeys/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var saMiKeyModel = await context.SaMiKeyModel.FindAsync(id);

            if (saMiKeyModel == null)
            {
                return NotFound();
            }

            var protector = protectionProvider.CreateProtector(purposes);

            saMiKeyModel.Key = protector.Unprotect(saMiKeyModel.Key);

            return View(saMiKeyModel);
        }

        // POST: SaMiKeys/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Key,Name,Description")] SaMiKeyModel saMiKeyModel)
        {
            if (id != saMiKeyModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var protector = protectionProvider.CreateProtector(purposes);

                    saMiKeyModel.Key = protector.Protect(saMiKeyModel.Key);

                    context.Update(saMiKeyModel);

                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SaMiKeyModelExists(saMiKeyModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            return View(saMiKeyModel);
        }

        // GET: SaMiKeys/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var saMiKeyModel = await context.SaMiKeyModel.FirstOrDefaultAsync(m => m.Id == id);

            if (saMiKeyModel == null)
            {
                return NotFound();
            }

            var protector = protectionProvider.CreateProtector(purposes);

            saMiKeyModel.Key = protector.Unprotect(saMiKeyModel.Key);

            return View(saMiKeyModel);
        }

        // POST: SaMiKeys/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var saMiKeyModel = await context.SaMiKeyModel.FindAsync(id);

            context.SaMiKeyModel.Remove(saMiKeyModel);

            await context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool SaMiKeyModelExists(int id)
        {
            return context.SaMiKeyModel.Any(e => e.Id == id);
        }
    }
}
