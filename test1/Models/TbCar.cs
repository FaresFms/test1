using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace test1.Models;

public partial class TbCar
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string? UserId { get; set; } = null!;
    [Required]
    public string Brand { get; set; } = null!;
    [Required]
    public string Model { get; set; } = null!;

    public string Status { get; set; } = null!;
    [Required]
    [Range(0, 500000, ErrorMessage = "Enter a valid number")]
   

    public int KiloMetrage { get; set; }
    [Required]
    public string Transmission { get; set; } = null!;
    [Required]
    public string Fuel { get; set; } = null!;

    public string Engine { get; set; } = null!;
    [Required]
    public string Year { get; set; } = null!;
    [Required]
    [Range(0, 500000, ErrorMessage = "Enter a valid number")]

    public int Price { get; set; }
  
    public string? Img1 { get; set; } = null!;

    public string? Img2 { get; set; } = null!;

    public string? Img3 { get; set; } = null!;
    [StringLength(200,ErrorMessage ="ادخل اقل من 200 محرف")]
    public string? Description { get; set; } = null!;

    public int Sold { get; set; } = 0;

    public string Color { get; set; } = null!;

    public string? Country { get; set; }

    public string? City { get; set; }
    public bool? IsDeleted { get; set; }
    public DateTime? CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }
}
