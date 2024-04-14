using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DATechShop.Models
{
	public class mapHoaDon
	{


		public List<HoaDon> DanhSacHoaDon(int? trangThaiDon)
		{
			var db = new DATotNghiepEntities();
			List<HoaDon> data; // Di chuyển khai báo ra ngoài phạm vi if-else

			if (trangThaiDon != null)
			{
				if (trangThaiDon == 4)
				{
					data = db.HoaDons.Where(m => m.trangThai == 4 || m.trangThai == 6).ToList();
					return data;
				}
				else
				{
					data = db.HoaDons.Where(m => m.trangThai == trangThaiDon).ToList();
					return data;
				}
			}
			else
			{
				data = db.HoaDons.ToList();
				return data;
			}
		}

		public List<HoaDon> locDanhSacHoaDon(int? trangThaiDon)
		{
			var db = new DATotNghiepEntities();
			List<HoaDon> data; // Di chuyển khai báo ra ngoài phạm vi if-else

			if (trangThaiDon != null)
			{
				if (trangThaiDon == 4)
				{
					data = db.HoaDons.Where(m => m.trangThai == 4 || m.trangThai == 6).ToList();
					return data;
				}
				else
				{
					data = db.HoaDons.Where(m => m.trangThai == trangThaiDon).ToList();
					return data;
				}
			}
			else
			{
				data = db.HoaDons.ToList();
				return data;
			}
		}
		public List<HoaDon> allHoaDon()

		{
			var db = new DATotNghiepEntities();
			var data = db.HoaDons.Where(km => km.TrangThaiXoa != false).ToList();
			return data;
		}


		public List<HoaDon> DanhSacHoaDonKhach(int? trangThaiDon, int? id_nguoiDung)
		{
			var db = new DATotNghiepEntities();
			List<HoaDon> data; // Di chuyển khai báo ra ngoài phạm vi if-else

			if (trangThaiDon != null)
			{
				if (trangThaiDon == 4)
				{
					data = db.HoaDons.Where(m => (m.trangThai == 4 || m.trangThai == 6) && m.id_NguoiDung == id_nguoiDung).ToList();
					return data;
				}
				else
				{
					data = db.HoaDons.Where(m => m.trangThai == trangThaiDon && m.id_NguoiDung == id_nguoiDung).ToList();
					return data;
				}
			}
			else
			{
				data = db.HoaDons.Where(m => m.id_NguoiDung == id_nguoiDung).ToList();
				return data;
			}
		}

		public List<HoaDon> DonHang(int id_nguoiDung)

		{
			var db = new DATotNghiepEntities();
			var Donhang = db.HoaDons.Where(d => d.id_NguoiDung == id_nguoiDung).ToList();
			return Donhang;
		}

		public List<LoaiThanhToan> loaiThanhToans(int id_hoaDon)

		{
			var db = new DATotNghiepEntities();
			var loaiThanhToan = db.LoaiThanhToans.Where(d => d.id_HoaDon == id_hoaDon).ToList();
			return loaiThanhToan;
		}

		public HoaDon hoaDonID(int id_hoaDon)
		{
			var db = new DATotNghiepEntities();
			var data = db.HoaDons.FirstOrDefault(m => m.id_HoaDon == id_hoaDon);
			return data;
		}


		public List<LoaiThanhToan> DanhSachLoaiThanhToan()

		{
			var db = new DATotNghiepEntities();
			var data = db.LoaiThanhToans.ToList();
			return data;
		}

		public List<ChiTietHoaDon> ChiTietHoaDon(int id_hoaDon)

		{
			var db = new DATotNghiepEntities();
			var data = db.ChiTietHoaDons.Where(d => d.id_HoaDon == id_hoaDon).ToList(); 
			return data;
		}

	}
}