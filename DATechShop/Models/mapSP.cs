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


		public ChiTietSPDTO layChiTietSP(int id)
		{
			var db = new DATotNghiepEntities();
			var chitietSP = db.ChitietSPs.Find(id);

			// Kiểm tra xem chi tiết sản phẩm có tồn tại không
			if (chitietSP == null)
			{
				return null; // Trả về null nếu không tìm thấy
			}

			// Tạo đối tượng DTO và gán các giá trị từ đối tượng chitietSP
			var chitietSPDTO = new ChiTietSPDTO
			{
				ID = chitietSP.id_chiTietSP,
				tenSP = chitietSP.SanPham.tenSP,
				giaSP = chitietSP.giaSP,
				// Gán các thuộc tính khác cần thiết
			};

			return chitietSPDTO;
		}

	}
}