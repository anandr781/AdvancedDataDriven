# Concurrency in C# Cookbook 

## Introduction to Concurrency 

_Concurrency_

_Doing more than one thing at a time._

_Multithreading_

_A form of concurrency that uses multiples threads of execution._

Multithreading, lives on the in the thread pool, a useful place to queue work that automatically adjusts itself according to demand. In turn the thread pool enables another form of concurrency : _parallel processing._

 _Parallel Processing_ 

_Doing lots of work by dividing it up among multiple threads that run concurrently._

Parallel processing uses multithreading to maximise the use of multiple processor cores. Modern CPUs have multiple cores, and if there is a lot of work to do, then it makes no sense to make one core do all the work while others are idle. So there is another form of concurrency called as _asynchronous programming._

_Asynchronous Programming_ 

A _future (or promise)_ is a type that represents some operation that will complete in the future. Some modern future types in .NET are _Task_ and _Task<TResult>_ . Basically the idea is centered around while the operation is in progress, it doesn't block the original thread; the thread that starts the operation is free to do other work. When the operation completes, it notifies its future or it invokes a callback or event to let the application know the operation is finished.

The _async_ and _await_ support in modern languages make asynchronous programming as easy as synchronous promgramming.

## Reactive Programming

Ashynchronous programming implies that the application will start an operation that will complete once at a later time. Reactive programming is closely related to asynchronous programming but its built on __*asynchronous events*__ instead of __*asynchronous operations*__.

It is a declarative style of programming where the application reacts to events. 

> NOTE : _Avoid async void method since if an exception occurs it can't be handled. So it must return a task type._



