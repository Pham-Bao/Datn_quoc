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
			mapKhuyenMai map = new mapKhuyenMai();
			var data = map.DanhSachKhuyenMai().OrderByDescending(x => x.id_KhuyenMai); // Sắp xếp theo ID hoặc trường khác nếu cần
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

	}
}