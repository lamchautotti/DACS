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
            List<Giohang> lstGiohang = Session["Giohang"] as List<Giohang>;
            if (lstGiohang == null)
            {
                lstGiohang = new List<Giohang>();
                Session["Giohang"] = lstGiohang;
            }
            return lstGiohang;
        }
        [HttpPost] //thuộc tính chỉ cho phép phương thức POST
        public ActionResult ThemGioHang(int id) // bỏ tham số strURL
        {
            List<Giohang> lstGiohang = Laygiohang();
            Giohang sanpham = lstGiohang.Find(n => n.masach == id);
            if (sanpham == null)
            {
                sanpham = new Giohang(id);
                lstGiohang.Add(sanpham);
            }
            else
            {
                sanpham.iSoluong++;
            }
            return Json(new { success = true, message = "Thêm thành công" }); // Trả về json thay vì chuyển hướng trực tiếp
        }
        public ActionResult ThemGioHang(int id, string strURL)
        {
            List<Giohang> lstGiohang = Laygiohang();
            Giohang sanpham = lstGiohang.Find(n => n.masach == id);
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
            List<Giohang> lstGiohang = Session["GioHang"] as List<Giohang>;
            if (lstGiohang != null)
            {
                tsl = lstGiohang.Sum(n => n.iSoluong);
            }
            return tsl;
        }
        [HttpPost] //thuộc tính chỉ cho phép phương thức POST
        public ActionResult TongSoLuongSanPhamJson()
        {
            int tsl = 0;
            List<Giohang> lstGiohang = Session["GioHang"] as List<Giohang>;
            if (lstGiohang != null)
            {
                tsl = lstGiohang.Count;
            }
            return Json(new { tsl = tsl }); // Trả về json tổng số lượng sản phẩm
        }
        public int TongSoLuongSanPham()
        {
            int tsl = 0;
            List<Giohang> lstGiohang = Session["GioHang"] as List<Giohang>;
            if (lstGiohang != null)
            {
                tsl = lstGiohang.Count;
            }
            return tsl;
        }

        private double TongTien()
        {
            double tt = 0;
            List<Giohang> lstGiohang = Session["GioHang"] as List<Giohang>;
            if (lstGiohang != null)
            {
                tt = lstGiohang.Sum(n => n.dThanhtien);
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
            Giohang sanpham = lstGiohang.SingleOrDefault(n => n.masach == id);
            if (sanpham != null)
            {
                lstGiohang.RemoveAll(n => n.masach == id);
                return RedirectToAction("GioHang");
            }
            return RedirectToAction("GioHang");
        }
        public ActionResult CapnhatGiohang(int id, FormCollection collection)
        {
            var sach = data.Saches.FirstOrDefault(m => m.masach == id); // Lấy sách dựa trên masach
            var txtSoLuong = Int32.Parse(collection["txtSoLg"]); //Số lượng từ form nhập vào
            List<Giohang> lstGiohang = Laygiohang();
            Giohang sanpham = lstGiohang.SingleOrDefault(n => n.masach == id);
            if (sanpham != null)
            {
                if (txtSoLuong > 0) // Kiểm tra số lượng lớn hơn 0
                {
                    if (sach.soluongton < txtSoLuong) // Nếu số lượng tồn nhỏ hơn số lượng nhập vào
                    {
                        TempData["Error"] = "Số lượng tồn không đủ"; // Báo lỗi
                    }
                    else
                    {
                        sanpham.iSoluong = txtSoLuong; // gán số lượng cho sanpham
                    }
                }
                else if (txtSoLuong == 0) // Nếu số lượng nhập vào là 0
                {
                    XoaGiohang(id); // xóa sản phẩm đó ra khỏi giỏ hàng
                }
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
                var searchSach = data.Saches.FirstOrDefault(m => m.masach == sach.masach); // lấy sách từ database dựa vào mã sách của item
                searchSach.soluongton = searchSach.soluongton - sach.iSoluong; // cập nhập lại số lượng tồn
                data.SubmitChanges(); // lưu lại
            }
            XoaTatCaGioHang(); // đặt xong thì xóa giỏ hàng
            return RedirectToAction("GioHang");
        }
        [HttpGet]
        public ActionResult DatHang()
        {
            if (Session["User"] == null || Session["User"].ToString() == "")
            {
                return RedirectToAction("DangNhap", "NguoiDung");
            }
            if (Session["Giohang"] == null)
            {
                return RedirectToAction("Index", "Sach");
            }
            List<Giohang> lstGiohang = Laygiohang();
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            ViewBag.Tongsoluongsanpham = TongSoLuongSanPham();
            return View(lstGiohang);
        }
        public ActionResult DatHang(FormCollection collection)
        {
            DonHang dh = new DonHang();
            KhachHang kh = (KhachHang)Session["User"];
            Sach s = new Sach();
            List<Giohang> gh = Laygiohang();
            var ngaygiao = String.Format("{0:MM/dd/yyyy}", collection["NgayGiao"]);
            if (DateTime.Parse(ngaygiao) <= DateTime.Now)
            {
                TempData["Error"] = "Ngày giao phải lớn hơn ngày đặt";
            }
            else
            {
                dh.makh = kh.makh;
                dh.ngaydat = DateTime.Now;
                dh.ngaygiao = DateTime.Parse(ngaygiao);
                dh.giaohang = false;
                dh.thanhtoan = false;
                data.DonHangs.InsertOnSubmit(dh);
                data.SubmitChanges();
                foreach (var item in gh)
                {
                    ChiTietDonHang ctdh = new ChiTietDonHang();
                    ctdh.madon = dh.madon;
                    ctdh.masach = item.masach;
                    ctdh.soluong = item.iSoluong;
                    ctdh.gia = (decimal)item.giaban;
                    s = data.Saches.Single(n => n.masach == item.masach);
                    s.soluongton = ctdh.soluong;
                    data.SubmitChanges();
                    data.ChiTietDonHangs.InsertOnSubmit(ctdh);
                }
                data.SubmitChanges();
                Session["Giohang"] = null;

                return RedirectToAction("XacnhanDonhang", "GioHang");
            }
            return RedirectToAction("DatHang", "GioHang");
        }
        public ActionResult XacNhanDonHang()
        {
            return View();
        }
    }
}