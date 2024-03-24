using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace DATechShop.Areas.Admin.Content
{
	public class AuthAttribute : ActionFilterAttribute
	{
		public class AdminAuthAttribute : ActionFilterAttribute
		{
			public override void OnActionExecuting(ActionExecutingContext filterContext)
			{
				var userAdmin = filterContext.HttpContext.Session["tk"];
				if (userAdmin == null)
				{
					filterContext.Result = new RedirectToRouteResult(
						new System.Web.Routing.RouteValueDictionary {
						{ "area", "Admin" },
						{ "controller", "NguoiDung" },
						{ "action", "DangNhap" }
						});
				}
				base.OnActionExecuting(filterContext);
			}
		}

		public class UserAuthAttribute : ActionFilterAttribute
		{
			public override void OnActionExecuting(ActionExecutingContext filterContext)
			{
				var user = filterContext.HttpContext.Session["id_NguoiDung"];
				if (user == null)
				{
					filterContext.Result = new RedirectToRouteResult(
						new System.Web.Routing.RouteValueDictionary {
						{ "area", "Admin" },
						{ "controller", "NguoiDung" },
						{ "action", "DangNhap" }
						});
				}
				base.OnActionExecuting(filterContext);
			}
		}
	}
}