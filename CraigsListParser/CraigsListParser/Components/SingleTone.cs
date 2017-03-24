
namespace CraigsListParser.Helpers
{    
    public class SingleTone<T> where T : class, new()
    {
        static protected T _instance = null;
        static public T Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new T();

                return _instance;
            }
        }
    }
}