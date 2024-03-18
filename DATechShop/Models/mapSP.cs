using System.Collections.Generic;
using System.Linq;

namespace DATechShop.Models
{
	public class mapSP
	{

		public List<MauSac> DanhSachMau()

		{
			var db = new DATotNghiepEntities();
			var data = db.MauSacs.ToList();
			return data;
		}

		public List<TuyChon> DanhSachTuyChon()

		{
			var db = new DATotNghiepEntities();
			var data = db.TuyChons.ToList();
			return data;
		}

		public List<SanPham> DanhSachSP()
		{
			var db = new DATotNghiepEntities();
			var data = db.SanPhams.ToList();
			return data;
		}


		public SanPham sanPham(float id)
		{
			var db = new DATotNghiepEntities();
			return db.SanPhams.Find(id);
		}

		public ChitietSP chiTietSP(float id)
		{
			var db = new DATotNghiepEntities();
			return db.ChitietSPs.Find(id);
		}
		public List<ChitietSP> chiTietSP(int id)
		{
			var db = new DATotNghiepEntities();
			var data = db.ChitietSPs.Where(m => m.id_sanPham == id).ToList();
			return data;
		}


		public List<SanPham> danhSachSPLoai(string loaiSP)
		{
			var db = new DATotNghiepEntities();
			var data = db.SanPhams.Where(m => m.loaiSP.ToLower().Contains(loaiSP.ToLower()) == true || string.IsNullOrEmpty(loaiSP)).ToList();
			return data;
		}

		public List<ChitietSP> danhSachCTSPLoai(string loaiSP)
		{
			var db = new DATotNghiepEntities();
			var data = db.ChitietSPs.Where(m => string.IsNullOrEmpty(loaiSP) || m.SanPham.loaiSP.ToLower().Contains(loaiSP.ToLower())).ToList();
			return data;
		}

		public List<ThongSoKyThuat> chiTietThongSo(int id)
		{
			var db = new DATotNghiepEntities();
			var data = db.ThongSoKyThuats.Where(m => m.id_sanPham == id).ToList();
			return data;
		}
		public ChitietSP chiTiet(int id)
		{
			var db = new DATotNghiepEntities();
			return db.ChitietSPs.Find(id);
		}


	}
}