using System.ComponentModel.DataAnnotations;

namespace rent.Models
{
    public class UserModel
    {
        public string Id { get; set; }

        public string UserName { get; set; }
        [EmailAddress]
        [Required(ErrorMessage = "Please enter the Email")]

        public string Email { get; set; }

        public IEnumerable<string>? roles { get; set; }
        public string? Number { get; set; }
        public bool? IsActive { get; set; }
        public string ProfileImg {  get; set; }
        public string City { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime LastVisit { get; set; }
    }
}
