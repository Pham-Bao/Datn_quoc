using DATechShop.Areas.Admin.Content;
using DATechShop.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static DATechShop.Areas.Admin.Content.AuthAttribute;

namespace DATechShop.Areas.Admin.Controllers
{
    public class SanPhamController : Controller
    {
		DATotNghiepEntities db = new DATotNghiepEntities();
		public ActionResult ThongKe()
		{
			float totalUsers = db.NguoiDungs.Count();
			float totalOders = db.HoaDons.Count();
			float totalConment = db.DanhGias.Count();
			var currentDate = DateTime.Now;

			var totalRevenueByMonth = db.HoaDons
				.Where(dh => dh.ngayTao != null && dh.ngayTao.Value.Year == currentDate.Year && dh.ngayTao.Value.Month == currentDate.Month)
				.Sum(dh => (double?)dh.tongTien) ?? 0;


			// Gửi các giá trị tính toán qua ViewBag
			ViewBag.TotalUsers = totalUsers;
			ViewBag.TotalOrders = totalOders;
			ViewBag.TotalRevenueByMonth = totalRevenueByMonth;
			ViewBag.totalConment = totalConment; 

			
			return View();
		}

		[HttpPost]
		public JsonResult TinhToan(string startDate, string endDate)
		{
			// Khai báo biến để lưu tổng số đơn và tổng tiền
			int totalOrders = 0;
			double totalRevenue = 0;

			// Kiểm tra xem startDate và endDate có được cung cấp không
			if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
			{
				// Parse startDate và endDate thành kiểu DateTime
				DateTime startDateTime = DateTime.Parse(startDate);
				DateTime endDateTime = DateTime.Parse(endDate).AddDays(1); // Add one day to include end date

				// Tính tổng số đơn trong khoảng thời gian startDate đến endDate
				totalOrders = db.HoaDons
					.Count(dh => dh.ngayTao != null && dh.ngayTao >= startDateTime && dh.ngayTao < endDateTime);

				// Tính tổng tiền trong khoảng thời gian startDate đến endDate
				totalRevenue = db.HoaDons
					.Where(dh => dh.ngayTao != null && dh.ngayTao >= startDateTime && dh.ngayTao < endDateTime)
					.Sum(dh => (double?)dh.tongTien) ?? 0;
			}
			else
			{
				// Nếu không có startDate và endDate được cung cấp, tính toán tổng số đơn và tổng tiền từ toàn bộ dữ liệu
				totalOrders = db.HoaDons.Count();
				totalRevenue = db.HoaDons.Sum(dh => (double?)dh.tongTien) ?? 0;
			}

			// Trả về kết quả dưới dạng JSON
			return Json(new { TotalOrders = totalOrders, TotalRevenue = totalRevenue }, JsonRequestBehavior.AllowGet);
		}


		// GET: Admin/SanPham
		[AdminAuth]
		public ActionResult ThemMauSac()
		{

			return View();
		}
		[AdminAuth]
		[HttpPost]
		public ActionResult ThemMauSac(FormCollection form)
		{
			string tenMau = form["tenMau"];
			string maMau = form["maMau"];

			var existingColor = db.MauSacs.FirstOrDefault(m => m.tenMau == tenMau);

			if (existingColor == null)
			{
				// Màu chưa tồn tại trong cơ sở dữ liệu, thêm màu mới
				MauSac mauSac = new MauSac
				{
					tenMau = tenMau,
					maMau = maMau
				};

				db.MauSacs.Add(mauSac);
				db.SaveChanges();
				TempData["SuccessMessage"] = "Thêm thành công";
			}
			else
			{
				TempData["ErrorMessage"] = "Màu đã tồn tại";
			}

			// Trả về ActionResult (có thể là RedirectToAction hoặc PartialView)
			return RedirectToAction("DanhSachMau");
		}



