using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DatingApp.API.Models;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly DataContext _context;
        public ValuesController(DataContext context)
        {
            _context = context;
        }

        // GET api/values
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Value>>> Get()
        {
            var values = await _context.Values.ToListAsync();
            return Ok(values);
        }

        // GET api/values/id
        [HttpGet("{id}")]
        public async Task<ActionResult<Value>> Get(int id)
        {
            var value = await _context.Values.FirstOrDefaultAsync(v => v.Id == id);
            if(value == null) 
            {
                return NotFound();
            }
            return Ok(value);
        }

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Value value)
        {
             _context.Values.Add(value);
            await _context.SaveChangesAsync();
            if(value == null) 
            {
                return Ok("Failed");
            }
            return Ok(value);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromBody]Value value, int id)
        {
            var v = await _context.Values.FirstOrDefaultAsync(x => x.Id == value.Id);
            if(id != v.Id) 
            {
                return BadRequest();
            }

            _context.Entry(value).State = EntityState.Modified;

            try 
            {
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException) 
            {
                if (!ValueExisted(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        bool ValueExisted(int id) {
            var value = _context.Values.FirstOrDefault(v => v.Id == id);
            if(value != null) {
                return true;
            }
            return false;
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Value>> Delete(int id)
        {
            var value = await _context.Values.FindAsync(id);
            if (value == null)
            {
                return NotFound();
            }

            _context.Values.Remove(value);
            await _context.SaveChangesAsync();

            return value;
        }
    }
}
