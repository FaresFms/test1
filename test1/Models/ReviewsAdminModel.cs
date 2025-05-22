using System.ComponentModel.DataAnnotations;

namespace test1.Models
{
    public class ReviewsAdminModel
    {
        public int Id { get; set; }
 
        public string Name { get; set; } = null!;  
        public string Number { get; set; } = null!;


        public string ContentMsg { get; set; } = null!;

        public String? IsPublic { get; set; }
  
        public int Stars { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? CarId { get; set; }
        public String? UserId { get; set; }

        public string? UserName { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
