using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;
using Server.Models.DTOs;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpController(AppDbContext _appDb, IWebHostEnvironment _webHostEnvironment, IHttpContextAccessor _contextAccessor) : ControllerBase
    {
        //[Authorize(Roles = "Admin,User")]
        [HttpGet("Get")]
        public async Task<IActionResult> UpdateAsync(int? id)
        {
            if (id == null || id == 0)
            {
                var employees = _appDb.Emps.ToList();
                return Ok(employees);
            }
            else
            {
                var user = _appDb.Emps.FirstOrDefault(_ => _.Id == id);
                if (user == null) return BadRequest("No Data found...");
                return Ok(user);
            }

        }

        [HttpPost("Upsert")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAsync([FromForm] Emp user, IFormFile files)
        {
            if (user == null) return BadRequest("Model is empty");
            if (user.Id == 0 || user.Id == null)
            {
                if (files != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(files.FileName);
                    string filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "images", fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        files.CopyTo(stream);
                    }
                    var urlFilePath = $"{_contextAccessor.HttpContext.Request.Scheme}://{_contextAccessor.HttpContext.Request.Host}{_contextAccessor.HttpContext.Request.PathBase}/images/{fileName}";

                    user.Img = urlFilePath;
                }
                var finalEmp = new Emp()
                {
                    Id = user.Id,
                    Name = user.Name,
                    Department = user.Department,
                    Img = user.Img,
                };
                _appDb.Emps.Add(finalEmp);
                _appDb.SaveChanges();
                return Ok("Added Successfully");
            }
            else
            {

                if (files != null)
                {
                    var old = _appDb.Emps.AsNoTracking().FirstOrDefault(u => u.Id == user.Id);
                    if (old.Img.ToString().Trim() != "")
                    {
                        var localFilePath = Path.Combine(_webHostEnvironment.ContentRootPath, "images");
                        var url = old.Img;
                        string OldPath = Path.GetFileName(new Uri(url).LocalPath);
                        string oldImagePath = Path.Combine(localFilePath, OldPath);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                }

                if (files != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(files.FileName);
                    string filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "images", fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        files.CopyTo(stream);
                    }
                    var urlFilePath = $"{_contextAccessor.HttpContext.Request.Scheme}://{_contextAccessor.HttpContext.Request.Host}{_contextAccessor.HttpContext.Request.PathBase}/images/{fileName}";

                    user.Img = urlFilePath;
                }
                else
                {
                    var ImgPth = _appDb.Emps.FirstOrDefault(u => u.Id == user.Id);
                    if (ImgPth is not null)
                        user.Img = ImgPth.Img;
                }
                var finalEmp = new Emp()
                {
                    Id = user.Id,
                    Name = user.Name,
                    Department = user.Department,
                    Img = user.Img,
                };
                _appDb.Emps.Update(finalEmp);
                _appDb.SaveChanges();
                return Ok("Updated Successfully");
            }

        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("Remove")]
        public async Task<IActionResult> UpdateAsync(int id)
        {
            if (id == null || id == 0) return BadRequest("No Data found...");
            var user = _appDb.Emps.FirstOrDefault(_ => _.Id == id);
            if (user == null) return BadRequest("No Data found...");

            _appDb.Emps.Remove(user);
            _appDb.SaveChanges();
            return Ok("Deleted Successfully");
        }

    }
}
