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
        int currentProxy; //запоминает позицию последнего извлеченного прокси
        public ProxySolver()
        {
            proxyList = new List<WebProxy>();
            StreamReader sr = null;
            try
            {
                sr = new StreamReader("proxylist.txt");

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
            currentProxy = -1;
            //А теперь исключим непигнуемые прокси
            proxyList.RemoveAll(i => ExcludeProxyByPing(i));
            for(int i = 0; i< proxyList.Count;i++)
            {
                if(ExcludeProxyByPing(proxyList[i]))
                {
                    proxyList.RemoveAt(i);
                    i--;
                }
            }
        }
        public WebProxy getNewProxy() //берет из листа новый прокси
        {
            WebProxy proxy = new WebProxy();
            if(currentProxy == -1)
            {
                proxy = proxyList[0];
                currentProxy = 0;
            }
            else
            {
                proxy = proxyList[increaseProxyIndex()];
            }
            
            return proxy;
        }

        private int increaseProxyIndex() //приращивваем индекс текущего прокси
        {
            if(currentProxy == proxyList.Count - 1) //если мы достигли конца списка
            {
                currentProxy = 0;
            }
            else
            {
                currentProxy++;
            }
            return currentProxy;
        }
        private bool ExcludeProxyByPing(WebProxy currentProxy)
        {
            // Ping's the proxy server.
            Ping pingSender = new Ping();

            PingReply reply = pingSender.Send(currentProxy.Address.Host);

            if (reply.Status != IPStatus.Success) //если сервак не пингуется, то адьё
            {
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
                if(reply.RoundtripTime > maxPing) //если пинг сервака не соответсвует значению в Resources.resx, то говорим, что его не нужно  
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }
}
