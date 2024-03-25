using DATechShop.Areas.Admin.Content;
using DATechShop.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static DATechShop.Areas.Admin.Content.AuthAttribute;

namespace DATechShop.Areas.Admin.Controllers
{
    public class HoaDonController : Controller
    {   DATotNghiepEntities db = new DATotNghiepEntities();

		[AdminAuth]
		public ActionResult DanhSachHoaDon(int? page, int? trangThaiDon)
		{
			mapHoaDon map = new mapHoaDon();
			var data = map.DanhSacHoaDon(trangThaiDon).OrderByDescending(x => x.id_HoaDon); 
			int pageSize = 6; 
			int pageNumber = (page ?? 1); 
			
			var pagedList = data.ToPagedList(pageNumber, pageSize);
			ViewBag.trangThaiDon = trangThaiDon;
			return View(pagedList);

		
		}


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



	}
}