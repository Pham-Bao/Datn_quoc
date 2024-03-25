using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace DATechShop.Controllers
{
	public class GmailController : Controller
	{
		// GET: OTP
		public ActionResult Send()
		{
			return View();
		}

		// POST: OTP/Send
		[HttpPost]
		public ActionResult Send(string emailAddress)
		{
			try
			{
				// Kiểm tra địa chỉ email có hợp lệ không
				if (!IsValidEmail(emailAddress))
				{
					ViewBag.Error = "Địa chỉ email không hợp lệ.";
					return View("Index");
				}

				// Tạo mã OTP ngẫu nhiên
				string otp = GenerateOTP();

				// Gửi mã OTP đến địa chỉ email của người dùng
				SendEmail(emailAddress, "Mã OTP", $"Mã OTP của bạn là: {otp}");

				// Chuyển hướng đến trang nhập mã OTP để người dùng nhập
				return RedirectToAction("Verify", new { emailAddress = emailAddress, otp = otp });
			}
			catch (Exception ex)
			{
				// Xử lý lỗi nếu có
				ViewBag.Error = "Đã xảy ra lỗi khi gửi mã OTP. Vui lòng thử lại sau.";
				return View("Index");
			}
		}

		// GET: OTP/Verify
		public ActionResult Verify(string emailAddress, string otp)
		{
			ViewBag.EmailAddress = emailAddress;
			ViewBag.OTP = otp;
			return View();
		}

		// POST: OTP/Verify
		[HttpPost]
		public ActionResult Verify(string emailAddress, string otpEntered, string otp)
		{
			if (otpEntered == otp)
			{
				// Mã OTP nhập đúng, thực hiện các hành động tiếp theo (ví dụ: xác thực người dùng)
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

		// Phương thức để tạo mã OTP ngẫu nhiên
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
	}
}
