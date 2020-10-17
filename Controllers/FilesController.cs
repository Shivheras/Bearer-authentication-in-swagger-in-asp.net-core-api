using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apis.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace apis.Controllers
{

    [ApiController]
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
    public class FilesController : ControllerBase
    {

        public FilesController()
        {
            this.FilesDomain = new FilesDomain();
        }
        [HttpGet]
        public IActionResult Get()
        {
           var data= this.FilesDomain.Get();
            return Ok(data);
        }
        public FilesDomain FilesDomain { get; set; }
    }
}