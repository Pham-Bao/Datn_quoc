using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DATechShop.Models
{
	public class mapMuaHang
	{
		public List<district> timQuanHuyen(int provinceId)
		{
			using (var db = new DATotNghiepEntities())
			{
				var districts = db.districts.Where(d => d.province_id == provinceId).ToList();
				return districts;
			}
		}

	}
}