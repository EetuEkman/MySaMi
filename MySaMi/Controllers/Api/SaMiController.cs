using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MySaMi.Data;
using MySaMi.Models;

namespace MySaMi.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaMiController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly IHttpClientFactory clientFactory;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly MySaMiDbContext context;

        public SaMiController(IConfiguration configuration, IHttpClientFactory clientFactory, UserManager<ApplicationUser> userManager, MySaMiDbContext context)
        {
            this.configuration = configuration;
            this.clientFactory = clientFactory;
            this.userManager = userManager;
            this.context = context;
        }

        // GET: Api/SaMi/Sensors
        [Route("Sensors", Name = "Sensors")]
        public async Task<IActionResult> Sensors([FromQuery, Required]string key)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var url = configuration.GetSection("Urls").GetSection("Sensors").Value + key;

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            request.Headers.Add("Accept", "application/json");

            var client = clientFactory.CreateClient();

            var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest();
            }

            var body = await response.Content.ReadAsStringAsync();

            var sensors = JsonSerializer.Deserialize<List<SensorModel>>(body);

            var json = JsonSerializer.Serialize(sensors);

            return Content(json, "application/json");
        }

        // GET: Api/SaMi/Measurements
        [Route("Measurements", Name = "Measurements")]
        public async Task<IActionResult> Measurements([FromQuery]SaMiQueryModel saMiQuery)
        {
            IncrementQueryCount();

            var user = await userManager.GetUserAsync(HttpContext.User);

            user.Queries.Add(saMiQuery);

            if (user.Queries.Count > 3)
            {
                user.Queries.RemoveAt(0);
            }

            userManager.UpdateAsync(user);

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var date = saMiQuery.Date;

            var tomorrow = date.AddDays(1);

            var url = $"{configuration.GetSection("Urls").GetSection("Measurements").Value}{saMiQuery.Key}?data-tags={saMiQuery.Tag}&from={date.ToShortDateString()}&to={tomorrow.ToShortDateString()}&take=288";

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            var client = clientFactory.CreateClient();

            var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest();
            }

            var body = await response.Content.ReadAsStringAsync();

            var measurements = JsonSerializer.Deserialize<List<SaMiMeasurementModel>>(body);

            var chartData = new ChartDataModel();

            var chartDataSet = new ChartDatasetModel();

            foreach (var measurement in measurements)
            {
                chartData.Labels.Add(measurement.TimestampISO8601.ToShortTimeString());
                chartDataSet.Data.Add(measurement.Data[0].Value);
            }

            chartData.Labels.Reverse();

            chartDataSet.Label = saMiQuery.Unit;

            chartDataSet.Data.Reverse();

            chartData.Datasets.Add(chartDataSet);

            body = JsonSerializer.Serialize(chartData);

            IncrementMeasurementCount();

            return Content(body, "application/json");
        }

        // GET: Api/SaMi/UserQueries
        [Route("UserQueries", Name = "UserQueries")]
        public async Task<IActionResult> UserQueries()
        {
            var user = await userManager.GetUserAsync(HttpContext.User);

            var queries = user.Queries;

            var json = JsonSerializer.Serialize(queries);

            return Content(json, "application/json");
        }

        public async Task<int> IncrementQueryCount()
        {
            var statistics = await context.Statistics.FirstOrDefaultAsync();

            if (statistics == null)
            {
                statistics = new StatisticsModel();

                context.Add(statistics);

                await context.SaveChangesAsync();
            }

            statistics.QueryCount += 1;

            context.Update(statistics);

            await context.SaveChangesAsync();

            return statistics.QueryCount;
        }

        public async Task<int> IncrementMeasurementCount()
        {
            var statistics = await context.Statistics.FirstOrDefaultAsync();

            if (statistics == null)
            {
                statistics = new StatisticsModel();

                context.Add(statistics);

                await context.SaveChangesAsync();
            }

            statistics.MeasurementCount += 1;

            context.Update(statistics);

            await context.SaveChangesAsync();

            return statistics.MeasurementCount;
        }
    }
}