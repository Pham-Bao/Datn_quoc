using DATechShop.Areas.Admin.Content;
using DATechShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static DATechShop.Areas.Admin.Content.AuthAttribute;

namespace DATechShop.Controllers
{
    public class ThongTinController : Controller
    {
		DATotNghiepEntities db = new DATotNghiepEntities();
		// GET: ThongTin
		[UserAuth]
		public ActionResult ThongTinTaiKhoan(int id_nguoiDung)
        {
			var danhSachHoaDon = db.HoaDons.Where(hd => hd.id_NguoiDung == id_nguoiDung).ToList();
			int tongSoLuongHoaDon = danhSachHoaDon.Count;
			ViewBag.tongSoDon = tongSoLuongHoaDon;

			var nguoiDung = db.NguoiDungs.FirstOrDefault(u => u.id_NguoiDung == id_nguoiDung);

			return View(nguoiDung);
        }
    }
}