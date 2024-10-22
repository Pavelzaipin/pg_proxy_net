using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetProxy
{
    public class Queries
    {
        int _id = 0;

        public List<Query> list = new List<Query>();

        private static readonly Object s_lock = new Object();
        private static Queries instance = null;

        private Queries()
        { }

        public void AddQuery(string query)
        {
            var clearSqlText = RemoveSpecialCharacters(query);

            var shortClearSqlText = clearSqlText.Clone() as string;
            if(clearSqlText.Length > 20)
            {
                //shortClearSqlText = shortClearSqlText.Substring(0, 80);
            }
            
            list.Add(new Query(ref _id) { ShortSqlText = shortClearSqlText, SqlText = clearSqlText });
        }

        public List<Query> GetQueriesList() 
        { 
            return list; 
        }

        public static Queries GetInstance
        {
            get
            {
                if (instance != null) return instance;
                Monitor.Enter(s_lock);
                Queries temp = new Queries();
                Interlocked.Exchange(ref instance, temp);
                Monitor.Exit(s_lock);
                return instance;
            }
        }

       private string RemoveSpecialCharacters(string str)
       {
            if (str.Length > 19)
            {
                return str.Remove(0, 19);
            }
            else
            {
                return str;
            }

       }
    }
}
