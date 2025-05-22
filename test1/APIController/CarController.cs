using Azure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using rent.Models;
using System.Security.Claims;
using test1.Bl;
using FuzzySharp;
using test1.Models;

namespace test1.APIController
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarController : ControllerBase
    {
        RentCarContext context;
        UserManager<ApplicationUser> userManager;
        ICar ClsCar;
        IReview ClsReview;
        public CarController(RentCarContext rentCar,UserManager<ApplicationUser> user,ICar icar,IReview review)
        {
            context = rentCar;
            userManager = user;
            ClsCar = icar;
            ClsReview = review;
        }
  


        // حفظ التعديلات على سيارة
        // - رفع الصور الجديدة (3 صور كحد أقصى)
        // - التحقق من ملكية المستخدم للسيارة (أو إذا كان مدير)
        // - تحديث تاريخ التعديل
        // - حالة النجاح: 200 مع رسالة تأكيد
        // - حالة الفشل: حذف الصور المرفوعة مع رسالة خطأ


        [HttpPost("SaveEditCar/{Id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ApiResponse> SaveEditCar([FromRoute]int Id, [FromForm] TbCar? car, [FromForm] List<IFormFile>? File1, [FromForm] List<IFormFile>? File2, [FromForm] List<IFormFile>? File3)
        {
            BL bL = new BL();
            ApiResponse apiResponse = new ApiResponse();
            if (!ModelState.IsValid)
            {
                apiResponse.errorMessage = "ادخال خاطئ";
                return apiResponse;
            }
            try
            {
                var userid = userManager.GetUserId(User);
                var newCar = ClsCar.ShowCar(Id);

                if (car.UserId == userid || userid == "2aafe808-0f84-4277-8836-d413e6e23cbb")
                {

                    car.Img1 = await bL.TryUpload(File1,car.Img1);

                   
                        car.Img2 = await bL.TryUpload(File2, car.Img2);

                
                        car.Img3 = await bL.TryUpload(File3, car.Img3);

                    
                    car.UpdateDate = DateTime.Now;
                    context.Entry(car).State = EntityState.Modified;

                    await context.SaveChangesAsync();
                }
                else
                {
                    apiResponse.errorMessage = "انت لست مالك السيارة";
                    apiResponse.status = 400;
                    return apiResponse;
                }
                apiResponse.data = "تم التعديل بنجاح";
                apiResponse.status = 200;
            }
            catch (Exception ex)
            {
                apiResponse.errorMessage = ex.Message;

                // Cleanup uploaded image if creation failed
                if (!string.IsNullOrEmpty(car.Img1))
                {
                    await bL.DeleteImageAsync(car.Img1);
                }

                if (!string.IsNullOrEmpty(car.Img2))
                {
                    await bL.DeleteImageAsync(car.Img2);
                }
                if (!string.IsNullOrEmpty(car.Img3))
                {
                    await bL.DeleteImageAsync(car.Img3);
                }
            }
                return apiResponse;

        }



        // إضافة سيارة جديدة
        // - رفع 3 صور إلزامية للسيارة
        // - تعيين مالك السيارة (المستخدم الحالي)
        // - حالة النجاح: 200 مع رسالة تأكيد
        // - حالة الفشل: حذف الصور المرفوعة مع رسالة خطأ

        [HttpPost("AddCar")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<ApiResponse> AddCar([FromForm] TbCar car, [FromForm] List<IFormFile> File1 , [FromForm] List<IFormFile> File2, [FromForm] List<IFormFile> File3)
        {
            BL bL = new BL(); 
            ApiResponse apiResponse = new ApiResponse();
            if (!ModelState.IsValid)
            {

                apiResponse.status = Unauthorized();
                apiResponse.errorMessage = "ادخال غير صحيح";
                return apiResponse;
            }
            try
            {
                car.Img1=await bL.UploadImage(File1);
                car.Img2=await bL.UploadImage(File2);
                car.Img3=await bL.UploadImage(File3);
                var userid = userManager.GetUserId(User);
                car.UserId = userid;
                car.CreateDate = DateTime.Now;
                car.IsDeleted = false;
                context.TbCars.Add(car);
                await context.SaveChangesAsync();
                apiResponse.data = "تم اضافة السيارة بنجاح";
                apiResponse.status = 200;
            
            }
            catch (Exception ex)
            {
                apiResponse.errorMessage = ex.Message;

                // Cleanup uploaded image if creation failed
                if (!string.IsNullOrEmpty(car.Img1))
                {
                    await bL.DeleteImageAsync(car.Img1);
                }

                if (!string.IsNullOrEmpty(car.Img2))
                {
                    await bL.DeleteImageAsync(car.Img2);
                }
                if (!string.IsNullOrEmpty(car.Img3))
                {
                    await bL.DeleteImageAsync(car.Img3);
                }
            }
            return apiResponse;
        }

        // عرض تفاصيل سيارة معينة
        // - تشمل معلومات:
        //   * بيانات السيارة الأساسية
        //   * معلومات المالك (البيع)
        //   * التقييمات المرتبطة بالسيارة
        // - حالة النجاح: 200 مع البيانات
        // - حالة الفشل: رسالة خطأ (400 إذا لم توجد السيارة)

        [HttpGet("ShowCar/{Id}")]
        public async Task< ApiResponse> ShowCar(int Id)
        {
            ApiResponse apiResponse = new ApiResponse();
            ShowCarModel model = new ShowCarModel();
            try
            {
                var car= ClsCar.ShowCar(Id);
                if (car==null)
                {
                    apiResponse.errorMessage = "السيارة غير موجودة";
                    return apiResponse;
                }
                if (car != null)
                {

                model.Car = car;
                var userid = await userManager.FindByIdAsync(model.Car.UserId);
                
                model.User = SellerProfile(userid);
                model.Reviews = ClsReview.GetReviews(car.Id);
                apiResponse.data = model;
                apiResponse.status=200;
                }
                else
                {
                    apiResponse.errorMessage = "السيارة غير موجودة";
                    apiResponse.status = 400;
                }

            }
            catch (Exception ex) 
            {
                apiResponse.errorMessage = ex.Message;
            }
            return apiResponse;
        }

        // عرض سيارات مع فلترة
        // - تستقبل معايير الفلترة في body الطلب
        // - حالة النجاح: 200 مع قائمة السيارات

        [HttpPost("ShowFilteredCars")]
        public ApiResponse ShowFilteredCars([FromBody]FilterCarModel Fcar)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                var car = ClsCar.ShowFilteredCars(Fcar);
                apiResponse.data = car;
                apiResponse.status = 200;

            }
            catch (Exception ex)
            {
                apiResponse.errorMessage = ex.Message;
            }
            return apiResponse;
        }



        // حذف سيارة (تعيين حالة IsDeleted=true)
        // - التحقق من ملكية المستخدم للسيارة (أو إذا كان مدير)
        // - حالة النجاح: 200 مع رسالة "done"
        // - حالة الفشل: رسالة خطأ (400 إذا لم يكن مالك السيارة)

        [HttpPost("Delete/{Id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ApiResponse DeleteCar(int Id)
        {
            ApiResponse apiResponse = new ApiResponse();

            try
            {
                var userid = userManager.GetUserId(User);
                var car = ClsCar.ShowCar(Id);
                if (car==null)
                {
                    apiResponse.errorMessage = "السيارة محذوفة بالفعل";
                    return apiResponse;
                }
                if (car.UserId == userid || userid== "2aafe808-0f84-4277-8836-d413e6e23cbb")
                {
                    car.IsDeleted=true;
                    context.Entry(car).State = EntityState.Modified;
                    context.SaveChanges();
                    apiResponse.data = "done";
                    apiResponse.status = 200;
                    return apiResponse;
                }
                else
                {
                    apiResponse.errorMessage = "انت لست مالك السيارة ";
                    apiResponse.status = "400";
                    return apiResponse;
                }
            }
            catch(Exception ex) { 
                apiResponse.errorMessage = ex.Message;
            return  apiResponse;
            }
        }


        // عرض أحدث السيارات المضافة
        // - حالة النجاح: 200 مع قائمة السيارات

        [HttpGet("ShowNewCars")]
        public ApiResponse ShowNewCars()
        {
                ApiResponse response = new ApiResponse();
            try
            {
                response.data = ClsCar.ShowLatestCars();
                response.status= 200;
                return response;
            }
            catch (Exception ex) { 
                response.errorMessage = ex.Message;
            return response;
            }
        }


        // عرض سيارات مستخدم معين
        // - التحقق من أن المستخدم غير محظور
        // - تدعم التقسيم إلى صفحات
        // - حالة النجاح: 200 مع قائمة السيارات
        // - حالة الفشل: رسالة خطأ إذا كان المستخدم محظوراً

        [HttpGet("ShowUserCars/{id}/{UserId}")]
        public async Task<ApiResponse> ShowUserCarsAsync(string UserId,int id)
        {
            ApiResponse response = new ApiResponse();
                   /*  response.data = ClsCar.ShowEightCars(id,UserId,0);*/
            UserCarsModel model = new UserCarsModel();
            try
            {
                if (await IsUserActive(UserId))
                {
                 
                   
                        model.Cars = ClsCar.ShowEightCars(id, UserId, 0);
                        var userid = await userManager.FindByIdAsync(UserId);

                        model.User = SellerProfile(userid);

                    response.data = model;
                    response.status = 200;
                    }
                else
                {
                    response.errorMessage = "السمتخدم غير نشط او غير موجود";
                }
                }
            catch (Exception ex)
            {
                response.errorMessage = ex.Message;
            }
                return response;
        }




        // عرض سيارات المستخدم الحالي
        // - Sold=0: السيارات غير المباعة
        // - Sold=1: السيارات المباعة
        // - تدعم التقسيم إلى صفحات
        // - حالة النجاح: 200 مع قائمة السيارات

        [HttpGet("ShowMyCars/{Id}/{Sold}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<ApiResponse> ShowMyCars(int Id,int Sold)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                var user = await userManager.GetUserAsync(User);
                if (Sold==0)
                {
                    
                response.data = ClsCar.ShowEightCars(Id,user.Id, 0);
                }
                else
                {
                    response.data = ClsCar.ShowEightCars(Id, user.Id, 1);

                }
                response.status = 200;
             
            }
            catch (Exception ex)
            {
                response.errorMessage = ex.Message;
            }
                return response;
        }

        // حساب عدد سيارات المستخدم الحالي
        // - Sold=0: السيارات غير المباعة
        // - Sold=1: السيارات المباعة
        // - حالة النجاح: 200 مع العدد

        [HttpGet("CountMyCars/{Sold}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<ApiResponse> CountMyCars(int Sold)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                var user = await userManager.GetUserAsync(User);
                if (Sold == 0)
                {

                    response.data = ClsCar.Count(user.Id,0);
                }
                else
                {
                    response.data = ClsCar.Count(user.Id,1);
                }
             
                response.status = 200;
                return response;
            }
            catch (Exception ex)
            {
                response.errorMessage = ex.Message;
                return response;
            }
        }


        // حساب عدد سيارات مستخدم معين
        // - التحقق من أن المستخدم غير محظور
        // - حالة النجاح: 200 مع العدد
        // - حالة الفشل: رسالة خطأ إذا كان المستخدم محظوراً

        [HttpGet("CountUserCars/{UserId}")]
        public async Task< ApiResponse> CountUserCars(string UserId)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                if (await IsUserActive(UserId))
                {
                    response.data = ClsCar.Count(UserId, 0);
                }
                else
                {
                    response.errorMessage = "هذا المستخدم محظور";
                    return response;
                }
              
                response.status = 200;
                return response;
            }
            catch (Exception ex)
            {
                response.errorMessage = ex.Message;
                return response;
            }
        }

        // عرض سيارات (8 لكل صفحة)
        // - إذا وجد UserId: عرض سيارات مستخدم معين
        // - إذا لم يوجد: عرض سيارات عامة
        // - التحقق من أن المستخدم غير محظور
        // - حالة النجاح: 200 مع قائمة السيارات


        [HttpGet("ShowSixCars/{Id}/{UserId?}")]
        public async Task<ApiResponse> ShowEightCars(int Id,string? UserId)
        {
                ApiResponse response = new ApiResponse();
            try
            {
                if (!UserId.IsNullOrEmpty())
                {


                    if (await IsUserActive(UserId))
                    {

                        response.data = ClsCar.ShowEightCars(Id, UserId, 0);
                    }
                    else
                    {
                        response.errorMessage = "هذا المستخدم محظور";
                        return response;
                    }
                }
                else
                {
                    response.data = ClsCar.ShowEightCars(Id, null,0);

                }
                response.status= 200;
                return response;
            }
            catch (Exception ex) { 
                response.errorMessage = ex.Message;
            return response;
            }
        }

        // حساب عدد السيارات الإجمالي
        // - حالة النجاح: 200 مع العدد

        [HttpGet("Count")]
        public ApiResponse Count()
        {
            ApiResponse response = new ApiResponse();
            try
            {
                response.data = ClsCar.Count(null, 0);
                response.status = 200;
                return response;
            }
            catch (Exception ex)
            {
                response.errorMessage = ex.Message;
                return response;
            }
        }

        // تمييز سيارة كمباعة
        // - التحقق من ملكية المستخدم للسيارة
        // - حالة النجاح: 200 مع رسالة تأكيد

        [HttpPost("Sold/{Id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ApiResponse Sold(int id)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                var userid = userManager.GetUserId(User);

                response.data = ClsCar.Sold(id,userid);
                response.status = 200;
                return response;
            }
            catch (Exception ex)
            {
                response.errorMessage = ex.Message;
                return response;
            }
        }


        // إضافة تقييم لسيارة
        // - إذا كان carId=null: إضافة تقييم عام
        // - حالة النجاح: 200 مع رسالة تأكيد

        [HttpPost("AddComment/{carId?}")]

        public ApiResponse AddComment([FromRoute]int? carId,[FromBody] TbReview review)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                if (carId == null)
                {
                    response.data = ClsReview.AddReview(review, null);
                }
                else
                {
                    response.data = ClsReview.AddReview(review, carId);
                }
                response.status = 200;
            }
            catch (Exception ex)
            {
                response.errorMessage = ex.Message;
            }
            return response;

        }

        // إنشاء نموذج بيانات البائع
        // - يستخدم لعرض معلومات المالك في تفاصيل السيارة
        public SellerModel SellerProfile(ApplicationUser applicationUser)
        {
            if (applicationUser != null)
            {
                SellerModel model = new SellerModel()
                {
                    Id = applicationUser.Id,
                    Name = applicationUser.FullName,
                    phone = applicationUser.PhoneNumber,
                    Picture = applicationUser.ProfileImage,
                    city =applicationUser.city
                    
                };
                return model;

            }
            return new SellerModel();
        }
        // التحقق من أن المستخدم غير محظور
        // - تستخدم قبل عرض سيارات مستخدم معين
        private async Task< bool> IsUserActive(string userId)
        {
            var MyUser = await userManager.FindByIdAsync(userId);
            if (MyUser.IsActive!=false)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
