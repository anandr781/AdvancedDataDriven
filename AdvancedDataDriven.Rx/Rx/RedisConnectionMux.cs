using StackExchange.Redis;

namespace AdvancedDataDriven.Rx
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