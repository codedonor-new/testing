using Microsoft.AspNetCore.Mvc;

namespace testing.Controllers
{
    [ApiController]
    public class TestUserController : ControllerBase
    {

        private readonly DataContext _applicationDbContext;
        public TestUserController(DataContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        [HttpPost]
        [Route("api/app/test-file")]
        public async Task<IActionResult> UploadFiles( FileModel fileModel)
        {
            return Ok();
        }

            [HttpGet]
        [Route("api/app/test-user")]
        public IActionResult GetByReferenceCode(string code)
        {
            var user = _applicationDbContext.TestUsers
                .Where(u => u.ReferenceCode.Trim() == code.Trim())
                .FirstOrDefault();

            if (user == null)
            {
                return NotFound($"No records found for the reference code: {code}");
            }

            return Ok(user);
        }
    }
}
