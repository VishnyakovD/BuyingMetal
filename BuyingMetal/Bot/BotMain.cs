using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telegram.Bot;

namespace BuyingMetal.Bot
{
	public class BotMain
	{
		protected TelegramBotClient Bot;
		string TelegramImagePath;
		string BotKey;


		public BotMain()
		{

		}

		public BotMain(string msg)
		{
			Bot = new TelegramBotClient("564776855:AAFVIkTln4cHl0InoiYVyQkd6uVxwoJ9rF4");


			Bot.SendTextMessageAsync(-1001343181152, msg);
		


		}
	}
}