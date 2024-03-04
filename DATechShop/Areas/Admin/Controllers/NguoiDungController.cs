using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DATechShop.Areas.Admin.Content;
using DATechShop.Models;

namespace DATechShop.Areas.Admin.Controllers
{
    public class NguoiDungController : Controller

    {
		// GET: Admin/NguoiDung
		DATotNghiepEntities db = new DATotNghiepEntities();
		[Auth]
        public ActionResult DanhSachNguoiDung()
        {
            return View();
        }

		public ActionResult DangKy()
		{
			return View();
		}

		[HttpPost]
		public ActionResult dangky(NguoiDung model, string matKhau, string mk)
		{


			if (matKhau == mk)
			{
				db.NguoiDungs.Add(model);
				db.SaveChanges();
				ViewBag.add = "Chúc mừng. Bạn đã đăng ký thành công";
				// return View();
				//them moi
				return View();
			}
			else
			{
				ViewBag.pass = "Mật Khẩu không khớp";
				return View();
			}

		}



		public ActionResult DangNhap()
		{

			return View();

		}

		[HttpPost]
		public ActionResult DangNhap(string tk, string mk)
		{
			if (tk.ToLower() == "admin" & mk.ToLower() == "1")
			{
				Session["tk"] = "Admin TechShop";
				return RedirectToAction("DanhSachNguoiDung", "NguoiDung");
			}
			else
			{
				return View();
			}
		}


	}
}