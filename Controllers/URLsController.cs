using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1;
using WebApplication1.Data;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class URLsController : ControllerBase
    {
        private readonly WebApplication1Context _context;

        public URLsController(WebApplication1Context context)
        {
            _context = context;
        }

        // GET: api/URLs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<URL>>> GetURL()
        {
            return await _context.URL.ToListAsync();
        }

        // GET: api/URLs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<URL>> GetURL(int id)
        {
            var uRL = await _context.URL.FindAsync(id);

            if (uRL == null)
            {
                return NotFound();
            }

            return uRL;
        }

        // PUT: api/URLs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutURL(int id, URL uRL)
        {
            if (id != uRL.Id)
            {
                return BadRequest();
            }

            _context.Entry(uRL).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!URLExists(id))
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

        // POST: api/URLs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<URL>> PostURL(URL uRL)
        {
            _context.URL.Add(uRL);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetURL", new { id = uRL.Id }, uRL);
        }

        // DELETE: api/URLs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteURL(int id)
        {
            var uRL = await _context.URL.FindAsync(id);
            if (uRL == null)
            {
                return NotFound();
            }

            _context.URL.Remove(uRL);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool URLExists(int id)
        {
            return _context.URL.Any(e => e.Id == id);
        }
        // POST: api/URLs/shorten
        [HttpPost("shorten")]
        public async Task<ActionResult<URL>> ShortenURL([FromBody] string longUrl)
        {
            if (string.IsNullOrEmpty(longUrl))
            {
                return BadRequest("URL không được để trống.");
            }

            // check URL 
            var existingUrl = await _context.URL
                .FirstOrDefaultAsync(u => u.LongUrl == longUrl);

            if (existingUrl != null)
            {
                return Ok(existingUrl); // return back to existed URL
            }

            // Create shortUrl
            string shortCode;
            do
            {
                shortCode = GenerateShortCode(6);
            }
            while (await _context.URL.AnyAsync(u => u.ShortCode == shortCode));

            // Create new object
            var newUrl = new URL
            {
                LongUrl = longUrl,
                ShortCode = shortCode,
                createAt = DateTime.UtcNow
            };

            _context.URL.Add(newUrl);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetURL", new { id = newUrl.Id }, newUrl);
        }
        // GET: api/URLs/go/{code}
        [HttpGet("go/{code}")]
        public async Task<IActionResult> RedirectTo(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return BadRequest();
            }

            // Find shortcode in db
            var urlEntry = await _context.URL
                .FirstOrDefaultAsync(u => u.ShortCode == code);

            if (urlEntry == null)
            {
                return NotFound("Không tìm thấy mã rút gọn này.");
            }

            // If it has, redirect
            return Redirect(urlEntry.LongUrl);
        }
        // Variable to create random numbers
        private string GenerateShortCode(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
