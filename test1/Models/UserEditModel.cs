using System.ComponentModel.DataAnnotations;

namespace test1.Models
{
    public class UserEditModel
    {

 

        public string? Name { get; set; }
        [EmailAddress]
   

        public string? Email { get; set; }

        public string? ProfileImg { get; set; }

        public string? Number { get; set; }

        public string? City { get; set; }   
    }
}
