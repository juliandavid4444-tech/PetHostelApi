using System.ComponentModel.DataAnnotations;

namespace PetHostelApi.Entities
{
	public class User
	{
        [Key]
        public int user_id { get; set; }
        public string user_name { get; set; }
        public string user_surnames { get; set; }
        public DateTime user_createdate { get; set; }
        public string user_user { get; set; }
        public string user_password { get; set; }
    
	}
}

