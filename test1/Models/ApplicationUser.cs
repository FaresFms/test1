using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace test1.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? ProfileImage { get; set; }
        [StringLength(17)]
        public string? city {  get; set; }

        public bool? IsActive { get; set;}
        [Required]
        [StringLength(17)]
        public string FullName {  get; set; }
        public DateTime CreationDate {  get; set; }
        public DateTime? UpdatedDate {  get; set; }
        public DateTime LastVisit {  get; set; }
        
        
    }

}
