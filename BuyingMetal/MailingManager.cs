using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;

namespace BuyingMetal
{
	public interface IMailingManager
	{
		void SendMailNewOrder(string tel);
		//string[] mails, string subject, string body
	}

	public class MailingManager : IMailingManager
	{
		public MailingManager()
		{

		}

		public void SendMailNewOrder(string tel)
		{
			try
			{
				var sMTPHost = WebConfigurationManager.AppSettings["SMTPHost"];
				var sMTPPort = 25;
				int.TryParse(WebConfigurationManager.AppSettings["SMTPPort"], out sMTPPort);
				var enableSsl = false;
				bool.TryParse(WebConfigurationManager.AppSettings["EnableSsl"], out enableSsl);
				var useDefaultCredentials = true;
				bool.TryParse(WebConfigurationManager.AppSettings["UseDefaultCredentials"], out useDefaultCredentials);
				var userName = WebConfigurationManager.AppSettings["UserName"];
				var userPassword = WebConfigurationManager.AppSettings["UserPassword"];
				var mailFrom = WebConfigurationManager.AppSettings["MailFrom"];
				var mailTo = WebConfigurationManager.AppSettings["MailTo"];
				//var MailTo2 = WebConfigurationManager.AppSettings["MailTo2"];
				var mailSubject = WebConfigurationManager.AppSettings["MailSubject"];
				var mailBody = WebConfigurationManager.AppSettings["MailBody"];
				var smtp = new SmtpClient(sMTPHost, sMTPPort);

				smtp.EnableSsl = enableSsl;
				smtp.UseDefaultCredentials = useDefaultCredentials;
				smtp.Credentials = new NetworkCredential(userName, userPassword);
				var message = new MailMessage();
				message.From = new MailAddress(mailFrom);
				// var mails = new MailAddressCollection();
				message.To.Add(string.Format("{0}", mailTo));
				message.Subject = string.Format(mailSubject, tel) ;
				message.Body = mailBody;
				smtp.SendMailAsync(message);
			}
			catch (Exception err)
			{
				throw;
			}
		}
	}

	public static class SendMailEx
	{
		public static Task SendMailExAsyncOrder(string tel, string msg)
		{

			var sMTPHost = WebConfigurationManager.AppSettings["SMTPHost"];
			var sMTPPort = 25;
			int.TryParse(WebConfigurationManager.AppSettings["SMTPPort"], out sMTPPort);
			var enableSsl = false;
			bool.TryParse(WebConfigurationManager.AppSettings["EnableSsl"], out enableSsl);
			var useDefaultCredentials = true;
			bool.TryParse(WebConfigurationManager.AppSettings["UseDefaultCredentials"], out useDefaultCredentials);
			var userName = WebConfigurationManager.AppSettings["UserName"];
			var userPassword = WebConfigurationManager.AppSettings["UserPassword"];

			var mailFrom = WebConfigurationManager.AppSettings["MailFrom"];
			var mailTo = WebConfigurationManager.AppSettings["MailTo"];
			//var MailTo2 = WebConfigurationManager.AppSettings["MailTo2"];
			var mailSubject = WebConfigurationManager.AppSettings["MailSubject"];
			//var mailBody = WebConfigurationManager.AppSettings["MailBody"];
			var mailBody = "Клиент " + tel +". "+msg;
			var smtp = new SmtpClient(sMTPHost, sMTPPort);

			smtp.EnableSsl = enableSsl;
			smtp.UseDefaultCredentials = useDefaultCredentials;
			smtp.Credentials = new NetworkCredential(userName, userPassword);

			var message = new MailMessage(mailFrom, mailTo);
			message.BodyEncoding = Encoding.UTF8;
			message.Body = mailBody;
			message.Subject = string.Format(mailSubject, tel);
			message.SubjectEncoding = Encoding.UTF8;


			return Task.Run(() => SendMailExImpl(smtp, message));
		}

		public static Task SendMailExAsyncMessage(string subject, string msg)
		{

			var sMTPHost = WebConfigurationManager.AppSettings["SMTPHost"];
			var sMTPPort = 25;
			int.TryParse(WebConfigurationManager.AppSettings["SMTPPort"], out sMTPPort);
			var enableSsl = false;
			bool.TryParse(WebConfigurationManager.AppSettings["EnableSsl"], out enableSsl);
			var useDefaultCredentials = true;
			bool.TryParse(WebConfigurationManager.AppSettings["UseDefaultCredentials"], out useDefaultCredentials);
			var userName = WebConfigurationManager.AppSettings["UserName"];
			var userPassword = WebConfigurationManager.AppSettings["UserPassword"];

			var mailFrom = WebConfigurationManager.AppSettings["MailFrom"];
			var mailTo = WebConfigurationManager.AppSettings["MailTo"];
			var mailSubject = subject;
			var mailBody = subject + ". " + msg;
			var smtp = new SmtpClient(sMTPHost, sMTPPort);

			smtp.EnableSsl = enableSsl;
			smtp.UseDefaultCredentials = useDefaultCredentials;
			smtp.Credentials = new NetworkCredential(userName, userPassword);

			var message = new MailMessage(mailFrom, mailTo);
			message.BodyEncoding = Encoding.UTF8;
			message.Body = mailBody;
			message.Subject = mailSubject;
			message.SubjectEncoding = Encoding.UTF8;


			return Task.Run(() => SendMailExImpl(smtp, message));
		}

		private static void SendMailExImpl(
			System.Net.Mail.SmtpClient client,
			System.Net.Mail.MailMessage message)
		{
			//token.ThrowIfCancellationRequested();

			var tcs = new TaskCompletionSource<bool>();
			System.Net.Mail.SendCompletedEventHandler handler = null;
			Action unsubscribe = () => client.SendCompleted -= handler;

			handler = async (s, e) =>
			{
				unsubscribe();

				// a hack to complete the handler asynchronously
				await Task.Yield();

				if (e.UserState != tcs)
					tcs.TrySetException(new InvalidOperationException("Unexpected UserState"));
				else if (e.Cancelled)
					tcs.TrySetCanceled();
				else if (e.Error != null)
					tcs.TrySetException(e.Error);
				else
					tcs.TrySetResult(true);
			};

			client.SendCompleted += handler;
			try
			{
				client.SendAsync(message, tcs);
				//using (token.Register(() => client.SendAsyncCancel(), useSynchronizationContext: false))
				//{
				//	await tcs.Task;
				//}
			}
			finally
			{
				unsubscribe();
			}
		}
	}
}