		[AdminAuth]
		public ActionResult DanhSachMau(int? page)
		{
			mapSP map = new mapSP();
			var data = map.DanhSachMau().OrderByDescending(x => x.id_Mau); 
			int pageSize = 5; 
			int pageNumber = (page ?? 1); 

			
			var pagedList = data.ToPagedList(pageNumber, pageSize);

			// Truyền thông báo từ TempData đến ViewBag
			if (TempData["SuccessMessage"] != null)
			{
				ViewBag.Success = TempData["SuccessMessage"];
			}

			if (TempData["ErrorMessage"] != null)
			{
				ViewBag.Error = TempData["ErrorMessage"];
			}

			return View(pagedList);
		}

		[AdminAuth]
		public ActionResult TuyChon()
		{

			return View();
		}
		[HttpPost]
		public ActionResult TuyChon(FormCollection form)
		{
			string tenTuyChon = form["tenTuyChon"];
			string giaTri = form["giaTri"];

			
			var existingOption = db.TuyChons.FirstOrDefault(tc => tc.tenTuyChon == tenTuyChon && tc.tuyChon1 == giaTri);

			if (existingOption == null)
			{
				
				TuyChon tuyChon = new TuyChon
				{
					tenTuyChon = tenTuyChon,
					tuyChon1 = giaTri
				};

				db.TuyChons.Add(tuyChon);
				db.SaveChanges();
				TempData["SuccessMessage"] = "Thêm thành công";
			}
			else
			{
				TempData["ErrorMessage"] = "Tùy chọn đã tồn tại";
			}

			return RedirectToAction("DanhSachTuyChon");
		}

		[AdminAuth]
		public ActionResult DanhSachTuyChon(int? page)
		{
			mapSP map = new mapSP();
			var data = map.DanhSachTuyChon().OrderByDescending(x => x.id_tuyChon); // Sắp xếp theo ID hoặc trường khác nếu cần
			int pageSize = 5; // Số mục trên mỗi trang
			int pageNumber = (page ?? 1); // Số trang hiện tại, mặc định là trang 1 nếu không có giá trị page

			// Sử dụng PagedList để phân trang dữ liệu
			var pagedList = data.ToPagedList(pageNumber, pageSize);
			if (TempData["SuccessMessage"] != null)
			{
				ViewBag.Success = TempData["SuccessMessage"];
			}

			if (TempData["ErrorMessage"] != null)
			{
				ViewBag.Error = TempData["ErrorMessage"];
			}
			return View(pagedList);
		}
		[AdminAuth]
		public ActionResult ThemSanPham()
		{

			return View();
		}
		[HttpPost]
		public ActionResult ThemSanPham(SanPham mode, HttpPostedFileBase uploadhinh)
		{
			if (uploadhinh != null && uploadhinh.ContentLength > 0)
			{
				try
				{
					DATotNghiepEntities db = new DATotNghiepEntities();
					db.SanPhams.Add(mode);
					db.SaveChanges();

					int id = int.Parse(db.SanPhams.ToList().Last().id_sanPham.ToString());

					string _FileName = "";
					int index = uploadhinh.FileName.IndexOf('.');
					_FileName = "sp" + id.ToString() + "." + uploadhinh.FileName.Substring(index + 1);
					string _path = Path.Combine(Server.MapPath("~/Upload/imgSPChung"), _FileName);
					uploadhinh.SaveAs(_path);

					SanPham unv = db.SanPhams.FirstOrDefault(x => x.id_sanPham == id);
					unv.anhSPChung = _FileName;
					db.SaveChanges();

					ViewBag.err = "Thêm thành công";
					return View();
				}
				catch (Exception ex)
				{
					ViewBag.err = "Lỗi khi thêm sản phẩm: " + ex.Message;
				}
			}
			else
			{
				ViewBag.err = "Không có ảnh" ;
			}

			return View();
		}

		[AdminAuth]
		public ActionResult DanhSachSP(int? page, string loaiSP)
		{
			mapSP map = new mapSP();
			var data = map.DanhSachSP(loaiSP).OrderByDescending(x => x.id_sanPham); 
			int pageSize = 6;
			int pageNumber = (page ?? 1); 

			// Sử dụng PagedList để phân trang dữ liệu
			var pagedList = data.ToPagedList(pageNumber, pageSize);

			ViewBag.loaiSP = loaiSP;

			return View(pagedList);
		}

