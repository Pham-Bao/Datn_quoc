using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DATechShop.Areas.Admin.Content
{
	public class AuthAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			var userId = filterContext.HttpContext.Session["tk"];
			if (userId == null)
			{
				filterContext.Result = new RedirectToRouteResult(
					new System.Web.Routing.RouteValueDictionary {
					{ "controller", "NguoiDung" },
					{ "action", "DangNhap" }
					});
			}
			base.OnActionExecuting(filterContext);
		}
	}
}