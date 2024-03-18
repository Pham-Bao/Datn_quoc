using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DATechShop.Areas.Admin.Content;
using DATechShop.Models;
using PagedList;

namespace DATechShop.Areas.Admin.Controllers
{
    public class NguoiDungController : Controller

    {
		// GET: Admin/NguoiDung
		DATotNghiepEntities db = new DATotNghiepEntities();
		[Auth]
		public ActionResult DanhSachNguoiDung(int? page)
		{
			mapNguoiDung map = new mapNguoiDung();
			var data = map.DanhSachNguoiDung().OrderByDescending(x => x.id_NguoiDung); // Sắp xếp theo ID hoặc trường khác nếu cần
			int pageSize = 6; // Số mục trên mỗi trang
			int pageNumber = (page ?? 1); // Số trang hiện tại, mặc định là trang 1 nếu không có giá trị page

			// Sử dụng PagedList để phân trang dữ liệu
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

			var user = db.NguoiDungs.FirstOrDefault(u => u.sdt == sdt && u.matKhau == mk);


			if (sdt.ToLower() == "admin" & mk.ToLower() == "1")
			{
				Session["tk"] = "AdminTechShop";
				return RedirectToAction("ThongKe", "SanPham");
			}
			if (user != null)
			{
				Session["id_NguoiDung"] = user.id_NguoiDung;
				Session["TenNguoiDung"] = user.ten; 
				Session["SoDienThoai"] = user.sdt;
				Session["DiaChi"] = user.diaChi;
				

				ViewBag.Error = "Đăng Nhập thành công";
				return View();
			}
			else
			{
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

			return View();
		}


	}
}