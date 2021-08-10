using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using ProgrammersBlog.Entities.Dtos;
using ProgrammersBlog.Shared.Utilities.Extensions;
using ProgrammersBlog.Shared.Utilities.Results.Abstract;
using ProgrammersBlog.Shared.Utilities.Results.ComplexTypes;
using ProgrammersBlog.Shared.Utilities.Results.Concrete;
using ProgrammersBlog.WebUI.Helpers.Abstract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProgrammersBlog.WebUI.Helpers.Concrete
{
    public class ImageHelper : IImageHelper
    {
        private readonly IWebHostEnvironment _env;
        // ~/img/user.Picture
        private readonly string _wwwroot ;
        private readonly string imgFolder = "img";
        public ImageHelper(IWebHostEnvironment env)
        {
            _env = env;
            _wwwroot = _env.WebRootPath;
        }

        public IDataResult<ImageDeletedDto> Delete(string pictureName)
        {
            var fileToDelete = Path.Combine($"{_wwwroot}/{imgFolder}/", pictureName);
            if (System.IO.File.Exists(fileToDelete)) //böyle bir dosya var mı
            {
                var fileInfo = new FileInfo(fileToDelete);
                var imageDeletedDto = new ImageDeletedDto 
                {
                    FullName = pictureName,
                    Extension = fileInfo.Extension,
                    Path = fileInfo.FullName,
                    Size = fileInfo.Length
                };
                System.IO.File.Delete(fileToDelete); //dosyayı silme
                return new DataResult<ImageDeletedDto>(ResultStatus.Success,imageDeletedDto) ;
            }
            else
            {
                return new DataResult<ImageDeletedDto>(ResultStatus.Error,$"Böyle bir resim bulunamadı", null); 
            }
        }

        public async Task<IDataResult<ImageUploadedDto>> UploadedUserImage(string userName, IFormFile pictureFile, string folderName="userImages")
        {
            if (!Directory.Exists($"{_wwwroot}/{imgFolder}/{folderName}"))
            {
                Directory.CreateDirectory($"{_wwwroot}/{imgFolder}/{folderName}");
            }

            //ahmetsavas 
             string oldFileName = Path.GetFileNameWithoutExtension(pictureFile.FileName); //dosya adını uzantısı olmadan alma
            //  .png
            string fileExtension = Path.GetExtension(pictureFile.FileName);
            DateTime dateTime = DateTime.Now;
            // AhmetSavas_587_38_12_3_10_2021.png
            string newFileName = $"{userName}_{dateTime.FullDateAndTimeStringwithUnderscore()}{fileExtension}";
            var path = Path.Combine($"{_wwwroot}/{imgFolder}/{folderName}", newFileName); //resim dosyasının kayd. path
            await using (var stream = new FileStream(path, FileMode.Create))
            {
                await pictureFile.CopyToAsync(stream);
            }

            return new DataResult<ImageUploadedDto>(ResultStatus.Success, $"{userName} adlı kullanıcının resmi başarıyla yüklenmistir",
            new ImageUploadedDto{
                FullName = $"{folderName}/{newFileName}",
                OldName = oldFileName,
                Extension = fileExtension,
                FolderName = folderName,
                Path = path,
                Size = pictureFile.Length
            });
        }
    }



    

}
