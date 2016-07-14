using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arkivverket.Arkade.Core
{
    public class BigSlowAsync
    {

        public async Task<int> DoSomethingRatherSlow(IProgress<BigSlowStatus> progress)
        {
            int processCount = await Task.Run<int>(() =>
            {
                int i = 0;
                for (i = 0; i < 100; i++)
                {
                    //await the processing and uploading logic here
                    int processed = TheSlowPart(i);
                    if (progress != null)
                    {
                        progress.Report(new BigSlowStatus(i, $"Kjære {i} landsmenn", false));
                    }
                }

                progress.Report(new BigSlowStatus(100, "Big slow is done", true));
                return i;
            });
            return processCount;
        }

        public int TheSlowPart(int i)
        {
            Thread.Sleep(75);
            return i;
        }



    }
}
