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
            
            int regionCounter;

            for(regionCounter = 0;regionCounter< citiesList.Count; regionCounter += 5)
            {                
                int step = (regionCounter + 4 >= citiesList.Count) ? citiesList.Count - regionCounter - 1 : 4; //на тот случай, если элементов в массиве не кратно шагу в цикле
                
                Parallel.ForEach(citiesList.GetRange(regionCounter, regionCounter + step), (city) =>
                {
                    Parser p = new Parser();
                    p.StartParsing(city);
                });            
            }

            Console.WriteLine("Работа парсера завершена. Для продолжения нажмите любую клавишу...");
            Console.ReadKey();
        }
    }
}
