using DATechShop.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DATechShop.Controllers
{
    public class TrangChuController : Controller
    {
        DATotNghiepEntities db = new DATotNghiepEntities();
        // GET: TrangChu
        public ActionResult Home()
        {
            return View();
        }
        public ActionResult chiTietSP()
        {
            return View();
        }

		public ActionResult chiTietSPLoai(String loaiSP)
		{
			var chitietSPs = new DATechShop.Models.mapSP().danhSachSPLoai(loaiSP);
			return View(chitietSPs);

		}

		public ActionResult LayChiTietSP(int id_sanPham)
		{
			// Tìm bản ghi trong bảng ChiTietSP theo id_sanPham
			var chiTietSP = db.ChitietSPs.FirstOrDefault(ct => ct.id_sanPham == id_sanPham);

			// Kiểm tra xem có bản ghi nào hay không
			if (chiTietSP == null)
			{
				// Trả về NotFound nếu không tìm thấy bản ghi
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
				// Thêm dữ liệu đánh giá vào cơ sở dữ liệu
				using (var db = new DATotNghiepEntities())
				{
					var danhGia = new DanhGia
					{
						id_sanPham = id_sanPham,
						id_NguoiDung = id_NguoiDung,
						diemDanhGia = rating,
						binhLuan = review,
						ngayDanhGia = DateTime.Now.Date // Lấy ngày hiện tại
					};

					db.DanhGias.Add(danhGia);
					db.SaveChanges();
				}

				// Trả về kết quả thành công
				return Json(new { success = true });
			}
			catch (Exception ex)
			{
				Console.WriteLine("Inner Exception: " + ex.InnerException.Message);
				return Json(new { success = false, error = ex.Message });
			}
		}


		public ActionResult timSP(string key)
		{
			var sanPhams = new DATechShop.Models.mapSP().timSP(key);
			return View(sanPhams);
		}


	}
}