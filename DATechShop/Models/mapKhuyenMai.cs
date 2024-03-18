using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DATechShop.Models
{
	public class mapKhuyenMai
	{
		public List<KhuyenMai> DanhSachKhuyenMai()

		{
			var db = new DATotNghiepEntities();
			var data = db.KhuyenMais.ToList();
			return data;
		}

		public KhuyenMai timKM(string tenKM)
		{
			using (var db = new DATotNghiepEntities())
			{
				var data = db.KhuyenMais.FirstOrDefault(m => string.IsNullOrEmpty(tenKM) || m.tenMa.ToLower().Contains(tenKM.ToLower()));
				return data;
			}
		}

	}
}