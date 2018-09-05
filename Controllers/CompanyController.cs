using CustomMiddleware.Model;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomMiddleware.Controllers
{
    [Route("api")]    
    public class CompanyController : Controller
    {
        [HttpGet("companies")]
        public async Task<IActionResult> GetAsync()
        {
            var companies = new List<Company>
            {
                new Company{ Id = 1, Name = "Google"},
                new Company{ Id = 1, Name = "Microsoft"},
                new Company{ Id = 1, Name = "Apple"}
            };
            
            return await Task.Run(()=> Ok(companies));
        }        
    }
}