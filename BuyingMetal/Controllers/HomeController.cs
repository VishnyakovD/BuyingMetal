using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using Microsoft.ApplicationInsights;

namespace BuyingMetal.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			return View();
		}

		public string Send(string tel)
		{
			try
			{
				if (!tel.IsNullOrWhiteSpace())
				{
					//IMailingManager mail = new MailingManager();
					//mail.SendMailNewOrder();
					SendMailEx.SendMailExAsyncOrder(tel);
					return "yes";
				}
				return "phone is ampty";
			}
			catch (Exception e)
			{
				return e.Message;
			}
		}
	}
}