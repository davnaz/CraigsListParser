﻿
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using CraigsListParser.DataProviders;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CraigsListParser.Components
{
    class Parser
    {
        private static WebProxy currentProxy;
        private static HtmlParser parser;

        public Parser()
        {
            currentProxy = ProxySolver.Instance.getNewProxy();
            parser = new HtmlParser(); //создание экземпляра парсера, он можнт быть использован несколько раз для одного потока(экземпляра класса Parser)
        }

        private WebProxy UpdateInternalProxy()
        {
            return currentProxy = ProxySolver.Instance.getNewProxy();
        }

        public void GetRegionsList(List<string> citiesList)
        {
            UpdateInternalProxy();
            Console.WriteLine("Текущий прокси: " + currentProxy.Address);
            var citiesLinksDomElement = parser.Parse(WebHelpers.GetHtmlThrowProxy(Resources.StartLink, currentProxy)).QuerySelectorAll(".height6.geo-site-list li > a");
            foreach (var link in citiesLinksDomElement)
            {
                citiesList.Add(link.GetAttribute(Constants.WebAttrsNames.href));
            }
            string beginlink = "";
            try
            {

                beginlink = File.ReadAllText("last.txt");

            }
            catch (Exception e)
            {
                Console.WriteLine("Файла с последней спарсенной ссылкой нет, начинаем с начала списка.  " + e.Message);
            }
            for (int i = 0; i < citiesList.Count; i++)
            {
                if (citiesList[i] == beginlink)
                {
                    citiesList.RemoveRange(0, i);
                    break;
                }
            }
        }

        /// <summary>
        /// Парсит регион сайта CraigsList  
        /// </summary>
        /// <param name="regionLink">Полный адрес сайта региона для выборки</param>
        public void StartParsing(string regionLink, int minPrice, int maxPrice)
        {
            string searchPageInline = Constants.WebAttrsNames.NotFound;
            for (int i = 0;i<3;i++)
            {
                searchPageInline = WebHelpers.GetHtmlThrowProxy(CompileLink(regionLink, Resources.HouseSearchLinkPostfix, minPrice, maxPrice), currentProxy); //парсим обычным образом
                if (searchPageInline == Constants.WebAttrsNames.NotFound)                                                      //, а если не получится, пробуем по другому постфиксу(для некоторых регионов
                {
                    searchPageInline = WebHelpers.GetHtmlThrowProxy(CompileLink(regionLink, Resources.AltHouseSearchLinkPostfix, minPrice, maxPrice), currentProxy);
                    if(searchPageInline != Constants.WebAttrsNames.NotFound)
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
                UpdateInternalProxy();
            }
            
            var searchPageDOM = parser.Parse(searchPageInline); //получаем стартовую страницу выдачи нужных предложений.
                                                                //проверяем, равно ли число общей выдачи для нашей ссылки 2500(максимальный)
            var totalcountElement = searchPageDOM.QuerySelector(".totalcount");
            if (totalcountElement != null) //если не отсутствует(это если результатов нет вообще), тогда проверим на 2500
            {
                if (totalcountElement.TextContent == "2500")
                {
                    if(maxPrice == Constants.PriceValues.undefinedValue || maxPrice == Constants.PriceValues.undefinedValue)
                    {
                        maxPrice = 1000000;
                        minPrice = 0;
                    }
                    if(maxPrice != minPrice)
                    {
                        int Average = (maxPrice + minPrice) / 2;
                        StartParsing(regionLink, minPrice, Average);
                        StartParsing(regionLink, Average + 1, maxPrice);
                        return;
                    }                    
                }
            }

            AngleSharp.Dom.IElement searchResultNextPageLink = null;
            try
            {
                searchResultNextPageLink = searchPageDOM.QuerySelector("a.button.next");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            //парсим результаты на текущей странице
            if (searchResultNextPageLink != null)  
            {
                string oldSearchResultNextPageLinkstring = "=";
                do
                {
                    Console.WriteLine("Начинаем парсить страницу выдачи: {0}{1}", regionLink, searchResultNextPageLink.GetAttribute(Constants.WebAttrsNames.href));
                    ParseOffersListPage(searchPageDOM, regionLink); //парсим предложения этой страницы
                    Console.WriteLine("Спарсили текущую страницу выдачи!");
                    var offersListPageHtmlDocument = parser.Parse(WebHelpers.GetHtmlThrowProxy(regionLink + searchResultNextPageLink.GetAttribute(Constants.WebAttrsNames.href), currentProxy));  //получаем DOM-документ одной страницы выдачи предложений

                    searchResultNextPageLink = offersListPageHtmlDocument.QuerySelector("a.button.next"); //берем элемент со ссылкой на следующую страницу

                    if (searchResultNextPageLink == null)
                    {
                        break;
                    }
                    else
                    {
                        if (WrongLink(searchResultNextPageLink, oldSearchResultNextPageLinkstring)) //проверяем, а не ушли ли мы в цикл
                        {
                            break; //если мы в цикле, тогда хватит парсить ту же страницу  до бесконечности
                        }
                        else
                        {

                            oldSearchResultNextPageLinkstring = searchResultNextPageLink.GetAttribute(Constants.WebAttrsNames.href); //новая ссылка для будущих итераций
                        }
                    }

                    Console.WriteLine(searchResultNextPageLink != null ? "Получено:" + regionLink + searchResultNextPageLink.GetAttribute(Constants.WebAttrsNames.href) : "На этой странице нет результатов и ссылок на следующую страницу!");
                    if (searchResultNextPageLink != null)
                    {
                        searchPageDOM = parser.Parse(WebHelpers.GetHtmlThrowProxy(regionLink + searchResultNextPageLink.GetAttribute(Constants.WebAttrsNames.href), currentProxy));
                    }
                    Console.WriteLine("------------------------------------------");

                } while (searchResultNextPageLink != null);
            }
            else
            {
                Console.WriteLine("Nothing to parse");
                UpdateInternalProxy();
                //File.AppendAllText("log.txt", String.Format("Регион не спарсился или там нет предложений жилья: {0}, {1}\n", regionLink, DateTime.Now)); //если не спарсилось, заносим в лог
                string UnsecureRegionLink = regionLink.Replace("https", "http");
                if (regionLink.Length != UnsecureRegionLink.Length)
                {
                    StartParsing(UnsecureRegionLink,minPrice,maxPrice);
                }
            }
            Console.WriteLine("Регион " + regionLink + "обработан!");
            //File.AppendAllText("log.txt", String.Format("Регион обработан: {0}, {1}\n", regionLink, DateTime.Now));
            //Console.ReadKey();
        }

        private string CompileLink(string regionLink, string houseSearchLinkPostfix, int minPrice, int maxPrice)
        {
            string minPricePostfix = minPrice == Constants.PriceValues.undefinedValue ? String.Empty : $"{Resources.CraigslistMinPricePostfixForGetrequest}{minPrice}";
            string maxPricePostfix = maxPrice == Constants.PriceValues.undefinedValue ? String.Empty : $"{Resources.CraigslistMaxPricePostfixForGetrequest}{maxPrice}";
            string result = regionLink + houseSearchLinkPostfix;
            if (minPricePostfix != String.Empty)
            {
                result += ("?" + minPricePostfix);
                if (maxPricePostfix != String.Empty)
                {
                    result += ("&" + maxPricePostfix);
                }
            }
            else
            {
                if (maxPricePostfix != String.Empty)
                {
                    result += ("?" + maxPricePostfix);
                }
            }
            return result;
        }

        private static bool WrongLink(IElement searchResultNextPageLink, string oldSearchResultNextPageLinkstring)
        {
            string newLink = searchResultNextPageLink.GetAttribute(Constants.WebAttrsNames.href);
            try
            {
                int newLinknum = Convert.ToInt32(newLink.Split('=')[newLink.Split('=').Length - 1]);
                int oldLinkNum = Convert.ToInt32(oldSearchResultNextPageLinkstring.Split('=')[oldSearchResultNextPageLinkstring.Split('=').Length - 1]);
            }
            catch
            {
                return false;
            }
            if (newLink == oldSearchResultNextPageLinkstring)
            {
                return true;
            }
            else
            {
                if (Convert.ToInt32(newLink.Split('=')[newLink.Split('=').Length - 1]) < Convert.ToInt32(oldSearchResultNextPageLinkstring.Split('=')[oldSearchResultNextPageLinkstring.Split('=').Length - 1]))
                {
                    return true;
                }
            }
            return false;
        }

        private void ParseOffersListPage(IHtmlDocument searchPageDOM, string regionLink)
        {
            UpdateInternalProxy();
            Console.WriteLine("Текущий прокси: " + currentProxy.Address);
            var links = searchPageDOM.QuerySelectorAll(".result-title.hdrlnk");
            List<IElement> linksList = new List<IElement>(links.ToList());
            for(int i = 0;i < linksList.Count;i++) // "craigslist.org" в ссылке - явный индикатор альтернативного предложения из другого региона
            {
                if(links[i].GetAttribute(Constants.WebAttrsNames.href).Contains("craigslist.org")) 
                {
                    Console.WriteLine(links[i].GetAttribute("href"));
                    linksList.RemoveRange(i,linksList.Count-1-i);
                    break;
                  
                }
            }

            SqlCommand insertOfferCommand = DataProvider.Instance.CreateSQLCommandForInsertSP();
            insertOfferCommand.Connection = DataProvider.Instance.Connection;
            insertOfferCommand.Connection.Open();
            for (int i = 0; i < linksList.Count; i++)
            {
                Offer o = ParseOffer(parser.Parse(WebHelpers.GetHtmlThrowProxy(regionLink + linksList[i].GetAttribute(Constants.WebAttrsNames.href), currentProxy))); //теперь парсим каждую отдельную ссылку(предложение)

                if (o != null)
                {
                    InsertIntoDB(o, insertOfferCommand);//запихиваем предложение в БД  
                }
                else
                {
                    //File.AppendAllText("log.txt", String.Format("Не спарсилось: {0},{1}\n", regionLink + links[i].GetAttribute(Constants.WebAttrsNames.href), DateTime.Now)); //если не спарсилось, заносим в лог
                }
            }
            insertOfferCommand.Connection.Close();
        }

        private static void InsertIntoDB(Offer o, SqlCommand insertOfferCommand)
        {

            //insertOfferCommand.Connection = DataProvider.Instance.Connection;
            insertOfferCommand.Parameters.Clear();
            insertOfferCommand.Parameters.AddWithValue(Constants.DbCellNames.City, o.City);
            insertOfferCommand.Parameters.AddWithValue(Constants.DbCellNames.PostID, o.PostID);
            insertOfferCommand.Parameters.AddWithValue(Constants.DbCellNames.Name, o.Name);
            insertOfferCommand.Parameters.AddWithValue(Constants.DbCellNames.Price, o.Price);
            insertOfferCommand.Parameters.AddWithValue(Constants.DbCellNames.PlaceName, o.PlaceName);
            insertOfferCommand.Parameters.AddWithValue(Constants.DbCellNames.Description, o.Description);
            insertOfferCommand.Parameters.AddWithValue(Constants.DbCellNames.Posted, o.Posted);
            insertOfferCommand.Parameters.AddWithValue(Constants.DbCellNames.Updated, o.Updated);
            insertOfferCommand.Parameters.AddWithValue(Constants.DbCellNames.PlaceMapsLink, o.PlaceMapsLink);
            insertOfferCommand.Parameters.AddWithValue(Constants.DbCellNames.Availability, o.Availability);
            insertOfferCommand.Parameters.AddWithValue(Constants.DbCellNames.BedRooms, o.BedRooms);
            insertOfferCommand.Parameters.AddWithValue(Constants.DbCellNames.BathRooms, o.BathRooms);
            insertOfferCommand.Parameters.AddWithValue(Constants.DbCellNames.Square, o.Square);
            insertOfferCommand.Parameters.AddWithValue(Constants.DbCellNames.Additional, o.Additional);
            insertOfferCommand.Parameters.AddWithValue(Constants.DbCellNames.Images, o.Images);
            DataProvider.Instance.ExecureSP(insertOfferCommand);
        }

        private Offer ParseOffer(IHtmlDocument htmlDocument) //парсит документ DOM конкретного предложения жилья
        {
            Offer o = new Offer();

            o.PostID = GetID(htmlDocument);
            o.Name = GetOfferName(htmlDocument);
            o.Description = GetDescription(htmlDocument);
            o.Price = getPrice(htmlDocument);
            setProperties(htmlDocument, o); //set bathrooms and bedrooms, Abailible, Square, Additional
            o.PlaceMapsLink = getPlaceMapLink(htmlDocument);
            o.PlaceName = getPlace(htmlDocument);
            o.Posted = getPosted(htmlDocument);
            o.Updated = getUpdated(htmlDocument);
            o.Images = getImages(htmlDocument);
            o.City = getCity(htmlDocument);
            if (htmlDocument.QuerySelector("link") == null)
            {
                Console.WriteLine("Ссылка битая!");
                UpdateInternalProxy();
                Console.WriteLine("Прокси обновлен:" + currentProxy.Address);
                return null;
            }
            else
            {
                Console.WriteLine("Предложение {0} спарсили!", htmlDocument.QuerySelector("link").GetAttribute("href"));
                return o;
            }
        }

        private string getCity(IHtmlDocument htmlDocument)
        {
            return htmlDocument.QuerySelector(Constants.OfferSelectorNames.City) != null ? htmlDocument.QuerySelector(Constants.OfferSelectorNames.City).TextContent : "No City"; //если на странице есть цена, возвращаем, иначе возвращаем 0
        }

        private string getImages(IHtmlDocument htmlDocument)
        {
            var scripts = htmlDocument.QuerySelectorAll("script");
            foreach (var script in scripts)
            {
                if (script.InnerHtml.Contains("imgList"))
                {
                    return script.InnerHtml.Replace("<!--\nvar imgList = ", "").Replace("\n-->", "");
                }
            }
            return "";
        }

        private long GetID(IHtmlDocument htmlDocument)
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
            string Description = "There is no Description";
            if (htmlDocument.QuerySelector("#postingbody") != null)
            {
                Description = htmlDocument.QuerySelector("#postingbody").TextContent.Replace("QR Code Link to This Post", "").TrimStart(new char[] { '\n', ' ' });
            }
            return Description;  //если на странице есть описание, возвращаем, иначе возвращаем "There is no Description"
        }

        private static double getPrice(IHtmlDocument htmlDocument)
        {
            return htmlDocument.QuerySelector("span.price") != null ? Double.Parse(htmlDocument.QuerySelector("span.price").TextContent.Trim('$')) : 0; //если на странице есть цена, возвращаем, иначе возвращаем 0
        }

        private static void setProperties(IHtmlDocument htmlDocument, Offer o)
        {
            var infocollection = htmlDocument.QuerySelectorAll("p.attrgroup");
            if (infocollection != null && infocollection.Length != 0)
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
            return htmlDocument.QuerySelector(Constants.OfferSelectorNames.PlaceMapsLink) != null ? htmlDocument.QuerySelector(Constants.OfferSelectorNames.PlaceMapsLink).GetAttribute(Constants.WebAttrsNames.href) : "No link";
        }

        private static string getPlace(IHtmlDocument htmlDocument)
        {
            return htmlDocument.QuerySelector(Constants.OfferSelectorNames.PlaceName) != null ? htmlDocument.QuerySelector(Constants.OfferSelectorNames.PlaceName).TextContent : "No Place Name";
        }

        private static DateTime getPosted(IHtmlDocument htmlDocument) //получает дату размещения публикации
        {
            var a = htmlDocument.QuerySelector(Constants.OfferSelectorNames.Posted);

            return a != null ? Convert.ToDateTime(a.TextContent) : DateTime.MinValue.AddYears(1800);  //добавляем 1800 лет для 0001 года, т.к. MS SQL не умеет работать с датами раньше 1753 года
        }

        private static DateTime getUpdated(IHtmlDocument htmlDocument) //получает дату размещения публикации, если дата обновления есть, тогда парсим, иначе берем из Posted
        {
            var postingInfos = htmlDocument.QuerySelectorAll(Constants.OfferSelectorNames.Updated);
            DateTime Updated = DateTime.MinValue.AddYears(1800);
            if (postingInfos != null)
            {
                foreach (var info in postingInfos)
                {
                    if (info.TextContent.Contains("updated:"))
                    {
                        Updated = Convert.ToDateTime(info.QuerySelector("time").TextContent);
                    }
                }
            }
            if (Updated == DateTime.MinValue.AddYears(1800))
            {
                Updated = getPosted(htmlDocument);
            }
            return Updated;
        }

    }
}
