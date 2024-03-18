using DATechShop.Areas.Admin.Content;
using DATechShop.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DATechShop.Areas.Admin.Controllers
{
    public class HoaDonController : Controller
    {
		[Auth]
		public ActionResult DanhSachHoaDon(int? page)
		{
			mapHoaDon map = new mapHoaDon();
			var data = map.DanhSacHoaDon().OrderByDescending(x => x.id_HoaDon); 
			int pageSize = 5; 
			int pageNumber = (page ?? 1); 
			
			var pagedList = data.ToPagedList(pageNumber, pageSize);

			return View(pagedList);
		}

	}
}