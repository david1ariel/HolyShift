using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HolyShift
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly AuthLogic Logic;
        public AuthController(AuthLogic logic, EmployeesLogic employeesLogic) : base (employeesLogic)
        {
            Logic = logic;
        }

        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] CredentialsModel credentials)
        {
            if (string.IsNullOrEmpty(credentials.Username) || string.IsNullOrEmpty(credentials.Password) || !Logic.IsAuthenticated(credentials))
            {
                return Unauthorized("Username or password is incorrect"); 
            }
            EmployeeModel employee = EmployeesLogic.GetSingleEmployee(credentials.Username);
            return Ok(new {employee, token = TokenGenerator.GenerateStringToken(employee, 24*60)});
        }

        [HttpGet("test"), Authorize]
        public IActionResult Test()
        {
            return Ok("Ok");
        }
    }
}
