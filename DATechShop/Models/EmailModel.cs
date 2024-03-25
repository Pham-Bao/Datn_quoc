using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DATechShop.Models
{
	public class EmailModel
	{
		[EmailAddress(ErrorMessage = "Invalid email address")]
		public string EmailAddress { get; set; }
	}
}