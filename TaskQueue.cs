using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;

namespace LogBuffer
{
    public delegate void TaskDelegate();

    public class TaskQueue
    {
        private Thread[] tasks;

        private ConcurrentQueue<TaskDelegate> taskDelegates;

        private int refCount;

        private bool isRunning;

        static private int defTaskCount = 5;

        public TaskQueue() : this(defTaskCount) { }
        public TaskQueue(int TasksCount)
        {
            this.tasks = new Thread[TasksCount];
            this.taskDelegates = new ConcurrentQueue<TaskDelegate>();
            this.isRunning = true;
            this.refCount = 0;

            for (int i = 0; i < TasksCount; i++)
            {
                this.tasks[i] = new Thread(new ThreadStart(this.AwaitLoop));          
                this.tasks[i].Start();
            }

        }

        private void AwaitLoop()
        {
            SpinWait spinWait = new SpinWait();
            TaskDelegate task;
            while (this.isRunning)
            {
                if (this.taskDelegates.TryDequeue(out task))
                {
                    Interlocked.Increment(ref this.refCount);
                    task.Invoke();
                    Interlocked.Decrement(ref this.refCount);
                }
                else
                    spinWait.SpinOnce();
            }
        }

        public void EnqueueTask(TaskDelegate task)
        {
            this.taskDelegates.Enqueue(task);
        }

        public void Stop()
        {
            SpinWait spinWait = new SpinWait();
            while (this.isRunning)
            {
                if (this.taskDelegates.IsEmpty && this.refCount == 0)
                    this.isRunning = false;
                else
                    spinWait.SpinOnce();
            }
            
        }
    }
}
