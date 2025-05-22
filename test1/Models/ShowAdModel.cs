using System.ComponentModel.DataAnnotations;

namespace test1.Models
{
    public class ShowAdModel
    {
        public int Id { get; set; }

        public string Name { get; set; }
     
        public string? Description { get; set; }

        public string? ImgName { get; set; } = null!;
   
        public string? Url { get; set; }
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
        public int? hit {  get; set; }

        public string? Status { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }
    }
}
