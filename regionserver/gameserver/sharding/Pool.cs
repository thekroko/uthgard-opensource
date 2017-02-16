using System;
using System.Collections.Generic;

namespace DOL.GS.Sharding
{
  /// <summary>
  /// Pool of reusable objects
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class Pool<T>
  {
    readonly Stack<T> pool;
    readonly Func<T> initAction;

    // Initializes the object pool to the specified size
    //
    // The "capacity" parameter is the maximum number of 
    // SocketAsyncEventArgs objects the pool can hold
    public Pool(int initialCapacity)
    {
      pool = new Stack<T>(initialCapacity);
    }

    public Pool(int initialCapacity, Func<T> initAction) : this(initialCapacity) {
      this.initAction = initAction;
      for (int i = 0; i < initialCapacity; i++) {
        Push(initAction());
      }
    }

    // Add a SocketAsyncEventArg instance to the pool
    //
    //The "item" parameter is the SocketAsyncEventArgs instance 
    // to add to the pool
    public void Push(T item)
    {
      if (item == null) { throw new ArgumentNullException("Items added to a pool cannot be null"); }
      lock (pool)
      {
        pool.Push(item);
      }
    }

    // Removes a SocketAsyncEventArgs instance from the pool
    // and returns the object removed from the pool
    public T Pop()
    {
      lock (pool) {
        if (pool.Count == 0 && initAction != null) {
          Push(initAction());
        }
        return pool.Pop();
      }
    }

    // The number of SocketAsyncEventArgs instances in the pool
    public int Count => pool.Count;
  }
}
