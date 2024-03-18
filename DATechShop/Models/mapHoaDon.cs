using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DATechShop.Models
{
	public class mapHoaDon
	{


		public List<HoaDon> DanhSacHoaDon()

		{
			var db = new DATotNghiepEntities();
			var data = db.HoaDons.ToList();
			return data;
		}
	}
}