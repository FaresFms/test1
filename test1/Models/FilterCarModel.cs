using System.ComponentModel.DataAnnotations;

namespace test1.Models
{
    public class FilterCarModel
    {
        public string? Brand { get; set; } = null!;

        public string? Model { get; set; } = null!;

        public string? Status { get; set; } = null!;


        [Range(0, 500000, ErrorMessage = "Enter a valid number")]
        public int? KiloMetrage { get; set; }


        public string? Transmission { get; set; } = null!;

    
        public string? Fuel { get; set; } = null!;

        public string? Engine { get; set; } = null!;

        public string? Year { get; set; } = null!;

        [Range(0, 500000, ErrorMessage = "Enter a valid price")]

        public int? Price { get; set; }

        public string?  Color { get; set; } = null!;

    
        public string? City { get; set; }


    }
}

