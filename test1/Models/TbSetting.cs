using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace test1.Models;

public partial class TbSetting
{
    [Key]
    public int id { get; set; }
    [Required(ErrorMessage = "الرجاء ادخال رابط اسم الموقع")]

    public string SiteName { get; set; }

    public string? Favicon { get; set; }

    public string? Logo { get; set; }
    [Url]
    [Required(ErrorMessage = "الرجاء ادخال رابط Facebook")]

    public string Facebook { get; set; }
    [Url]
    [Required(ErrorMessage = "الرجاء ادخال رابط Instagram")]
    public string Instagram { get; set; }
    [Required(ErrorMessage = "الرجاء ادخال رابط whatsapp")]

    public string Whatsapp { get; set; }
    
    public string? HomeImg1 { get; set; }
    
    public string? HomeImg2 { get; set; }

    public string? HomeImg3 { get; set; }
    [MaxLength(500,ErrorMessage ="ادخل اقل من 500 محرف")]
    public string? HomeTxt1 { get; set; }
    [MaxLength(500, ErrorMessage = "ادخل اقل من 500 محرف")]

    public string? HomeTxt2 { get; set; }
    [MaxLength(500, ErrorMessage = "ادخل اقل من 500 محرف")]

    public string? HomeTxt3 { get; set; }
    [MaxLength(500, ErrorMessage = "ادخل اقل من 500 محرف")]

    public string? Description { get; set; }
}
