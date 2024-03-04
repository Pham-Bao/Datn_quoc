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
	}
}