using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using Microsoft.Ajax.Utilities;
using Microsoft.ApplicationInsights;
using System.Web.Hosting;
using System.IO;
using BuyingMetal.Models;
using BuyingMetal.Modules;
using BuyingMetal.Bot;

namespace BuyingMetal.Controllers
{
	public class HController : Controller
	{
		private PriceModel GetModel()
		{
			var model = new PriceModel();
			var path = HostingEnvironment.ApplicationPhysicalPath + "model.json";
			if (!System.IO.File.Exists(path))
			{
				throw new Exception("Невозможно зачитать файл с прайс-листом");
			}

			var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
			using (var sr = System.IO.File.OpenText(path))
			{
				model = serializer.Deserialize<PriceModel>(sr.ReadToEnd());
			}
			return model;
		}

		private bool SetModel(PriceModel model)
		{

			var path = HostingEnvironment.ApplicationPhysicalPath + "model.json";
			if (!System.IO.File.Exists(path))
			{
				SendMailEx.SendMailExAsyncMessage("Нет файла model.json", "");
				return false;
			}

			var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
			var serializedResult = serializer.Serialize(model);

			System.IO.File.WriteAllText(path, serializedResult);
			return true;
		}

		public ActionResult I(int sort = 0, string message="")
		{
			try
			{
				var model = GetModel();
				switch (sort)
				{
					case 1:
						model.ListPriceItem = model.ListPriceItem.OrderBy(i => i.Id).ToList();
						break;
					case 2:
						model.ListPriceItem = model.ListPriceItem.OrderBy(i => i.Name).ToList();
						break;
					default:
						model.ListPriceItem = model.ListPriceItem.OrderBy(i => i.Sort).ToList();
						break;
				}
				if (!message.IsNullOrWhiteSpace())
				{
					ViewData["Message"] = message;
					message = "";
					return View(model);
				}
				return View(model);
			}
			catch (Exception ex)
			{
				SendMailEx.SendMailExAsyncOrder("ОШИБКА", ex.Message);
				return View();
			}
		}

