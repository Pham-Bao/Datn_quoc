﻿using DATechShop.Areas.Admin.Content;
using DATechShop.Models;
using DATechShop.Others;
using Newtonsoft.Json;
using PagedList;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI;
using static DATechShop.Areas.Admin.Content.AuthAttribute;
using static System.Net.WebRequestMethods;
using System.Web.Helpers;


namespace DATechShop.Controllers
{
    public class MuaHangController : Controller
    {


        DATotNghiepEntities db = new DATotNghiepEntities();	
        [HttpPost]
        public ActionResult LayIdChiTietSP(int id_sanPham, string mauSac, string tuyChon)
        {
           

			if (mauSac == null && tuyChon == null)
			{
				var chiTietSp = db.ChitietSPs.FirstOrDefault(ct => ct.id_sanPham == id_sanPham);
				if (chiTietSp != null)
				{

					return Json(new { success = true, id_chiTietSP = chiTietSp.id_chiTietSP });
				}
			}
			else
			{
			
				var chiTietSP = db.ChitietSPs.FirstOrDefault(ct => ct.id_sanPham == id_sanPham && ct.MauSac.maMau == mauSac && ct.TuyChon.tuyChon1 == tuyChon);
				if (chiTietSP != null)
				{

					return Json(new { success = true, id_chiTietSP = chiTietSP.id_chiTietSP });
				}
				else
				{

					return Json(new { success = false });
				}
			}

			return Json(new { success = false });



		}
		public ActionResult GioHang(List<ChitietSP> chitietSPs)
        {
			ViewBag.CartItems = chitietSPs;
			return View(chitietSPs);
        }


        public ActionResult row(int id, int soLuong)
        {
			var chiTietSP = db.ChitietSPs.FirstOrDefault(ct => ct.id_chiTietSP == id);
			var giaSP = chiTietSP.giaSP;
			var thanhTien = giaSP * soLuong;
			ViewBag.ThanhTien = thanhTien;
			ViewBag.SoLuong = soLuong;
			return View(chiTietSP);
        }

        [HttpPost]
        public ActionResult KiemTraMaGiamGia(string maGiamGia)
        {
            
            string phanTramGiam = "0%";

            
            if (!string.IsNullOrEmpty(maGiamGia))
            {
            
                var khuyenMai = new mapKhuyenMai().timKM(maGiamGia);

               
                if (khuyenMai != null)
                {
                    
                    phanTramGiam = khuyenMai.phanTramGiam.ToString() + "%";
                }
                else
                {
                    
                    phanTramGiam = "Không có";
                }
            }

            
            return Json(new { phanTramGiam });
        }
		[UserAuth]
		public ActionResult datHang(float phanTramGiam)
		{
			//hiển thị list các tỉnh
			var provinces = db.provinces.ToList();
			ViewBag.Provinces = new SelectList(provinces, "id", "name");
			ViewBag.phamTramGiam = phanTramGiam; 
			return View();
		}


		public ActionResult ChonDiaChi()
		{
			var provinces = db.provinces.ToList();
			ViewBag.Provinces = new SelectList(provinces, "id", "name");
			return View();
		}

		public ActionResult LayTinhThanhPho()
		{
			var provinces = db.provinces.ToList();
			return Json(provinces, JsonRequestBehavior.AllowGet);
		}
		public ActionResult LayQuanHuyen(int? provinceId)
		{
			List<district> districtsList = new List<district>();

			if (provinceId == null)
			{
				var districts = db.districts.ToList();
				foreach (var d in districts)
				{
					districtsList.Add(new district
					{
						id = d.id,
						code = d.code,
						name = d.name,
						province_id = d.province_id
					});
				}
				return Json(districtsList, JsonRequestBehavior.AllowGet);
			}

			var filteredDistricts = db.districts.Where(d => d.province_id == provinceId).ToList();
			foreach (var d in filteredDistricts)
			{
				districtsList.Add(new district
				{
					id = d.id,
					code = d.code,
					name = d.name,
					province_id = d.province_id
				});
			}
			return Json(districtsList, JsonRequestBehavior.AllowGet);
		}

