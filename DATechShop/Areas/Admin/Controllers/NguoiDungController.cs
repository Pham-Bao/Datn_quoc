using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DATechShop.Areas.Admin.Content;
using DATechShop.Models;
using PagedList;
using static DATechShop.Areas.Admin.Content.AuthAttribute;
using System.Web.Helpers;
using System.Security;
using System.Data.Entity;

namespace DATechShop.Areas.Admin.Controllers
{
    public class NguoiDungController : Controller

    {
		// GET: Admin/NguoiDung
		DATotNghiepEntities db = new DATotNghiepEntities();
		[AdminAuth]
		public ActionResult DanhSachNguoiDung(int? page)
		{
			mapNguoiDung map = new mapNguoiDung();
			var data = map.DanhSachNguoiDung().OrderByDescending(x => x.id_NguoiDung); 
			int pageSize = 6; 
			int pageNumber = (page ?? 1);

			
			var pagedList = data.ToPagedList(pageNumber, pageSize);

			return View(pagedList);
		}
		public ActionResult DangKyTaiKhoan()
		{

			return View();

		}
		[HttpPost]
		public ActionResult DangKyTaiKhoan(NguoiDung model, string matKhau, string mk)
		{
			var existingUser = db.NguoiDungs.FirstOrDefault(u => u.sdt == model.sdt);
			if (existingUser != null)
			{
				ViewBag.phone = "(*)Số điện thoại đã được sử dụng!";
				return View();
			}
			var existingEmail = db.NguoiDungs.FirstOrDefault(u => u.email == model.email);
			if (existingEmail != null)
			{
				ViewBag.phone = "(*)Email đã được sử dụng!";
				return View();
			}

			if (matKhau != mk)
			{
				ViewBag.error = "Mật Khẩu không khớp";
				return View();
			}
            var email = model.email;
			try
			{
				// Kiểm tra địa chỉ email có hợp lệ không
				if (!IsValidEmail(email))
				{
					ViewBag.Error = "Địa chỉ email không hợp lệ.";
					return View("Index");
				}

				// Tạo mã OTP ngẫu nhiên
				string otp = GenerateOTP();

				// Gửi mã OTP đến địa chỉ email của người dùng
				SendEmail(email, "Mã OTP", $"Mã OTP của bạn là: {otp}");
				string hashedPassword = HashingHelper.HashPassword(matKhau);
				model.matKhau = hashedPassword;

				db.NguoiDungs.Add(model);
				db.SaveChanges();
				// Chuyển hướng đến trang nhập mã OTP để người dùng nhập
				return RedirectToAction("Verify", new { emailAddress = email, otp = otp });
			}
			catch (Exception ex)
			{
				// Xử lý lỗi nếu có
				ViewBag.Error = "Đã xảy ra lỗi khi gửi mã OTP. Vui lòng thử lại sau.";
				return View("Index");
			}
		

			ViewBag.success = "Chúc mừng. Bạn đã đăng ký thành công";
			return View();
		}
		// GET: OTP/Verify
		public ActionResult Verify(string emailAddress, string otp)
		{
			ViewBag.EmailAddress = emailAddress;
			ViewBag.OTP = otp;
			return View();
		}
		[HttpPost]
		public ActionResult Verify(string emailAddress, string otpEntered, string otp)
		{
			var existingUser = db.NguoiDungs.FirstOrDefault(u => u.email == emailAddress);
			if (otpEntered == otp)
			{
			
				existingUser.TrangThaiXoa = true; 
				db.SaveChanges();
				ViewBag.Success = "Xác thực thành công!";
			}
			else
			{
				// Mã OTP nhập không đúng
				ViewBag.Error = "Mã OTP không đúng. Vui lòng thử lại.";
			}

			ViewBag.EmailAddress = emailAddress;
			ViewBag.OTP = otp;
			return View();
		}


		public ActionResult DangNhap()
		{

			return View();

		}

