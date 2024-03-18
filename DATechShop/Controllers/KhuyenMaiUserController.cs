using DATechShop.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DATechShop.Controllers
{
    public class KhuyenMaiUserController : Controller
    {
		// GET: KhuyenMaiUser
		public ActionResult DanhSachKM(int? page)
		{
			mapKhuyenMai map = new mapKhuyenMai();
			var data = map.DanhSachKhuyenMai().OrderByDescending(x => x.id_KhuyenMai); // Sắp xếp theo ID hoặc trường khác nếu cần
			int pageSize = 5; // Số mục trên mỗi trang
			int pageNumber = (page ?? 1); // Số trang hiện tại, mặc định là trang 1 nếu không có giá trị page

			// Sử dụng PagedList để phân trang dữ liệu
			var pagedList = data.ToPagedList(pageNumber, pageSize);

			return View(pagedList);
		}
	}
}