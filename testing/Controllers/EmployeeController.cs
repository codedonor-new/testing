using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace testing.Controllers
{
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly DataContext _applicationDbContext;
        private readonly IHttpClientFactory _httpClientFactory;
        public EmployeeController(DataContext applicationDbContext, IHttpClientFactory httpClientFactory)
        {
            _applicationDbContext = applicationDbContext;
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost]
        [Route("api/employee")]
        public IActionResult Post(string name)
        {
            var employee = new Employee();
            employee.Name = name;
            _applicationDbContext.Employees.Add(employee);
            _applicationDbContext.SaveChanges();

            return Ok("inserted");
        }

        [HttpPost]
        [Route("api/employee/update")]
        public IActionResult Update(string name,int id)
        {
            var employee = _applicationDbContext.Employees.FirstOrDefault(x => x.Id==id);
            employee.Name = name;
            _applicationDbContext.Employees.Update(employee);
            _applicationDbContext.SaveChanges();

            return Ok("updated");
        }

        [HttpGet]
        [Route("api/employees")]
        public IActionResult Get()
        {
            //var students = _applicationDbContext.Students.Where(x => x.Id== id);

            // var students = _applicationDbContext.Students.ToList();
            var students = _applicationDbContext.Employees.ToList();

            return Ok(students);
        }

    }
}
