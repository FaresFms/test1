using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rent.Models;
using test1.Bl;
using test1.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace test1.APIController
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]

    public class AdminController : ControllerBase
    {
        UserManager<ApplicationUser> userManager;
        IAd ClsAd;
        IReview ClsReview;
        RentCarContext Context;
        IAdmin clsAdmin;
        ICar ClsCar;
        public AdminController(UserManager<ApplicationUser> UserManager, IAd ad, IReview review, IAdmin Cadmin, RentCarContext rentCar, ICar clsCar)
        {
            userManager = UserManager;
            ClsAd = ad;
            ClsReview = review;
            Context = rentCar;
            clsAdmin = Cadmin;
            ClsCar = clsCar;

        }


        // حساب عدد المستخدمين المسجلين في النظام
        // - ترجع العدد الإجمالي للمستخدمين
        // - حالة النجاح: 200

        [HttpGet("CountUsers")]
        public async Task<ApiResponse> CountUsers()
        {
            ApiResponse response = new ApiResponse();
            try
            {
                var users = userManager.Users.Count();

                response.data = users;
                response.status = 200;
            }
            catch (Exception ex) { 
                response.errorMessage = ex.Message;
            }
            return response;

        }


        // استرجاع قائمة المستخدمين مع تفاصيلهم
        // - تدعم التقسيم إلى صفحات (8 مستخدمين لكل صفحة)
        // - تشمل معلومات:
        //   * الأدوار (Roles)
        //   * حالة الحساب (IsActive)
        //   * تواريخ الإنشاء/التحديث/آخر زيارة
        // - حالة النجاح: 200

        [HttpGet("GetUsers/{pageNumber}")]
        public async Task<ApiResponse> GetUsers(int pageNumber)
        {
           
            ApiResponse response = new ApiResponse();
            try
            {
                const int pageSize = 8; 

                var query = userManager.Users
                    .OrderBy(u => u.Id); 

              
                var users = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
                var userRolesViewModel = new List<UserModel>();

                foreach (var user in users)
                {
                    var roles = await userManager.GetRolesAsync(user);
                    userRolesViewModel.Add(new UserModel
                    {
                        Id = user.Id,
                        UserName = user.FullName,
                        roles = roles,
                        Number = user.PhoneNumber,
                        Email = user.Email,
                        City = user.city,
                        ProfileImg=user.ProfileImage,
                        IsActive = user.IsActive,
                        CreationDate = user.CreationDate,
                        UpdatedDate= user.UpdatedDate,
                        LastVisit = user.LastVisit
                    });
                }
                response.data = userRolesViewModel;
                response.status = 200;
            }
            catch (Exception ex)
            {
                response.errorMessage = ex.Message;
            }
            return response;
        }


        // تعطيل حساب مستخدم
        // - تغيير حالة IsActive إلى false
        // - التحقق من أن المستخدم غير معطل بالفعل
        // - حالة النجاح: 200 مع رسالة "Done"
        // - حالة الفشل: رسالة خطأ توضيحية

        [HttpPost("DeActive/{id}")]
        public async Task<ApiResponse> DeActive(string id)
        {
            ApiResponse response = new ApiResponse();
            try
            {

                var user = await userManager.FindByIdAsync(id);
                if (user.IsActive == false)
                {
                    response.errorMessage = "المستخدم غير نشط بالفعل";
                    return response;
                }
                if (user != null)
                {
                    user.IsActive = false;
                    await userManager.UpdateAsync(user);
                    response.data = "Done";
                    response.status = 200;
                }
                else
                {
                    response.errorMessage = "خطأ في معرف المستخدم";
                }

            }
            catch (Exception ex)
            {

                response.errorMessage = ex.Message;
            }
            return response;
        }



        // تفعيل حساب مستخدم معطل
        // - تغيير حالة IsActive إلى true
        // - التحقق من أن المستخدم غير مفعل بالفعل
        // - حالة النجاح: 200 مع رسالة "تم التفعيل"
        // - حالة الفشل: رسالة خطأ توضيحية

        [HttpPost("Active/{id}")]
        public async Task<ApiResponse> Active(string id)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                var user = await userManager.FindByIdAsync(id);
                if (user == null)
                {
                    response.errorMessage = " خطأ في معرف المستخدم";
                    return response;
                }
                if (user.IsActive == true)
                {
                    response.errorMessage = "المستخدم نشط بالفعل";
                    return response;
                }
                if (user != null )
                {
                    user.IsActive = true;
                    await userManager.UpdateAsync(user);
                    response.data = "تم التفعيل";
                    response.status = 200;
                }
                else
                {
                    response.errorMessage = " خطأ في معرف المستخدم";
                }

            }
            catch (Exception ex)
            {

                response.errorMessage = ex.Message;
            }
            return response;
        }


        // إنشاء إعلان جديد
        // - رفع صورة الإعلان (إلزامي)
        // - تعيين الحالة الافتراضية "Available"
        // - تنظيف الصورة المرفوعة إذا فشلت العملية
        // - حالة النجاح: رسالة "Done"
        // - حالة الفشل: رسالة خطأ مع حذف الصورة المرفوعة
        [HttpPost("CreateAd")]
        public async Task<ApiResponse> CreateAd([FromForm] TbAd tb, [FromForm] List<IFormFile> File1)
        {
            ApiResponse response = new ApiResponse();
            BL bl = new BL();
            try
            {

                if (tb == null)
                {
                    response.errorMessage = "قم بتعبئة الحقول";
                    return response;
                }

                if (File1 == null || !File1.Any())
                {
                    response.errorMessage = "الرجاء ادخال الصورة ";
                    return response;
                }


                tb.Status = "Available";
                tb.CreateDate = DateTime.Now;
                tb.Hit = 0;
                tb.ImgName = await bl.UploadImage(File1);

                await Context.TbAds.AddAsync(tb);
                await Context.SaveChangesAsync();
                response.data = "Done";

            }

            catch (Exception ex)
            {
                response.errorMessage = ex.Message;

                // Cleanup uploaded image if creation failed
                if (!string.IsNullOrEmpty(tb.ImgName))
                {
                    await bl.DeleteImageAsync(tb.ImgName);
                }


            }


            return response;

        }


        // حذف إعلان (تغيير حالته إلى "Deleted")
        // - حالة النجاح: 200 مع رسالة التأكيد
        // - حالة الفشل: رسالة الخطأ

        [HttpPost("DeleteAd/{id}")]
        public ApiResponse DeleteAd(int id)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                response.data = ClsAd.Delete(id);
                response.status = 200;
                return response;
            }
            catch (Exception ex)
            {
                response.errorMessage = ex.Message;
                return response;
            }
        }


        // الموافقة على تقييم
        // - تغيير حالة التقييم إلى "Approved"
        // - حالة النجاح: 200 مع رسالة التأكيد

        [HttpPost("ApproveComment/{id}")]
        public ApiResponse ApproveComment(int id)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                response.data = ClsReview.ApproveReview(id);
                response.status = 200;
            }
            catch (Exception ex)
            {
                response.errorMessage = ex.Message;
            }
            return response;
        }
             [HttpPost("DeleteComment/{id}")]
        public ApiResponse DeleteComment(int id)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                response.data = ClsReview.DeleteReview(id);
                response.status = 200;
            }
            catch (Exception ex)
            {
                response.errorMessage = ex.Message;
            }
            return response;
        }


        // عرض جميع التقييمات للإدارة
        // - تشمل التقييمات المعتمدة وتحت المراجعة
        // - تدعم التقسيم إلى صفحات
        // - حالة النجاح: 200
        [HttpGet("ShowALLComments/{id}")]
        public async Task< ApiResponse> ShowComments(int id)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                response.data = await ClsReview.GetReviewsAdmin(id);
                response.status = 200;
            }
            catch (Exception ex)
            {
                response.errorMessage = ex.Message;
            }
            return response;
        }

        // حساب عدد التقييمات (المعتمدة وتحت المراجعة)
        // - حالة النجاح: 200 مع العدد
        [HttpGet("CountComments")]
        public ApiResponse CountComments()
        {
            ApiResponse response = new ApiResponse();
            try
            {
                response.data = ClsReview.Count();
                response.status = 200;
            }
            catch (Exception ex)
            {
                response.errorMessage = ex.Message;
            }
            return response;
        }

        // رفض تقييم
        // - تغيير حالة التقييم إلى "Declined"
        // - حالة النجاح: 200 مع رسالة التأكيد
        [HttpPost("DeclineComment/{id}")]
        public ApiResponse DeclineComment(int id)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                response.data = ClsReview.DeclineReview(id);
                response.status = 200;
            }
            catch (Exception ex)
            {
                response.errorMessage = ex.Message;
            }
            return response;
        }



        // حساب عدد السيارات في النظام
        // - حالة النجاح: 200 مع العدد
        [HttpGet("CountAllCars")]
        public async Task<ApiResponse> CountAllCars()
        {
            ApiResponse response = new ApiResponse();
            try
            {
           

                response.data = ClsCar.CountAllCars();
                response.status = 200;
            }
            catch (Exception ex)
            {
                response.errorMessage = ex.Message;
            }
            return response;

        }



        // عرض جميع السيارات مع تفاصيلها
        // - تدعم التقسيم إلى صفحات
        // - حالة النجاح: 200
        [HttpGet("GetAllCars/{id}")]
        public async Task<ApiResponse> GetAllCars(int id)
        {
            ApiResponse response = new ApiResponse();
            try
            {


                response.data = ClsCar.ShowALLCars(id);
                response.status = 200;
            }
            catch (Exception ex)
            {
                response.errorMessage = ex.Message;
            }
            return response;

        }


        // عرض الإعلانات المنتهية الصلاحية
        // - تستخدم ClsAd.Show(true, id) للعرض
        // - تدعم التقسيم إلى صفحات
        // - حالة النجاح: 200

        [HttpGet("ShowAdExp/{id}")]

        public ApiResponse ShowAdExp(int id)
        {

            ApiResponse response = new ApiResponse();
            try
            {
                response.data = ClsAd.Show(true, id);

                response.status = 200;
                return response;
            }
            catch (Exception ex)
            {
                response.errorMessage = ex.Message;
                return response;

            }

        }




        // عرض الإعلانات النشطة
        // - تستخدم ClsAd.Show(false, id) للعرض
        // - تدعم التقسيم إلى صفحات
        // - حالة النجاح: 200
        [HttpGet("ShowAds/{id}")]

        public ApiResponse ShowAds(int id)
        {

            ApiResponse response = new ApiResponse();
            try
            {
                response.data = ClsAd.Show(false, id);

                response.status = 200;
                return response;
            }
            catch (Exception ex)
            {
                response.errorMessage = ex.Message;
                return response;

            }

        }



        // حساب عدد الإعلانات المنتهية الصلاحية
        // - حالة النجاح: 200 مع العدد
        [HttpGet("CountExpAds")]
        public async Task<ApiResponse> CountExpAds()
        {
            ApiResponse response = new ApiResponse();
            try
            {
               

                response.data = ClsAd.Count(true);
                response.status = 200;
            }
            catch (Exception ex)
            {
                response.errorMessage = ex.Message;
            }
            return response;

        }



        // حساب عدد الإعلانات النشطة
        // - حالة النجاح: 200 مع العدد
        [HttpGet("CountAds")]
        public async Task<ApiResponse> CountAds()
        {
            ApiResponse response = new ApiResponse();
            try
            {
               

                response.data = ClsAd.Count(false);
                response.status = 200;
            }
            catch (Exception ex)
            {
                response.errorMessage = ex.Message;
            }
            return response;

        }



        // عرض السيارات المباعة
        // - تدعم التقسيم إلى صفحات
        // - حالة النجاح: 200
        [HttpGet("ShowSoldCars/{Id}")]

        public async Task<ApiResponse> ShowSoldCars(int Id)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                response.data = ClsCar.ShowSoldCars(Id);
                response.status = 200;

            }
            catch (Exception ex)
            {
                response.errorMessage = ex.Message;
            }
            return response;
        }


        // حساب عدد السيارات المباعة
        // - حالة النجاح: 200 مع العدد
        [HttpGet("CountSold")]
        public async Task<ApiResponse> CountSold()
        {
            ApiResponse response = new ApiResponse();
            try
            {

                response.data = ClsCar.CountSold();



                response.status = 200;
                return response;      
            }                         
            catch (Exception ex)      
            {                         
                response.errorMessage = ex.Message;
                return response;
            }
        }


    }
}
