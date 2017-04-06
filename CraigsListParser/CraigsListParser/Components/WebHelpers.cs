using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;


namespace CraigsListParser.Components
{
    class WebHelpers : SingleTone<WebHelpers>
    {
        public static string GetHtml(string link,WebProxy currentProxy) //получаем страницу в виде строки, которую будем парсить, но с использованием прокси
        {

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(link);
            req.Method = "GET";
            req.Timeout = 2000;
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            req.UserAgent = "Mozilla/5.0 (Windows NT 6.3; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36";
            req.KeepAlive = true;
            req.Proxy = currentProxy;
            //req.Host = "www.craigslist.com.au";
            //if (cookies != null)
            //{
            //    req.CookieContainer = cookies; //самый важный пункт: сюда добавляем печеньку с идентификатором авторизации
            //}
            req.AllowAutoRedirect = true;
            try
            {
                Console.WriteLine("Получаю страницу...");
                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                Console.WriteLine(res.StatusCode + " , " + (int)res.StatusCode);
                System.IO.Stream ReceiveStream = res.GetResponseStream();
                System.IO.StreamReader sr2 = new System.IO.StreamReader(ReceiveStream, Encoding.UTF8);
                //Кодировка указывается в зависимости от кодировки ответа сервера
                Char[] read = new Char[256];
                int count = sr2.Read(read, 0, 256);
                string htmlString = String.Empty;
                while (count > 0)
                {
                    String str = new String(read, 0, count);
                    htmlString += str;
                    count = sr2.Read(read, 0, 256);
                }
                return htmlString;
            }
            catch
            {
                return Constants.WebAttrsNames.NotFound;
            }
        }

        internal static void ProxyPing(WebProxy currentProxy)
        {
            // Ping's the local machine.
            Ping pingSender = new Ping();
            
            PingReply reply = pingSender.Send(currentProxy.Address.Host);

            if (reply.Status == IPStatus.Success)
            {
                Console.WriteLine("Address: {0}", reply.Address.ToString());
                Console.WriteLine("RoundTrip time: {0}", reply.RoundtripTime);
                //Console.WriteLine("Time to live: {0}", reply.Options.Ttl);
                //Console.WriteLine("Don't fragment: {0}", reply.Options.DontFragment);
                //Console.WriteLine("Buffer size: {0}", reply.Buffer.Length);
            }
            else
            {
                Console.WriteLine(reply.Status);
            }
        }

        public static string GetHtml(string link) //получаем страницу в виде строки, которую будем парсить
        {

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(link);
            req.Method = "GET";
            req.Timeout = 2000;
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            req.UserAgent = "Mozilla/5.0 (Windows NT 6.3; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36";
            req.KeepAlive = true;
            //req.Host = "www.craigslist.com.au";
            //if (cookies != null)
            //{
            //    req.CookieContainer = cookies; //самый важный пункт: сюда добавляем печеньку с идентификатором авторизации
            //}
            req.AllowAutoRedirect = true;
            try
            {
                Console.WriteLine("Получаю страницу...");
                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                Console.WriteLine(res.StatusCode + " , " + (int)res.StatusCode);
                System.IO.Stream ReceiveStream = res.GetResponseStream();
                System.IO.StreamReader sr2 = new System.IO.StreamReader(ReceiveStream, Encoding.UTF8);
                //Кодировка указывается в зависимости от кодировки ответа сервера
                Char[] read = new Char[256];
                int count = sr2.Read(read, 0, 256);
                string htmlString = String.Empty;
                while (count > 0)
                {
                    String str = new String(read, 0, count);
                    htmlString += str;
                    count = sr2.Read(read, 0, 256);
                }
                return htmlString;
            }
            catch
            {
                return Constants.WebAttrsNames.NotFound;
            }
        }


    }
}
