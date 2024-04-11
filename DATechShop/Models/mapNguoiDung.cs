using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DATechShop.Models
{
	public class mapNguoiDung
	{
		public List<NguoiDung> DanhSachNguoiDung()

		{
			var db = new DATotNghiepEntities();
			var data = db.NguoiDungs.Where(km => km.TrangThaiXoa != false).ToList();
			return data;
		}

		public NguoiDung timUserSDT(string sdt)
		{
			using (var db = new DATotNghiepEntities())
			{
				var data = db.NguoiDungs.FirstOrDefault(m => string.IsNullOrEmpty(sdt) || m.sdt.ToLower().Contains(sdt.ToLower()));
				return data;
			}
		}
	}
}