using StackExchange.Redis;

namespace AdvancedDataDrivenRx
{

  internal  class RedisConnectionMux 
  {

     private static ConnectionMultiplexer rconn = ConnectionMultiplexer.Connect("localhost");

     public static ConnectionMultiplexer GetConnectionMultiplexer()
     {
         return rconn;
     }

  }
    
}