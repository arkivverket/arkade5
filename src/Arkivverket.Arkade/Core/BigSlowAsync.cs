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

        public async Task<int> DoSomethingRatherSlow(IProgress<int> progress)
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
                        progress.Report(i);
                    }
                }

                return i;
            });
            return processCount;
        }

        public int TheSlowPart(int i)
        {
            Thread.Sleep(50);
            return i;
        }



    }
}
