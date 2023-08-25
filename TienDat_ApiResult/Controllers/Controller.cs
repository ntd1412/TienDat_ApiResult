using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using TienDat_ApiResult.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TienDat_ApiResult.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TienDatApiController : ControllerBase
    {
        private readonly ApiContext _dbContext;
        private readonly IMemoryCache _cache;

        public TienDatApiController(ApiContext dbContext, IMemoryCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }
        /// <summary>
        /// get theo id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult GetUser(Guid id)
        {
            if (_cache.TryGetValue($"User_{id}", out var cachedUser))
            {
                return Ok(cachedUser);
            }

            var user = _dbContext.Employees.FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            _cache.Set($"User_{id}", user, TimeSpan.FromMinutes(10)); // Lưu trong 10 phút

            return Ok(user);
        }
        /// <summary>
        /// get toàn bộ
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetAllEmployees()
        {
            if (_cache.TryGetValue("AllEmployees", out var cachedEmployees))
            {
                return Ok(cachedEmployees);
            }

            var employees = _dbContext.Employees.ToList();

            _cache.Set("AllEmployees", employees, TimeSpan.FromMinutes(10)); // Lưu trong 10 phút

            return Ok(employees);
        }
        /// <summary>
        /// xóa theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public IActionResult DeleteEmployee(Guid id)
        {
            var employee = _dbContext.Employees.FirstOrDefault(e => e.Id == id);

            if (employee == null)
            {
                return NotFound();
            }

            _dbContext.Employees.Remove(employee);
            _dbContext.SaveChanges();

            // Xóa cache liên quan khi dữ liệu bị thay đổi
            _cache.Remove($"User_{id}");
            _cache.Remove("AllEmployees");

            return NoContent();
        }
    }
}