		[AdminAuth]
		public ActionResult ThemChiTietSP(int id)
		{
			// Lấy thông tin chi tiết của sản phẩm từ id được chọn
			var sanPham = db.SanPhams.FirstOrDefault(sp => sp.id_sanPham == id);

			
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

		[AdminAuth]
		[HttpPost]
		public ActionResult ThemChiTietSP(ThemChiTietSPViewModel viewModel, HttpPostedFileBase uploadhinh)
		{
			if (uploadhinh == null || viewModel.ChitietSP.giaSP == null)
			{
				ViewBag.Error = "Chưa chọn ảnh hoặc nhập giá.";
			}
			else if (ModelState.IsValid)
			{
				// Kiểm tra xem đã tồn tại bản ghi trong bảng chi tiết sản phẩm với các giá trị đã cho
				var existingRecord = db.ChitietSPs.FirstOrDefault(c => c.id_sanPham == viewModel.SanPham.id_sanPham &&
																		c.id_Mau == viewModel.SelectedMau &&
																		c.id_tuyChon == viewModel.SelectedTuyChon);

				if (existingRecord != null)
				{
					// Nếu bản ghi đã tồn tại, hiển thị thông báo lỗi và quay lại view
					ViewBag.Error = "Bản ghi đã tồn tại trong bảng chi tiết sản phẩm.";
					return View(viewModel);
				}

				// Nếu không có bản ghi tồn tại, tiến hành thêm bản ghi mới
				var chiTietSP = new ChitietSP();
				chiTietSP.id_sanPham = viewModel.SanPham.id_sanPham;
				chiTietSP.id_Mau = viewModel.SelectedMau;
				chiTietSP.id_tuyChon = viewModel.SelectedTuyChon;
				chiTietSP.giaSP = viewModel.ChitietSP.giaSP;

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

				// Thêm bản ghi mới vào cơ sở dữ liệu
				db.ChitietSPs.Add(chiTietSP);
				db.SaveChanges();
				ViewBag.Success = "Thêm thành công";
			}

			ViewBag.Error = "Thêm lỗi!";
			return View(viewModel);
		}


		[AdminAuth]
		public ActionResult ChiTietSP(int id, int? page)
		{
			mapSP map = new mapSP();
			var data = map.chiTietSP(id);

			int pageSize = 5; // Số lượng mục trên mỗi trang
			int pageNumber = (page ?? 1); // Trang hiện tại, mặc định là trang 1 nếu không có giá trị page

			// Sử dụng PagedList để phân trang dữ liệu
			var pagedList = data.ToPagedList(pageNumber, pageSize);
			ViewBag.id_sanPham = id;
			return View(pagedList);
		}
		[AdminAuth]
		public ActionResult DanhSachThongSo(int id, int? page)
		{
			mapSP map = new mapSP();
			var data = map.chiTietThongSo(id);

			int pageSize = 5; // Số lượng mục trên mỗi trang
			int pageNumber = (page ?? 1); // Trang hiện tại, mặc định là trang 1 nếu không có giá trị page

			// Sử dụng PagedList để phân trang dữ liệu
			var pagedList = data.ToPagedList(pageNumber, pageSize);

			ViewBag.IdSanPham = id; // Truyền id sản phẩm sang view

			return View(pagedList);
		}

		[HttpPost]
		public ActionResult ThemThongSo(FormCollection form)
		{
			// Lấy giá trị id_sanPham từ form post
			int id_sanPham;
			if (!int.TryParse(form["id_sanPham"], out id_sanPham))
			{
				// Xử lý nếu không có giá trị id_sanPham
				return HttpNotFound();
			}

			// Kiểm tra xem id_sanPham có hợp lệ không
			SanPham sanPham = db.SanPhams.Find(id_sanPham);
			if (sanPham == null)
			{
				return HttpNotFound();
			}

			// Lấy thông tin từ form post
			string tenThongSo = form["tenThongSo"];
			string giaTri = form["giaTri"];

			// Tạo một đối tượng mới ThongSoKyThuat
			ThongSoKyThuat thongSo = new ThongSoKyThuat
			{
				id_sanPham = id_sanPham,
				tenThongSo = tenThongSo,
				giaTri = giaTri
			};

			// Thêm thông số mới vào cơ sở dữ liệu
			db.ThongSoKyThuats.Add(thongSo);
			db.SaveChanges();

			// Redirect về action hiển thị danh sách thông số
			return RedirectToAction("DanhSachThongSo", new { id = id_sanPham });
		}



		[HttpPost]
		public ActionResult KiemTraTuyChon(int tuyChonId, int id_sanPham)
		{
			var chiTietSP = db.ChitietSPs.FirstOrDefault(c => c.id_tuyChon == tuyChonId && c.id_sanPham == id_sanPham);
			if (chiTietSP != null)
			{
				return Json(new { success = true, giaSP = chiTietSP.giaSP });
			}
			else
			{
				return Json(new { success = false, message = "Tùy chọn không tồn tại trong cơ sở dữ liệu." });
			}
		}


		[AdminAuth]
		public ActionResult xoaMau(int id_mau)
		{
			try
			{
				// Tìm khuyến mãi theo id
				using (var db = new DATotNghiepEntities())
				{
					var mau = db.MauSacs.FirstOrDefault(km => km.id_Mau == id_mau);
					if (mau != null)
					{
						mau.TrangThaiXoa = false;
						db.SaveChanges();
						TempData["Success"] = "Xóa khuyến mãi thành công!";
						return Json(new { success = true });
					}
					else
					{
						TempData["Error"] = "Không tìm thấy khuyến mãi có id " + id_mau;
						return Json(new { success = false, message = "Không tìm thấy khuyến mãi có id " + id_mau });
					}
				}
			}
			catch (Exception ex)
			{
				TempData["Error"] = "Lỗi: " + ex.Message;
				return Json(new { success = false, message = "Lỗi: " + ex.Message });
			}
		}


		[AdminAuth]
		public ActionResult xoaTuyChon(int id_tuyChon)
		{
			try
			{
				// Tìm khuyến mãi theo id
				using (var db = new DATotNghiepEntities())
				{
					var tuyChon = db.TuyChons.FirstOrDefault(km => km.id_tuyChon == id_tuyChon);
					if (tuyChon != null)
					{
						tuyChon.TrangThaiXoa = false;
						db.SaveChanges();
						TempData["Success"] = "Xóa khuyến mãi thành công!";
						return Json(new { success = true });
					}
					else
					{
						TempData["Error"] = "Không tìm thấy khuyến mãi có id " + id_tuyChon;
						return Json(new { success = false, message = "Không tìm thấy khuyến mãi có id " + id_tuyChon });
					}
				}
			}
			catch (Exception ex)
			{
				TempData["Error"] = "Lỗi: " + ex.Message;
				return Json(new { success = false, message = "Lỗi: " + ex.Message });
			}
		}



		[AdminAuth]
		public ActionResult xoaThongSo(int id_thongSo)
		{
			try
			{
				// Tìm khuyến mãi theo id
				using (var db = new DATotNghiepEntities())
				{
					var thongSo = db.ThongSoKyThuats.FirstOrDefault(km => km.id_ThongSo  == id_thongSo);
					if (thongSo != null)
					{
						thongSo.TrangThaiXoa = false;
						db.SaveChanges();
						TempData["Success"] = "Xóa khuyến mãi thành công!";
						return Json(new { success = true });
					}
					else
					{
						TempData["Error"] = "Không tìm thấy khuyến mãi có id " + id_thongSo;
						return Json(new { success = false, message = "Không tìm thấy khuyến mãi có id " + id_thongSo });
					}
				}
			}
			catch (Exception ex)
			{
				TempData["Error"] = "Lỗi: " + ex.Message;
				return Json(new { success = false, message = "Lỗi: " + ex.Message });
			}
		}


	}
}