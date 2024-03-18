using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DATechShop.Models
{
	public class ThongSoViewModel
	{
		public SanPham SanPham { get; set; }
		public List<ThongSoKyThuat> DanhSachThongSo { get; set; }
	}
}