using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReviewAPIProj.Data;
using ReviewAPIProj.Models;

namespace ReviewAPIProj.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestLinesController : ControllerBase
    {
        private readonly ProjContext _context;

        public RequestLinesController(ProjContext context)
        {
            _context = context;
        }



        private async Task RecalculateRequestTotal(int RequestId)
        {

            var request = await _context.Request.FindAsync(RequestId);
                if (request == null) throw new Exception("Fatal Error unable to determine Calculation");


            request.Total = (from r in _context.RequestLine
                             join p in _context.Product
                             on r.ProductId equals p.Id
                             where r.Id == RequestId
                             select new { LineTotal = r.Quantity * p.Price })
                             .Sum(x => x.LineTotal);

            request.Total = request.Total;
            await _context.SaveChangesAsync();





        }


        // GET: api/RequestLines
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RequestLine>>> GetRequestLine()
        {
            return await _context.RequestLine.ToListAsync();
        }

        // GET: api/RequestLines/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RequestLine>> GetRequestLine(int id)
        {
            var requestLine = await _context.RequestLine.FindAsync(id);

            if (requestLine == null)
            {
                return NotFound();
            }

            return requestLine;
        }

        // PUT: api/RequestLines/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRequestLine(int id, RequestLine requestLine)
        {
            if (id != requestLine.Id)
            {
                return BadRequest();
            }

            _context.Entry(requestLine).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RequestLineExists(id))
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

        // POST: api/RequestLines
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<RequestLine>> PostRequestLine(RequestLine requestLine)
        {
            _context.RequestLine.Add(requestLine);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRequestLine", new { id = requestLine.Id }, requestLine);
        }

        // DELETE: api/RequestLines/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<RequestLine>> DeleteRequestLine(int id)
        {
            var requestLine = await _context.RequestLine.FindAsync(id);
            if (requestLine == null)
            {
                return NotFound();
            }

            _context.RequestLine.Remove(requestLine);
            await _context.SaveChangesAsync();

            return requestLine;
        }

        private bool RequestLineExists(int id)
        {
            return _context.RequestLine.Any(e => e.Id == id);
        }
    }
}
