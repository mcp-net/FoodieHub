using Microsoft.AspNetCore.Mvc;
using FoodieHub.UI.Models;
using System.Text;
using System.Text.Json;

namespace FoodieHub.UI.Controllers
{
    public class CitiesController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CitiesController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<CityDto> response = new List<CityDto>();

            try
            {
                // Get All Cities from Web API
                var client = _httpClientFactory.CreateClient();

                var httpResponseMessage = await client.GetAsync("https://localhost:7081/api/cities");

                httpResponseMessage.EnsureSuccessStatusCode();

                response.AddRange(await httpResponseMessage.Content.ReadFromJsonAsync<IEnumerable<CityDto>>());
            }
            catch (Exception ex)
            {
                // Log the exception
            }

            return View(response);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddCityViewModel model)
        {
            var client = _httpClientFactory.CreateClient();

            var httpRequestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://localhost:7081/api/cities"),
                Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json")
            };

            var httpResponseMessage = await client.SendAsync(httpRequestMessage);
            httpResponseMessage.EnsureSuccessStatusCode();

            var response = await httpResponseMessage.Content.ReadFromJsonAsync<CityDto>();

            if (response is not null)
            {
                return RedirectToAction("Index", "Cities");
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var client = _httpClientFactory.CreateClient();

            var response = await client.GetFromJsonAsync<CityDto>($"https://localhost:7081/api/cities/{id.ToString()}");

            if (response is not null)
            {
                return View(response);
            }

            return View(null);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CityDto request)
        {
            var client = _httpClientFactory.CreateClient();

            var httpRequestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri($"https://localhost:7081/api/cities/{request.Id}"),
                Content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json")
            };

            var httpResponseMessage = await client.SendAsync(httpRequestMessage);
            httpResponseMessage.EnsureSuccessStatusCode();

            var response = await httpResponseMessage.Content.ReadFromJsonAsync<CityDto>();

            if (response is not null)
            {
                return RedirectToAction("Edit", "Cities");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(CityDto request)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();

                var httpResponseMessage = await client.DeleteAsync($"https://localhost:7081/api/cities/{request.Id}");

                httpResponseMessage.EnsureSuccessStatusCode();

                return RedirectToAction("Index", "Cities");
            }
            catch (Exception ex)
            {
                // Console
            }

            return View("Edit");
        }
    }
}