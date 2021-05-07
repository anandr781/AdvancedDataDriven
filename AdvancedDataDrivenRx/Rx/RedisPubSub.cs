using System.Reactive.Disposables;
using System.Reactive.Linq;
using StackExchange.Redis;
using System;

namespace AdvancedDataDrivenRx
{
    public class RedisPubSub
    {

        ConnectionMultiplexer rconn = RedisConnectionMux.GetConnectionMultiplexer();

        public void RedisPublish(string redisChannelName, string value)
        {
            var subscriber = rconn.GetSubscriber();

            subscriber.PublishAsync(redisChannelName, new RedisValue(value));
        }

        /// This is for Redis Pub/Sub    
        public IObservable<RedisValue> RedisSubscribe(string redisChannelName)
        {
            var subscriber = rconn.GetSubscriber();

            return Observable.Create<RedisValue>(observer =>
            {

                subscriber.SubscribeAsync(redisChannelName, delegate (RedisChannel rc, RedisValue rv)
                {

                    if (string.IsNullOrEmpty(rv))
                        return;

                    observer.OnNext(rv);

                });
                return Disposable.Create(delegate ()
                {
                    subscriber.Unsubscribe(redisChannelName);
                });
            });
        }

    }
}