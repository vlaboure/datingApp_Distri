using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DattingApp.api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Controllers
{

    //http port :5000/api/values
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly DataContext _context;

        public ValuesController(DataContext context)
        {
            _context = context;

        }
        [AllowAnonymous]
        // GET api/values
        [HttpGet]
        //public ActionResult<IEnumerable<string>> Get()
        //IActionResult --> execution assynchrone
        public async Task<IActionResult> GetValues()
        {       
            // ToListAsync --> propriété de entity pour travailler en asyncrone    
            var values = await _context.Values.ToListAsync();
            //Ok reponse API
            return Ok(values);
        }

        [AllowAnonymous]
        // GET api/values/5
        [HttpGet("{id}")]
        public async Task <IActionResult> GetValue(int id)
        {          
             // ToListAsync --> propriété de entity pour travailler en asyncrone      
            var value = await _context.Values.FirstOrDefaultAsync(x=>x.Id == id);
            return Ok(value); 
        }


        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}