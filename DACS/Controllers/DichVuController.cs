using DACS.Models;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DACS.Controllers
{
    public class DichVuController : Controller
    {
        MyDataDataContext data = new MyDataDataContext();
        // GET: Dich Vu
        public List<DichVu> SearchByName(string searchString)
        {
            var all_dd = (from tt in data.DichVus select tt).Where(m => m.TenDV.Contains(searchString)).ToList();
            return all_dd;
        }
        public ActionResult ListDichVu(string searchString)
        {
            var allDichVu = from tt in data.DichVus select tt;
            if (searchString != null)
            {
                ViewBag.Keyword = searchString;
                return View(SearchByName(searchString));
            }
            return View(allDichVu);
        }
        public ActionResult Detail(int id)
        {
            //var product = GetDichVuById(id);
            ////var D_dichvu = data.DichVus.FirstOrDefault(m => m.MaDV == id);
            //return View(product);
            var D_dicvu = data.DichVus.Where(m => m.MaDV == id).First();
            return View(D_dicvu);
        }
        private DichVu GetDichVuById(int id)
        {
            var D_dichvu = data.DichVus.Where(m => m.MaDV == id).First();
            return D_dichvu;
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(FormCollection collection, DichVu s)
        {
            var E_tendichvu = collection["TenDv"];
            var E_hinh = collection["Hinh"];
            var E_gioigio = collection["GoiGio"];
            var E_gia = Convert.ToDecimal(collection["Gia"]);         
            if (string.IsNullOrEmpty(E_tendichvu))
            {
                ViewData["Error"] = "Don't empty!";
            }
            else
            {
                s.TenDV = E_tendichvu.ToString();
                s.Hinh = E_hinh.ToString();
                s.GoiGio = E_gioigio.ToString();
                s.GiaDV = E_gia;

                data.DichVus.InsertOnSubmit(s);
                data.SubmitChanges();
                return RedirectToAction("ListDichVu");
            }
            return this.Create();
        }
        public ActionResult Edit(int id)
        {
            var E_dichvu = data.DichVus.First(m => m.MaDV == id);
            return View(E_dichvu);
        }
        [HttpPost]
        public string ProcessUpload(HttpPostedFileBase file)
        {
            if (file == null)
            {
                return "";
            }
            file.SaveAs(Server.MapPath("~/Content/images/" + file.FileName));
            return "/Content/images/" + file.FileName;
        }
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            var E_dichvu = data.DichVus.First(m => m.MaDV == id);
            var E_tendichvu = collection["TenDv"];
            var E_hinh = collection["Hinh"];
            var E_goigio = collection["GoiGio"];
            var E_gia = Convert.ToDecimal(collection["Gia"]);
            E_dichvu.MaDV = id;
            if (string.IsNullOrEmpty(E_tendichvu))
            {
                ViewData["Error"] = "Don't empty!";
            }
            else
            {
                E_dichvu.TenDV = E_tendichvu;
                E_dichvu.Hinh = E_hinh;
                E_dichvu.GoiGio = E_goigio;
                E_dichvu.GiaDV = E_gia;
                UpdateModel(E_dichvu);
                data.SubmitChanges();
                return RedirectToAction("ListDichVu");
            }
            return this.Edit(id);
        }
        public ActionResult Delete(int id)
        {
            var D_dichvu = data.DichVus.First(m => m.MaDV == id);
            return View(D_dichvu);
        }
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            var D_dichvu = data.DichVus.Where(m => m.MaDV == id).First();
            data.DichVus.DeleteOnSubmit(D_dichvu);
            data.SubmitChanges();
            return RedirectToAction("ListDichVu");
        }
    }
}