		[HttpPost]
		public ActionResult DangNhap(string email, string mk)
		{
			
			var user = db.NguoiDungs.FirstOrDefault(u => u.email == email);


			if (email != null && mk != null && email.ToLower() == "admin@gmail.com" && mk.ToLower() == "1")
			{
				Session["tk"] = "AdminTechShop";
				return RedirectToAction("ThongKe", "SanPham");
			}
			if (user != null && HashingHelper.VerifyPassword(mk, user.matKhau))
			{
				// Đăng nhập thành công
				Session["id_NguoiDung"] = user.id_NguoiDung;
				Session["TenNguoiDung"] = user.ten;
				Session["SoDienThoai"] = user.sdt;
				Session["DiaChi"] = user.diaChi;

				ViewBag.Success = "Đăng Nhập thành công";
				TempData["SuccessMessage"] = "Thêm sản phẩm thành công";

				// Gọi hàm JavaScript để hiển thị thông báo
				ViewBag.NotificationMessage = TempData["SuccessMessage"];
				return RedirectToAction("Home", "TrangChu", new { area = "" });

			}
			else
			{
				// Đăng nhập không thành công
				ViewBag.Error = "(*)Số điện thoại hoặc mật khẩu không đúng.";
				return View();
			}
		}


		public ActionResult DangXuat()
		{
			Session.Remove("id_NguoiDung");
			Session.Remove("TenNguoiDung");
			Session.Remove("SoDienThoai");
			Session.Remove("DiaChi");
			Session.Remove("tk");
			
			return RedirectToAction("Home", "TrangChu", new { area = "" });

		}


		private string GenerateOTP()
		{
			Random rand = new Random();
			return rand.Next(100000, 999999).ToString(); // Mã OTP là một số có 6 chữ số
		}
		private DateTime GetExpiryTime()
		{
			return DateTime.Now.AddMinutes(1); // Mã OTP chỉ có hiệu lực trong 1 phút
		}

		// Phương thức để gửi email chứa mã OTP đến địa chỉ email của người dùng
		private void SendEmail(string toAddress, string subject, string body)
		{
			var fromAddress = new MailAddress(ConfigurationManager.AppSettings["FromEmailAddress"]);
			var toAddr = new MailAddress(toAddress);
			string smtpServer = ConfigurationManager.AppSettings["SMTPServer"];
			int smtpPort = int.Parse(ConfigurationManager.AppSettings["SMTPPort"]);
			string username = ConfigurationManager.AppSettings["SMTPUsername"];
			string password = ConfigurationManager.AppSettings["SMTPPassword"];

			var smtp = new SmtpClient
			{
				Host = smtpServer,
				Port = smtpPort,
				EnableSsl = true,
				DeliveryMethod = SmtpDeliveryMethod.Network,
				UseDefaultCredentials = false,
				Credentials = new NetworkCredential(username, password)
			};

			using (var message = new MailMessage(fromAddress, toAddr)
			{
				Subject = subject,
				Body = body
			})
			{
				smtp.Send(message);
			}
		}

		// Phương thức để kiểm tra địa chỉ email có hợp lệ không
		private bool IsValidEmail(string email)
		{
			try
			{
				var addr = new MailAddress(email);
				return addr.Address == email;
			}
			catch
			{
				return false;
			}
		}

		[AdminAuth]
		public ActionResult xoaNguoiDung(int id_nguoiDung)
		{
			try
			{
				// Tìm khuyến mãi theo id
				using (var db = new DATotNghiepEntities())
				{
					var nguoiDung = db.NguoiDungs.FirstOrDefault(km => km.id_NguoiDung == id_nguoiDung);
					if (nguoiDung != null)
					{
						nguoiDung.TrangThaiXoa = false;
						db.SaveChanges();
						TempData["Success"] = "Xóa  thành công!";
						return Json(new { success = true });
					}
					else
					{
						TempData["Error"] = "Không tìm thấy " + id_nguoiDung;
						return Json(new { success = false, message = "Không tìm thấy " + id_nguoiDung });
					}
				}
			}
			catch (Exception ex)
			{
				TempData["Error"] = "Lỗi: " + ex.Message;
				return Json(new { success = false, message = "Lỗi: " + ex.Message });
			}
		}


