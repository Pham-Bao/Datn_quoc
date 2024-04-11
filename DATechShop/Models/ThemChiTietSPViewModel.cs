using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DATechShop.Models
{
	public class ThemChiTietSPViewModel
	{
		public SanPham SanPham { get; set; }
		public ChitietSP ChitietSP { get; set; }

		

		public List<MauSac> DanhSachMau { get; set; }
		public List<TuyChon> DanhSachTuyChon { get; set; } // Thêm DanhSachTuyChon

		// Thêm các thuộc tính khác nếu cần
		public int? SelectedMau { get; set; }
		public int? SelectedTuyChon { get; set; }


		public ThemChiTietSPViewModel()
		{
			DanhSachMau = new List<MauSac>();
			DanhSachTuyChon = new List<TuyChon>();
		}

	}
}