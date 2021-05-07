using StackExchange.Redis;
using System;

namespace AdvancedDataDriven.Rx
{
    public class RedisObserver : IObserver<StreamEntriesArgs>
    {
        Action<StreamEntriesArgs> dataCallback; 

        public RedisObserver(Action<StreamEntriesArgs> dataCallback)
        {
            this.dataCallback = dataCallback;
        }
        public void OnCompleted()
        {
           // Console.WriteLine("Completed");
        }

        public void OnError(Exception error)
        {
           throw new Exception("Error occured in Redis Connection", error);
        }

        public void OnNext(StreamEntriesArgs value)
        {
            dataCallback(value);
        }
    }
}
