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

        protected void AddOrAppendToDict(Dictionary<string, string> dict, string key, string value)
        {
            string junk;
            if (dict.TryGetValue(key, out junk))
            {
                dict[key] += Environment.NewLine + value;
            }
            else
            {
                dict[key] = value;
            }
        }


    }
}
