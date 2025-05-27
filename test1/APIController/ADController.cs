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
    public class ADController : ControllerBase
    {
        IAd ClsAd;
        public ADController(IAd ad)
        {
            ClsAd = ad;
        }
        // عرض جميع الإعلانات النشطة المتاحة
        // - لا تتطلب مصادقة ([AllowAnonymous])
        // - تستدعي ClsAd.Show(false,0) لاسترجاع:
        //   * جميع الإعلانات غير المنتهية (false)
        //   * بدون ترقيم الصفحات (0)
        // - ترجع:
        //   * حالة 200 مع بيانات الإعلانات عند النجاح
        //   * رسالة الخطأ عند حدوث مشكلة
        [HttpGet("ShowAd")]
        [AllowAnonymous]
        public ApiResponse Show()
        {
         
            ApiResponse response = new ApiResponse();
            try
            {
                response.data = ClsAd.Show(false,0);
            
                response.status = 200;
                return response;
            }
            catch (Exception ex) 
            {response.errorMessage = ex.Message;
                return response;

            }
           
        }



        // زيادة عدد النقرات على إعلان معين
        // - لا تتطلب مصادقة ([AllowAnonymous])
        // - تستدعي ClsAd.Click(id) ل:
        //   * زيادة عداد النقرات (Hit) للإعلان
        //   * التحقق من أن الإعلان متاح وصالح
        // - لا ترجع أي بيانات (void)
        [HttpPatch("Hit/{id}")]
        [AllowAnonymous]
        public void Hit(int id)
        {
            ClsAd.Click(id);
        }

    }
}
