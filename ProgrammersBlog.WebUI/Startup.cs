using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using ProgrammersBlog.Services.AutoMapper.Profiles;
using ProgrammersBlog.Services.Extensions;
using ProgrammersBlog.WebUI.AutoMapper.Profiles;
using ProgrammersBlog.WebUI.Helpers.Abstract;
using ProgrammersBlog.WebUI.Helpers.Concrete;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProgrammersBlog.WebUI
{
    public class Startup
    {
        public IConfiguration Configuration { get;} //AppSettings e erismek icin kullan.

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration; 
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews().AddRazorRuntimeCompilation().AddJsonOptions(opt=> 
            {
                opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());//ResultStatus = 1 success gibi sayýsal enum deðerlerini kullan. için
                //eðer ResultStatus === "success" gibi ResultStatus enum ý mýz varsa opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)),
                opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve; //Ic ice olan objelerde objeler birb. referans ettiklerinde sorun yasamamak icin
            }); //MVC yap. için.., ve uyg. derlemeden kaydettikten sonra sonuclarý gorucez.
            services.AddSession(); // kullanýcý oturumu ekl.
            services.AddAutoMapper(typeof(CategoryProfile),typeof(ArticleProfile),typeof(UserProfile)); //Services Katmanýnda kulland. AutoMapper Class'larýnýn oto. taranmasý            
            services.LoadMyServices(connectionString:Configuration.GetConnectionString("LocalDb")); //ServiceCollectionExtensions class içeris. yazmýs old. metod ve AppSettings iceris. olan connectionString imizi ekledik
            services.AddScoped<IImageHelper, ImageHelper>();
            services.ConfigureApplicationCookie(options => 
            {
                options.LoginPath = new PathString("/Admin/User/Login"); //user giris path i
                options.LogoutPath = new PathString("/Admin/User/Logout");// user cýkýs path i
                options.Cookie = new CookieBuilder
                {
                    Name = "ProgrammersBlog", //cookie adý
                    HttpOnly = true, //cookie bilg. ele gecirilmemesi icin
                    SameSite = SameSiteMode.Strict, //cookie bilg. sadece kendi sitemizden geld. zaman. CSRF saldýrýnýn onune gecer. 
                    SecurePolicy = CookieSecurePolicy.SameAsRequest //Always olmalý,test asamasýnda old. icin býraktýk yoksa bir gucenlik acýgý
                
                };
                options.SlidingExpiration = true; // kull. siteye girs. yapt. cookilerin zamanlý bir sek. tut.
                options.ExpireTimeSpan = System.TimeSpan.FromDays(7); //cookie nin silinme süresi
                options.AccessDeniedPath = new PathString("/Admin/User/AccessDenied"); //yetkisi olmayan bir yere giris yap. kull. icin
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseStatusCodePages(); //Hata yasad. bize yardýmcý olacak
            }
            app.UseSession(); // kullanýcý oturumu
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
          FileProvider = new PhysicalFileProvider(
         Path.Combine(env.ContentRootPath, "MyStaticFiles")),
                RequestPath = "/StaticFiles"
            });
            app.UseRouting();
            //projemize kimlik dogrulamayý eklemek icin. routing den sonra kull. cunkü routing calýt. sonra yetki sorgulama yapýlýr. 
            app.UseAuthentication();
            //yetki dogrulama
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAreaControllerRoute(
                    name:"Admin",
                    areaName:"Admin",
                    pattern:"Admin/{controller=Home}/{action=Index}/{id?}"
                    
                    );
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
