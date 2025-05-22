namespace rent.Models
{
    public class ApiResponse
    {
        public object data { get; set; }
        public object errorMessage { get; set; }
        public object status { get; set; }
        public bool? IsAdmin { get; set; }=false;
    }
}
