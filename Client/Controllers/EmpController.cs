using Client.Extra.Service;
using Client.Models;
using Client.Report;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Client.Controllers
{
    public class EmpController : Controller
    {
        private readonly HttpClient _httpClient;
        public EmpController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MyApiClient"); ;
        }
        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetStringAsync("Emp/Get");
            var emp = System.Text.Json.JsonSerializer.Deserialize<List<Employee>>(response);
            return View(emp);
        }
        public async Task<IActionResult> Print(int? id)
        {
            string outputPath = "EmployeeDetails.pdf";
            var response = await _httpClient.GetStringAsync("Emp/Get?id=" + id);
            //var emp = System.Text.Json.JsonSerializer.Deserialize<List<Employee>>(response);
            var emp = System.Text.Json.JsonSerializer.Deserialize<List<Employee>>(response);
            // Generate PDF
            //PdfGenerator.GeneratePdf(emp, outputPath);
            if (emp == null || emp.Count == 0)
            {
                return NotFound("No employee data found.");
                return RedirectToAction("Index");
            }

            // Generate PDF as a byte array
            byte[] pdfBytes = PdfGenerator.GeneratePdf(emp);

            // Return the PDF as a file download
            return File(pdfBytes, "application/pdf", "EmployeeDetails.pdf");
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> AddOrUpdate(Employee employee, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                var url = "Emp/Upsert"; // Your API URL
                employee.img = " ";
                if (employee == null || string.IsNullOrWhiteSpace(employee.name))
                {
                    TempData["error"] = "Employee is null or name is required.";
                    return RedirectToAction("Upsert");
                }
                employee.imageFile = imageFile;
                using var content = new MultipartFormDataContent();
                content.Add(new StringContent(employee.id.ToString()), nameof(employee.id));
                content.Add(new StringContent(employee.name), nameof(employee.name));
                content.Add(new StringContent(employee.department), nameof(employee.department));
                content.Add(new StringContent(employee.img), nameof(employee.img));

                if (imageFile != null && imageFile.Length > 0)
                {
                    var fileContent = new StreamContent(imageFile.OpenReadStream());
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(imageFile.ContentType);
                    content.Add(fileContent, "files", imageFile.FileName);
                }


                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Globle.Token);

                try
                {
                    // Send POST request with multipart data
                    var response = await _httpClient.PostAsync(url, content);
                    var message = await response.Content.ReadAsStringAsync();

                    // Check response status
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["success"] = message;
                    }
                    else
                    {
                        TempData["error"] = message;
                        return RedirectToAction("Index");
                    }
                }
                catch (HttpRequestException e)
                {
                    TempData["error"] = e.Message;
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int? id)
        {
            var url = "Emp/Remove?id=" + id;
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Globle.Token);
            try
            {
                // Send POST request with multipart data
                var response = await _httpClient.DeleteAsync(url);
                var message = await response.Content.ReadAsStringAsync();

                // Check response status
                if (response.IsSuccessStatusCode)
                {
                    TempData["success"] = message;
                }
                else
                {
                    TempData["error"] = message;
                    return RedirectToAction("Index");
                }
            }
            catch (HttpRequestException e)
            {
                TempData["error"] = e.Message;
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

    }
}
