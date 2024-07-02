using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HolyShift
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected readonly EmployeesLogic EmployeesLogic;
        public BaseController(EmployeesLogic employeesLogic)
        {
            EmployeesLogic = employeesLogic;
        }
    }
}