		[HttpPost]
		public ActionResult S(string tel, string message)
		{
			try
			{
				if (!tel.IsNullOrWhiteSpace())
				{

					new BotMain(tel+@"
"+message);
					//IMailingManager mail = new MailingManager();
					//mail.SendMailNewOrder();
					SendMailEx.SendMailExAsyncOrder(tel, message);
					return RedirectToAction("I", "H", new { message = "Мы передали ваш запрос. Ожидайте звонка от нашего менеджера!" });
				}
				return RedirectToAction("I", "H", new { message = "Не правильно введен телефон" });
			}
			catch (Exception e)
			{
				return RedirectToAction("I", "H", new { message = e.Message });
			}
		}

		//public string WM(string guid)
		//{
		//	try
		//	{
		//		var sguid = System.Web.Configuration.WebConfigurationManager.AppSettings["sguid"];
		//		if (guid == null || guid != sguid)
		//		{
		//			SendMailEx.SendMailExAsyncMessage("Изменение model.json: Не првильный GUID", "");
		//			return false.ToString();
		//		}
		//		var model = new PriceModel();
		//		var noPrice = "договорная";

		//		model.Block1.Id = -1111;
		//		model.Block1.Name = "ВОЛЬФРАМ";
		//		model.Block1.Price = "600";
		//		model.Block1.Description = "Самый тугоплавкий из металлов. Более высокую температуру плавления имеет только неметаллический элемент — углерод. При стандартных условиях химически стоек. При нормальных условиях представляет собой твёрдый блестящий серебристо-серый переходный металл.";

		//		model.Block2.Id = -2222;
		//		model.Block2.Name = "ПОБЕДИТ";
		//		model.Block2.Price = "410";
		//		model.Block2.Description = "Твёрдый сплав карбида вольфрама и кобальта в массовом соотношении 96:4. Обладая высокой твёрдостью, применяется при бурении горных пород, металлообработке, деревообработке и в качестве ответственных деталей, для которых требуется высокая твёрдость или жаропрочность.";

		//		model.Block3.Id = -3333;
		//		model.Block3.Name = "МОЛИБДЕН";
		//		model.Block3.Price = "400";
		//		model.Block3.Description = "Переходный металл светло-серого цвета. Молибден используется для легирования сталей как компонент жаропрочных и коррозионностойких сплавов. Молибденовая проволока служит для изготовления высокотемпературных печей, вводов электрического тока в лампочках.";

		//		var i = 1;
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = "от 400 до 750 грн.", Name = "Вольфрам ВА (лом, лист, пруток, электрод, проволока)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = "от 200 до 700 грн.", Name = "Вольфрам борид (WB2)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = "от 500 до 750 грн.", Name = "Вольфрам карбид (WC) (порошок, лом, изделия)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = "от 200 до 700 грн.", Name = "Вольфрам концентрат (шлам, порошок)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = "от 300 до 600 грн.", Name = "Вольфрам Оксид, паровольфрамат (WO2) (порошок)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = "от 500 до 700 грн.", Name = "Вольфрам (W) (порошок)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = "от 200 до 700 грн.", Name = "Вольфрам силицид (WSi2) (порошок)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = "от 600 до 700 грн.", Name = "Вольфрам ШВВ (штабик)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = "от 400 до 700 грн.", Name = "Вольфрам ВА, ВЛ, СВИ (лом, порошок, пруток, электрод)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = "от 400 до 700 грн.", Name = "Рэлит литой - карбид вольфрама З, ЛЗ (зерно, лента)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = "от 300 до 500 грн.", Name = "Сердечники вольфрамовые ВН-8, ВНЖ-90, ВК (пруток, стержень)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = "от 410 до 500 грн.", Name = "Победит, Твердый сплав ВК, ТК, ВН, ВНЖ, ВНК, ВНМ (лом, порошок, изделия, фильера)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = "от 450 до 800 грн.", Name = "Победит ВК-6,ВК-8, Т5К10, Т15К6, Т14К8 (порошки, лом, пластина)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = "от 500 до 800 грн.", Name = "Твердый сплав ВК-6, ВК-8, ВК-10, ВК-15, ВК-20 (порошок)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = "от 500 до 800 грн.", Name = "Пластина твердосплавная ВК, ТК, ВН, ВНЖ, ВНК (Ассортимент)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = "от 500 до 800 грн.", Name = "Смеси твердосплавные ВК, ТК (порошки)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = "от 350 до 700 грн.", Name = "Молибден МЧ (порошок, стружка, лом, пруток, проволока)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = "от 300 до 700 грн.", Name = "Карбид молибдена (порошок)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = "от 400 грн.", Name = "Оксид Молибдена (MoO2) (порошок)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = "от 250 до 600 грн.", Name = "Никель Н-1, Н-2, Н-3 (катод, анод, гранула, лист, проволока, лента, лом)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = "от 500 грн.", Name = "Никель ПНК-1л5, ПНК (порошок)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = "от 500 грн.", Name = "Никель ПНЭ-1 (порошок)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = "от 600 грн.", Name = "Олово О-1, ОПЧ (пруток, порошок, гранула)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = "от 200 до 400 грн.", Name = "Порошки наплавочные - Торез, Полема", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = "от 70 грн.", Name = "Бабит Б-16 (чушка)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = "от 250 до 350 грн.", Name = "Бабит Б-83 (чушка)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = noPrice, Name = "Бор аморфный", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = noPrice, Name = "Бор кристалический", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = noPrice, Name = "Борид титана", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = noPrice, Name = "Бориды разные (порошки)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = "от 350 до 410 грн.", Name = "Валки твердосплавные ВНК-20 (лом, стружка, изделия)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = "от 250 до 300 грн.", Name = "Висмут ВИ-00, ВИ-0, ВИ-1 (чушка, пруток, гранула)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = noPrice, Name = "Гафний карбид (порошок)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = noPrice, Name = "Гафний борид (порошок)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = noPrice, Name = "Гафний силицид (порошок)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = noPrice, Name = "Гафний оксид (порошок)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = noPrice, Name = "Гафний ойдидный (пруток)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = noPrice, Name = "Графит (лом)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = noPrice, Name = "Графит МГ, ЭГО (изделия, заготовки, лом)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = noPrice, Name = "Графит МПГ (блоки любых размеров)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = "от 300 до 500 грн.", Name = "Дисульфид молибдена (MoS2) ДМИ-1 (порошок)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = "от 300 до 500 грн.", Name = "Дисульфид молибдена (MoS2) ДМИ-7 (порошок)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = "от 600 грн.", Name = "Инструмент твердосплавный ВК, ТК (пластина, фильера, высадка)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = "от 300 грн.", Name = "Карбид бора (порошок)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = "от 300 грн.", Name = "Карбид бора (шлифовальный порошок)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = "от 1000 до 1200 грн.", Name = "Кобальт К-1, К-0 (чушка, подушечка, гранула, анод, лист)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = "от 1200 грн.", Name = "Кобальт ПК-1у, ПК (порошок)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = noPrice, Name = "Борид ниобия (порошок)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = noPrice, Name = "Ниобий (порошок, лист, пруток, проволока, поковка)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = noPrice, Name = "Ниобий ВН-2АЭ (проволока, пруток, лист, поковка)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = noPrice, Name = "Карбид ниобия (порошок)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = noPrice, Name = "Ниобий НБ-1, НБ-2 (проволока, пруток, лист)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = noPrice, Name = "Ниобий (порошок)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = noPrice, Name = "Нитрид бора вюрцит подобный (порошок)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = noPrice, Name = "Нитрид бора гексагональный (порошок)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = "от 150 до 450 грн.", Name = "Нихром Х20Н80 (проволока, лист, лента, полоса, порошок)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = "от 100 до 350 грн.", Name = "Нихром Х15Н60 (проволока, лист, лента, полоса, порошок)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = "от 130 до 400 грн.", Name = "Припой ПОС (пруток, чушка, проволока)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = noPrice, Name = "Сталь быстрорежущяя Р6М5, Р18 (лом, прокат, порошок)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = noPrice, Name = "Борид тантала (порошок)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = noPrice, Name = "Карбид тантала (порошок)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = noPrice, Name = "Оксид тантала (порошок)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = noPrice, Name = "Тантал (прокат, пруток, проволока, лист, фольга, лента)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = noPrice, Name = "Тантал силицид (порошок)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = noPrice, Name = "Титан ВТ-1.0 (пруток, лист, полоса, проволока)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = noPrice, Name = "Титан (порошок)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = noPrice, Name = "Титан ВТ (пруток, лист, полоса, проволока)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = noPrice, Name = "Титан борид (порошок)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = "от 910 грн.", Name = "Титан йодидный (пруток, обрезки)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = "от 500 грн.", Name = "Карбид титана (порошок)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = noPrice, Name = "Нитрид титана (порошок)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = noPrice, Name = "Дисилицид титана (порошок)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = "от 260 до 400 грн.", Name = "Ферромолибден 50-60% (порошок, кусок)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = noPrice, Name = "Феррохром (порошок, кусок)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = noPrice, Name = "Борид хрома (порошок)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = noPrice, Name = "Карбид хрома (порошок)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = noPrice, Name = "Нитрид хрома (порошок)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = noPrice, Name = "Хром ПХ1м (порошок)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = noPrice, Name = "Хром ПХ1с (порошок)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = noPrice, Name = "Хром 999 (пруток)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = noPrice, Name = "Хром Х99 (кусок, порошок)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = "от 500 грн.", Name = "Хром ЭРХ, РХ (чешуйчатый)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = "от 1000 грн.", Name = "Цирконий 999 (чушка, мишень)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = "от 1000 грн.", Name = "Цирконий иодидный (пруток, обрезки)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = noPrice, Name = "Карбид циркония (порошок)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = noPrice, Name = "Цирконий силицид", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = noPrice, Name = "Оксид циркония (порошок)", Description = "" });
		//		model.ListPriceItem.Add(new PriceItemModel() { Id = i++, Price = noPrice, Name = "Цирконий ПЦРК1, ПЦРК2, ПЦРК3 (порошок)", Description = "" });

		//		SetModel(model);
		//	}
		//	catch(Exception err)
		//	{
		//		SendMailEx.SendMailExAsyncMessage("Exception", err.Message);
		//		return false.ToString();
		//	}
		//	return true.ToString();
		//}



		public ActionResult IAdmin(string guid, string message = "")
		{
			try
			{
				ViewData["guid"] = guid;
				var sguid = System.Web.Configuration.WebConfigurationManager.AppSettings["sguid"];
				if (guid == null || guid != sguid)
				{
					SendMailEx.SendMailExAsyncMessage("Изменение model.json: Не првильный GUID", "");
					return RedirectToAction("I", "H");
				}

				var model = GetModel();
				if (!message.IsNullOrWhiteSpace())
				{
					ViewData["Message"] = message;
					message = "";
					return View(model);
				}
				return View(model);
			}
			catch (Exception ex)
			{
				SendMailEx.SendMailExAsyncOrder("ОШИБКА", ex.Message);
			}
			return RedirectToAction("I", "H");
		}

		[HttpPost]
		public ActionResult EL(string guid, int id, string name, string price, string description, int sort=9999)
		{
			bool msg=false;
			ViewData["guid"] = guid;
			try
			{
				var sguid = System.Web.Configuration.WebConfigurationManager.AppSettings["sguid"];
				if (guid==null||guid!= sguid)
				{
					SendMailEx.SendMailExAsyncMessage("Изменение model.json: Не првильный GUID", "");
					return RedirectToAction("I", "H");
				}
				var model = GetModel();
				if (model == null)
				{
					SendMailEx.SendMailExAsyncMessage("Изменение model.json: Пустая модель", "");
					return RedirectToAction("I", "H");
				}

				if (model.ListPriceItem == null)
				{
					model.ListPriceItem = new List<PriceItemModel>();
				}

				switch (id)
				{
					case -1111:
						model.Block1.Name = name;
						model.Block1.Price = price;
						model.Block1.Description = description;
						break;
					case -2222:
						model.Block2.Name = name;
						model.Block2.Price = price;
						model.Block2.Description = description;
						break;
					case -3333:
						model.Block3.Name = name;
						model.Block3.Price = price;
						model.Block3.Description = description;
						break;
					case -9999://добавление элемента
						var lastId = 1;
						var last = model.ListPriceItem.Last();
						if (last != null)
						{
							lastId = last.Id;
						}	

						model.ListPriceItem.Add(new PriceItemModel { Description = description, Id = ++lastId, Name=name,Price=price,Sort=sort });
						break;
					default:
						//int i = 0;
						//model.ListPriceItem.ForEach(it =>
						//{
						//	it.Id = ++i;
						//	it.Sort = it.Id;
						//});
						var item = model.ListPriceItem.First(it => it.Id == id);
						if (item != null)
						{
							item.Name = name;
							item.Price = price;
							item.Description = description;
							item.Sort = sort;
						}
						break;
				}
				msg = SetModel(model);
			}
			catch (Exception e)
			{
				msg = false;
				SendMailEx.SendMailExAsyncMessage("Exception", e.Message);
			}

			return RedirectToAction("IAdmin", "H", new{ guid = guid, message=msg.ToString()} );
		}

		public string Q() {
			return AesManagedManager.Encrypt("мама мыла раму");
		}
	}
}