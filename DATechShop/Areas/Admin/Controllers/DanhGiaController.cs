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
    public class DanhGiaController : Controller
    {
		[AdminAuth]
		public ActionResult DanhSachDanhGia(int? page)
		{
			mapDanhGia map = new mapDanhGia();
			var data = map.DanhSachDanhGia().OrderByDescending(x => x.ngayDanhGia);
			int pageSize = 5; 
			int pageNumber = (page ?? 1); 

			var pagedList = data.ToPagedList(pageNumber, pageSize);

			return View(pagedList);
		}

		[AdminAuth]
		public ActionResult xoaDanhGia(int id_danhGia)
		{
			try
			{
				// Tìm khuyến mãi theo id
				using (var db = new DATotNghiepEntities())
				{
					var danhGia = db.DanhGias.FirstOrDefault(km => km.id_DanhGia == id_danhGia);
					if (danhGia != null)
					{
						danhGia.TrangThaiXoa = false;
						db.SaveChanges();
						TempData["Success"] = "Xóa khuyến mãi thành công!";
						return Json(new { success = true });
					}
					else
					{
						TempData["Error"] = "Không tìm thấy khuyến mãi có id " + id_danhGia;
						return Json(new { success = false, message = "Không tìm thấy khuyến mãi có id " + id_danhGia });
					}
				}
			}
			catch (Exception ex)
			{
				TempData["Error"] = "Lỗi: " + ex.Message;
				return Json(new { success = false, message = "Lỗi: " + ex.Message });
			}
		}
	}
}