namespace test1.Models
{
    public class ShowCarModel
    {
        public TbCar Car { get; set; }
        public SellerModel User { get; set; }
        public List<TbReview>? Reviews { get; set; }
    }
}
