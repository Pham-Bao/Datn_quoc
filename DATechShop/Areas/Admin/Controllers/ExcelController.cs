using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using ClosedXML.Excel;
using DATechShop.Models;

namespace DATechShop.Areas.Admin.Controllers
{
	public class ExcelController : Controller
	{
		public ActionResult ExportToExcel(string startDate, string endDate)
		{
			try
			{
				DateTime startDateTime = DateTime.Parse(startDate);
				DateTime endDateTime = DateTime.Parse(endDate);

				using (DATotNghiepEntities db = new DATotNghiepEntities())
				{
					var data = db.LoaiThanhToans.Where(ltt => ltt.ngayThanhToan >= startDateTime && ltt.ngayThanhToan <= endDateTime).ToList();

					if (data.Any())
					{
						DataTable dt = new DataTable("Export");
						dt.Columns.AddRange(new DataColumn[6] {
						new DataColumn("ID_ThanhToan", typeof(int)),
						new DataColumn("ID_HoaDon", typeof(int)),
						new DataColumn("LoaiThanhToan", typeof(string)),
						new DataColumn("TrangThai", typeof(string)),
						new DataColumn("NgayThanhToan", typeof(DateTime)),
						new DataColumn("TongTien", typeof(decimal))
					});

						foreach (var item in data)
						{
							var hoaDon = db.HoaDons.FirstOrDefault(hd => hd.id_HoaDon == item.id_HoaDon);
							decimal tongTien = hoaDon != null ? Convert.ToDecimal(hoaDon.tongTien) : 0;
							dt.Rows.Add(item.id_ThanhToan, item.id_HoaDon, item.loaiThanhToan1, item.trangThai, item.ngayThanhToan, tongTien);
						}

						using (XLWorkbook wb = new XLWorkbook())
						{
							wb.Worksheets.Add(dt);
							using (MemoryStream stream = new MemoryStream())
							{
								wb.SaveAs(stream);
								return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "LichSuChuyenTien.xlsx");
							}
						}
					}
					else
					{
						ViewBag.ErrorMessage = "Không có dữ liệu để xuất.";
						TempData["ErrorMessage"] = "Không có dữ liệu để xuất.";
						return View("Error");
					}
				}
			}
			catch (Exception ex)
			{
				ViewBag.ErrorMessage = "Đã xảy ra lỗi: " + ex.Message;
				return View("Error");
			}
		}
	}
}