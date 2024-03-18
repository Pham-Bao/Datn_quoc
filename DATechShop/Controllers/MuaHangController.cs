using DATechShop.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace DATechShop.Controllers
{
    public class MuaHangController : Controller
    {


        DATotNghiepEntities db = new DATotNghiepEntities();	
        [HttpPost]
        public ActionResult LayIdChiTietSP(int id_sanPham, string mauSac, string tuyChon)
        {
            Console.WriteLine($"id_sanPham: {id_sanPham}, mauSac: {mauSac}, tuyChon: {tuyChon}");
            // Tìm id_chiTietSP dựa trên id_sanPham, màu sắc và tùy chọn
            var chiTietSP = db.ChitietSPs.FirstOrDefault(ct => ct.id_sanPham == id_sanPham && ct.MauSac.maMau == mauSac && ct.TuyChon.tuyChon1 == tuyChon);

            if (chiTietSP != null)
            {
                // Trả về id_chiTietSP nếu tìm thấy
                return Json(new { success = true, id_chiTietSP = chiTietSP.id_chiTietSP });
            }
            else
            {
                // Trả về thông báo lỗi nếu không tìm thấy
                return Json(new { success = false });
            }
        }
        public ActionResult GioHang(List<ChitietSP> chitietSPs)
        {
            return View(chitietSPs);
        }


        public ActionResult row(int id)
        {
            var sp = new mapSP().chiTiet(id);
            return View(sp);

        }

        [HttpPost]
        public ActionResult KiemTraMaGiamGia(string maGiamGia)
        {
            
            string phanTramGiam = "0%";

            
            if (!string.IsNullOrEmpty(maGiamGia))
            {
            
                var khuyenMai = new mapKhuyenMai().timKM(maGiamGia);

                // Nếu mã giảm giá tồn tại
                if (khuyenMai != null)
                {
                    // Cập nhật phần trăm giảm giá
                    phanTramGiam = khuyenMai.phanTramGiam.ToString() + "%";
                }
                else
                {
                    // Nếu mã giảm giá không tồn tại, trả về thông báo không có
                    phanTramGiam = "Không có";
                }
            }

            // Trả về phần trăm giảm giá qua Json
            return Json(new { phanTramGiam });
        }

        public ActionResult datHang()
        {
			var provinces = db.provinces.ToList();
			ViewBag.Provinces = new SelectList(provinces, "id", "name");
			return View();
		}

		public ActionResult ChonDiaChi()
		{
			var provinces = db.provinces.ToList();
			ViewBag.Provinces = new SelectList(provinces, "id", "name");
			return View();
		}

		public ActionResult LayTinhThanhPho()
		{
			var provinces = db.provinces.ToList();
			return Json(provinces, JsonRequestBehavior.AllowGet);
		}
		public ActionResult LayQuanHuyen(int? provinceId)
		{
			List<district> districtsList = new List<district>(); // Khởi tạo danh sách mới

			if (provinceId == null)
			{
				var districts = db.districts.ToList();
				foreach (var d in districts)
				{
					districtsList.Add(new district
					{
						id = d.id,
						code = d.code,
						name = d.name,
						province_id = d.province_id
					});
				}
				return Json(districtsList, JsonRequestBehavior.AllowGet);
			}

			var filteredDistricts = db.districts.Where(d => d.province_id == provinceId).ToList();
			foreach (var d in filteredDistricts)
			{
				districtsList.Add(new district
				{
					id = d.id,
					code = d.code,
					name = d.name,
					province_id = d.province_id
				});
			}
			return Json(districtsList, JsonRequestBehavior.AllowGet);
		}

		public ActionResult LayXaPhuong(int? districtId)
		{
			List<ward> wardsList = new List<ward>(); // Khởi tạo danh sách mới

			if (districtId == null)
			{
				var wards = db.wards.ToList();
				foreach (var w in wards)
				{
					wardsList.Add(new ward
					{
						id = w.id,
						code = w.code,
						name = w.name,
						province_id = w.province_id,
						district_id = w.district_id
					});
				}
				return Json(wardsList, JsonRequestBehavior.AllowGet);
			}

			var filteredWards = db.wards.Where(w => w.district_id == districtId).ToList();
			foreach (var w in filteredWards)
			{
				wardsList.Add(new ward
				{
					id = w.id,
					code = w.code,
					name = w.name,
					province_id = w.province_id,
					district_id = w.district_id
				});
			}
			return Json(wardsList, JsonRequestBehavior.AllowGet);
		}


	}
}