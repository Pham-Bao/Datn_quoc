using DATechShop.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DATechShop.Areas.Admin.Controllers
{
    public class SanPhamController : Controller
    {
		DATotNghiepEntities db = new DATotNghiepEntities();
		// GET: Admin/SanPham
		public ActionResult ThemMauSac()
		{

			return View();
		}
		[HttpPost]
		public ActionResult ThemMauSac(MauSac mode)
		{
			db.MauSacs.Add(mode);
			db.SaveChanges();
			return View();
		}

	

		public ActionResult DanhSachMau(int? page)
		{
			mapSP map = new mapSP();
			var data = map.DanhSachMau().OrderByDescending(x => x.id_Mau); // Sắp xếp theo ID hoặc trường khác nếu cần
			int pageSize = 5; // Số mục trên mỗi trang
			int pageNumber = (page ?? 1); // Số trang hiện tại, mặc định là trang 1 nếu không có giá trị page

			// Sử dụng PagedList để phân trang dữ liệu
			var pagedList = data.ToPagedList(pageNumber, pageSize);

			return View(pagedList);
		}


		public ActionResult TuyChon()
		{

			return View();
		}
		[HttpPost]
		public ActionResult TuyChon(TuyChon mode)
		{
			db.TuyChons.Add(mode);
			db.SaveChanges();
			return View();
		}

		public ActionResult DanhSachTuyChon(int? page)
		{
			mapSP map = new mapSP();
			var data = map.DanhSachTuyChon().OrderByDescending(x => x.id_tuyChon); // Sắp xếp theo ID hoặc trường khác nếu cần
			int pageSize = 5; // Số mục trên mỗi trang
			int pageNumber = (page ?? 1); // Số trang hiện tại, mặc định là trang 1 nếu không có giá trị page

			// Sử dụng PagedList để phân trang dữ liệu
			var pagedList = data.ToPagedList(pageNumber, pageSize);

			return View(pagedList);
		}

		public ActionResult ThemSanPham()
		{

			return View();
		}
		[HttpPost]
		public ActionResult ThemSanPham(SanPham mode)
		{
			db.SanPhams.Add(mode);
			db.SaveChanges();
			return View();
		}

		public ActionResult DanhSachSP(int? page)
		{
			mapSP map = new mapSP();
			var data = map.DanhSachSP().OrderByDescending(x => x.id_sanPham); // Sắp xếp theo ID hoặc trường khác nếu cần
			int pageSize = 5; // Số mục trên mỗi trang
			int pageNumber = (page ?? 1); // Số trang hiện tại, mặc định là trang 1 nếu không có giá trị page

			// Sử dụng PagedList để phân trang dữ liệu
			var pagedList = data.ToPagedList(pageNumber, pageSize);

			return View(pagedList);
		}


		public ActionResult ThemChiTietSP(int id)
		{
			// Lấy thông tin chi tiết của sản phẩm từ id được chọn
			var sanPham = db.SanPhams.FirstOrDefault(sp => sp.id_sanPham == id);

			// Nếu sản phẩm không tồn tại, có thể xử lý lỗi ở đây

			// Truy vấn danh sách các màu sắc và phiên bản để hiển thị trên giao diện
			var danhSachMau = db.MauSacs.ToList();
			var danhSachTuyChon = db.TuyChons.ToList();

			// Tạo view model chứa thông tin sản phẩm và danh sách màu sắc, phiên bản
			var viewModel = new ThemChiTietSPViewModel
			{
				SanPham = sanPham,
				DanhSachMau = danhSachMau,
				DanhSachTuyChon = danhSachTuyChon
			};

			return View(viewModel);
		}
		[HttpPost]
		public ActionResult ThemChiTietSP(ThemChiTietSPViewModel viewModel, HttpPostedFileBase uploadhinh)
		{
			// Kiểm tra ModelState.IsValid trước khi thực hiện xử lý
			if (ModelState.IsValid)
			{
				// Lưu thông tin chi tiết sản phẩm vào cơ sở dữ liệu
				var chiTietSP = new ChitietSP();

				// Kiểm tra viewModel.SanPham không null trước khi sử dụng
				if (viewModel.SanPham != null)
				{
					chiTietSP.id_sanPham = viewModel.SanPham.id_sanPham;
				}
				else
				{
					// Xử lý trường hợp khi viewModel.SanPham là null
					// Có thể quay lại view hoặc xử lý logic khác
					return View(viewModel);
				}

				chiTietSP.id_Mau = viewModel.SelectedMau;
				chiTietSP.id_tuyChon = viewModel.SelectedTuyChon;
				chiTietSP.giaSP = viewModel.ChitietSP.giaSP; // Gán giá sản phẩm từ form vào đối tượng chiTietSP

				// Xử lý upload ảnh
				if (uploadhinh != null && uploadhinh.ContentLength > 0)
				{
					string _FileName = "";
					int id = int.Parse(db.ChitietSPs.ToList().Last().id_chiTietSP.ToString());
					int index = uploadhinh.FileName.IndexOf('.');
					_FileName = "anhSP_" + id.ToString() + "." + uploadhinh.FileName.Substring(index + 1);
					string _path = Path.Combine(Server.MapPath("~/Upload/imgSP"), _FileName);
					uploadhinh.SaveAs(_path);

					// Gán đường dẫn ảnh vào đối tượng chiTietSP
					chiTietSP.anhSP = _FileName;
				}

				db.ChitietSPs.Add(chiTietSP);
				db.SaveChanges();

				// Redirect hoặc trả về view tùy theo logic của bạn
			}

			// Nếu ModelState không hợp lệ, quay lại view để hiển thị thông báo lỗi
			return View(viewModel);
		}

		public ActionResult ChiTietSP(int id, int? page)
		{
			mapSP map = new mapSP();
			var data = map.chiTietSP(id);

			int pageSize = 5; // Số lượng mục trên mỗi trang
			int pageNumber = (page ?? 1); // Trang hiện tại, mặc định là trang 1 nếu không có giá trị page

			// Sử dụng PagedList để phân trang dữ liệu
			var pagedList = data.ToPagedList(pageNumber, pageSize);

			return View(pagedList);
		}




	}
}