		public ActionResult LayXaPhuong(int? districtId)
		{
			List<ward> wardsList = new List<ward>(); // Khởi tạo danh sách mới

			if (districtId == null)
			{
				var wards = db.wards.ToList();
				foreach (var w in wards)
				{
					wardsList.Add(new ward
					{
						id = w.id,
						code = w.code,
						name = w.name,
						province_id = w.province_id,
						district_id = w.district_id
					});
				}
				return Json(wardsList, JsonRequestBehavior.AllowGet);
			}

			var filteredWards = db.wards.Where(w => w.district_id == districtId).ToList();
			foreach (var w in filteredWards)
			{
				wardsList.Add(new ward
				{
					id = w.id,
					code = w.code,
					name = w.name,
					province_id = w.province_id,
					district_id = w.district_id
				});
			}
			return Json(wardsList, JsonRequestBehavior.AllowGet);
		}
		public ActionResult LayThongTinSanPham(int chiTietSPId)
		{
			
			var thongTinSanPham = new mapSP().layChiTietSP(chiTietSPId);
			return Json(thongTinSanPham, JsonRequestBehavior.AllowGet);
		}




		[HttpPost]
		public ActionResult TaoHoaDon(string sdt, int idTinh, int idHuyen, int idXa, string soNha, string note, float tongCong, float phamTramGiam, string selectedPaymentMethod)
		{
			
				var tenNguoiDung = Session["TenNguoiDung"] as string;
				var soDienThoai = Session["SoDienThoai"] as string;
				var idNguoiDung = Session["id_NguoiDung"] as int?;
			    var nguoiDung = db.NguoiDungs.FirstOrDefault(ct => ct.id_NguoiDung == idNguoiDung);
			    var email = nguoiDung.email;



			var tenTinhObj = db.provinces.FirstOrDefault(p => p.id == idTinh);
			var tenTinh = tenTinhObj != null ? tenTinhObj.name : "";

			// Lấy tên của quận/huyện từ ID
			var tenHuyenObj = db.districts.FirstOrDefault(d => d.id == idHuyen);
			var tenHuyen = tenHuyenObj != null ? tenHuyenObj.name : "";

			// Lấy tên của xã/phường từ ID
			var tenXaObj = db.wards.FirstOrDefault(w => w.id == idXa);
			var tenXa = tenXaObj != null ? tenXaObj.name : "";

			var giamGia = Math.Round(tongCong * (phamTramGiam / 100)); // Giả sử phamTramGiam là một số nguyên
			var tongTien = tongCong - giamGia;
			// Tạo hóa đơn mới
			var newHoaDon = new HoaDon
			{
				id_NguoiDung = idNguoiDung,
				ngayTao = DateTime.Now,
				trangThai = 1,
				sdt = sdt,
				diaChi = $"{soNha}, {tenXa}, {tenHuyen}, {tenTinh}",
				tongTien = tongTien,
				giamGia = giamGia
			};

			    db.HoaDons.Add(newHoaDon);
				db.SaveChanges();
			

			if (selectedPaymentMethod == "vnpay")
			{

			

			string url = ConfigurationManager.AppSettings["Url"];
			string returnUrl = ConfigurationManager.AppSettings["ReturnUrl"];
			string tmnCode = ConfigurationManager.AppSettings["TmnCode"];
			string hashSecret = ConfigurationManager.AppSettings["HashSecret"];

			PayLib pay = new PayLib();

			var tongTienS = (tongTien * 100).ToString();
			var hoaDonId = newHoaDon.id_HoaDon.ToString();

			pay.AddRequestData("vnp_Version", "2.1.0"); //Phiên bản api mà merchant kết nối. Phiên bản hiện tại là 2.1.0
			pay.AddRequestData("vnp_Command", "pay"); //Mã API sử dụng, mã cho giao dịch thanh toán là 'pay'
			pay.AddRequestData("vnp_TmnCode", tmnCode); //Mã website của merchant trên hệ thống của VNPAY (khi đăng ký tài khoản sẽ có trong mail VNPAY gửi về)
			pay.AddRequestData("vnp_Amount", tongTienS); //số tiền cần thanh toán, công thức: số tiền * 100 - ví dụ 10.000 (mười nghìn đồng) --> 1000000
			pay.AddRequestData("vnp_BankCode", ""); //Mã Ngân hàng thanh toán (tham khảo: https://sandbox.vnpayment.vn/apis/danh-sach-ngan-hang/), có thể để trống, người dùng có thể chọn trên cổng thanh toán VNPAY
													// Lấy thời gian hiện tại
				DateTime now = DateTime.Now;

				// Chuyển múi giờ về UTC+7 bằng cách thêm 7 giờ
				DateTime utc7Time = now.AddHours(7);

				// Định dạng thời gian theo yêu cầu "yyyyMMddHHmmss"
				string formattedTime = utc7Time.ToString("yyyyMMddHHmmss");

				// Thêm dữ liệu vào request
				pay.AddRequestData("vnp_CreateDate", formattedTime); pay.AddRequestData("vnp_CurrCode", "VND"); //Đơn vị tiền tệ sử dụng thanh toán. Hiện tại chỉ hỗ trợ VND
			pay.AddRequestData("vnp_IpAddr", Util.GetIpAddress()); //Địa chỉ IP của khách hàng thực hiện giao dịch
			pay.AddRequestData("vnp_Locale", "vn"); //Ngôn ngữ giao diện hiển thị - Tiếng Việt (vn), Tiếng Anh (en)
			pay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang"); //Thông tin mô tả nội dung thanh toán
			pay.AddRequestData("vnp_OrderType", "other"); //topup: Nạp tiền điện thoại - billpayment: Thanh toán hóa đơn - fashion: Thời trang - other: Thanh toán trực tuyến
			pay.AddRequestData("vnp_ReturnUrl", returnUrl); //URL thông báo kết quả giao dịch khi Khách hàng kết thúc thanh toán
			pay.AddRequestData("vnp_TxnRef", hoaDonId); //mã hóa đơn


			string paymentUrl = pay.CreateRequestUrl(url, hashSecret);
			


	     	return Json(new { success = true, hoaDonId = newHoaDon.id_HoaDon, paymentUrl});
			}
			var id_hoaDon = newHoaDon.id_HoaDon;
			var newLoaiThanhToan = new LoaiThanhToan
			{
			
				id_HoaDon = id_hoaDon,
				ngayThanhToan = DateTime.Now,
				loaiThanhToan1 = "Tiền Mặt",
				trangThai = 0,
			};

			db.LoaiThanhToans.Add(newLoaiThanhToan);
			db.SaveChanges();
			var id_HoaDon = newLoaiThanhToan.id_HoaDon;
			var loaiThanhToan = newLoaiThanhToan.loaiThanhToan1;

			SendEmail(email, "TechShop xin cảm ơn!", $"Quý khách đã mua thành công đơn hàng. Mã hóa đơn: {id_HoaDon}. Thanh toán bằng{loaiThanhToan}");


			return Json(new { success = true, hoaDonId = newHoaDon.id_HoaDon });



		}


