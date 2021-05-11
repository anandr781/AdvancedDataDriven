# AdvancedDataDrivenApps
Advanced Data Driven Apps - Intends to demonstate the highly scalable nature of asynchronous and reactive applications . At highlevel aims to demo : 
  * Reactive I/O Operations - Read large files 
  * Reactive Redis
  * Asynchronous APIs [Paging APIs , Large result] - Yet to come

## Aspnet Core Best Practices : 
https://docs.microsoft.com/en-us/aspnet/core/performance/performance-best-practices?view=aspnetcore-5.0#understand-hot-code-paths

## Run Redis Container and Redis Streams to run the project
```shell

C:\>docker run --name redis-container -p 6379:6379 -d redis
dbbfd02af41cf4e684428ffa5ad50a4a43faad159522355a8db08f9575ef0771

C:\>docker ps
CONTAINER ID   IMAGE     COMMAND                  CREATED        STATUS        PORTS                    NAMES
75e6262bc7b1   redis     "docker-entrypoint.s…"   19 hours ago   Up 19 hours   0.0.0.0:6379->6379/tcp   redis-image


127.0.0.1:6379> XADD myStream * sensorId 1
1620237180298-0

127.0.0.1:6379> DEL myStream
(integer) 1

127.0.0.1:6379> XADD myStream * sensorId 123
1620238350909-0
127.0.0.1:6379> XADD myStream * sensorId 234
1620238365000-0

127.0.0.1:6379> XLEN myStream
(integer) 2

127.0.0.1:6379> XRANGE myStream - +
1) 1) 1620246011187-0
   2) 1) "sensorId"
      2) "1"
2) 1) 1620246104902-0
   2) 1) "sensorId"
      2) "2"
3) 1) 1620246366324-0
   2) 1) "sensorId"
      2) "3"
127.0.0.1:6379> XINFO STREAM myStream
1) length
2) 3
3) radix-tree-keys
4) 1
5) radix-tree-nodes
6) 2
7) last-generated-id
8) 1620246366324-0
9) groups
10) 1
11) first-entry
12) 1) 1620246011187-0
   2) 1) "sensorId"
      2) "1"
13) last-entry
14) 1) 1620246366324-0
   2) 1) "sensorId"
      2) "3"
```
