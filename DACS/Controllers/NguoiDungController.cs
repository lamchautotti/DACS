using DACS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

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
        public ActionResult Dangky(FormCollection collection, TaiKhoanND kh)
        {
            var tentk = collection["TenTK"];
            var matkhau = collection["MK"];
            var tennd = collection["TenND"];
            var MatkhauXacNhan = collection["MatkhauXacNhan"];
            var dienthoai = collection["SDT"];
            var email = collection["Mail"];
            var cccd = collection["CCCD"];
            var MaND = collection["MND"];
            var checkEmail = data.TaiKhoanNDs.FirstOrDefault(x => x.Mail == email);
            var checkTendangnhap = data.TaiKhoanNDs.FirstOrDefault(x => x.TenTK == tentk);
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
                kh.TenTK = tentk;
                kh.MK = matkhau;
                kh.TenND = tennd;
                kh.SDT = dienthoai;
                kh.Mail = email;
                kh.CCCD = cccd;
                kh.MND = 2;
                data.TaiKhoanNDs.InsertOnSubmit(kh);
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
            var tentk = collection["TenTK"];
            var matkhau = collection["MK"];
            TaiKhoanND kh = data.TaiKhoanNDs.SingleOrDefault(n => n.TenTK == tentk && n.MK == matkhau);
            if (kh != null)
            {
                if(kh.MND == 2)
                {
                    Session["TaiKhoanND"] = kh;
                    return RedirectToAction("Index", "Home");
                }
                else
                    Session["TaiKhoanND"] = kh;
                    return RedirectToAction("ListDichVu", "DichVu");
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
            Session["TaiKhoanND"]=null;
            return RedirectToAction("DangNhap", "NguoiDung");
        }
    }
}