		[HttpPost]
		public ActionResult TaoChiTietHoaDon(int hoaDonId, List<int> chiTietSPIds, List<int> soLuongs)
		{


			if (chiTietSPIds.Count != soLuongs.Count)
			{
				return Json(new { success = false, message = "Số lượng chi tiết sản phẩm không khớp với số lượng sản phẩm." });
			}


			for (int i = 0; i < chiTietSPIds.Count; i++)
			{
				var chiTietSPId = chiTietSPIds[i];
				var soLuong = soLuongs[i];

				var chiTietSP = db.ChitietSPs.FirstOrDefault(ct => ct.id_chiTietSP == chiTietSPId);
				if (chiTietSP == null)
				{
					return Json(new { success = false, message = $"Không tìm thấy chi tiết sản phẩm với ID {chiTietSPId}." });
				}


				var chiTietHoaDon = new ChiTietHoaDon
				{
					id_HoaDon = hoaDonId,
					id_chiTietSP = chiTietSPId,
					soLuong = soLuong,
					gia = chiTietSP.giaSP * soLuong
				};


				db.ChiTietHoaDons.Add(chiTietHoaDon);
			}


			db.SaveChanges();
			return Redirect("~/trangChu/home");


		}


		public ActionResult PaymentConfirm()
		{
			if (Request.QueryString.Count > 0)
			{
				string hashSecret = ConfigurationManager.AppSettings["HashSecret"]; //Chuỗi bí mật
				var vnpayData = Request.QueryString;
				PayLib pay = new PayLib();

				//lấy toàn bộ dữ liệu được trả về
				foreach (string s in vnpayData)
				{
					if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
					{
						pay.AddResponseData(s, vnpayData[s]);
					}
				}

				long orderId = Convert.ToInt64(pay.GetResponseData("vnp_TxnRef")); //mã hóa đơn
				long vnpayTranId = Convert.ToInt64(pay.GetResponseData("vnp_TransactionNo")); //mã giao dịch tại hệ thống VNPAY
				string vnp_ResponseCode = pay.GetResponseData("vnp_ResponseCode"); //response code: 00 - thành công, khác 00 - xem thêm https://sandbox.vnpayment.vn/apis/docs/bang-ma-loi/
				string vnp_SecureHash = Request.QueryString["vnp_SecureHash"]; //hash của dữ liệu trả về

				bool checkSignature = pay.ValidateSignature(vnp_SecureHash, hashSecret); //check chữ ký đúng hay không?

				if (checkSignature)
				{
					if (vnp_ResponseCode == "00")
					{
						
						//Thanh toán thành công
						ViewBag.Message = "Thanh toán thành công hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId;
						var hoaDon = db.HoaDons.Find(orderId);
						hoaDon.trangThai = 2;
						db.SaveChanges();

						var emailNguoiDung = hoaDon.NguoiDung.email;
						SendEmail(emailNguoiDung, "TechShop xin cảm ơn!", $"Quý khách đã mua thành công đơn hàng. Mã hóa đơn: {hoaDon.id_HoaDon}. Thanh toán bằng VNP, mã giao dịch: {vnpayTranId}");



						int orderIdInt = Convert.ToInt32(orderId);
						var newLoaiThanhToan = new LoaiThanhToan
						{
							
					     	id_HoaDon = orderIdInt,
							ngayThanhToan = DateTime.Now,
							loaiThanhToan1 = "VNPay",
							trangThai = 1,
						};

						db.LoaiThanhToans.Add(newLoaiThanhToan);
						db.SaveChanges();
					}
					else
					{
						var loaiThanhToan = db.LoaiThanhToans.Find(orderId);
						loaiThanhToan.trangThai = 0;
						db.SaveChanges();
						ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId + " | Mã lỗi: " + vnp_ResponseCode;
					}
				}
				else
				{
					ViewBag.Error = "Có lỗi xảy ra trong quá trình xử lý";
				}
			}

			return View();
		}


