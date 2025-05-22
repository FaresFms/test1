using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using test1.Models;

namespace test1.Bl
{
   public interface Isettings
    {
        public TbSetting Show();
        public bool Edit(TbSetting setting);

    }
    public class ClsSettings: Isettings
    {
        string cacheKey = "site_settings";
        RentCarContext context;
        IMemoryCache memoryCache;
        public ClsSettings(RentCarContext carContext,IMemoryCache memory)
        {
            context = carContext;
            memoryCache = memory;
        }

        public TbSetting Show() //دالة تقوم بعرض اعدادات الموقع
        {
         
            try
            {
                if (!memoryCache.TryGetValue(cacheKey, out TbSetting settings))
                {
                    settings = context.TbSettings.Where(a => a.id == 1).FirstOrDefault();

                    if (settings != null)
                    {
                        memoryCache.Set(cacheKey, settings);
                    }
                }
            return settings ?? new TbSetting();
            }

        
            catch (Exception ex)
            {
                return new TbSetting();
            }
        }
    public bool Edit(TbSetting setting)
{
    try
    {
        var settings = context.TbSettings.FirstOrDefault(a => a.id == 1);
        if (settings == null) return false;

        // نصوص
        settings.Description = string.IsNullOrEmpty(setting.Description) ? settings.Description : setting.Description;
        settings.HomeTxt1 = string.IsNullOrEmpty(setting.HomeTxt1) ? settings.HomeTxt1 : setting.HomeTxt1;
        settings.HomeTxt2 = string.IsNullOrEmpty(setting.HomeTxt2) ? settings.HomeTxt2 : setting.HomeTxt2;
        settings.HomeTxt3 = string.IsNullOrEmpty(setting.HomeTxt3) ? settings.HomeTxt3 : setting.HomeTxt3;
        settings.Whatsapp = string.IsNullOrEmpty(setting.Whatsapp) ? settings.Whatsapp : setting.Whatsapp;
        settings.Facebook = string.IsNullOrEmpty(setting.Facebook) ? settings.Facebook : setting.Facebook;
        settings.Instagram = string.IsNullOrEmpty(setting.Instagram) ? settings.Instagram : setting.Instagram;
        settings.SiteName = string.IsNullOrEmpty(setting.SiteName) ? settings.SiteName   : setting.SiteName;

        // صور
        settings.Logo = string.IsNullOrEmpty(setting.Logo) ? settings.Logo : setting.Logo;
        settings.Favicon = string.IsNullOrEmpty(setting.Favicon) ? settings.Favicon : setting.Favicon;
        settings.HomeImg1 = string.IsNullOrEmpty(setting.HomeImg1) ? settings.HomeImg1 : setting.HomeImg1;
        settings.HomeImg2 = string.IsNullOrEmpty(setting.HomeImg2) ? settings.HomeImg2 : setting.HomeImg2;
        settings.HomeImg3 = string.IsNullOrEmpty(setting.HomeImg3) ? settings.HomeImg3 : setting.HomeImg3;

        context.SaveChanges();
                memoryCache.Remove(cacheKey);

                return true;
    }
    catch
    {
        return false;
    }
}
    }
}
