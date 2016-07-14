using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkivverket.Arkade.Core
{
    public class BigSlowStatus
    {
        public int PctDone { get; set; }
        public string MyMessageToTheWorld { get; set; }
        public bool IsDone { get; set; }

        public BigSlowStatus(int pctDone, string myMessageToTheWord, bool isDone)
        {
            PctDone = pctDone;
            MyMessageToTheWorld = myMessageToTheWord;
            IsDone = isDone;
        }
    }
}
