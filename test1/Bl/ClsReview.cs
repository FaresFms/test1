using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using test1.Models;

namespace test1.Bl
{
    public interface IReview
    {
        public List<TbReview> GetReviews(int? id);
       public List<TbReview> Get5Stars();
        public string AddReview(TbReview review, int? carId);
        public string ApproveReview(int id);
        public string DeclineReview(int id);
        public string DeleteReview(int id);

        public int? Count();
        public int? ApprovedCount();
        public int? OnHoldCount();
        public  Task<List<ReviewsAdminModel>> GetReviewsAdmin(int id);


    }
    public class ClsReview : IReview
    {
    
        UserManager<ApplicationUser> UserManager;
        RentCarContext context;
        public ClsReview(RentCarContext carContext,UserManager<ApplicationUser> userManager)
        {
            context = carContext;   
            UserManager = userManager;
        }
        // إضافة تقييم جديد للنظام
        // - إذا لم يتم العثور على السيارة (carId=null) يتم تعيين قيم افتراضية
        // - يتم تعيين حالة التقييم إلى "OnHold" (قيد الانتظار) وتاريخ الإنشاء الحالي
        // - ترجع رسالة تأكيد إرسال التقييم للإدارة
        public string AddReview(TbReview review, int? carId )
        {
            try
            {
                var car = context.TbCars.FirstOrDefault(c => c.Id == carId);
                if (car == null)
                {

                    review.CarId = 0;
                    var userId = "0";
                }
                else
                {
                    review.CarId = car.Id;

               
                }
                review.CreatedDate = DateTime.Now;
                review.IsPublic = "OnHold";

                context.TbReviews.Add(review);
                context.SaveChanges();
                return "تم ارسال الرسالة الى الادمن";
            }
            catch (Exception ex)
            {
 
                return ex.ToString();
            }
        }



        // الموافقة على التقييم المحدد ( تغيير حالته إلى "Approved")
        // ترجع رسالة تأكيد الموافقة على التقييم
        public string ApproveReview(int id)
        {
            try
            {
                var review = context.TbReviews.FirstOrDefault(review => review.Id == id);
                review.IsPublic = "Approved";
                context.Entry(review).State = EntityState.Modified;

                context.SaveChanges();
                return "The message Has been approved";
            }
            catch(Exception ex) { return ex.ToString(); }
        }

        public string DeleteReview(int id)
        {
            try
            {
                var review = context.TbReviews.FirstOrDefault(review => review.Id == id);
                review.IsPublic = "Deleted";
                context.Entry(review).State = EntityState.Modified;

                context.SaveChanges();
                return "The message Has been approved";
            }
            catch (Exception ex) { return ex.ToString(); }
        }

