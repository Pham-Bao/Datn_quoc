using System.Collections.Generic;
using System.Linq;

namespace DATechShop.Models
{
	public class mapSP
	{

		public List<MauSac> DanhSachMau()

		{
			var db = new DATotNghiepEntities();
			var data = db.MauSacs.Where(km => km.TrangThaiXoa != false).ToList();
			return data;
		}

		public List<TuyChon> DanhSachTuyChon()

		{
			var db = new DATotNghiepEntities();
			var data = db.TuyChons.Where(km => km.TrangThaiXoa != false).ToList();
			return data;
		}

		public List<SanPham> DanhSachSP(string loaiSP)
		{
			var db = new DATotNghiepEntities();

			if (!string.IsNullOrEmpty(loaiSP))
			{
				var lowerLoaiSP = loaiSP.ToLower();
				var data = db.SanPhams.Where(m => m.loaiSP.ToLower().Contains(lowerLoaiSP)).ToList();
				return data;
			}
			else
			{
				var data = db.SanPhams.ToList();
				return data;
			}
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


		public List<SanPham> danhSachSPLoai(string loaiSP, int? value)
		{
			var db = new DATotNghiepEntities();
			var data = new List<SanPham>();

				if(value == null)
				{
				data = db.SanPhams.Where(m => m.loaiSP.ToLower().Contains(loaiSP.ToLower()) || string.IsNullOrEmpty(loaiSP)).ToList();

				}

			// giảm dần
			if (value == 1)
			{
				var query = from cts in db.ChitietSPs
							join sp in db.SanPhams on cts.id_sanPham equals sp.id_sanPham
							where sp.loaiSP.ToLower().Contains(loaiSP.ToLower()) || string.IsNullOrEmpty(loaiSP)
							orderby cts.giaSP descending
							select sp;

				data = query.ToList();
			}
			// Mua nhiều
			else if (value == 2)
			{
				var query = from sp in db.SanPhams
							join cthd in db.ChiTietHoaDons on sp.id_sanPham equals cthd.id_chiTietSP into cthdGroup
							select new
							{
								SanPham = sp,
								TotalQuantity = cthdGroup.Sum(cthd => cthd.soLuong)
							};

				// Sắp xếp danh sách sản phẩm theo lượt mua giảm dần
				var sortedData = query
									.Where(x => x.SanPham.loaiSP.ToLower().Contains(loaiSP.ToLower()) || string.IsNullOrEmpty(loaiSP))
									.OrderByDescending(x => x.TotalQuantity)
									.Select(x => x.SanPham)
									.ToList();

				data = sortedData;
			}

			//khuyến mãi nhiều
			else if (value == 3)
			{
				data = db.SanPhams.Where(m => m.loaiSP.ToLower().Contains(loaiSP.ToLower()) || string.IsNullOrEmpty(loaiSP))
						 .OrderByDescending(m => m.khuyenMai)
						 .ToList();
			}
			//mới nhất
			else if(value == 4)
			{
				data = db.SanPhams.Where(m => m.loaiSP.ToLower().Contains(loaiSP.ToLower()) || string.IsNullOrEmpty(loaiSP))
						 .OrderByDescending(m => m.id_sanPham)
						 .ToList();
			}
			//giá tăng dần
			else if(value == 5)
			{
				data = (from sp in db.SanPhams
						join cts in db.ChitietSPs on sp.id_sanPham equals cts.id_sanPham
						where sp.loaiSP.ToLower().Contains(loaiSP.ToLower()) || string.IsNullOrEmpty(loaiSP)
						orderby cts.giaSP ascending
						select sp).ToList();
			}

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
			var data = db.ThongSoKyThuats.Where(m => m.id_sanPham == id && m.TrangThaiXoa != false).ToList();
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

		public List<SanPham> timSP(string key)
		{
			var db = new DATotNghiepEntities();
			var data = db.SanPhams.Where(m => m.tenSP.ToLower().Contains(key.ToLower()) || string.IsNullOrEmpty(key)).ToList();
			return data;
		}

		public List<SanPham> goiYSanPham(string key)
		{
			
			var db = new DATotNghiepEntities();
			var data = db.SanPhams.Where(m => m.ghiChu.Replace(" ", "").ToLower().Contains(key.Trim().Replace(" ", "").ToLower()) || string.IsNullOrEmpty(key)).ToList();
			return data;
		}

	}
}