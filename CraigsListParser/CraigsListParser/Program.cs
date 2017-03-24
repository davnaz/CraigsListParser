using AngleSharp.Parser.Html;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CraigsListParser.DataProviders;
using CraigsListParser.Components;

namespace CraigsListParser
{
    class Program
    {
        //private static string mainLink = "https://losangeles.craigslist.org/search/apa"; //start link for parsing
        //private static string craigslistBaselink = "https://losangeles.craigslist.org"; //craigslist
        //private static string dbConnectionString = "Data Source=NOTEBOOK;Initial Catalog=CraigsList;Integrated Security=True"; //string for database with Offers
        private static Uri craigslistUri = new Uri(Resources.BaseLink); //необходимое зло
        private static HtmlParser parser; //один для всех страниц экземпляр парсера
        SqlConnection dbConnection = DataProvider.Instance.Connection;  //переменная подключения к БД

        static void Main(string[] args)
        {
            Init(out parser);
            //File.WriteAllText("out.html", GetHtml("https://losangeles.craigslist.org/sfv/apa/6046751995.html"));
            //System.Diagnostics.Process.Start("chrome.exe", "out.html");
            var startPageDOM = parser.Parse(GetHtml(Resources.MainLink));
            AngleSharp.Dom.IElement searchResultNextPageLink = null;
            try
            {
                searchResultNextPageLink = startPageDOM.QuerySelector("a.button.next");
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            if(searchResultNextPageLink != null)
            {
                do
                {
                    Console.WriteLine("Начинаем парсить страницу выдачи: {0}{1}", Resources.MainLink, searchResultNextPageLink.GetAttribute(Constants.WebAttrsNames.href));


                    //ParseOffersListPage(conn, client, parser, offersListPageHtmlDocument); //парсим предложения этой страницы


                    Console.WriteLine("Спарсили текущую страницу выдачи!");

                    var offersListPageHtmlDocument = parser.Parse(GetHtml(Resources.MainLink + searchResultNextPageLink.GetAttribute(Constants.WebAttrsNames.href)));  //получаем DOM-документ одной страницы выдачи предложений

                    searchResultNextPageLink = offersListPageHtmlDocument.QuerySelector("a.button.next");
                    //Console.WriteLine("Получено:" + Resources.MainLink + searchResultNextPageLink.GetAttribute(Constants.WebAttrsNames.href));
                    Console.WriteLine("-------------------------------------");

                } while (searchResultNextPageLink != null);
            }
            else
            {
                Console.WriteLine("Nothing to parse");
            }

            Console.ReadKey();
        }

        private static string GetHtml(string link) //получаем страницу в виде строки, которую будем парсить
        {
            
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(link);
            req.Method = "GET";
            req.Timeout = 100000;
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            req.UserAgent = "Mozilla/5.0 (Windows NT 6.3; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36";
            req.KeepAlive = true;
            //req.Host = "www.craigslist.com.au";
            //if (cookies != null)
            //{
            //    req.CookieContainer = cookies; //самый важный пункт: сюда добавляем печеньку с идентификатором авторизации
            //}
            req.AllowAutoRedirect = true;
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();
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

        private static void Init(out HtmlParser parser)
        {
            parser = new HtmlParser(); //создание экземпляра парсера, он можнт быть использован несколько раз, т.е. для всей программы
            //dbConnection = new SqlConnection(Resources.DbConnectionString); //bинициализация подключения к БД
            //cookies = new CookieContainer();
        }

        //private static bool OpenSqlConnection(SqlConnection conn) //Функцйия открытия соединения к БД
        //{
        //    try
        //    {
        //        if (conn.State == System.Data.ConnectionState.Open) conn.Close();
        //        conn.Open(); // Открыть
        //        return true;
        //    }
        //    catch (Exception ex) // Исключение
        //    {
        //        Console.WriteLine(ex.Message);
        //        return false;
        //    }
        //}
    }
}