		[UserAuth]
		public ActionResult DonHang(int id_nguoiDung, int? page, int? trangThaiDon)
		{

			mapHoaDon map = new mapHoaDon();
			var data = map.DanhSacHoaDonKhach(trangThaiDon, id_nguoiDung).OrderByDescending(x => x.id_HoaDon);
			int pageSize = 6;
			int pageNumber = (page ?? 1);

			var pagedList = data.ToPagedList(pageNumber, pageSize);
			ViewBag.trangThaiDon = trangThaiDon;
			ViewBag.id_nguoiDung = id_nguoiDung;
			ViewBag.DbContext = new DATotNghiepEntities();
			return View(pagedList);
		}
	


		public ActionResult doiTrangThai(int value, int id_hoaDon)
		{
			using (var db = new DATotNghiepEntities())
			{

				var hoaDon = db.HoaDons.FirstOrDefault(d => d.id_HoaDon == id_hoaDon);

				if (hoaDon != null)
				{

					hoaDon.trangThai = value;

					db.SaveChanges();


					return RedirectToAction("DonHang", "muaHang", new { id_nguoiDung = hoaDon.id_NguoiDung });

				}
				else
				{

					ViewBag.ErrorMessage = "Không tìm thấy hóa đơn có ID " + id_hoaDon;
					return View("Error");
				}
			}
		}



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


		private DateTime GetExpiryTime()
		{
			return DateTime.Now.AddMinutes(1); // Mã OTP chỉ có hiệu lực trong 1 phút
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



		[UserAuth]
		public ActionResult ChiTietHDNguoiDung(int id_hoaDon, int? page)
		{

			mapHoaDon map = new mapHoaDon();
			var data = map.ChiTietHoaDon(id_hoaDon).OrderByDescending(x => x.id_HoaDon);
			int pageSize = 3;
			int pageNumber = (page ?? 1);
			var pagedList = data.ToPagedList(pageNumber, pageSize);

			ViewBag.id_chitietHoaDon = id_hoaDon;
			return View(pagedList);
		}

		public ActionResult SearchGiamGia(string key)
		{
			var data = db.KhuyenMais
				.Where(m => string.IsNullOrEmpty(key) || m.moTaKhuyenMai.ToLower().Contains(key.ToLower()))
				.Select(m => new
				{
					m.hinhAnh,
					m.tenMa,
					m.phanTramGiam,
					m.moTaKhuyenMai
					
					// Add more properties if needed
				})
				.ToList();

			return Json(data, JsonRequestBehavior.AllowGet);
		}





	}
}