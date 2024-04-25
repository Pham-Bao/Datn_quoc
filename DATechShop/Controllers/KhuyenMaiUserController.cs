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
		public ActionResult DanhSachKM(int? page)
		{
			mapKhuyenMai map = new mapKhuyenMai();
			var data = map.DanhSachKhuyenMai().OrderByDescending(x => x.id_KhuyenMai); 
			int pageSize = 5;
			int pageNumber = (page ?? 1);

			var pagedList = data.ToPagedList(pageNumber, pageSize);

			return View(pagedList);
		}
	}
}