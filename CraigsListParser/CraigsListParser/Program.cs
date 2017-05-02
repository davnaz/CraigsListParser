using AngleSharp.Parser.Html;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Text;
using CraigsListParser.DataProviders;
using AngleSharp.Dom.Html;
using AngleSharp.Dom;
using System.Threading;
using System.Net.NetworkInformation;
using CraigsListParser.Components;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CraigsListParser
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser myParser = new Parser();
            List<string> citiesList = new List<string>();
            for(int i = 0;i<400;i++) //пытаемся через несколько прокси получить список регионов, прокси обновляется в методе
            {
                myParser.GetRegionsList(citiesList);
                if (citiesList.Count !=0) { break; }
            }
            
            //ParallelOptions options = new ParallelOptions();
            //options.MaxDegreeOfParallelism = Convert.ToInt32(Resources.MaxDegreeOfParallelism);
            //Parallel.ForEach(citiesList, options, (city) =>
            //{
            //    Parser p = new Parser();
            //    p.StartParsing(city, 0, 1000000);
            //});
            Parser p = new Parser();
            p.StartParsing("https://boston.craigslist.org", 0, 250000);

            Console.WriteLine("Работа парсера завершена. Для продолжения нажмите любую клавишу...");
            Console.ReadKey();
        }
    }
}
