using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using Microsoft.Ajax.Utilities;
using Microsoft.ApplicationInsights;

namespace BuyingMetal.Controllers
{
	public class HController : Controller
	{
		public ActionResult I(string message)
		{
			if (!message.IsNullOrWhiteSpace())
			{
				ViewData["Message"] = message;
				message = "";
				return View();
			}
			return View();
		}

		[HttpPost]
		public ActionResult S(string tel, string message)
		{
			try
			{
				if (!tel.IsNullOrWhiteSpace())
				{
					//IMailingManager mail = new MailingManager();
					//mail.SendMailNewOrder();
					SendMailEx.SendMailExAsyncOrder(tel, message);
					return RedirectToAction("I", "H",new{message="Мы передали ваш запрос. Ожидайте звонка от нашего менеджера!"});
				}
				return RedirectToAction("I", "H", new { message = "Не правильно введен телефон" });
			}
			catch (Exception e)
			{
				return RedirectToAction("I", "H", new { message = e.Message });
			}
		}
	}
}