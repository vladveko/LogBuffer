using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LogBuffer
{
    class Program
    {
        static void Main(string[] args)
        {
            TaskDelegate[] tasks = new TaskDelegate[5];
            for (int i = 0; i < 5; i++)
            {
                int temp = i;
                tasks[i] = delegate { Console.WriteLine("Task {0} working...", temp); };
            }
            Parallel.WaitAll(tasks);
            Console.WriteLine("All tasks done");

            LogBuffer logBuffer = new LogBuffer("logFile.txt", 20, 5000);
            int counter = 0;
            while (counter < 20)
            {
                Thread.Sleep(2000);
                logBuffer.Add($"Message {counter}");
                counter++;
            }
            Console.WriteLine("LogBuffer stoppped.");
            Console.ReadLine();
        }
    }
}
