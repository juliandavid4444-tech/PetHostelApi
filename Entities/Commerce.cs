using System;
using System.ComponentModel.DataAnnotations;

namespace PetHostelApi.Entities
{
    public class Commerce
    {
        [Key]
        public int com_id { get; set; }
        public string com_name { get; set; }
        public string com_address { get; set; }
        public string com_services { get; set; }
        public DateTime? com_createdate { get; set; }
        public Decimal? com_latitudes { get; set; }
        public Decimal? com_longitudes { get; set; }
    }
}

