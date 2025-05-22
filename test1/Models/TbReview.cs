using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace test1.Models;

public partial class TbReview
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required(ErrorMessage = "الرجاء ادخال محتوى الاسم")]

    [StringLength(20,ErrorMessage ="الرجاء ادخال اسم اقل من 20 محرف ")]
    public string Name { get; set; } = null!;
    [Required(ErrorMessage = "الرجاء ادخال محتوى رقم الهاتف ")]
    [RegularExpression(@"^09\d{8}$", ErrorMessage = "الرجاء ادخال رقم هاتف فعال")]
    public string Number { get; set; }
    [Required(ErrorMessage ="الرجاء ادخال محتوى الرسالة")]
    [StringLength(200)]
    public string ContentMsg { get; set; } = null!;

    public String? IsPublic { get; set; }
    [Required]
    [Range(0, 5, ErrorMessage = "الرجاء وضع التقييم")]

    public int Stars { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CarId { get; set; }
  
    public string? UserId { get; set; }
    public DateTime? UpdatedDate { get; set; }
}
