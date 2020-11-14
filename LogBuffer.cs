using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LogBuffer
{
    public class LogBuffer
    {
        private List<string> msgList;
        private int msgLimit;
        private string filePath;
        private Timer timer;
        private object msgListLock = new object();
        private object writeLock = new object();

        public LogBuffer(string FilePath, int MessageLimit, int TimerInterval)
        {
            if (TimerInterval < 1 || MessageLimit < 1)
                throw new ArgumentException("Invalid Buffer paramenters.");

            this.filePath = FilePath;
            this.msgLimit = MessageLimit;
            this.timer = new Timer(new TimerCallback(this.ClearBufferAsync), null, 0, TimerInterval);
        }

        public async void Add(string item)
        {
            bool reset = false;
            lock (msgListLock)
            {
                msgList.Add(item);
                reset = msgList.Count >= msgLimit;
            }

            if (reset)
                await Task.Run(() => this.WriteToFile());
        }

        private async void ClearBufferAsync(object state)
        {
            await Task.Run(() => this.WriteToFile());
        }

        private void WriteToFile()
        {
            List<string> bufferMsgList = null;
            lock (msgListLock)
            {
                bufferMsgList = new List<string>(this.msgList);
                this.msgList.Clear();
            }

            lock (writeLock)
            {
                using (StreamWriter stream = File.AppendText(this.filePath))
                {
                    foreach(string message in bufferMsgList)
                    {
                        stream.WriteLine(message);
                    }
                }
            }
        }
    }
}
