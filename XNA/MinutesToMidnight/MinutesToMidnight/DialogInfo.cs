using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinutesToMidnight
{
    public class DialogInfo
    {
        public string Info;
        public string Source;
        public string ResponsePrompt;
        public DialogInfo(string info, string source, string responsePrompt)
        {
            Info = info;
            Source = source;
            ResponsePrompt = responsePrompt;
        }
    }
}
