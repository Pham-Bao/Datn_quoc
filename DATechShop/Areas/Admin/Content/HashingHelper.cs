using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace DATechShop.Areas.Admin.Content
{
	public class HashingHelper
	{
		public static string HashPassword(string password)
		{
			using (SHA256 sha256 = SHA256.Create())
			{
				byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
				StringBuilder builder = new StringBuilder();
				for (int i = 0; i < hashedBytes.Length; i++)
				{
					builder.Append(hashedBytes[i].ToString("x2"));
				}
				return builder.ToString();
			}
		}

		internal static bool VerifyPassword(string mk, string hashedPassword)
		{
			// Băm mật khẩu người dùng nhập vào để so sánh với mật khẩu đã được băm trong cơ sở dữ liệu
			string hashedInputPassword = HashPassword(mk);
			// So sánh hai giá trị băm
			return string.Equals(hashedInputPassword, hashedPassword, StringComparison.OrdinalIgnoreCase);
		}
	}
	
}