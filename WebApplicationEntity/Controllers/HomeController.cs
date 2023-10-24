using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using WebApplicationEntity.Data;
using WebApplicationEntity.Models;

namespace WebApplicationEntity.Controllers
{
    public class HomeController : Controller
    {
        private ModelContext context = new ModelContext();

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            HomePageData pageData = new HomePageData();
            try {
                pageData.banners = context.Banners.OrderByDescending(q => q.ID).Take(3).ToArray();
            }
            catch {
                pageData.banners = new Banner[0];
            }

            try
            {
                pageData.subpicture = context.SubPictures.OrderBy(r => Guid.NewGuid()).Take(3).ToArray();
            }
            catch
            {
                pageData.subpicture = new SubPicture[0];
            }

            try
            {
                pageData.articles = context.Articles.OrderByDescending(q => q.Time).Take(3).ToArray();
            }
            catch
            {
                pageData.articles = new Article[0];
            }

            return View(pageData);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Register()
        {
            if (HttpContext.Session.GetString("UserName") == null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterAsync([Bind] User user)
        {
            if (HttpContext.Session.GetString("UserName") == null)
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        User userdata = context.Users.Where(q => q.UserName == user.UserName).First();
                        if (userdata != null)
                        {
                            TempData["msg"] = "Username already exists";
                            return View(user);
                        }
                    }
                    catch {
                        try
                        {
                            user.Password = hashSHA256(user.Password);
                            user.UserRole = "m";

                            context.Add(user);
                            context.SaveChanges();
                            TempData["msg"] = "Registration Success";
                        }
                        catch
                        {

                            TempData["msg"] = "Registration Failed";
                        }
                    }
                }
            }
            return View(user);
        }

        //---

        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("UserName") == null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> LoginAsync([Bind] User user)
        {
            if (HttpContext.Session.GetString("UserName") == null)
            {
                if (user.UserName == null || user.Password == null)
                {
                    TempData["msg"] = "Please enter your Username and Password";
                    return View(user);
                }

                try
                {
                    User userdata = context.Users.Where(q => q.UserName == user.UserName && q.Password == hashSHA256(user.Password)).First();
                        TempData["msg"] = "Login Completed";
                        HttpContext.Session.SetString("UserName", user.UserName);
                        HttpContext.Session.SetString("FirstName", userdata.FirstName);
                        HttpContext.Session.SetString("LastName", userdata.LastName);
                        HttpContext.Session.SetString("Email", userdata.Email);
                        HttpContext.Session.SetString("UserRole", userdata.UserRole);
                }
                catch {
                    TempData["msg"] = "User not found / incorrect Username or Password";
                }
            }
            return View(user);
        }

        //-------------

        public ActionResult Logout()
        {

            HttpContext.Session.Clear();
            HttpContext.Session.Remove("UserName");
            HttpContext.Session.Remove("FirstName");
            HttpContext.Session.Remove("LastName");
            HttpContext.Session.Remove("Email");
            HttpContext.Session.Remove("UserRole");

            return RedirectToAction("Login");
        }

        //------------

        public IActionResult User()
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                User user = new User();
                user.UserName = HttpContext.Session.GetString("UserName");
                user.FirstName = HttpContext.Session.GetString("FirstName");
                user.LastName = HttpContext.Session.GetString("LastName");
                user.Email = HttpContext.Session.GetString("Email");
                return View(user);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        //---------------

        public async Task<IActionResult> Article(int? id)
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                if (id == null)
                {
                    try
                    {
                        Article[] allarticle = context.Articles.ToArray();
                        return View(allarticle);
                    }
                    catch {
                        Article[] allarticle = new Article[0];
                        return View(allarticle);
                    }
                }

                else
                {
                    TempData["ArticleID"] = id;
                    Article[] article = new Article[1];
                    try
                    {
                        article[0] = context.Articles.Where(q => q.ID == id).First();
                        if (article[0] == null) return NotFound();
                        return View(article);
                    }
                    catch {
                        return NotFound();
                    }
                }

            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public async Task<IActionResult> Picture(int? id)
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                if (id == null)
                {

                    try
                    {
                        Banner[] allbanner = context.Banners.ToArray();
                        return View(allbanner);
                    }
                    catch
                    {
                        Banner[] allbanner = new Banner[0];
                        return View(allbanner);
                    }

                }

                else
                {
                    TempData["PictureID"] = id;
                    Banner[] banner = new Banner[1];
                    try
                    {
                        banner[0] = context.Banners.Where(q => q.ID == id).First();
                        if (banner[0] == null) return NotFound();
                        try { banner[0].SubPictures = context.SubPictures.Where(q => q.BannerID == id).ToArray(); }
                        catch { banner[0].SubPictures = new SubPicture[0]; }
                        return View(banner);
                    }
                    catch
                    {
                        return NotFound();
                    }
                }

            }
            else
            {
                return RedirectToAction("Login");
            }
        }


        private string hashSHA256(string password)
        {
            string hash = String.Empty;
            // Initialize a SHA256 hash object
            using (SHA256 sha256 = SHA256.Create())
            {
                // Compute the hash of the given string
                byte[] hashValue = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Convert the byte array to string format
                foreach (byte b in hashValue)
                {
                    hash += $"{b:X2}";
                }
            }

            return hash;
        }
    }
}