        // رفض التقييم المحدد (تغيير حالته إلى "Declined")
        // ترجع رسالة تأكيد رفض التقييم
        public string DeclineReview(int id)
        {
            try
            {
                var review = context.TbReviews.FirstOrDefault(review => review.Id == id);
                review.IsPublic = "Declined";
                context.Entry(review).State = EntityState.Modified;

                context.SaveChanges();
                return "The message Has been Declined";
            }
            catch (Exception ex) { return ex.ToString(); }
        }
        // الحصول على قائمة بأفضل التقييمات (4 نجوم أو أكثر)
        // - تعرض فقط التقييمات المعتمدة (Approved) والتي ليست مرتبطة بسيارة معينة (CarId=0)
        // - الحد الأقصى 8 تقييمات
        public List<TbReview> Get5Stars()
        {
            try
            {
                var reviews=context.TbReviews.Where(a=>a.Stars>=4 && a.IsPublic=="Approved" && a.CarId==0 ).Take(8).ToList();
                return reviews;
            }
            catch
            {
                return new List<TbReview>();
            }
        }
        // الحصول على جميع التقييمات المعتمدة لسيارة معينة
        // - إذا كان id=null ترجع قائمة فارغة
        // - ترجع فقط التقييمات المعتمدة (Approved)
        public List<TbReview> GetReviews(int? id)
        {
            try
            {
                var reviews = context.TbReviews.Where(a => a.CarId== id && a.IsPublic == "Approved").ToList();
                return reviews;
            }
            catch
            {
                return new List<TbReview>();
            }
        }
        // الحصول على التقييمات للإدارة 
        // - تعرض التقييمات المعتمدة أو قيد الانتظار (Approved/OnHold)
        // - تقسم النتائج إلى صفحات (8 تقييمات لكل صفحة)
        // - تضيف معلومات إضافية مثل اسم المستخدم المرتبط بالتقييم
        public async Task<List<ReviewsAdminModel>> GetReviewsAdmin(int id)
        {
            try
            {
                var comments = await context.TbReviews
                    .Where(a => a.IsPublic == "Approved" || a.IsPublic == "OnHold")
                    .OrderBy(a => a.Id) // يجب إضافة ترتيب عند استخدام الترقيم
                    .Select( a => new ReviewsAdminModel
                    {
                        Id = a.Id,
                        CarId = a.CarId,
                        ContentMsg = a.ContentMsg,
                        CreatedDate = a.CreatedDate,
                        IsPublic = a.IsPublic,
                        Name = a.Name,
                        Stars = a.Stars,
                        UpdatedDate = a.UpdatedDate,
                        Number=a.Number
                        
                       
                        
                       
                    })
                    .Skip((id - 1) * 8)
                    .Take(8)
                    .AsNoTracking() // مفيد للقراءة فقط
                    .ToListAsync();
                foreach(var comment in comments)
                {
               
                    var car = context.TbCars.FirstOrDefault(a => (a.Id == comment.CarId));
                    if (car==null)
                    {
                        comment.UserId = "0";
                        comment.CarId = 0;
                        comment.UserName = "Our webSite";

                    }
                    else
                    {
                        comment.UserName = await SellerCommentAsync(comment.CarId);
                    var user = await UserManager.FindByIdAsync(car.UserId);
                    if (user != null)
                    {
                        comment.UserId= user.Id;
                    }
                    else
                    {
                        comment.UserId = "0";
                    }
                    }

                }
                return comments;
            }
            catch 
            {
              
                return new List<ReviewsAdminModel>();
            }
        }
        // حساب عدد التقييمات المعتمدة أو قيد الانتظار
        // - ترجع null إذا لم توجد تقييمات
        public int? Count()
        {
            try
            {
                var Count = context.TbReviews.Where(a =>  a.IsPublic == "Approved" || a.IsPublic=="OnHold").Count();
                if (Count==0)
                {
                    return null;
                }
                return Count;
            }
            catch
            {
                return null ;
            }
        }
        // حساب عدد التقييمات قيد الانتظار (OnHold)
        // - ترجع null إذا لم توجد تقييمات قيد الانتظار
        public int? OnHoldCount()
        {
            try
            {
                var Count = context.TbReviews.Where(a =>  a.IsPublic=="OnHold").Count();
                if (Count==0)
                {
                    return null;
                }
                return Count;
            }
            catch
            {
                return null ;
            }
        }

        // حساب عدد التقييمات المعتمدة (Approved)
        // - ترجع null إذا لم توجد تقييمات معتمدة
        public int? ApprovedCount()
        {
            try
            {
                var Count = context.TbReviews.Where(a =>  a.IsPublic == "Approved").Count();
                if (Count==0)
                {
                    return null;
                }
                return Count;
            }
            catch
            {
                return null ;
            }
        }

        // دالة مساعدة خاصة (private)
        // تحصل على اسم المستخدم المرتبط بسيارة معينة
        // تستخدم من قبل GetReviewsAdmin لإضافة معلومات إضافية
        private async Task<string> SellerCommentAsync(int? carId)
        {

            var car = context.TbCars.FirstOrDefault(a => (a.Id == carId));
            if (car != null)
            {
                var username = await UserManager.FindByIdAsync(car.UserId);
            return username.UserName;
            }
            else
            {
                return "";
            }
        }
    }
}
