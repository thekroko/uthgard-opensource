using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Nodes.Structures
{
  /// <summary>
  /// Pool of reusable objects
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class Pool<T>
  {
    readonly Stack<T> m_pool;

    // Initializes the object pool to the specified size
    //
    // The "capacity" parameter is the maximum number of 
    // SocketAsyncEventArgs objects the pool can hold
    public Pool(int capacity)
    {
      m_pool = new Stack<T>(capacity);
    }

    // Add a SocketAsyncEventArg instance to the pool
    //
    //The "item" parameter is the SocketAsyncEventArgs instance 
    // to add to the pool
    public void Push(T item)
    {
      if (item == null) { throw new ArgumentNullException("Items added to a pool cannot be null"); }
      lock (m_pool)
      {
        m_pool.Push(item);
      }
    }

    // Removes a SocketAsyncEventArgs instance from the pool
    // and returns the object removed from the pool
    public async Task<T> Pop() {
      while (true) {
        lock (m_pool) {
          if (m_pool.Count > 0)
            return m_pool.Pop();
        }
        await Task.Delay(1); // TODO(mlinder): Find a better way to do this
      }
    }

    // The number of SocketAsyncEventArgs instances in the pool
    public int Count => m_pool.Count;
  }
}
