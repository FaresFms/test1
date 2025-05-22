using Microsoft.AspNetCore.Identity;
using test1.Models;

namespace test1.Bl
{
   public interface IAdmin
    {
        public Dashboard GetDashboard();

    }
    public class ClsAdmin  : IAdmin
    {
        IReview ClsReview;
        ICar clsCars;
        IAd clsAd;
        UserManager<ApplicationUser> UserManager;
        public ClsAdmin(ICar cars,IAd ad,IReview review, UserManager<ApplicationUser> _UserManager)
        {
       
            clsCars = cars; 
            clsAd = ad;
            ClsReview = review;
            UserManager = _UserManager; 
        }
        public Dashboard GetDashboard()
        {
       
            Dashboard dashboard = new Dashboard();
            try
            {
                dashboard.SoldCars = clsCars.CountSold();
                dashboard.Ads = clsAd.Count(false);
                dashboard.ExistCars = clsCars.Count(null, 0);
                dashboard.Users = UserManager.Users.Count();
                dashboard.CommentsOnHold = ClsReview.OnHoldCount();
                dashboard.ApprovedComments =ClsReview.ApprovedCount();
            }
            catch
            { 
                dashboard = new Dashboard();
            }
            return dashboard;
        }
        
    }
}
