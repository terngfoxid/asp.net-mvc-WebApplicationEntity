using MessagePack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text.RegularExpressions;
using WebApplicationEntity.Data;
using WebApplicationEntity.Models;

namespace WebApplicationEntity.Controllers
{
    public class BackendController : Controller
    {
        private ModelContext context = new ModelContext();

        // GET: BackendController
        public ActionResult Index()
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                if (HttpContext.Session.GetString("UserRole").Equals("a")) {
                    return View();
                }
                else return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Login","Home");
            }
        }

        //Picture----------------------------------------------------
        public async Task<IActionResult> PictureManager(int? id)
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                if (HttpContext.Session.GetString("UserRole").Equals("a"))
                {
                    //do some thing
                    if (id == null)
                    {
                        Banner[] allbanner = context.Banners.ToArray();
                        return View(allbanner);
                    }
                    else
                    {
                        Banner[] banner = new Banner[1];
                        TempData["PictureManagerID"] = id;
                        banner[0] = context.Banners.Where(q => q.ID == id).First();
                        if (banner[0] == null) return NotFound();
                        try
                        {
                            banner[0].SubPictures = context.SubPictures.Where(q => q.BannerID == id).ToArray();
                        }
                        catch {
                            banner[0].SubPictures = new SubPicture[0];
                        }
                        return View(banner);
                    }
                }
                else return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        //Picture-Banner------------------------------------------------------

        public ActionResult BannerCreate() {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                if (HttpContext.Session.GetString("UserRole").Equals("a"))
                {
                    //do some thing
                    return View();
                }
                else return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BannerCreateAsync(BannerCreate banner)
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                if (HttpContext.Session.GetString("UserRole").Equals("a"))
                {
                    //do some thing            
                    if (ModelState.IsValid)
                    {
                        //save local
                        if (isImage(banner.Image))
                        {
                            banner.Path = await saveFileAsync(banner.Image, "Banners");
                            if (banner.Path == null) {
                                TempData["BannerCreate"] = "Create new Banner failed (Cant save file)";
                                return View(banner);
                            }
                            try
                            {
                                Banner buffer = new Banner();
                                buffer.Title = banner.Title;
                                buffer.Description = banner.Description;
                                buffer.Path = banner.Path;
                                context.Add(buffer);
                                context.SaveChanges();
                                TempData["BannerCreate"] = "Create Banner Completed";
                            }
                            catch {
                                TempData["BannerCreate"] = "Create new Banner failed (Cant add record)";
                            }
                            return View(banner);
                        }
                        else {
                            TempData["BannerCreate"] = "Please input Image file type";
                            return View(banner);
                        }
                    }
                    else
                    {

                        //if (Image == null || Image.Length == 0) TempData["BannerCreate"] = "Please input upload file";
                        return View(banner);
                    }
                }
                else return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public async Task<ActionResult> BannerEdit(int id)
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                if (HttpContext.Session.GetString("UserRole").Equals("a"))
                {
                    //do some thing            
                    Banner banner = context.Banners.Where(q => q.ID == id).First();
                    BannerEdit buffer = new BannerEdit();
                    buffer.Title = banner.Title;
                    buffer.Description = banner.Description;
                    buffer.Path = banner.Path;
                    return View(buffer);
                }
                else return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BannerEditAsync(int id, BannerEdit banner)
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                if (HttpContext.Session.GetString("UserRole").Equals("a"))
                {
                    //do some thing            
                    if (id == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        if (ModelState.IsValid)
                        {
                            if (banner.Image == null)
                            {
                                try
                                {
                                    var entity = context.Banners.Find(id);
                                    entity.Title = banner.Title;
                                    entity.Description = banner.Description;
                                    context.Entry(entity);
                                    context.SaveChanges();
                                    TempData["BannerUpdate"] = "Banner update success";
                                }
                                catch (Exception)
                                {
                                    TempData["BannerUpdate"] = "Banner update Failed";
                                }
                                return View(banner);
                            }
                            return View(banner);
                        }
                        else
                        {
                            try {
                                var entity = context.Banners.Find(id);
                                banner.Path = entity.Path;
                            }catch (Exception)
                            {

                            }
                            return View(banner); 
                        }
                    }
                }
                else return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public async Task<ActionResult> BannerDelete(int id)
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                if (HttpContext.Session.GetString("UserRole").Equals("a"))
                {
                    //do some thing            
                    if (id == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        try
                        {
                            Banner banner = context.Banners.Where(q => q.ID == id).First();
                            return View(banner);
                        }
                        catch
                        {
                            return NotFound();
                        }
                    }
                }
                else return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BannerDeleteAsync(int id, Banner banner)
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                if (HttpContext.Session.GetString("UserRole").Equals("a"))
                {
                    //do some thing            
                    if (id == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        try
                        {
                            context.Remove(banner);
                            context.SaveChanges();
                            TempData["BannerDelete"] = "Delete banner success";
                            return View(banner);
                        }
                        catch
                        {
                            TempData["BannerDelete"] = "Delete banner failed";
                            return View(banner);
                        }
                    }
                }
                else return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }
        //------------Sub-Picture-------------------------------------

        public async Task<IActionResult> SubPictureManager(int? id) {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                if (HttpContext.Session.GetString("UserRole").Equals("a"))
                {
                    //do some thing
                    if (id == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        try
                        {
                            Banner banner = context.Banners.Where(q => q.ID == id).First();
                            try {
                                banner.SubPictures = context.SubPictures.Where(q => q.BannerID == id).ToArray();
                            }
                            catch {
                                banner.SubPictures = new SubPicture[0];
                            }
                            TempData["PictureManagerID"] = id;
                            return View(banner);
                        }
                        catch 
                        {
                            return NotFound();
                        }
                    }
                }
                else return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SubPictureCreate(int? id, IFormFile? image) {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                if (HttpContext.Session.GetString("UserRole").Equals("a"))
                {
                    //do some thing
                    if (id == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        if (image == null || image.Length == 0) {
                            TempData["SubPictureMSG"] = "Create sub picture failed [No input file]";
                            TempData["nextroute"] = id;
                            return View();
                        }
                        //----
                        try
                        {
                            SubPicture subPicture = new SubPicture();
                            subPicture.Path = await saveFileAsync(image, "Pictures");
                            subPicture.BannerID = id;
                            context.Add(subPicture);
                            context.SaveChanges();
                            TempData["SubPictureMSG"] = "Create Sub Picture Completed";
                            TempData["nextroute"] = id;
                            return View();
                        }
                        catch 
                        {
                            TempData["SubPictureMSG"] = "Create sub picture failed";
                            TempData["nextroute"] = id;
                            return View();
                        }
                    }
                }
                else return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public async Task<IActionResult> SubPictureDelete(int? id,int? nextId) {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                if (HttpContext.Session.GetString("UserRole").Equals("a"))
                {
                    //do some thing
                    if (id == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        try
                        {
                            var entity = context.SubPictures.Find(id);
                            context.Remove(entity);
                            context.SaveChanges();
                            return RedirectToAction("SubPictureManager", "Backend", new { id = nextId });
                        }
                        catch {
                            @TempData["nextroute"] = nextId;
                            TempData["SubPictureMSG"] = "Delete Sub picture failed";
                            return View();
                        }
                        return RedirectToAction("SubPictureManager", "Backend", new { id = nextId });

                    }
                }
                else return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        //Article------------------------------------------------------
        public async Task<IActionResult> ArticleManager(int? id)
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                if (HttpContext.Session.GetString("UserRole").Equals("a"))
                {
                    //do some thing
                    if (id == null)
                    {
                        Article[] allarticle = context.Articles.ToArray();
                        return View(allarticle);
                    }
                    else
                    {
                        Article[] article = new Article[1];
                        TempData["ArticleManagerID"] = id;
                        article[0] = context.Articles.Where(q => q.ID == id).First();
                        if (article[0] == null) return NotFound();
                        return View(article);
                    }
                }
                else return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public ActionResult ArticleCreate() {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                if (HttpContext.Session.GetString("UserRole").Equals("a"))
                {
                    //do some thing            
                    return View();
                }
                else return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ArticleCreateAsync(Article article){
            if (HttpContext.Session.GetString("UserName") != null)
            {
                if (HttpContext.Session.GetString("UserRole").Equals("a"))
                {
                    //do some thing            
                    if (ModelState.IsValid)
                    {
                        //db connect and insert
                        try
                        {
                            article.Time = DateTime.Now;
                            context.Add(article);
                            context.SaveChanges();
                            TempData["ArticleCreate"] = "Create Article Completed";
                            
                        }
                        catch {
                            TempData["ArticleCreate"] = "Create Article Failed";
                        }
                        return View(article);
                    }
                    else
                    {
                        return View(article);
                    }
                }
                else return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public async Task<ActionResult> ArticleEdit(int id)
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                if (HttpContext.Session.GetString("UserRole").Equals("a"))
                {
                    //do some thing            
                    if (id == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        try
                        {
                            Article article = context.Articles.Where(q => q.ID == id).First();
                            return View(article);
                        }
                        catch { 
                            return NotFound();
                        }
                    }
                }
                else return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ArticleEditAsync(int id,Article article)
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                if (HttpContext.Session.GetString("UserRole").Equals("a"))
                {
                    //do some thing            
                    if (id == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        if (ModelState.IsValid)
                        {
                            try {
                                article.Time = DateTime.Now;
                                context.Update(article);
                                context.SaveChanges();
                                TempData["ArticleUpdate"] = "Article update success";
                            }
                            catch {
                                TempData["ArticleUpdate"] = "Article update Failed";
                            }
                            TempData["ArticleManagerID"] = id;
                            return View(article);
                        }
                        else { 
                            return View(article);
                        }
                    }
                }
                else return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public async Task<ActionResult> ArticleDelete(int id)
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                if (HttpContext.Session.GetString("UserRole").Equals("a"))
                {
                    //do some thing            
                    if (id == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        try
                        {
                            Article article = context.Articles.Where(q => q.ID == id).First();
                            return View(article);
                        }
                        catch
                        {
                            return NotFound();
                        }
                    }
                }
                else return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ArticleDeleteAsync(int id, Article article)
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                if (HttpContext.Session.GetString("UserRole").Equals("a"))
                {
                    //do some thing            
                    if (id == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        try
                        {
                            context.Remove(article);
                            context.SaveChanges();
                            TempData["ArticleDelete"] = "Delete article success";
                            return View(article);
                        }
                        catch {
                            TempData["ArticleDelete"] = "Delete article failed";
                            return View(article);
                        }
                    }
                }
                else return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }


        //check is image
        private const int ImageMinimumBytes = 512;
        private bool isImage(IFormFile file)
        {
            if (file.ContentType.ToLower() != "image/jpg" &&
                file.ContentType.ToLower() != "image/jpeg" &&
                file.ContentType.ToLower() != "image/pjpeg" &&
                file.ContentType.ToLower() != "image/gif" &&
                file.ContentType.ToLower() != "image/x-png" &&
                file.ContentType.ToLower() != "image/png")
            {
                return false;
            }

            if (Path.GetExtension(file.FileName).ToLower() != ".jpg"
            && Path.GetExtension(file.FileName).ToLower() != ".png"
            && Path.GetExtension(file.FileName).ToLower() != ".gif"
            && Path.GetExtension(file.FileName).ToLower() != ".jpeg")
            {
                return false;
            }

            try
            {
                if (!file.OpenReadStream().CanRead)
                {
                    return false;
                }
                //------------------------------------------
                //check whether the image size exceeding the limit or not
                //------------------------------------------ 
                if (file.Length < ImageMinimumBytes)
                {
                    return false;
                }

                byte[] buffer = new byte[ImageMinimumBytes];
                file.OpenReadStream().Read(buffer, 0, ImageMinimumBytes);
                string content = System.Text.Encoding.UTF8.GetString(buffer);
                if (Regex.IsMatch(content, @"<script|<html|<head|<title|<body|<pre|<table|<a\s+href|<img|<plaintext|<cross\-domain\-policy",
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline))
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

            //-------------------------------------------
            //  Try to instantiate new Bitmap, if .NET will throw exception
            //  we can assume that it's not a valid image
            //-------------------------------------------

            try
            {
                using (var bitmap = new System.Drawing.Bitmap(file.OpenReadStream()))
                {
                }
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                file.OpenReadStream().Position = 0;
            }

            return true;
        }

        private async Task<string> saveFileAsync(IFormFile file, string location) {
            string newFileName = "[" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + "]" +file.FileName;
            string path = Path.Combine("/Uploads/"+ location, Path.GetFileName(newFileName));

            try
            {
                using (Stream fileStream = new FileStream("./wwwroot" + path, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                return path;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
