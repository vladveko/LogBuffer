using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogBuffer
{
    public static class Parallel
    {
        private static TaskQueue taskQueue = new TaskQueue();
        public static void WaitAll(TaskDelegate[] tasks)
        {
            foreach(var task in tasks)
            {
                taskQueue.EnqueueTask(task);
            }

            taskQueue.Stop();
        }
    }
}
