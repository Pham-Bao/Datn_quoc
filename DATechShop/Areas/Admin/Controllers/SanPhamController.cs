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


			ViewBag.TotalUsers = totalUsers;
			ViewBag.TotalOrders = totalOders;
			ViewBag.TotalRevenueByMonth = totalRevenueByMonth;
			ViewBag.totalConment = totalConment; 

			
			return View();
		}

		[HttpPost]
		public JsonResult TinhToan(string startDate, string endDate)
		{
			int totalOrders = 0;
			double totalRevenue = 0;

			if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
			{
				DateTime startDateTime = DateTime.Parse(startDate);
				DateTime endDateTime = DateTime.Parse(endDate).AddDays(1); 

				totalOrders = db.HoaDons
					.Count(dh => dh.ngayTao != null && dh.ngayTao >= startDateTime && dh.ngayTao < endDateTime);

				totalRevenue = db.HoaDons
					.Where(dh => dh.ngayTao != null && dh.ngayTao >= startDateTime && dh.ngayTao < endDateTime)
					.Sum(dh => (double?)dh.tongTien) ?? 0;
			}
			else
			{
				totalOrders = db.HoaDons.Count();
				totalRevenue = db.HoaDons.Sum(dh => (double?)dh.tongTien) ?? 0;
			}

			return Json(new { TotalOrders = totalOrders, TotalRevenue = totalRevenue }, JsonRequestBehavior.AllowGet);
		}


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
			var data = map.DanhSachTuyChon().OrderByDescending(x => x.id_tuyChon); 
			int pageSize = 5; 
			int pageNumber = (page ?? 1); 

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

					TempData["SuccessMessage"] = "Thêm sản phẩm thành công!";
					return RedirectToAction("DanhSachSP", "SanPham");
					
				}
				catch (Exception ex)
				{
					ViewBag.Error = "Thêm lỗi!";
					ViewBag.err = "Lỗi khi thêm sản phẩm: " + ex.Message;
					TempData["ErrorMessage"] = "Lỗi khi thêm sản phẩm: ";

				}
			}
			else
			{
				ViewBag.Error = "Thêm lỗi!";
				ViewBag.err = "Không có ảnh" ;
				TempData["ErrorMessage"] = "Lỗi khi thêm sản phẩm: ";

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

			var pagedList = data.ToPagedList(pageNumber, pageSize);

			ViewBag.loaiSP = loaiSP;

			return View(pagedList);
		}

		[AdminAuth]
		public ActionResult ThemChiTietSP(int id)
		{


			var sanPham = db.SanPhams.FirstOrDefault(sp => sp.id_sanPham == id);
			if (sanPham.TrangThaiXoa != null)
			{
				TempData["ErrorMessage"] = "Sản phẩm đã được xóa khỏi hệ thống không thể thêm!";
				return RedirectToAction("DanhSachSP", "SanPham");
			}
			
			var danhSachMau = db.MauSacs.ToList();
			var danhSachTuyChon = db.TuyChons.ToList();

			var viewModel = new ThemChiTietSPViewModel
			{
				SanPham = sanPham,
				DanhSachMau = danhSachMau,
				DanhSachTuyChon = danhSachTuyChon
			};


			return View(viewModel);
		}


		//Action này chỉ cho phép người dùng có quyền admin thêm thông tin chi tiết cho sản phẩm.
		//Chi tiết sản phẩm
		[AdminAuth]
		[HttpPost]
		public ActionResult ThemChiTietSP(ThemChiTietSPViewModel viewModel, HttpPostedFileBase uploadhinh)
		{
			try
			{   // Kiểm tra sự toàn vẹn của dữ liệu
				if (uploadhinh == null || viewModel.ChitietSP.giaSP == null)
				{
					ViewBag.Error = "Chưa chọn ảnh hoặc nhập giá.";
				}
				else if (ModelState.IsValid)
				{
					// Kiểm tra trùng lặp
					var existingRecord = db.ChitietSPs.FirstOrDefault(c => c.id_sanPham == viewModel.SanPham.id_sanPham &&
																			c.id_Mau == viewModel.SelectedMau &&
																			c.id_tuyChon == viewModel.SelectedTuyChon);
					// nếu mà nội dung này đã có trong cơ sở dữ liệu thì hiển thị thông báo đã tồn tại.
					if (existingRecord != null)
					{
						ViewBag.Error = "Bản ghi đã tồn tại trong bảng chi tiết sản phẩm.";
						return View(viewModel);
					}
					// trường hợp có thể thêm mới được 


					var chiTietSP = new ChitietSP();
					chiTietSP.id_sanPham = viewModel.SanPham.id_sanPham;
					chiTietSP.id_Mau = viewModel.SelectedMau;
					chiTietSP.id_tuyChon = viewModel.SelectedTuyChon;
					chiTietSP.giaSP = viewModel.ChitietSP.giaSP;

					if (uploadhinh != null && uploadhinh.ContentLength > 0)
					{
						string _FileName = "";
						int id = int.Parse(db.ChitietSPs.ToList().Last().id_chiTietSP.ToString());
						int index = uploadhinh.FileName.IndexOf('.');
						_FileName = "anhSP_" + id.ToString() + "." + uploadhinh.FileName.Substring(index + 1);
						string _path = Path.Combine(Server.MapPath("~/Upload/imgSP"), _FileName);
						uploadhinh.SaveAs(_path);

						chiTietSP.anhSP = _FileName;
					}

					db.ChitietSPs.Add(chiTietSP);
					db.SaveChanges();
					ViewBag.Success = "Thêm thành công";
					TempData["SuccessMessage"] = "Thêm sản phẩm thành công!";
				}

				return RedirectToAction("ChiTietSP", "SanPham", new { id = viewModel.SanPham.id_sanPham });
			}
			catch (Exception ex)
			{
				ViewBag.Error = "Đã xảy ra lỗi khi thêm chi tiết sản phẩm. Vui lòng thử lại sau.";
				return View(viewModel);
			}
		}


		[AdminAuth]
		public ActionResult ChiTietSP(int id, int? page)
		{
			mapSP map = new mapSP();
			var data = map.chiTietSP(id);
			var sanPham = map.sanPham(id);
			int pageSize = 5;
			int pageNumber = (page ?? 1); 

			var pagedList = data.ToPagedList(pageNumber, pageSize);
			ViewBag.id_sanPham = id;
			ViewBag.sanPham = sanPham;
			return View(pagedList);
		}
		[AdminAuth]
		public ActionResult DanhSachThongSo(int id, int? page)
		{
			mapSP map = new mapSP();
			var data = map.chiTietThongSo(id);

			int pageSize = 5; 
			int pageNumber = (page ?? 1); 

			var pagedList = data.ToPagedList(pageNumber, pageSize);

			ViewBag.IdSanPham = id; 

			return View(pagedList);
		}

		[HttpPost]
		public ActionResult ThemThongSo(FormCollection form)
		{
			int id_sanPham;
			if (!int.TryParse(form["id_sanPham"], out id_sanPham))
			{
				return HttpNotFound();
			}

			SanPham sanPham = db.SanPhams.Find(id_sanPham);
			if (sanPham == null)
			{
				return HttpNotFound();
			}

			string tenThongSo = form["tenThongSo"];
			string giaTri = form["giaTri"];

			ThongSoKyThuat thongSo = new ThongSoKyThuat
			{
				id_sanPham = id_sanPham,
				tenThongSo = tenThongSo,
				giaTri = giaTri
			};

			db.ThongSoKyThuats.Add(thongSo);
			db.SaveChanges();

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







		[AdminAuth]
		public ActionResult SuaSanPham(int id_sanPham)
		{
			using (var db = new DATotNghiepEntities())
			{
				var sanPham = db.SanPhams.FirstOrDefault(km => km.id_sanPham == id_sanPham);
				if (sanPham == null)
				{
					TempData["ErrorMessage"] = "Không tìm thấy sản phẩm có id " + id_sanPham;
					return RedirectToAction("DanhSachSP", "SanPham");
				}
				return View(sanPham);
			}
		}



		[HttpPost]
		[AdminAuth]
		public ActionResult LuuSuaSanPham(SanPham model)
		{
			try
			{
				using (var db = new DATotNghiepEntities())
				{
					var sanPham = db.SanPhams.FirstOrDefault(km => km.id_sanPham == model.id_sanPham);

					if (sanPham == null)
					{
						TempData["ErrorMessage"] = "Không tìm thấy sản phẩm có ID " + model.id_sanPham;
						return RedirectToAction("DanhSachSP", "SanPham");
					}

					sanPham.tenSP = model.tenSP;
					sanPham.loaiSP = model.loaiSP;
					sanPham.moTa = model.moTa;
					sanPham.ghiChu = model.ghiChu;
					sanPham.khuyenMai = model.khuyenMai;

					db.SaveChanges();

					TempData["SuccessMessage"] = "Cập nhật thông tin  thành công!";
					return RedirectToAction("DanhSachSP", "SanPham");
				}
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = "Lỗi khi cập nhật thông tin: " + ex.Message;
				return RedirectToAction("DanhSachSP", "SanPham");
			}
		}



		[AdminAuth]
		public ActionResult xoaChiTietSP(int id_chiTietSP)
		{
			try
			{
				using (var db = new DATotNghiepEntities())
				{
					var chiTietSP = db.ChitietSPs.FirstOrDefault(km => km.id_chiTietSP == id_chiTietSP);
					if (chiTietSP != null)
					{
						chiTietSP.TrangThaiXoa = false;
						db.SaveChanges();
						TempData["SuccessMessage"] = "Sản phẩm đã không được bán";
						return Json(new { success = true });
					}
					else
					{
						TempData["ErrorMessage"] = "Không tìm thấy sản phẩm muốn xóa " + chiTietSP;
						return Json(new { success = false, message = "Không tìm thấy sản phẩm muốn xóa" + chiTietSP });
					}
				}
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = "Lỗi: " + ex.Message;
				return Json(new { success = false, message = "Lỗi: " + ex.Message });
			}
		}


		[AdminAuth]
		public ActionResult xoaSanPham(int id_sanPham)
		{
			try
			{
				using (var db = new DATotNghiepEntities())
				{
					var sanPham = db.SanPhams.FirstOrDefault(km => km.id_sanPham == id_sanPham);
					if (sanPham != null)
					{
						sanPham.TrangThaiXoa = false;
						db.SaveChanges();
						TempData["SuccessMessage"] = "Xóa khuyến mãi thành công!";
						return Json(new { success = true });
					}
					else
					{
						TempData["ErrorMessage"] = "Không tìm thấy khuyến mãi có id ";
						return Json(new { success = false, message = "Không tìm thấy khuyến mãi có id "});
					}
				}
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = "Lỗi: " + ex.Message;
				return Json(new { success = false, message = "Lỗi: " + ex.Message });
			}
		}
	}
}