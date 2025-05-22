using Microsoft.AspNetCore.Mvc;

namespace test1.Bl
{
    public class BL
    {
        public async Task<string> TryUpload(List<IFormFile>? files, string currentValue)
        {
            if (files != null && files.Any(f => f.Length > 0))
            {
                return await UploadImage(files);
            }
            return currentValue;
        }

        public async Task<string> UploadImage(List<IFormFile> files)
        {
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    string imageName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", imageName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    return imageName;
                }
            }
            return "";
        }
        public async Task<string> DeleteImageAsync(string imageName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(imageName))
                {
                    return "اسم الصورة غير صالح";
                }


                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload");
                string filePath = Path.Combine(uploadsFolder, imageName);

                if (!File.Exists(filePath))
                {
                    return "الصورة غير موجودة على السيرفر";
                }

                await Task.Run(() => File.Delete(filePath));
                return null;
            }
         
            catch (IOException ioEx)
            {
                return $"خطأ في النظام: {ioEx.Message}";
            }
            catch (Exception ex)
            {
                return $"حدث خطأ غير متوقع: {ex.Message}";
            }
        }


    }
}
