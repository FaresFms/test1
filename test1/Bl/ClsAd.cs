using Microsoft.EntityFrameworkCore;
using test1.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace test1.Bl
{
    public interface IAd
    {
        string Click(int Id);
        int Count(bool Exp);
        TbAd GetAdById(int Id);
        string Delete(int id);
        List<ShowAdModel> Show(bool Exp,int id);
    }

    public class ClsAd : IAd
    {
        private readonly RentCarContext _context;
  

        public ClsAd(RentCarContext context, ILogger<ClsAd> logger)
        {
            _context = context;
          
        }

        public string Click(int Id) // هذه الدالة تزيد عدد النقرات
        {
            try
            {
                var ad = GetAdById(Id);

                ad.Hit=ad.Hit+1;
                _context.Entry(ad).State = EntityState.Modified;
                _context.SaveChanges();
                return "تم";
            }
            catch (Exception ex)
            {
                
                return ex.Message;
            }
        }

        public TbAd GetAdById(int Id)   // هذه الدالة تجلب بيانات إعلان معين حسب المعرف (Id)
        {
            try
            {
                return _context.TbAds
                    .AsNoTracking()
                    .FirstOrDefault(a => a.Id == Id &&   a.Status == "Available" &&  a.EndDate >= DateTime.Now && a.StartDate<=DateTime.Now);
            }
            catch (Exception ex)
            {
             
                return new TbAd();
            }
        }


        public string Delete(int id) // هذه الدالة تحذف إعلانًا معينًا (تغير حالته إلى "Deleted")
        {
            try
            {
                var ad = GetAdById(id);
                if (ad == null || ad.Status== "Deleted")
                {
                    return "الاعلان غير موجود";
     
                }

                ad.Status = "Deleted";
                _context.Entry(ad).State = EntityState.Modified;
                _context.SaveChanges();
                return "تم الحذف بنجاح";
            }
            catch (Exception ex)
            {
              return ex.Message;
            }
        }

        public List<ShowAdModel> Show(bool Exp, int id) // هذه الدالة تعرض قائمة بالإعلانات مع إمكانية التصفية
        {
            try
            {
                const int pageSize = 8;
                IQueryable<TbAd> query;

                if (Exp)
                    query = _context.TbAds.Where(a => a.EndDate < DateTime.Now && a.Status != "Deleted");
                else
                    query = _context.TbAds.Where(a => a.EndDate >= DateTime.Now && a.StartDate <= DateTime.Now && a.Status != "Deleted");

                if (id > 0)
                    query = query.Skip((id - 1) * pageSize).Take(pageSize);

                return query.Select(a => new ShowAdModel
                {
                    StartDate = a.StartDate,
                    EndDate = a.EndDate,
                    Description = a.Description,
                    Id = a.Id,
                    ImgName = a.ImgName,
                    Name = a.Name,
                    Url = a.Url,
                    hit = a.Hit,
                    CreateDate = a.CreateDate,
                    UpdateDate = a.UpdateDate,
                    Status = a.Status
                }).ToList();
            }
            catch
            {
                return new List<ShowAdModel>();
            }
        }

        public int Count(bool Exp)  // هذه الدالة تحسب عدد الإعلانات النشطة أو المنتهية
        {
            try
            {
                int Counter=0;
                if (Exp==true)
                {
                    
                    Counter = _context.TbAds.Where(a => a.Status != "Deleted" && a.EndDate < DateTime.Now).Count();
                }
                else
                {
                    Counter = _context.TbAds.Where(a => a.EndDate >= DateTime.Now && a.StartDate <= DateTime.Now && a.Status != "Deleted").Count();

                }


                return Counter;
            }
            catch
            {
                return 0;
            }
        }
    }
}