using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using Newtonsoft.Json;

namespace WebApplication1.Controllers
{
    public class JokesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const string URL = "https://localhost:7036/";
        private string urlParameters;
        private HttpClient client = new HttpClient();
        public JokesController(ApplicationDbContext context)
        {
            _context = context;

            
            client.BaseAddress = new Uri(URL);
           
        }

        // GET: Jokes
        public async Task<IActionResult> Index()
        {
            List<Jokes> jokes = new List<Jokes>();

            client.DefaultRequestHeaders.Clear();
            //Define request data format
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //Sending request to find web api REST service resource Jokes using HttpClient
            HttpResponseMessage Res = await client.GetAsync("api/Jokes/");
            //Checking the response is successful or not which is sent using HttpClient
            if (Res.IsSuccessStatusCode)
            {
                //Storing the response details recieved from web api
                var EmpResponse = Res.Content.ReadAsStringAsync().Result;
                //Deserializing the response recieved from web api and storing into the joke list
                jokes = JsonConvert.DeserializeObject<List<Jokes>>(EmpResponse);
            }
            return View(jokes);
        }

        // GET: Jokes Search Form
        public async Task<IActionResult> ShowSearchForm()
        {
            return View();
        }

        // GET: Jokes Search Results
        public async Task<IActionResult> ShowSearchResults(String SearchText)
        {
            string apiloc = "api/Jokes/Search/" + SearchText;
            List<Jokes> jokes = new List<Jokes>();

            client.DefaultRequestHeaders.Clear();
            //Define request data format
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //Sending request to find web api REST service resource Jokes using HttpClient
            HttpResponseMessage Res = await client.GetAsync(apiloc);
            //Checking the response is successful or not which is sent using HttpClient
            if (Res.IsSuccessStatusCode)
            {
                //Storing the response details recieved from web api
                var EmpResponse = Res.Content.ReadAsStringAsync().Result;
                //Deserializing the response recieved from web api and storing into the joke list
                jokes = JsonConvert.DeserializeObject<List<Jokes>>(EmpResponse);
            }
            return View("Index", jokes);
        }
        // GET: Jokes/Details/5
        public async Task<IActionResult> Details(int? id)
        {

            string apiloc = "api/Jokes/" + id;   
            if (id == null)
            {
                return NotFound();
            }
            var gettask = client.GetFromJsonAsync<Jokes>(apiloc);
            gettask.Wait();

            Jokes jokes = gettask.Result;
            
            if (jokes == null)
            {
                return NotFound();
            }

            return View(jokes);
        }

        // GET: Jokes/Create

        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Jokes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Question,Answer")] Jokes jokes)
        {

            //HTTP Post
            var postTask = client.PostAsJsonAsync<Jokes>("api/Jokes/", jokes);
            postTask.Wait();

            var result = postTask.Result;
            if (result.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
        

        ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");

        return View(jokes);

            
        }

        // GET: Jokes/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            string apiloc = "api/Jokes/" + id;
            
            var gettask = client.GetFromJsonAsync<Jokes>(apiloc);
            gettask.Wait();

            Jokes jokes = gettask.Result;

            return View(jokes);

        }

        // POST: Jokes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Question,Answer")] Jokes jokes)
        {
            string apiloc = "api/Jokes/" + id;
            if (id != jokes.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                //HTTP Post
                var postTask = client.PutAsJsonAsync<Jokes>(apiloc, jokes);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }

            }
                ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");

                return View(jokes);
            
            }

        // GET: Jokes/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jokes = await _context.Jokes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jokes == null)
            {
                return NotFound();
            }

            return View(jokes);
        }

        // POST: Jokes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var jokes = await _context.Jokes.FindAsync(id);
            _context.Jokes.Remove(jokes);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JokesExists(int id)
        {
            return _context.Jokes.Any(e => e.Id == id);
        }
    }
}
