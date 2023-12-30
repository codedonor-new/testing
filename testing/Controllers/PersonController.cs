using Microsoft.AspNetCore.Mvc;

namespace testing.Controllers
{

    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly DataContext _applicationDbContext;
        private readonly IHttpClientFactory _httpClientFactory;
        public PersonController(DataContext applicationDbContext, IHttpClientFactory httpClientFactory)
        {
            _applicationDbContext = applicationDbContext;
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost]
        [Route("api/person")]
        public IActionResult Post(string name)
        {
            var employee = new Person();
            employee.Name = name;
            _applicationDbContext.Persons.Add(employee);
            _applicationDbContext.SaveChanges();

            return Ok("inserted");
        }

        [HttpPost]
        [Route("api/person/update")]
        public IActionResult Update(string name, int id)
        {
            var employee = _applicationDbContext.Persons.FirstOrDefault(x => x.Id == id);
            employee.Name = name;
            _applicationDbContext.Persons.Update(employee);
            _applicationDbContext.SaveChanges();

            return Ok("updated");
        }

        [HttpGet]
        [Route("api/person")]
        public IActionResult Get()
        {
            //var students = _applicationDbContext.Students.Where(x => x.Id== id);

            // var students = _applicationDbContext.Students.ToList();
            var students = _applicationDbContext.Persons.ToList();

            return Ok(students);
        }

    }
}
