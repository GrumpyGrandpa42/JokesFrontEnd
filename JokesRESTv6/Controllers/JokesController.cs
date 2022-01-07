using JokesRESTv6.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace JokesRESTv6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JokesController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        public JokesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/<JokesController>
        [HttpGet]
        public IEnumerable<Jokes> Get()
        {
            return _context.Jokes.ToArray();
        }

        // GET api/<JokesController>/5
        [HttpGet("{id}")]
        public async Task<Jokes> GetAsync(int id)
        {
            

            var jokes = await _context.Jokes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jokes == null)
            {
                return null;
            }

            return jokes;
        }
        // GET api/<JokesController>/Search/
        [HttpGet("Search/{searchtext}")]
        public  IEnumerable<Jokes> Get(string searchtext)
        {


            var jokes = _context.Jokes.Where(j => j.Question.Contains(searchtext)).ToArray();
            if (jokes == null)
            {
                return null;
            }

            return jokes;
        }

        // POST api/<JokesController>
        [HttpPost]
        public void Post([Bind("Question,Answer")] Jokes jokes)
        {
            if (ModelState.IsValid)
            {
                _context.Add(jokes);
                _context.SaveChanges();
                
            }
        }

        // PUT api/<JokesController>/5
        [HttpPut("{id}")]
        public void Put([Bind("Id,Question,Answer")] Jokes jokes)
        {
            _context.Update(jokes);
            _context.SaveChanges();
        }

        // DELETE api/<JokesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {

            var jokes = _context.Jokes.Find(id);
            _context.Jokes.Remove(jokes);
            _context.SaveChanges();
        }
    }
}
