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


    }
}

