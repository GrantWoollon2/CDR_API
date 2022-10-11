using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CDR_API.Models
{
    [Index(nameof(reference), IsUnique = true)]
    public class Call
    {
        //Defining the data structure of the Calls table
        [Required]
        public int Id { get; set; }

        [Required]
        public string caller_id { get; set; }

        [Required]
        public string recipient { get; set; }

        [Required]
        public DateTime call_date { get; set; }

        [Required]
        public DateTime end_time { get; set; }

        [Required]
        public int duration { get; set; }

        [Required]
        public decimal cost { get; set; }

        [Required]
        public string reference { get; set; }

        [Required]
        public string currency { get; set; }

        [Required]
        public int type { get; set; }


        public static Call ParseFromCSV(string line)
        {
            //function to parse a line from the CSV into a valid call object.
            var call = new Call();
            string[] fields = line.Split(','); //split csv format into array

            //if the caller_id is empty in the csv file, enter it as "Unknown" into the database
            call.caller_id = (fields[0] == "") ? "Unknown" : fields[0];

            call.recipient = fields[1];

            //split call_date field into yyyy:mm:dd then convert it to DateTime format for the database
            var splitDate = fields[2].Split(new[] { "/" }, StringSplitOptions.None);
            call.call_date = new DateTime(Int32.Parse(splitDate[2]), Int32.Parse(splitDate[1]), Int32.Parse(splitDate[0]), 0, 0, 0);

            //split end_time field into hh:mm:ss then convert it to DateTime format for the database
            var splitTime = fields[3].Split(new[] { ":" }, StringSplitOptions.None);
            TimeSpan end_time = new TimeSpan(Int32.Parse(splitTime[0]), Int32.Parse(splitTime[1]), Int32.Parse(splitTime[2]));
            call.end_time = call.call_date + end_time;

            //check that the duration is a valid integer
            int parsedDuration;
            if (Int32.TryParse(fields[4], out parsedDuration))
            {
                call.duration = parsedDuration;
            }
            else
            {
                //duration not a valid integer, setting it to 0
                call.duration = 0;
            }

            //check that the cost is a valid decimal
            decimal parsedCost;
            if (decimal.TryParse(fields[5], out parsedCost))
            {
                call.cost = parsedCost;
            }
            else
            {
                //cost not a valid decimal, setting it to 0
                call.cost = 0;
            }
            Math.Round(call.cost, 3); //round cost to 3 decimal places

            call.reference = fields[6];

            call.currency = fields[7];

            call.type = Int32.Parse(fields[8]);

            return call;
        }

    }
}
