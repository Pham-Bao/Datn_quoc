using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DATechShop.Models
{
	public class mapDanhGia
	{
		public List<DanhGia> DanhSachDanhGia(int? id)
		{
			if (!id.HasValue)
			{
				// Xử lý trường hợp id là null (nếu cần)
				return new List<DanhGia>(); // Hoặc trả về null hoặc giá trị mặc định khác
			}

			var db = new DATotNghiepEntities();
			var data = db.DanhGias.Where(hd => hd.TrangThaiXoa != false && hd.id_sanPham == id).ToList();
			return data;
		}

		public List<DanhGia> DanhSachDanhGia()

		{
			var db = new DATotNghiepEntities();
			var data = db.DanhGias.Where(km => km.TrangThaiXoa != false).ToList();
			return data;
		}
	}
}