using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication3.Models
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

    }
}
