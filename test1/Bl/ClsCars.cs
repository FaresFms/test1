using AutoMapper;
using FuzzySharp;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using test1.Models;

namespace test1.Bl
{
    public interface ICar
    {
        public TbCar ShowCar(int? Id);
        public List<TbCar> ShowALLCars(int id);
        public List<PageCarModel> ShowEightCars(int Counter, string? userId,int Sold);
        public List<TbCar> ShowLatestCars();
        public int Count(string? userId,int Sold);
        public List<TbCar> ShowSoldCars(int id);
        public int CountSold();
        public List<TbCar> ShowFilteredCars(FilterCarModel car);
        public string DeleteCar(int Id);
        public String Sold(int id,string userid);
        public int CountAllCars();

    }
    public class ClsCars : ICar
    {
        RentCarContext context;
        IMapper mapper;
        public ClsCars(RentCarContext _context,IReview review,IMapper _Mapper)
        {
            context = _context;
            mapper = _Mapper;
           
        }
        public TbCar ShowCar(int? Id)  // جلب سيارة واحدة حسب المعرف (تتجاهل المحذوفة)
        {
            try
            {

                var cars = context.TbCars.FirstOrDefault(a =>( a.Id == Id)&&(a.IsDeleted!=true));
                return cars;
            }
            catch
            {
                return new TbCar();
            }
        }
        public List<TbCar> ShowALLCars(int id)    // جلب قائمة السيارات مع الترقيم (تتجاهل المحذوفة
        {
            try
            {
                const int pageSize = 8;

            
                var    LstCars = context.TbCars.Where(a => a.IsDeleted != true).Skip((id - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();


                return LstCars;
            }

            catch
            {
                return new List<TbCar>();
            }
        }     

        public List<TbCar> ShowSoldCars(int id)  // جلب السيارات المباعة مع الترقيم
        {
            try
            {
                const int pageSize = 8;


                var LstCars = context.TbCars.Where(a => a.IsDeleted != true && a.Sold == 1).Skip((id - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();


                return LstCars;
            }

            catch
            {
                return new List<TbCar>();
            }
        }
        /*  public List<TbCar> ShowFilteredCars(FilterCarModel car)     // فلترة السيارات حسب معايير البحث

          {
              try
              {
                  var query = context.TbCars.Where(a=> a.Sold == 0 && a.IsDeleted==false).AsQueryable();

                  if (!string.IsNullOrEmpty(car.Transmission))
                      query = query.Where(a => a.Transmission.Contains(car.Transmission) && a.Sold == 0);

                  if (!string.IsNullOrEmpty(car.Fuel))
                      query = query.Where(a => a.Fuel.Contains(car.Fuel) && a.Sold == 0);

                  if (!string.IsNullOrEmpty(car.Model))
                      query = query.Where(a => a.Model.Contains(car.Model) && a.Sold == 0);

                  if (car.Year != null)
                      query = query.Where(a => a.Year == car.Year && a.Sold == 0);


                  if (!string.IsNullOrEmpty(car.Brand))
                      query = query.Where(a => a.Brand.Contains(car.Brand) && a.Sold == 0);

                  if (!string.IsNullOrEmpty(car.Color))
                      query = query.Where(a => a.Color.Contains(car.Color) && a.Sold == 0);
                     if (!string.IsNullOrEmpty(car.Status))
                      query = query.Where(a => a.Status.Contains(car.Status) && a.Sold == 0);


                  if (car.KiloMetrage != null)
                  {
                      var min = car.KiloMetrage.Value - 5000;
                      var max = car.KiloMetrage.Value + 5000;
                      query = query.Where(a => a.KiloMetrage >= min && a.KiloMetrage <= max && a.Sold == 0);
                  }  
                  if (car.Price != null)
                  {
                      var min = car.Price.Value - 500;
                      var max = car.Price.Value + 500;
                      query = query.Where(a => a.Price >= min && a.Price <= max && a.Sold == 0);
                  }
                  return query.Where(a=>a.Sold == 0).ToList();
              }
              catch
              {
                  return new List<TbCar>();
              }
          }
          */
        public List<TbCar> ShowFilteredCars(FilterCarModel car)
        {
            try
            {
                var query = context.TbCars
                    .Where(a => a.Sold == 0 && a.IsDeleted == false)
                    .ToList(); // نحتاج لجلب البيانات هنا أولاً لأنه سيتم استخدام Fuzzy محلياً

                if (!string.IsNullOrEmpty(car.Brand))
                {
                    query = query.Where(a =>
                        Fuzz.Ratio(a.Brand.ToLower(), car.Brand.ToLower()) > 40
                    ).ToList();
                }

                if (!string.IsNullOrEmpty(car.Model))
                {
                    query = query.Where(a =>
                        Fuzz.Ratio(a.Model.ToLower(), car.Model.ToLower()) > 50
                    ).ToList();
                }

                if (!string.IsNullOrEmpty(car.Transmission))
                {
                    query = query.Where(a =>
                        Fuzz.Ratio(a.Transmission.ToLower(), car.Transmission.ToLower()) > 50
                    ).ToList();
                }

                if (!string.IsNullOrEmpty(car.Fuel))
                {
                    query = query.Where(a =>
                        Fuzz.Ratio(a.Fuel.ToLower(), car.Fuel.ToLower()) > 50
                    ).ToList();
                }

                if (!string.IsNullOrEmpty(car.Color))
                {
                    query = query.Where(a =>
                        Fuzz.Ratio(a.Color.ToLower(), car.Color.ToLower()) > 50
                    ).ToList();
                }

                if (!string.IsNullOrEmpty(car.Status))
                {
                    query = query.Where(a =>
                        Fuzz.Ratio(a.Status.ToLower(), car.Status.ToLower()) > 70
                    ).ToList();
                }

                if (car.Year != null)
                {
                    query = query.Where(a => a.Year == car.Year).ToList();
                }

                if (car.KiloMetrage != null)
                {
                    var min = car.KiloMetrage.Value - 5000;
                    var max = car.KiloMetrage.Value + 5000;
                    query = query.Where(a => a.KiloMetrage >= min && a.KiloMetrage <= max).ToList();
                }

                if (car.Price != null)
                {
                    var min = car.Price.Value - 500;
                    var max = car.Price.Value + 500;
                    query = query.Where(a => a.Price >= min && a.Price <= max).ToList();
                }

                return query;
            }
            catch
            {
                return new List<TbCar>();
            }
        }
            public string DeleteCar(int Id) //دالة لحذف السيارة
        {
            try
            {
                var car = ShowCar(Id);
                car.IsDeleted = true;
                context.Entry(car).State = EntityState.Modified;
                context.SaveChanges();
                return "تم حذف السيارة ";

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        // جلب 8 سيارات للعرض مع عدد التعليقات
        /*        public List<PageCarModel> ShowEightCars(int Counter, string? userId, int Sold)
                {
                    try
                    {

                        List<PageCarModel> carsWithReviews = new List<PageCarModel>();


                        if (userId.IsNullOrEmpty())
                        {
                            carsWithReviews = context.TbCars
            .Where(a => a.IsDeleted!=true && a.Sold!=1 ).
           Select(t => new PageCarModel
           {
               Id = t.Id,
               Brand = t.Brand,
               Model = t.Model,
               Transmission = t.Transmission,
               Status = t.Status,
               City = t.City,
               Color = t.Color,
               Engine = t.Engine,
               CreateDate = t.CreateDate,
               KiloMetrage = t.KiloMetrage,
               Img1 = t.Img1,
               Comments = context.TbReviews.Count(r => r.CarId == t.Id),
               Price = t.Price,
               UserId = t.UserId,
               Fuel = t.Fuel,
               Year = t.Year,
               UpdateDate = t.UpdateDate,

           }).Skip((Counter - 1) * 8)
                            .Take(8).ToList();


                        }
                        else
                        {
                            if (Sold == 1)
                            {


                                carsWithReviews = context.TbCars
                  .Where(t => t.IsDeleted != true && t.UserId == userId && t.Sold == 1)
                  .Select(t => new PageCarModel
                  {
                      Id = t.Id,
                      Brand = t.Brand,
                      Model = t.Model,
                      Transmission = t.Transmission,
                      Status = t.Status,
                      City = t.City,
                      Color = t.Color,
                      Engine = t.Engine,
                      CreateDate = t.CreateDate,
                      KiloMetrage = t.KiloMetrage,
                      Img1 = t.Img1,
                      Comments = context.TbReviews.Count(r => r.CarId == t.Id),
                      Price=t.Price,
                      UserId = t.UserId,
                      Fuel = t.Fuel,
                      Year = t.Year,
                      UpdateDate = t.UpdateDate,
                  }).Skip((Counter - 1) * 8)
                                .Take(8).ToList();
                            }
                            else
                            {

                                carsWithReviews = context.TbCars
                  .Where(t => t.IsDeleted != true && t.UserId == userId && t.Sold == 0)
                  .Select(t => new PageCarModel
                  {
                      Id = t.Id,
                      Brand = t.Brand,
                      Model = t.Model,
                      Transmission = t.Transmission,
                      Status = t.Status,
                      City = t.City,
                      Color = t.Color,
                      Engine = t.Engine,
                      CreateDate = t.CreateDate,
                      KiloMetrage = t.KiloMetrage,
                      Img1 = t.Img1,
                      Comments = context.TbReviews.Count(r => r.CarId == t.Id),
                      Price = t.Price,
                      UserId = t.UserId,
                      Fuel = t.Fuel,
                      Year = t.Year,
                      UpdateDate = t.UpdateDate,
                  }).Skip((Counter - 1) * 8)
                                .Take(8).ToList();
                            }
                        }

                        return carsWithReviews;
                    }
                    catch
                    {
                        return new List<PageCarModel>();
                    }
                }
        */
        public List<PageCarModel> ShowEightCars(int Counter, string? userId, int Sold)
        {
            try
            {
                List<TbCar> cars;

                if (userId.IsNullOrEmpty())
                {
                    cars = context.TbCars
                        .Where(a => a.IsDeleted != true && a.Sold != 1)
                        .Skip((Counter - 1) * 8)
                        .Take(8)
                        .ToList();
                }
                else
                {
                    if (Sold == 1)
                    {
                        cars = context.TbCars
                            .Where(t => t.IsDeleted != true && t.UserId == userId && t.Sold == 1)
                            .Skip((Counter - 1) * 8)
                            .Take(8)
                            .ToList();
                    }
                    else
                    {
                        cars = context.TbCars
                            .Where(t => t.IsDeleted != true && t.UserId == userId && t.Sold == 0)
                            .Skip((Counter - 1) * 8)
                            .Take(8)
                            .ToList();
                    }
                }

                // عمل المابينغ من TbCar إلى PageCarModel
                var mappedCars = mapper.Map<List<PageCarModel>>(cars);

                // تعبئة عدد التعليقات بشكل يدوي لأنو AutoMapper ما فيو يعرف يحسبها
                foreach (var car in mappedCars)
                {
                    car.Comments = context.TbReviews.Count(r => r.CarId == car.Id);
                }

                return mappedCars;
            }
            catch
            {
                return new List<PageCarModel>();
            }
        }

        public int Count(string? userId, int Sold)   // حساب عدد السيارات حسب حالة البيع
        {
            try
            {
                int LstCars;
                if (userId.IsNullOrEmpty())
                {
                    LstCars = context.TbCars.Where(a =>a.IsDeleted != true && a.Sold!=1).Count();

                }
                else
                {
                    if (Sold==1)
                    {
                        
                    LstCars = context.TbCars.Where(a =>a.IsDeleted != true && a.UserId==userId && a.Sold==1).Count();
                    }
                    else
                    {
                        LstCars = context.TbCars.Where(a => a.IsDeleted != true && a.UserId == userId && a.Sold == 0).Count();

                    }
                }
                return LstCars;
            }
            catch
            {
                return 0;
            }
        }
        // حساب عدد جميع السيارات (باستثناء المحذوفة)
        public int CountAllCars()
        {
            try
            {
                int LstCars;
              
                    LstCars = context.TbCars.Where(a =>a.IsDeleted != true).Count();

                return LstCars;
              
            }
            catch
            {
                return 0;
            }
        }

        // جلب أحدث 8 سيارات مضاف
        public List<TbCar> ShowLatestCars()
        {
            try
            {
                var LstCars = context.TbCars.Where(a => a.IsDeleted != true && a.Sold != 1).OrderBy(a=>a.CreateDate).Take(8).ToList();
                return LstCars;
            }
            catch
            {  
                return new List<TbCar>();
            }
        }

        public string Sold(int id,string userid)      // تحديث حالة السيارة إلى "مباعة"
        {
            try
            {
                var car = context.TbCars.FirstOrDefault(a => (a.Id == id) && (a.IsDeleted != true) && (a.Sold != 1));
                if (car == null) {
                    return "السيارة مباعة او محذوفة";
                }
                if (car.UserId==userid)
                {

                    car.Sold = 1;
                    context.SaveChanges();
                    return "تم تحديث الحالة";
                }
                else return "يجب ان تكون مالك السيارة";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        // حساب عدد السيارات المباعة
        public int CountSold()
        {
            try
            {
               
                 var    LstCars = context.TbCars.Where(a => a.IsDeleted != true && a.Sold == 1).Count();

            
                return LstCars;
            }
            catch
            {
                return 0;
            }
        }
    }
}
//comment