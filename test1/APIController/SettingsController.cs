using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rent.Models;
using test1.Bl;
using test1.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace test1.APIController
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        Isettings ClsSettings;
        IReview ClsReview;
        IAd ClsAd;
        IAdmin ClsAdmin;
        public SettingsController(Isettings isettings,IReview review,IAd ad,IAdmin admin)
        {
            ClsAd= ad;
            ClsSettings = isettings;
            ClsReview = review;
            ClsAdmin = admin;
        }


        // عرض إعدادات الموقع
        // - لا تتطلب مصادقة ([AllowAnonymous])
        // - تستدعي ClsSettings.Show() لاسترجاع الإعدادات
        // - حالة النجاح: 200 مع بيانات الإعدادات
        // - حالة الفشل: رسالة الخطأ

        [HttpGet("Show")]
        public ApiResponse Show()
        {
                ApiResponse response = new ApiResponse();
            try
            {
                response.data = ClsSettings.Show();
                response.status = 200;
                return response;
            }
            catch (Exception ex)
            {
                response.errorMessage = ex.Message;
                return response;
            }
        }

        // تعديل إعدادات الموقع
        // - تتطلب صلاحية "Admin"
        // - تقبل تعديل:
        //   * البيانات الأساسية (setting)
        //   * أيقونة الموقع (favicon)
        //   * لوجو الموقع (Logo)
        //   * صور الصفحة الرئيسية (3 صور)
        // - تحافظ على الصور القديمة إذا لم يتم رفع صور جديدة
        // - حالة النجاح: 200 مع رسالة التأكيد
        // - حالة الفشل: 
        //   * 400 إذا كانت البيانات فارغة
        //   * 500 إذا حدث خطأ داخلي
        [HttpPost("Edit")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,Roles = "Admin")]
        public async Task<ApiResponse> Edit(
          [FromForm] TbSetting? setting,
          [FromForm] List<IFormFile>? favicon,
          [FromForm] List<IFormFile>? Logo,
          [FromForm] List<IFormFile>? HomeImg1,
          [FromForm] List<IFormFile>? HomeImg2,
          [FromForm] List<IFormFile>? HomeImg3)
        {
            BL bL=new BL();
            ApiResponse response = new ApiResponse();
            try
            {
              
                if (setting == null)
                {
                    response.errorMessage = "خطأ بالبيانات";
                    response.status = 400;
                    return response;
                }
                setting.id = 1;

             
                setting.Logo = await bL.TryUpload(Logo, setting.Logo);
                setting.Favicon = await bL.TryUpload(favicon, setting.Favicon);
                setting.HomeImg1 = await bL.TryUpload(HomeImg1, setting.HomeImg1);
                setting.HomeImg2 = await bL.TryUpload(HomeImg2, setting.HomeImg2);
                setting.HomeImg3 = await bL.TryUpload(HomeImg3, setting.HomeImg3);
           
                response.data = ClsSettings.Edit(setting);
                response.status = 200;
                return response;
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(setting.Logo))
                {
                    await bL.DeleteImageAsync(setting.Logo);
                }
                if (!string.IsNullOrEmpty(setting.Favicon))
                {
                    await bL.DeleteImageAsync(setting.Favicon);
                }
                if (!string.IsNullOrEmpty(setting.HomeImg1))
                {
                    await bL.DeleteImageAsync(setting.HomeImg1);
                }
                if (!string.IsNullOrEmpty(setting.HomeImg2))
                {
                    await bL.DeleteImageAsync(setting.HomeImg2);
                }
                if (!string.IsNullOrEmpty(setting.HomeImg3))
                {
                    await bL.DeleteImageAsync(setting.HomeImg3);
                }
                response.status = 500;
                response.errorMessage = ex.Message;
                return response;
            }
        }
        // عرض أفضل التقييمات (5 نجوم)
        // - لا تتطلب مصادقة ([AllowAnonymous])
        // - تستدعي ClsReview.Get5Stars() لاسترجاع:
        //   * التقييمات ذات 4 نجوم أو أكثر
        //   * التقييمات المعتمدة فقط
        //   * الحد الأقصى 8 تقييمات
        // - حالة النجاح: 200 مع قائمة التقييمات
        [HttpGet("ShowFiveStars")]
        [AllowAnonymous]
        public ApiResponse ShowFiveStars()
        {
            ApiResponse response = new ApiResponse();
            try
            {
                response.data = ClsReview.Get5Stars();
                response.status = 200;
                return response;
            }
            catch (Exception ex)
            {
                response.errorMessage = ex.Message;
                return response;

            }

        }


        // عرض لوحة تحكم الإدارة
        // - تستدعي ClsAdmin.GetDashboard() لاسترجاع:
        //   * إحصائيات الموقع
        //   * البيانات المهمة للإدارة
        // - حالة النجاح: 200 مع بيانات لوحة التحكم

        [HttpGet("Dashboard")]

        public ApiResponse Dashboard()
        {
            ApiResponse response = new ApiResponse();

            try
            {
                response.data = ClsAdmin.GetDashboard();
                response.status = 200;

            }
            catch (Exception ex)
            {
                response.errorMessage = ex.Message;

            }
            return response;
        }

    }
}
