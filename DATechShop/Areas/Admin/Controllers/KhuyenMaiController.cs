using DATechShop.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using DATechShop.Areas.Admin.Content;
using static DATechShop.Areas.Admin.Content.AuthAttribute;

namespace DATechShop.Areas.Admin.Controllers
{
    public class KhuyenMaiController : Controller
    {
		// GET: Admin/KhuyenMai
		[AdminAuth]
		public ActionResult DanhSachKhuyenMai(int? page)
		{
			ViewBag.Success = TempData["Success"];
			ViewBag.Error = TempData["Error"];
			mapKhuyenMai map = new mapKhuyenMai();
			var data = map.DanhSachKhuyenMai().OrderByDescending(x => x.id_KhuyenMai);
			int pageSize = 5; // Số mục trên mỗi trang
			int pageNumber = (page ?? 1); // Số trang hiện tại, mặc định là trang 1 nếu không có giá trị page

			// Sử dụng PagedList để phân trang dữ liệu
			var pagedList = data.ToPagedList(pageNumber, pageSize);

			return View(pagedList);
		}

		[AdminAuth]
		public ActionResult ThemKM()
		{
			return View();
		}


		[HttpPost]
		public ActionResult themKM(KhuyenMai km, HttpPostedFileBase uploadhinh)
		{


			if (uploadhinh != null && uploadhinh.ContentLength > 0)
			{
				DATotNghiepEntities db = new DATotNghiepEntities();
				db.KhuyenMais.Add(km);
				db.SaveChanges();

				int id = int.Parse(db.KhuyenMais.ToList().Last().id_KhuyenMai.ToString());

				string _FileName = "";
				int index = uploadhinh.FileName.IndexOf('.');
				_FileName = "km" + id.ToString() + "." + uploadhinh.FileName.Substring(index + 1);
				string _path = Path.Combine(Server.MapPath("~/Upload/img"), _FileName);
				uploadhinh.SaveAs(_path);

				KhuyenMai unv = db.KhuyenMais.FirstOrDefault(x => x.id_KhuyenMai == id);
				unv.hinhAnh = _FileName;
				db.SaveChanges();
				ViewBag.err = "Thêm thành công";
				return View();
			}
			else
			{
				ViewBag.err = "Không  ảnh";
				return View();
			}


		}
		[AdminAuth]
		public ActionResult xoaKM(int id_khuyenMai)
		{
			try
			{
				// Tìm khuyến mãi theo id
				using (var db = new DATotNghiepEntities())
				{
					var khuyenMai = db.KhuyenMais.FirstOrDefault(km => km.id_KhuyenMai == id_khuyenMai);
					if (khuyenMai != null)
					{
						khuyenMai.TrangThaiXoa = false;
						db.SaveChanges();
						TempData["Success"] = "Xóa khuyến mãi thành công!";
						return Json(new { success = true });
					}
					else
					{
						TempData["Error"] = "Không tìm thấy khuyến mãi có id " + id_khuyenMai;
						return Json(new { success = false, message = "Không tìm thấy khuyến mãi có id " + id_khuyenMai });
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