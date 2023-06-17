using DACS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DACS.Controllers
{
    public class GioHangController : Controller
    {
        // GET: GioHang
        MyDataDataContext data = new MyDataDataContext();
        public List<Giohang> Laygiohang()
        {
            List<Giohang> lstGiohang = Session["HoaDon"] as List<Giohang>;
            if (lstGiohang == null)
            {
                lstGiohang = new List<Giohang>();
                Session["HoaDon"] = lstGiohang;
            }
            return lstGiohang;
        }
        [HttpPost] //thuộc tính chỉ cho phép phương thức POST
        public ActionResult ThemGioHang(int id) // bỏ tham số strURL
        {
            List<Giohang> lstGiohang = Laygiohang();
            Giohang dichvu = lstGiohang.Find(n => n.madv == id);
            if (dichvu == null)
            {
                dichvu = new Giohang(id);
                lstGiohang.Add(dichvu);
            }
            else
            {
                dichvu.iSoluong++;
            }
            return Json(new { success = true, message = "Thêm thành công" }); // Trả về json thay vì chuyển hướng trực tiếp
        }
        public ActionResult ThemGioHang(int id, string strURL)
        {
            List<Giohang> lstGiohang = Laygiohang();
            Giohang sanpham = lstGiohang.Find(n => n.madv == id);
            if (sanpham == null)
            {
                sanpham = new Giohang(id);
                lstGiohang.Add(sanpham);
                return Redirect(strURL);
            }
            else
            {
                sanpham.iSoluong++;
                return Redirect(strURL);
            }
        }
        private int TongSoLuong()
        {
            int tsl = 0;
            List<Giohang> lstGiohang = Laygiohang();
            foreach (Giohang giohang in lstGiohang)
            {
                tsl += giohang.iSoluong;
            }
            return tsl;
        }
        [HttpPost] //thuộc tính chỉ cho phép phương thức POST

        public ActionResult TongSoLuongSanPhamJson()
        {
            int tsl = 0;
            List<Giohang> lstGiohang = Laygiohang();
            foreach (Giohang giohang in lstGiohang)
            {
                tsl += giohang.iSoluong;
            }
            return Json(new { tsl = tsl }); // Trả về json tổng số lượng sản phẩm
        }
        public int TongSoLuongSanPham()
        {
            List<Giohang> lstGiohang = Laygiohang();
            int tongSoLoai = lstGiohang.Count();

            return tongSoLoai;
        }

        private double TongTien()
        {
            double tt = 0;
            List<Giohang> lstGiohang = Laygiohang();
            foreach (Giohang giohang in lstGiohang)
            {
                tt += giohang.dThanhtien;
            }
            return tt;
        }
        public ActionResult GioHang()
        {
            List<Giohang> lstGiohang = Laygiohang();
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            ViewBag.Tongsoluongsanpham = TongSoLuongSanPham();
            return View(lstGiohang);
        }
        public ActionResult GioHangPartial()
        {
            ViewBag.Tongtien = TongTien();
            ViewBag.Tongsoluongsanpham = TongSoLuongSanPham();
            ViewBag.Tongsoluong = TongSoLuong();
            return PartialView();
        }
        public ActionResult XoaGiohang(int id)
        {
            List<Giohang> lstGiohang = Laygiohang();
            Giohang sanpham = lstGiohang.SingleOrDefault(n => n.madv == id);
            if (sanpham != null)
            {
                lstGiohang.RemoveAll(n => n.madv == id);
                return RedirectToAction("GioHang");
            }
            return RedirectToAction("GioHang");
        }
        public ActionResult CapnhatGiohang(int id, FormCollection collection)
        {
            var sach = data.DichVus.FirstOrDefault(m => m.MaDV == id); // Lấy sách dựa trên masach
            var txtSoLuong = Int32.Parse(collection["txtSoLg"]); //Số lượng từ form nhập vào
            List<Giohang> lstGiohang = Laygiohang();
            Giohang sanpham = lstGiohang.SingleOrDefault(n => n.madv == id);

            if (sanpham != null)
            {
                if (txtSoLuong > 0) // Kiểm tra số lượng lớn hơn 0
                {
                    sanpham.iSoluong = txtSoLuong; // gán số lượng cho sanpham
                }
                //else if (txtSoLuong == 0) // Nếu số lượng nhập vào là 0
                //{
                //    XoaGiohang(id); // xóa sản phẩm đó ra khỏi giỏ hàng
                //}
                else
                {
                    TempData["Error"] = "Số lượng không hợp lệ"; // lỗi số lượng âm
                }
            }
            return RedirectToAction("GioHang");
        }
        public ActionResult XoaTatCaGioHang()
        {
            List<Giohang> lstGiohang = Laygiohang();
            lstGiohang.Clear();
            return RedirectToAction("GioHang");
        }
        public ActionResult DatHang1()
        {
            List<Giohang> lstGiohang = Laygiohang();

            foreach (var sach in lstGiohang) // lấy từng item trong giỏ hàng
            {
                var searchSach = data.DichVus.FirstOrDefault(m => m.MaDV == sach.madv); // lấy sách từ database dựa vào mã sách của item
                //searchSach.soluongton = searchSach.soluongton - sach.iSoluong; // cập nhập lại số lượng tồn
                data.SubmitChanges(); // lưu lại
            }
            XoaTatCaGioHang(); // đặt xong thì xóa giỏ hàng
            return RedirectToAction("GioHang");
        }
        [HttpGet]
        public ActionResult DatHang()
        {
            if (Session["TaiKhoanND"] == null || Session["TaiKhoanND"].ToString() == "")
            {
                return RedirectToAction("DangNhap", "NguoiDung");
            }
            if (Session["HoaDon"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            List<Giohang> lstGiohang = Laygiohang();
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            ViewBag.Tongsoluongsanpham = TongSoLuongSanPham();
            return View(lstGiohang);
        }
        public ActionResult DatHang(FormCollection collection)
        {
            HoaDon hd = new HoaDon();
            TaiKhoanND kh = (TaiKhoanND)Session["TaiKhoanND"];
            DichVu s = new DichVu();
            List<Giohang> gh = Laygiohang();
            var ngaythuchien = String.Format("{0:d/M/yyyy}", collection["NgayThucHien"]);
            //if (DateTime.Parse(ngaythuchien) <= DateTime.Now)
            //{
            //    TempData["Error"] = "Ngày giao phải lớn hơn ngày đặt";
            //}
            //else
            //{
            hd.NgayLap = DateTime.Now;
            hd.NgayThucHien = Convert.ToDateTime(ngaythuchien);
            hd.TenTK= kh.TenTK;
            hd.MaTT=2;
            data.HoaDons.InsertOnSubmit(hd);
            data.SubmitChanges();
            foreach (var item in gh)
            {
                ChiTietDichVu ctdv = new ChiTietDichVu();
                ctdv.MaHD = hd.MaHD;
                ctdv.MaDV = item.madv;
                ctdv.SoLuong = item.iSoluong;
                ctdv.Gia = (decimal)item.gia;
                s = data.DichVus.Single(n => n.MaDV == item.madv);
                data.SubmitChanges();
                data.ChiTietDichVus.InsertOnSubmit(ctdv);
            }
            data.SubmitChanges();
            Session["Giohang"] = null;

            return RedirectToAction("XacnhanDonhang", "GioHang");
            //}
            return RedirectToAction("DatHang", "GioHang");
        }
        public ActionResult XacNhanDonHang()
        {
            return View();
        }
    }
}