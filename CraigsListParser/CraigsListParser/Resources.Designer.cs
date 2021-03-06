﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CraigsListParser {
    using System;
    
    
    /// <summary>
    ///   Класс ресурса со строгой типизацией для поиска локализованных строк и т.д.
    /// </summary>
    // Этот класс создан автоматически классом StronglyTypedResourceBuilder
    // с помощью такого средства, как ResGen или Visual Studio.
    // Чтобы добавить или удалить член, измените файл .ResX и снова запустите ResGen
    // с параметром /str или перестройте свой проект VS.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Возвращает кэшированный экземпляр ResourceManager, использованный этим классом.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("CraigsListParser.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Перезаписывает свойство CurrentUICulture текущего потока для всех
        ///   обращений к ресурсу с помощью этого класса ресурса со строгой типизацией.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на /search/aap.
        /// </summary>
        internal static string AltHouseSearchLinkPostfix {
            get {
                return ResourceManager.GetString("AltHouseSearchLinkPostfix", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на https://losangeles.craigslist.org.
        /// </summary>
        internal static string BaseLink {
            get {
                return ResourceManager.GetString("BaseLink", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на max_price=.
        /// </summary>
        internal static string CraigslistMaxPricePostfixForGetrequest {
            get {
                return ResourceManager.GetString("CraigslistMaxPricePostfixForGetrequest", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на 2500.
        /// </summary>
        internal static string CraigsListMaxSearchresvalue {
            get {
                return ResourceManager.GetString("CraigsListMaxSearchresvalue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на min_price=.
        /// </summary>
        internal static string CraigslistMinPricePostfixForGetrequest {
            get {
                return ResourceManager.GetString("CraigslistMinPricePostfixForGetrequest", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Data Source=NOTEBOOK;Initial Catalog=CraigsList;Integrated Security=True.
        /// </summary>
        internal static string DbConnectionString {
            get {
                return ResourceManager.GetString("DbConnectionString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на /search/apa.
        /// </summary>
        internal static string HouseSearchLinkPostfix {
            get {
                return ResourceManager.GetString("HouseSearchLinkPostfix", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на AddOrUpdateOffer.
        /// </summary>
        internal static string InsertOrUpdateOfferStoredProcedure {
            get {
                return ResourceManager.GetString("InsertOrUpdateOfferStoredProcedure", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на https://losangeles.craigslist.org/search/apa.
        /// </summary>
        internal static string MainLink {
            get {
                return ResourceManager.GetString("MainLink", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на 10.
        /// </summary>
        internal static string MaxDegreeOfParallelism {
            get {
                return ResourceManager.GetString("MaxDegreeOfParallelism", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на 400.
        /// </summary>
        internal static string MaxProxyPing {
            get {
                return ResourceManager.GetString("MaxProxyPing", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на proxylist.txt.
        /// </summary>
        internal static string ProxyList {
            get {
                return ResourceManager.GetString("ProxyList", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на https://geo.craigslist.org/iso/us.
        /// </summary>
        internal static string StartLink {
            get {
                return ResourceManager.GetString("StartLink", resourceCulture);
            }
        }
    }
}
