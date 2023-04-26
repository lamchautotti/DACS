using DACS.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace DACS.Controllers
{
    public class HomeController : Controller
    {
        MyDataDataContext data = new MyDataDataContext();
        public List<Sach> SearchByName(string searchString)
        {
            var all_sach = (from ss in data.Saches select ss).Where(m => m.soluongton > 0 && m.tensach.Contains(searchString)).ToList();
            return all_sach;
        }
        public ActionResult Index(int? page, string searchString)
        {
            if (page == null)
            {
                page = 1;
            }
            int page_size = 2;
            int page_num = page ?? 1;
            var all_sach = (from ss in data.Saches select ss).Where(m => m.soluongton > 0).OrderBy(m => m.masach);
            if (searchString != null)
            {
                ViewBag.Keyword = searchString;
                return View(SearchByName(searchString).ToPagedList(page_num, page_size));
            }
            return View(all_sach.ToPagedList(page_num, page_size));
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}