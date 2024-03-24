using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DATechShop.Areas.Admin.Content;
using DATechShop.Models;
using PagedList;
using static DATechShop.Areas.Admin.Content.AuthAttribute;

namespace DATechShop.Areas.Admin.Controllers
{
    public class NguoiDungController : Controller

    {
		// GET: Admin/NguoiDung
		DATotNghiepEntities db = new DATotNghiepEntities();
		[AdminAuth]
		public ActionResult DanhSachNguoiDung(int? page)
		{
			mapNguoiDung map = new mapNguoiDung();
			var data = map.DanhSachNguoiDung().OrderByDescending(x => x.id_NguoiDung); 
			int pageSize = 6; 
			int pageNumber = (page ?? 1);

			
			var pagedList = data.ToPagedList(pageNumber, pageSize);

			return View(pagedList);
		}
		public ActionResult DangKyTaiKhoan()
		{

			return View();

		}
		[HttpPost]
		public ActionResult DangKyTaiKhoan(NguoiDung model, string matKhau, string mk)
		{
			var existingUser = db.NguoiDungs.FirstOrDefault(u => u.sdt == model.sdt);
			if (existingUser != null)
			{
				ViewBag.phone = "(*)Số điện thoại đã được sử dụng!";
				return View();
			}

			if (matKhau != mk)
			{
				ViewBag.error = "Mật Khẩu không khớp";
				return View();
			}

			
			string hashedPassword = HashingHelper.HashPassword(matKhau);
			model.matKhau = hashedPassword; 

			db.NguoiDungs.Add(model);
			db.SaveChanges();

			ViewBag.success = "Chúc mừng. Bạn đã đăng ký thành công";
			return View();
		}




		public ActionResult DangNhap()
		{

			return View();

		}

		[HttpPost]
		public ActionResult DangNhap(string sdt, string mk)
		{
			
			var user = db.NguoiDungs.FirstOrDefault(u => u.sdt == sdt);


			if (sdt.ToLower() == "admin" & mk.ToLower() == "1")
			{
				Session["tk"] = "AdminTechShop";
				return RedirectToAction("ThongKe", "SanPham");
			}
			if (user != null && HashingHelper.VerifyPassword(mk, user.matKhau))
			{
				// Đăng nhập thành công
				Session["id_NguoiDung"] = user.id_NguoiDung;
				Session["TenNguoiDung"] = user.ten;
				Session["SoDienThoai"] = user.sdt;
				Session["DiaChi"] = user.diaChi;

				ViewBag.Success = "Đăng Nhập thành công";
				TempData["SuccessMessage"] = "Thêm sản phẩm thành công";

				// Gọi hàm JavaScript để hiển thị thông báo
				ViewBag.NotificationMessage = TempData["SuccessMessage"];
				return View();
			}
			else
			{
				// Đăng nhập không thành công
				ViewBag.Error = "Số điện thoại hoặc mật khẩu không đúng.";
				return View();
			}
		}


		public ActionResult DangXuat()
		{
			Session.Remove("id_NguoiDung");
			Session.Remove("TenNguoiDung");
			Session.Remove("SoDienThoai");
			Session.Remove("DiaChi");
			Session.Remove("tk");

			return RedirectToAction("DangNhap", "NguoiDung");
		}


	}
}