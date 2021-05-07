Reference - http://scienceadvantage.net/wp-content/uploads/2020/09/C-8.0-In-A-Nutshell-The-Definitive-Reference-02.06.2020.-.pdf

## Threading, Concurrency and Asynchrony

### Threading
A __Thread__ is an execution path that can proceed independently of other parts of a program.

```c#
  ... 
  Thread t = new Thread(() => CallSomeMethod());
  t.Start();
  t.Join(); //ends the thread . 
  ....
```
_Thread.Sleep(XX)_ -> relinquishes the thread's current time slice immediately, voluntarily handing over the CPU to other threads. _Thread.Yield()_ does the same thing except that it relinquishes only to threads running on the same processor.

__While waiting on a Sleep or Join, a thread is blocked.__

### Blocking 
A thread is deemed _blocked_ when its execution is paused for some reason, such as when Sleeping or waiting for another to end via Join. 
__A blocked thread immediately *yields* its processor time slice, and then from then on it consumes no processor time until its blocking condition is satisfied. You can test for a thread being blocked via ThreadState property__. 

When a thread blocks or unblocks the OS performs a _context switch_. This incurs a small overhead usually 1 or 2 microseconds. 

Refer this blog to understand I/O vs CPU bound work : https://blog.jkl.gg/io-cpu-bound-threads/
And read Chapter 14 - Blocking versus spinning.

### Exception Handling 
Any _try/catch/finally_ has no relevance to the new thread when it starts executing . Consider : 

```java
void main() 
{
   try {
       new Thread(Go).Start();
   }
   catch(Exception ex)
   {
       Console.WriteLine(ex.Message); //We will never get here 
   }
}
 .....
 static void Go() { throw new NullReferenceException();} 
```
This _try/catch_ statement above is ineffective since the exception occurs inside the thread that is spawned . So the solution is to __move the try/catch block to the thread where its expected__. 

### Thread Pool
Whenever you start a thread, a few hundred microseconds are spent in organizing such things as a fresh local variable stack. The _thread pool_ cuts this overhead by having a pool of pre-created recyclable threads. Thread Pooling is essential for efficient parallel programming and fine-grained concurrency; it allows short operations to run without being overwhelmed with overhead of thread startup. 

There are a few things to be wary of when using thread pools: 
*   You cannot set the Name of the pooled thread, making debugging more difficult. 
*   Pooled Threads are always _background threads_.
*   Blocking pooled threads can degrade performance. 
*   Use thread pools for CPU bound tasks and not I/O. 

You  can  determine  whether  you’re  currently  executing  on  a  pooled  thread  via  theproperty _Thread.CurrentThread.IsThreadPoolThread_.

The following use the thread pool implicitly : 
*   ASP.NET Core and WebAPI application servers.
*   System.Timer.Timer and System.Threading.Timer 
*   The parallel programming constructs (like PLINQ and ParallelCollections)
*   Legacy BackgroundWorker class. 

### Hygiene in the Thread pool
The thread pool serves another function, which is to ensure that a temporary excess of compute-bound work does not cause CPU oversubscription . 

__Oversubscription is a condition where there is more number of active threads than CPU cores, with the OS having to time-slice threads.__ 

Oversubscription hurts performance because time-slicing requires expensive _context switches_ and can invalidate the CPU caches that have become essential in delivering performance to modern processors. 

The CLR avoids oversubscription in the thread pool by queuing tasks and throttling their startup. It begins by running as many concurrent tasks as there are hardware cores, and then tunes the level of concurrency via a hill-climbing algorithm, continually adjusting the workload in a particular direction. If throughput improves, it continues in the same direction (otherwise reverses). This ensures it always follows an optimal path.

Blocking  is  troublesome  because  it  gives  the  CLR  the  false  idea  that  it’s  loading  upthe  CPU.  The  CLR  is  smart  enough  to  detect  and  compensate  (by  injecting  morethreads  into  the  pool),  although  this  can  make  the  pool  vulnerable  to  subsequent oversubscription. 

### Tasks
A Thread is a low-level tool for creating concurrency but has its short-comings. 
* Although its easy pass input param data into a thread that you start, its difficult to get "_return value_" back for a thread that you Join. We need somekind of a shared field for that. 

* You can't tell a thread go do something else when its finished; instead you must Join it (and that too synchronously).

These limitations discourage fine-grained concurrency in other words they make it difficult to compose larger concurrent operations by combining smaller ones . This in turns creates greater reliance on __manual synchronisation techniques (like locking, signaling and so on)__ and comes a hefty share of blocking problems. 

In a I/O bound applications , a thread-based approach consumes hundreds or thousands of MB of memory purely in thread overhead due to IOWait . 

__So Tasks are a higher level abstraction and they can use *thread pool* to reduce latency and with a *TaskCompletionSource* , they can employ a callback approach that avoids threads altogether while waiting on I/O bound operations. Tasks are also *compositional* and can be chained with *continuations*__ .

### Long-running Tasks 
By default, the CLR runs tasks on _pooled threads_. This is ideal for short CPU-bound work. But for long-running and blocking operations you can avoid thread-pools by : 

```java
Task task = Task.Factory.StartNew(()=> ..., TaskCreationOptions.LongRunning);

//But use TaskCompletionSource rather than the approach mentioned in the line above.

```
Running one long running task on a pooled thread won't cause trouble; its when its multiple long running ones come in parallel performance can suffer. And in that case, there are usually better solutions than TaskCreationsOptions.LongRunning. 

### Continuations 

```java
void main() {
  Task t = Task.Run(()=> { doSomething(); });
  var awaiter = t.GetAwaiter();

  //Provide the callback function to be called on completion 
  awaiter.OnCompleted(()=> {doOnCompleted();});

}
```

### Nuances with Asynchrony - ABSOLUTELY IMPORTANT
https://devblogs.microsoft.com/pfxteam/should-i-expose-asynchronous-wrappers-for-synchronous-methods/

https://blog.stephencleary.com/2013/11/taskrun-etiquette-examples-dont-use.html

https://stackoverflow.com/questions/31733734/what-is-naturally-asynchronous-or-pure-asynchronous

> MUST READ : 
  https://igorpopov.io/2018/06/30/asynchronous-programming-in-csharp/
  http://introtorx.com/Content/v1.0.10621.0/01_WhyRx.html#WhenRx
  https://docs.microsoft.com/en-us/archive/msdn-magazine/2013/march/async-await-best-practices-in-asynchronous-programming
  https://docs.microsoft.com/en-us/dotnet/standard/async-in-depth

> MUST LISTEN :
https://www.youtube.com/watch?v=G3tz9rxts8E&list=LL&index=2 -> Async / Await Internals - SynchronizationContext
https://www.youtube.com/watch?v=-Tq4wLyen7Q&list=LL&index=1&t=2915s -> Asynchronous Streams 
  

> The ability to invoke a synchronous method asynchronously does nothing for scalability, because you’re typically still consuming the same amount
> of resources you would have if you’d invoked it synchronously (in fact, you’re using a bit more, since there’s overhead incurred to scheduling 
> something ), you’re just using different resources to do it, e.g. a thread from a thread pool instead of the specific thread you were executing on. 
> The scalability benefits touted for asynchronous implementations are achieved by decreasing the amount of resources you use, and that needs to be baked into the implementation of an asynchronous method… it’s not something achieved by wrapping around it.




## Processes and Process Threads

## Advanced Threads and Locking - Mutex,Semaphore

## Garbage Collection and Memory 

