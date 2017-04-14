using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace CraigsListParser.Components
{
     
    public class ProxySolver: SingleTone<ProxySolver>
    {
        private List<WebProxy> proxyList; //при создании экземпляра класса этот список наполняется списком прокси
        private int currentProxyIndex; //запоминает позицию последнего извлеченного прокси

 

        public ProxySolver()
        {
            proxyList = new List<WebProxy>();
            StreamReader sr = null;
            try
            {
                Console.WriteLine("Открываю файл с проксилистом {0}",Resources.ProxyList);

                sr = new StreamReader(Resources.ProxyList);

                while (!sr.EndOfStream)
                    {
                        String proxyLine = sr.ReadLine();
                        try
                        {
                            proxyList.Add(new WebProxy(proxyLine.Split(':')[0], Convert.ToInt32(proxyLine.Split(':')[1])));
                        }
                        catch
                        {
                            continue;
                        }
                    }
                sr.Close();
                Console.WriteLine("Получены прокси: {0}", proxyList.Count);
                
            }
            catch
            {
                Console.WriteLine("Получить прокси не удалось!");
                throw new Exception();
            }
            finally
            {
                sr.Dispose();
            }
            currentProxyIndex = -1;

            //А теперь исключим непигнуемые прокси
            Console.WriteLine("Проверяю прокси на доступность...");
            List<WebProxy> filteredProxies =
                    proxyList.AsParallel()
                                .Where(i => !ExcludeProxyByPing(i))
                                .ToList();
            proxyList = filteredProxies;
            File.AppendAllText("log.txt", String.Format("Количество рабочих прокси на {1}: {0}\n", proxyList.Count, DateTime.Now));
            //proxyList.RemoveAll(i => ExcludeProxyByPing(i));
            //Console.WriteLine("Отфильтрованы по пингу в {0}мс:",Resources.MaxProxyPing);
            proxyList.ForEach(i => Console.WriteLine(i.Address));
            //Console.ReadKey();
        }
        /// <summary>
        /// Получает текущий прокси. 
        /// Если он не был ранее получен методом  getNewProxy(), то возвращает результат вызова getNewProxy()
        /// </summary>
        public WebProxy CurrentProxy {
            get
            {
                return currentProxyIndex != -1 ? proxyList[currentProxyIndex] : getNewProxy();
            }
        }


        /// <summary>
        /// Получает новый прокси из List'а типа System.Net.WebProxy сгенерированных прокси
        /// </summary>
        /// <returns>Значение типа System.Net.WebProxy из списка экземпляра класса ProxySolver</returns>
        public WebProxy getNewProxy() //берет из листа новый прокси
        {
            WebProxy proxy = new WebProxy();
            if(currentProxyIndex == -1)
            {
                proxy = proxyList[0];
                currentProxyIndex = 0;
            }
            else
            {
                proxy = proxyList[increaseProxyIndex()];
            }
            
            return proxy;
        }

        private int increaseProxyIndex() //приращивваем индекс текущего прокси
        {
            if(currentProxyIndex == proxyList.Count - 1) //если мы достигли конца списка
            {
                currentProxyIndex = 0;
            }
            else
            {
                currentProxyIndex++;
            }
            return currentProxyIndex;
        }

        private bool ExcludeProxyByPing(WebProxy currentProxy)
        {
            //Console.Write("Прокси {0} проверяется...", currentProxy.Address);
            // Ping's the proxy server.
            Ping pingSender = new Ping();

            PingReply reply = pingSender.Send(currentProxy.Address.Host);

            if (reply.Status != IPStatus.Success) //если сервак не пингуется, то адьё
            {
                //Console.WriteLine(" Не доступен!");
                return true;
            }
            else
            {
                int maxPing;
                try
                {
                    maxPing = Convert.ToInt32(Resources.MaxProxyPing);
                }
                catch
                {
                    Console.WriteLine("В ресурсном файле неправильно записано значение максимального значения пинга для проски-сервера");
                    throw new Exception();
                }
                if (reply.RoundtripTime > maxPing) //если пинг сервака не соответсвует значению в Resources.resx, то говорим, что его не нужно  
                {
                    Console.WriteLine("Прокси {1} Не годится по пингу: ({0})", reply.RoundtripTime, currentProxy.Address);
                    return true;
                }
                else
                {
                    Console.WriteLine("Прокси {0} Годится!", currentProxy.Address);
                    return false;
                }
            }
        }
    }
}
