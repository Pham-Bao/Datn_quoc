using DATechShop.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;


namespace DATechShop.Controllers
{
    public class TrangChuController : Controller
    {
        DATotNghiepEntities db = new DATotNghiepEntities();
        // GET: TrangChu
        public ActionResult Home(string filterType)
        {
			List<SanPham> products = new List<SanPham>();

			if (filterType == "bestseller")
			{
				
			}


			else if (filterType == "newarrival")
			{
				products = db.SanPhams.OrderByDescending(x => x.id_sanPham).ToList();
			}
			else if (filterType == "onsale")
			{
				products = db.SanPhams.OrderByDescending(x => x.khuyenMai).ToList();
			}
			ViewBag.FilterType = filterType;


			return View(products);
		}
        public ActionResult chiTietSP()
        {
            return View();
        }

		public ActionResult chiTietSPLoai(String loaiSP, int? value)
		{
			var chitietSPs = new DATechShop.Models.mapSP().danhSachSPLoai(loaiSP, value);
			if (loaiSP == "DienThoai")
			{

				ViewBag.loaiSPView = "Điện thoại";
				ViewBag.loaiSanPham = "DienThoai";
				return View(chitietSPs);
			}
			if (loaiSP == "LapTop")
			{
				ViewBag.loaiSPView = "LapTop";
				ViewBag.loaiSanPham = "LapTop";

				return View(chitietSPs);
			}
			if (loaiSP == "AmThanh")
			{
				ViewBag.loaiSPView = "Âm thanh";
				ViewBag.loaiSanPham = "AmThanh";

				return View(chitietSPs);
			}
			if (loaiSP == "PhuKien")
			{
				ViewBag.loaiSPView = "Phụ kiện";
				ViewBag.loaiSanPham = "PhuKien";
				return View(chitietSPs);
			}
			return View(chitietSPs);

		}

		public ActionResult LayChiTietSP(int id_sanPham)
		{
			// Tìm bản ghi trong bảng ChiTietSP theo id_sanPham
			var chiTietSP = db.ChitietSPs.FirstOrDefault(ct => ct.id_sanPham == id_sanPham);

			if (chiTietSP == null)
			{
				return HttpNotFound();
			}

			// Trả về view với dữ liệu của bản ghi tìm được
			return View(chiTietSP);
		}

		[HttpPost]
		public ActionResult DanhSachThongSo(int id_sanPham)
		{
			mapSP map = new mapSP();
			var data = map.chiTietThongSo(id_sanPham);

			// Trả về dữ liệu JSON thay vì view
			return Json(data);
		}



		[HttpPost]
		public JsonResult SubmitReview(int id_sanPham, int id_NguoiDung, int rating, string review)
		{
			try
			{
				using (var db = new DATotNghiepEntities())
				{
					// Kiểm tra xem người dùng đã mua sản phẩm hay chưa
					bool hasPurchased = db.ChiTietHoaDons.Any(cthd => cthd.HoaDon.id_NguoiDung == id_NguoiDung && cthd.ChitietSP.SanPham.id_sanPham == id_sanPham);

					if (hasPurchased)
					{
						// Thêm dữ liệu đánh giá vào cơ sở dữ liệu
						var danhGia = new DanhGia
						{
							id_sanPham = id_sanPham,
							id_NguoiDung = id_NguoiDung,
							diemDanhGia = rating,
							binhLuan = review,
							ngayDanhGia = DateTime.Now.Date
						};

						db.DanhGias.Add(danhGia);
						db.SaveChanges();
						return Json(new { success = true });
					}
					else
					{
						return Json(new { success = false, message = "Bạn cần mua sản phẩm trước khi đánh giá." });
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Inner Exception: " + ex.InnerException.Message);
				return Json(new { success = false, error = ex.Message });
			}
		}


		
	}
}
