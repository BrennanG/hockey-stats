using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HockeyStats
{
    public abstract class Parser
    {
        protected Dictionary<string, string> dictionaryToFill;

        public void SetDictionaryToFill(Dictionary<string, string> dictionaryToFill)
        {
            this.dictionaryToFill = dictionaryToFill;
        }

        protected void AddOrAppendToDict(string key, string value)
        {
            string junk;
            if (dictionaryToFill.TryGetValue(key, out junk))
            {
                throw new Exception();
            }
            else
            {
                dictionaryToFill[key] = value;
            }
        }


    }
}
