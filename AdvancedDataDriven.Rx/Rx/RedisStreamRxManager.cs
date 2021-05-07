
using System;
using System.Reactive.Linq;
using StackExchange.Redis;
using System.Timers;

/// For StackExchange.Redis Streams documentation visit : 
/// https://github.com/StackExchange/StackExchange.Redis/blob/main/docs/Streams.md

namespace AdvancedDataDriven.Rx
{

    public class RedisStreamRxManager
    {

        private event OnStreamEntriesReceivedEventHandler OnStreamEntriesReceivedEvent;
        ConnectionMultiplexer rconn = RedisConnectionMux.GetConnectionMultiplexer();


        public IObservable<StreamEntriesArgs> RedisStreamRxSubscriber(string redisStreamName)
        {

            var db = rconn.GetDatabase();
            var timer = new System.Timers.Timer(5000);
            string consumerGroupName = redisStreamName + "ConsGroup";
            timer.Elapsed += async (object s, ElapsedEventArgs e) =>
            {

                var t = await db.StreamReadGroupAsync(redisStreamName, consumerGroupName, "a", ">", 1, true); //noAck =true and a random consumer name  
                if (t != null && t.Length > 0)
                    OnStreamEntriesReceivedEvent(s, new StreamEntriesArgs { StreamEntries = t });
            };

            //Do Redis.ConsumerGroup Initialization
            db.StreamDeleteConsumerGroup(redisStreamName, consumerGroupName);
            db.StreamCreateConsumerGroup(redisStreamName, consumerGroupName, StreamPosition.NewMessages);

            timer.Start();

            var obs = Observable.FromEventPattern<OnStreamEntriesReceivedEventHandler, StreamEntriesArgs>(
                evh => this.OnStreamEntriesReceivedEvent += evh,
                evh => this.OnStreamEntriesReceivedEvent -= evh
            ).Select(x => x.EventArgs);

            return obs;

        }

        public void RedisStreamRxPublisher(string redisStreamName, string message)
        {
            var db = rconn.GetDatabase();

        }

    }

    #region Event Definition
    public delegate void OnStreamEntriesReceivedEventHandler(object sender, StreamEntriesArgs e);

    public class StreamEntriesArgs : EventArgs
    {
        public StreamEntry[] StreamEntries { get; set; }
    }

    #endregion
}