		public ActionResult forgotPass()
		{ 
			return View();
		}


		[HttpPost]
		public ActionResult forgotPass(string otpEntered)
		{
			var email = otpEntered;
			try
			{
				// Kiểm tra địa chỉ email có hợp lệ không
				if (!IsValidEmail(email))
				{
					ViewBag.Error = "Địa chỉ email không hợp lệ.";
					return View("Index");
				}

				// Tạo mã OTP ngẫu nhiên
				string otp = GenerateOTP();

				// Gửi mã OTP đến địa chỉ email của người dùng
				SendEmail(email, "Mã OTP", $"Mã OTP của bạn là: {otp}");
			
				// Chuyển hướng đến trang nhập mã OTP để người dùng nhập
				return RedirectToAction("otp", new { emailAddress = email, otp = otp });
			}
			catch (Exception ex)
			{
				// Xử lý lỗi nếu có
				ViewBag.Error = "Đã xảy ra lỗi khi gửi mã OTP. Vui lòng thử lại sau.";
				return View("Index");
			}
		}




		public ActionResult otp(string emailAddress, string otp)
		{
			ViewBag.EmailAddress = emailAddress;
			ViewBag.OTP = otp;
			return View();
		}
		[HttpPost]
		public ActionResult otp(string emailAddress, string otpEntered, string otp)
		{
			var existingUser = db.NguoiDungs.FirstOrDefault(u => u.email == emailAddress);
			if (otpEntered == otp)
			{
				ViewBag.EmailAddress = emailAddress;
				//existingUser.TrangThaiXoa = true;
				db.SaveChanges();
				ViewBag.Success = "Xác thực thành công!";
				return RedirectToAction("resetPass", new { emailAddress = emailAddress });

			}
			else
			{
				// Mã OTP nhập không đúng
				ViewBag.Error = "Mã OTP không đúng. Vui lòng thử lại.";
			}

			ViewBag.EmailAddress = emailAddress;
			ViewBag.OTP = otp;
			return View();
		}

		public ActionResult resetPass(string emailAddress)
		{
			var email = emailAddress;
			ViewBag.EmailAddress = email;
			return View();
		}

		[HttpPost]
		public ActionResult resetPass(string mk1, string mk2, string emailAddress)
		{
			if (mk1 != mk2)
			{
				ViewBag.Error = "Mật khẩu không khớp";
				return View();
			}
			if(emailAddress == null)
			{
				ViewBag.Error = "Email bằng O";

			}
			// Kiểm tra xem email có tồn tại trong cơ sở dữ liệu hay không
			var user = db.NguoiDungs.FirstOrDefault(u => u.email == emailAddress);
			if (user == null)
			{
				ViewBag.Error = "Email không tồn tại trong hệ thống."+ emailAddress;
				return View();
			}

			try
			{
				string hashedPassword = HashingHelper.HashPassword(mk1);
				user.matKhau = hashedPassword;
				db.SaveChanges();

				ViewBag.Success = "Đổi mật khẩu thành công!";
				Session["id_NguoiDung"] = user.id_NguoiDung;
				Session["TenNguoiDung"] = user.ten;
				Session["SoDienThoai"] = user.sdt;
				Session["DiaChi"] = user.diaChi;
			}
			catch (Exception ex)
			{
				ViewBag.Message = "Đã xảy ra lỗi khi thực hiện đổi mật khẩu: " + ex.Message;
			}


			return RedirectToAction("Home", "TrangChu", new { area = "" });


		}



	}
}