using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace DACS.Models
{
    public class Giohang
    {
        MyDataDataContext data = new MyDataDataContext();
        public int madv { get; set; }
        [Display(Name = "Tên dich vu")]
        public string tendv { get; set; }
        [Display(Name = "Hinh")]
        public string hinh { get; set; }
        [Display(Name = "Gói giờ")]
        public string goigio { get; set; }
        [Display(Name = "Giá ")]
        public Double gia { get; set; }
        [Display(Name = "Số lượng")]
        public int iSoluong { get; set; }
        [Display(Name = "Thành tiền")]
        public Double dThanhtien
        {
            get { return iSoluong * gia; }
        }
        public Giohang(int id)
        {
            madv = id;
            DichVu dichvu = data.DichVus.Single(n =>n.MaDV == madv);
            tendv = dichvu.TenDV;
            hinh = dichvu.Hinh;
            goigio = dichvu.GoiGio;
            iSoluong = 1;
            gia = double.Parse(dichvu.GiaDV.ToString());
        }

        public Giohang()
        {
        }
    }
}