using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using CDR_API.Data;
using CDR_API.Models;
using System.Linq;
using System.Net;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Query;
using CDR_API.Data;

namespace CDR_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CallController : ControllerBase
    {
        private readonly CallDbContext _context;
        public CallController(CallDbContext context) => _context = context;

        //Endpoint to return table schema
        [HttpGet]
        public async Task<IEnumerable<Call>> Get()
            => await _context.Calls.ToListAsync();


        //Endpoint to upload a CSV file. It is then parsed with LINQ and Call.ParseFromCSV into a list then entered into the calls table.
        [HttpPost("UploadFile")] //Sets the endpoint address to /UploadFile
        public async Task<IActionResult> Post(List<IFormFile> files)
        {
            var csvPath = Path.GetTempFileName(); //Generate a temporary storage location for the file
            //Stores the file
            if (files.Count > 0 && files[0].Length > 0)
            {
                using (var stream = new FileStream(csvPath, FileMode.Create))
                {
                    await files[0].CopyToAsync(stream);
                }
                //Use LINQ to parse the file together with Call.ParseFromCSV.
                List<Call> calls = System.IO.File.ReadAllLines(csvPath)
                                      .Skip(1)
                                      .Select(k => Call.ParseFromCSV(k))
                                      .ToList();
                //Now add the list of call objects into the calls table.
                try
                {
                    await _context.AddRangeAsync(calls);
                    await _context.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception e)
                {
                    return BadRequest(new {message = "SQL error."});
                }
            }
            else
            {
                return BadRequest(new { message = "File not uploaded." });
            }
        }

        //Endpoint to take a reference string and find the matching record.
        [HttpGet("GetByReference")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)] //Allows Swagger to display what a normal 200 response looks like.
        [ProducesResponseType(typeof(int), StatusCodes.Status404NotFound)] //Same for 404 responses, which will be returned if no record matches the reference.
        public async Task<IActionResult> GetByReference(string reference)
        {
            var call = await _context.Calls.FirstOrDefaultAsync(x => x.reference == reference); //Find the first matching record, comparing the reference
            //Send 404 if no record found (null), send 200 with attached record if it is found.
            return call == null ? NotFound(new { message = "No matching record." }) : Ok(call); 
        }

        //Endpoint to take 2 dates and an optional type and count the calls in that period with the total duration.
        [HttpGet("GetCountDuration")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(int), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCountDuration(DateTime earlyDate, DateTime laterDate, int type)
        {
            //DateTime input is in the form yyyy-mm-dd
            //return error if difference in dates is more than 31 days.
            if (laterDate.Subtract(earlyDate).Days > 31) { return BadRequest(new { message = "Time period must not be more than 1 month." }); }

            List<Call> calls = null;
            if (type > 0)
            {
                //Will find calls made after earlyDate and before laterDate, matching the specified type
                calls = await _context.Calls.Where(x => ((x.call_date <= laterDate && x.call_date >= earlyDate)) && x.type == type).ToListAsync();
            }
            else
            {
                //Will find calls made after earlyDate and before laterDate, ignoring type.
                calls = await _context.Calls.Where(x => (x.call_date <= laterDate && x.call_date >= earlyDate)).ToListAsync();
            }

            //Total the durations.
            var totalDuration = 0;
            foreach (Call c in calls)
            {
                totalDuration += c.duration;
            }
            return calls == null ? NotFound(new { message = "No matching records." }) : Ok(new { count = calls.Count, totalDuration = totalDuration });
        }

        //Endpoint to take 2 dates, a caller_id and an optional type. Returns records matching caller_id/type in the period (1 month limit).
        [HttpGet("GetCallsByCallerID")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(int), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCallsByCallerID(DateTime earlyDate, DateTime laterDate, String caller_id, int type)
        {
            //return error if difference in dates is more than 31 days.
            if (laterDate.Subtract(earlyDate).Days > 31) { return BadRequest(new { message = "Time period must not be more than 1 month." }); }

            List<Call> calls = null;
            if (type > 0)
            {
                //Will find calls made after earlyDate and before laterDate, matching the specified type and caller_id.
                calls = await _context.Calls.Where(x => caller_id == x.caller_id && ((x.call_date <= laterDate && x.call_date >= earlyDate)) && x.type == type).ToListAsync();
            }
            else
            {
                //Will find calls made after earlyDate and before laterDate, matching the caller_id
                calls = await _context.Calls.Where(x => caller_id == x.caller_id && (x.call_date <= laterDate && x.call_date >= earlyDate)).ToListAsync();
            }
            return calls == null ? NotFound(new { message = "No matching records." }) : Ok(calls);
        }

        //Endpoint to take 2 dates, caller_id, rows and optional type. Returns top n rows by cost matching time period (1 month limit), caller_id and type.
        [HttpGet("GetMostExpensiveCallsByCallerID")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(int), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMostExpensiveCallsByCallerID(DateTime earlyDate, DateTime laterDate, String caller_id, int rows, int type)
        {
            //return error if difference in dates is more than 31 days.
            if (laterDate.Subtract(earlyDate).Days > 31) { return BadRequest(new { message = "Time period must not be more than 1 month." }); }

            List<Call> calls = null;
            if (type > 0)
            {
                //Will find calls made after earlyDate and before laterDate, matching the specified type, caller_id and currency as GBP, sorted in descending order
                calls = await _context.Calls.OrderByDescending(x => x.cost).Where(x => caller_id == x.caller_id && x.currency == "GBP" && ((x.call_date <= laterDate && x.call_date >= earlyDate)) && x.type == type).ToListAsync();
            }
            else
            {
                //Will find calls made after earlyDate and before laterDate, matching the caller_id and currency as GBP, sorted in descending order
                calls = await _context.Calls.OrderByDescending(x => x.cost).Where(x => caller_id == x.caller_id && x.currency == "GBP" && (x.call_date <= laterDate && x.call_date >= earlyDate)).ToListAsync();
            }

            //remove all rows except the first n rows the user entered.
            if (calls.Count > rows)
            {
                calls.RemoveRange(rows, calls.Count - rows);
            }
            return calls == null ? NotFound(new { message = "No matching records." }) : Ok(calls);
        }
    }
}

