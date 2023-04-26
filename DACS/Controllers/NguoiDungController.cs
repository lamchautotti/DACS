using DACS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace DACS.Controllers
{
    public class NguoiDungController : Controller
    {
        // GET: NguoiDung
        MyDataDataContext data = new MyDataDataContext();
        public bool isContainsOnlyDigits(string s)
        {
            bool containsOnlyDigits = true;

            foreach (char c in s)
            {
                if (!Char.IsDigit(c))
                {
                    containsOnlyDigits = false;
                    break;
                }
            }
            return containsOnlyDigits;
        }

        public bool ValidatePhoneNumber(string phoneNumber)
        {
            phoneNumber = phoneNumber.Replace("+84", "0");
            if (phoneNumber.Length != 10)
            {
                return false;
            }
            if (isContainsOnlyDigits(phoneNumber) == false)
            {
                return false;
            }
            return true;
        }

        public static bool ValidateVNPhoneNumber(string phoneNumber)
        {
            phoneNumber = phoneNumber.Replace("+84", "0");
            Regex regex = new Regex(@"^(0)(86|96|97|98|32|33|34|35|36|37|38|39|91|94|83|84|85|81|82|90|93|70|79|77|76|78|92|56|58|99|59|55|87)\d{7}$");
            return regex.IsMatch(phoneNumber);
        }
        public bool ValidateEmail(string email)
        {
            Regex regex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
            return regex.IsMatch(email);
        }
        public bool ValidatePassword(string password)
        {
            Regex regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");
            return regex.IsMatch(password);
        }
        public ActionResult Dangky()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Dangky(FormCollection collection, KhachHang kh)
        {
            var hoten = collection["hoten"];
            var tendangnhap = collection["tendangnhap"];
            var matkhau = collection["matkhau"];
            var MatkhauXacNhan = collection["MatkhauXacNhan"];
            var email = collection["email"];
            var diachi = collection["diachi"];
            var dienthoai = collection["dienthoai"];
            var ngaysinh = String.Format("{0:MM/dd/yyyy}", collection["ngaysinh"]);
            var checkEmail = data.KhachHangs.FirstOrDefault(x => x.email == email);
            var checkTendangnhap = data.KhachHangs.FirstOrDefault(x => x.tendangnhap == tendangnhap);
            if (String.IsNullOrEmpty(MatkhauXacNhan))
            {
                TempData["Error"] = "Phải nhập mật khẩu xác nhận!";
            }
            else if (!matkhau.Equals(MatkhauXacNhan))
            {
                TempData["Error"] = "Mật khẩu và mật khẩu xác nhận phải giống nhau";
            }
            else if (ValidatePassword(matkhau) == false)
            {
                TempData["Error"] = "Mật khẩu ít nhất 8 kí tự (Có 1 chữ hoa + thường + số + kí tự đặc biệt)";
            }
            else if (checkEmail != null)
            {
                TempData["Error"] = "Email đã tồn tại";
            }
            else if (checkTendangnhap != null)
            {
                TempData["Error"] = "Tên đăng nhập đã tồn tại";
            }
            else if (ValidateVNPhoneNumber(dienthoai) == false)
            {
                TempData["Error"] = "Số điện thoại không hợp lệ";
            }
            else if (ValidateEmail(email) == false)
            {
                TempData["Error"] = "Email không hợp lệ";
            }
            else
            {
                kh.hoten = hoten;
                kh.tendangnhap = tendangnhap;
                kh.matkhau = matkhau;
                kh.email = email;
                kh.diachi = diachi;
                kh.dienthoai = dienthoai;
                kh.ngaysinh = DateTime.Parse(ngaysinh);
                data.KhachHangs.InsertOnSubmit(kh);
                data.SubmitChanges();
                return RedirectToAction("DangNhap");
            }

            return this.Dangky();
        }
        [HttpGet]
        public ActionResult DangNhap()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DangNhap(FormCollection collection)
        {
            var tendangnhap = collection["tendangnhap"];
            var matkhau = collection["matkhau"];
            KhachHang kh = data.KhachHangs.SingleOrDefault(n => n.tendangnhap == tendangnhap && n.matkhau == matkhau);
            if (kh != null)
            {
                Session["User"] = kh;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["Error"] = "Tên đăng nhập hoặc mật khẩu không đúng";
            }
            return this.DangNhap();
        }
        [HttpGet]
        public ActionResult DangXuat()
        {
            Session.Remove("User");
            return RedirectToAction("DangNhap", "NguoiDung");
        }
    }
}