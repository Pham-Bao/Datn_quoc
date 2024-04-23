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
		[AdminAuth]
		public ActionResult DanhSachKhuyenMai(int? page)
		{
			
			mapKhuyenMai map = new mapKhuyenMai();
			var data = map.DanhSachKhuyenMai().OrderByDescending(x => x.id_KhuyenMai);
			int pageSize = 5; 
			int pageNumber = (page ?? 1); 

			var pagedList = data.ToPagedList(pageNumber, pageSize);

			return View(pagedList);
		}

		[AdminAuth]
		public ActionResult ThemKM()
		{
			return View();
		}

		[AdminAuth]
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
				TempData["SuccessMessage"] = "Thêm sản phẩm thành công!";
				return RedirectToAction("DanhSachKhuyenMai", "KhuyenMai");
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
				using (var db = new DATotNghiepEntities())
				{
					var khuyenMai = db.KhuyenMais.FirstOrDefault(km => km.id_KhuyenMai == id_khuyenMai);
					if (khuyenMai != null)
					{
						khuyenMai.TrangThaiXoa = false;
						db.SaveChanges();
						TempData["SuccessMessage"] = "Xóa khuyến mãi thành công!";
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

		[AdminAuth]
		public ActionResult SuaKhuyenMai(int id_khuyenMai)
		{
			using (var db = new DATotNghiepEntities())
			{
				var khuyenMai = db.KhuyenMais.FirstOrDefault(km => km.id_KhuyenMai == id_khuyenMai);
				if (khuyenMai == null)
				{
					TempData["ErrorMessage"] = "Không tìm thấy khuyến mãi có id " + id_khuyenMai;
					return RedirectToAction("DanhSachKhuyenMai", "KhuyenMai");
				}
				return View(khuyenMai);
			}
		}


		[HttpPost]
		[AdminAuth]
		public ActionResult LuuSuaKhuyenMai(KhuyenMai model)
		{
			try
			{
				using (var db = new DATotNghiepEntities())
				{
					var khuyenMai = db.KhuyenMais.FirstOrDefault(km => km.id_KhuyenMai == model.id_KhuyenMai);

					if (khuyenMai == null)
					{
						TempData["ErrorMessage"] = "Không tìm thấy khuyến mãi có ID " + model.id_KhuyenMai;
						return RedirectToAction("DanhSachKhuyenMai", "KhuyenMai");
					}

					khuyenMai.tenMa = model.tenMa;
					khuyenMai.phanTramGiam = model.phanTramGiam;
					khuyenMai.moTaKhuyenMai = model.moTaKhuyenMai;
					khuyenMai.ngayHet = model.ngayHet;

					db.SaveChanges();

					TempData["SuccessMessage"] = "Cập nhật thông tin khuyến mãi thành công!";
					return RedirectToAction("DanhSachKhuyenMai", "KhuyenMai");
				}
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = "Lỗi khi cập nhật thông tin khuyến mãi: " + ex.Message;
				return RedirectToAction("DanhSachKhuyenMai", "KhuyenMai");
			}
		}


	}
}