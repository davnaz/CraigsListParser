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
using AngleSharp.Dom.Html;
using AngleSharp.Dom;

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
            StartParsing();
        }

        private static void StartParsing()
        {
            var searchPageDOM = parser.Parse(GetHtml(Resources.MainLink)); //получаем стартовую страницу выдачи нужных предложений.
            AngleSharp.Dom.IElement searchResultNextPageLink = null;
            try
            {
                searchResultNextPageLink = searchPageDOM.QuerySelector("a.button.next");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            if (searchResultNextPageLink != null)
            {
                do
                {
                    Console.WriteLine("Начинаем парсить страницу выдачи: {0}{1}", Resources.BaseLink, searchResultNextPageLink.GetAttribute(Constants.WebAttrsNames.href));


                    ParseOffersListPage(searchPageDOM); //парсим предложения этой страницы


                    Console.WriteLine("Спарсили текущую страницу выдачи!");

                    var offersListPageHtmlDocument = parser.Parse(GetHtml(Resources.BaseLink + searchResultNextPageLink.GetAttribute(Constants.WebAttrsNames.href)));  //получаем DOM-документ одной страницы выдачи предложений

                    searchResultNextPageLink = offersListPageHtmlDocument.QuerySelector("a.button.next");
                    Console.WriteLine(searchResultNextPageLink != null ? "Получено:" + Resources.BaseLink + searchResultNextPageLink.GetAttribute(Constants.WebAttrsNames.href) : "На этой странице нет результатов и ссылок на следующую страницу!");
                    searchPageDOM = parser.Parse(GetHtml(Resources.BaseLink + searchResultNextPageLink.GetAttribute(Constants.WebAttrsNames.href)));
                    Console.WriteLine("------------------------------------------");

                } while (searchResultNextPageLink != null);
            }
            else
            {
                Console.WriteLine("Nothing to parse");
            }

            Console.ReadKey();
        }

        private static void ParseOffersListPage(IHtmlDocument searchPageDOM)
        {
            var links = searchPageDOM.QuerySelectorAll(".result-title.hdrlnk");
            for(int i = 0;i< links.Length;i++)
            {
                Offer o = ParseOffer(parser.Parse(GetHtml(Resources.BaseLink + links[i].GetAttribute(Constants.WebAttrsNames.href)))); //теперь парсим каждую отдельную ссылку(предложение)
                InsertIntoDB(o);//запихиваем предложение в БД                
            }
        }

        private static void InsertIntoDB(Offer o)
        {
            
        }

        private static Offer ParseOffer(IHtmlDocument htmlDocument) //парсит документ DOM конкретного предложения жилья
        {
            Offer o = new Offer();           
            
            o.ID = GetID(htmlDocument);
            o.Name = GetOfferName(htmlDocument);
            o.Description = GetDescription(htmlDocument);
            o.Price = getPrice(htmlDocument);
            setProperties(htmlDocument,o); //set bathrooms and bedrooms, Abailible, Square, Additional
            o.PlaceMapsLink = getPlaceMapLink(htmlDocument);
            o.PlaceName = getPlace(htmlDocument);
            o.Posted = getPosted(htmlDocument);
            o.Updated = getUpdated(htmlDocument);
            o.Images = getImages(htmlDocument);

            Console.WriteLine("Предложение {0} спарсили!", htmlDocument.QuerySelector("link").GetAttribute("href"));
            return null;
        }

        private static string getImages(IHtmlDocument htmlDocument)
        {
            var scripts = htmlDocument.QuerySelectorAll("script");
            foreach (var script in scripts)
            {
                if(script.InnerHtml.Contains("imgList"))
                {
                    return script.InnerHtml.Replace("<!--\nvar imgList = ", "").Replace("\n-->","");
                }
            }
            return "";
        }

        private static long GetID(IHtmlDocument htmlDocument)
        {
            var a = htmlDocument.QuerySelector("link[rel=canonical]");

            if (a != null)
            {
                string id = a.GetAttribute(Constants.WebAttrsNames.href).Split('/')[a.GetAttribute(Constants.WebAttrsNames.href).Split('/').Length - 1].Replace(".html", "");
                return Convert.ToInt64(id);
            }

            return -1; //ID на странице нет
        }

        private static string GetOfferName(IHtmlDocument htmlDocument)
        {            
            return htmlDocument.QuerySelector("#titletextonly") != null ? htmlDocument.QuerySelector("#titletextonly").TextContent : "There is no OfferName"; //если на странице есть имя, возвращаем, иначе возвращаем "There is no OfferName"
        }

        private static string GetDescription(IHtmlDocument htmlDocument)
        {
            return htmlDocument.QuerySelector("#postingbody") != null ? htmlDocument.QuerySelector("#postingbody").TextContent : "There is no Description"; //если на странице есть описание, возвращаем, иначе возвращаем "There is no Description"
        }

        private static double getPrice(IHtmlDocument htmlDocument)
        {
            return htmlDocument.QuerySelector("span.price") != null ? Double.Parse(htmlDocument.QuerySelector("span.price").TextContent.Trim('$')) : 0; //если на странице есть цена, возвращаем, иначе возвращаем 0
        }

        private static void setProperties(IHtmlDocument htmlDocument, Offer o)
        {
            var infocollection = htmlDocument.QuerySelectorAll("p.attrgroup");
            if (infocollection != null)
            {
                var firstLineProperties = infocollection[0];

                //Console.WriteLine(firstLineProperties.QuerySelectorAll("span")[0].TextContent);

                foreach (var span in firstLineProperties.QuerySelectorAll("span"))
                {
                    if (span.TextContent.Contains("/")) //если есть строка со слешем, значит атм прописаны параметры спален и ванн, парсим
                    {
                        o.BedRooms = span.TextContent.Split('/')[0].Trim();  //задаем количество спален
                        o.BathRooms = span.TextContent.Split('/')[1].Trim(); //задаем количество спален
                    }
                    if (span.TextContent.Contains("ft2"))
                    {
                        o.Square = Double.Parse(span.TextContent.Replace("ft2", ""));
                    }
                    if (span.TextContent.Contains("available"))
                    {
                        o.Availability = span.TextContent.Replace("available", "").Trim();
                    }

                }
                if (infocollection.Length > 1)
                {
                    o.Additional = infocollection[infocollection.Length - 1].TextContent.Replace("           ", ""); //если на странице в p.attrgroup есть доп.инфа, заполняем поле
                }

                //Console.WriteLine();
                //Console.WriteLine(infocollection[infocollection.Length - 1].TextContent);
            }
        }

        private static string getPlaceMapLink(IHtmlDocument htmlDocument)
        {
            return htmlDocument.QuerySelector(Constants.OfferSelectorNames.PlaceMapsLink) != null ? htmlDocument.QuerySelector(Constants.OfferSelectorNames.PlaceMapsLink).GetAttribute(Constants.WebAttrsNames.href): "No link";
        }

        private static string getPlace(IHtmlDocument htmlDocument)
        {
            return htmlDocument.QuerySelector(Constants.OfferSelectorNames.PlaceName) != null ? htmlDocument.QuerySelector(Constants.OfferSelectorNames.PlaceName).TextContent : "No Place Name";
        }

        private static DateTime getPosted(IHtmlDocument htmlDocument) //получает дату размещения публикации
        {
            var a = htmlDocument.QuerySelector(Constants.OfferSelectorNames.Posted);
            
            return a != null ? Convert.ToDateTime(a.TextContent) : DateTime.MinValue;
        }

        private static DateTime getUpdated(IHtmlDocument htmlDocument) //получает дату размещения публикации, если дата обновления есть, тогда парсим, иначе берем из Posted
        {
            var postingInfos = htmlDocument.QuerySelectorAll(Constants.OfferSelectorNames.Updated);
            DateTime Updated = DateTime.MinValue;
            if (postingInfos != null)
            {
                foreach (var info in postingInfos)
                {
                    if(info.TextContent.Contains("updated:"))
                    {
                        Updated =  Convert.ToDateTime(info.QuerySelector("time").TextContent);
                    }
                }
            }
            if (Updated == DateTime.MinValue)
            {
                Updated = getPosted(htmlDocument);
            }
            return Updated;
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
