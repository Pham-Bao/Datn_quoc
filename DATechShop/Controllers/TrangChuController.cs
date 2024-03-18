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


		



	}
}