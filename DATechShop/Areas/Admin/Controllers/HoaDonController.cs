using DATechShop.Areas.Admin.Content;
using DATechShop.Models;
using Microsoft.Ajax.Utilities;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using static DATechShop.Areas.Admin.Content.AuthAttribute;

namespace DATechShop.Areas.Admin.Controllers
{
    public class HoaDonController : Controller
    {   DATotNghiepEntities db = new DATotNghiepEntities();

		//[AdminAuth]
		public ActionResult DanhSachHoaDon(int? page)
		{
			mapHoaDon map = new mapHoaDon();
			var data = map.allHoaDon().OrderByDescending(x => x.id_HoaDon);
			int pageSize = 6; // Số mục trên mỗi trang
			int pageNumber = (page ?? 1); // Số trang hiện tại, mặc định là trang 1 nếu không có giá trị page
			ViewBag.DbContext = new DATotNghiepEntities();

			// Sử dụng PagedList để phân trang dữ liệu
			var pagedList = data.ToPagedList(pageNumber, pageSize);

			return View(pagedList);


		}

		public ActionResult locDanhSachHoaDon(string startDate, string endDate, int? trangThaiDon, int? page)
		{
			var dbContext = new DATotNghiepEntities();
			var data = dbContext.HoaDons.AsQueryable();

			if (trangThaiDon != 0)
			{
				data = data.Where(x => x.trangThai == trangThaiDon);
			}

			if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
			{
				DateTime startDateTime = DateTime.Parse(startDate);
				DateTime endDateTime = DateTime.Parse(endDate).AddDays(1);
				data = data.Where(x => x.ngayTao >= startDateTime && x.ngayTao < endDateTime);
			}

			data = data.OrderByDescending(x => x.id_HoaDon);

			int pageSize = 6;
			int pageNumber = (page ?? 1);

			// Chuyển đổi danh sách thành IPagedList
			var pagedList = data.ToPagedList(pageNumber, pageSize);

			return View("locDanhSachHoaDon", pagedList);
		}




		[AdminAuth]
		public ActionResult GetChartData()
		{   DATotNghiepEntities db = new DATotNghiepEntities();
			var chartData = db.HoaDons
							.Where(h => h.ngayTao != null) // Lọc bỏ các giá trị null
							.GroupBy(h => new {
								Year = h.ngayTao.Value.Year,
								Month = h.ngayTao.Value.Month
							})
							.Select(g => new {
								Month = g.Key.Month + "-" + g.Key.Year, // Chuỗi biểu diễn tháng và năm
								Total = g.Sum(h => h.tongTien) // Tính tổng tổng tiền cho từng tháng
							})
							.OrderBy(g => g.Month) // Sắp xếp theo tháng
							.ToList();

			return Json(chartData, JsonRequestBehavior.AllowGet);
		}

		[AdminAuth]
		public ActionResult doiTrangThai(int value, int id_hoaDon)
		{
			using (var db = new DATotNghiepEntities())
			{
				
				var hoaDon = db.HoaDons.FirstOrDefault(d => d.id_HoaDon == id_hoaDon);

				if (hoaDon != null)
				{
					
					hoaDon.trangThai = value;

					db.SaveChanges();


					return RedirectToAction("DanhSachHoaDon", "HoaDon", new { area = "Admin" });

				}
				else
				{
					
					ViewBag.ErrorMessage = "Không tìm thấy hóa đơn có ID " + id_hoaDon;
					return View("Error");
				}
			}
		}
		[AdminAuth]
		public ActionResult ChiTietHD(int id_hoaDon, int? page)
		{

			mapHoaDon map = new mapHoaDon();
			var data = map.ChiTietHoaDon(id_hoaDon).OrderByDescending(x => x.id_HoaDon);
			int pageSize = 3;
			int pageNumber = (page ?? 1);
			var pagedList = data.ToPagedList(pageNumber, pageSize);

			ViewBag.id_chitietHoaDon = id_hoaDon;
			return View(pagedList);
		}




		[AdminAuth]
		public ActionResult danhSachLoaiThanhToan(int? page)
		{
			mapHoaDon map = new mapHoaDon();
			var data = map.DanhSachLoaiThanhToan().OrderByDescending(x => x.ngayThanhToan); 
			int pageSize = 5; 
			int pageNumber = (page ?? 1); 

			
			var pagedList = data.ToPagedList(pageNumber, pageSize);

			return View(pagedList);
		}
	

	}
}