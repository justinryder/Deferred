using System.Collections.Generic;
using System.Linq;

namespace Deferred
{
  public class Promise<T> : IPromise<T>
  {
    private readonly Queue<PromiseAlwaysHandler<T>> _alwaysHandlers = new Queue<PromiseAlwaysHandler<T>>();
    private readonly Queue<PromiseDoneHandler<T>> _doneHandlers = new Queue<PromiseDoneHandler<T>>();
    private readonly Queue<PromiseFailHandler<T>> _failHandlers = new Queue<PromiseFailHandler<T>>();

    protected bool IsDone { get; private set; }
    protected bool HasFailed { get; private set; }

    private T _alwaysArgs;
    private T _doneArgs;
    private T _failArgs;

    /// <summary>Internal to enforce initialization as a <see cref="Deferred"/></summary>
    internal Promise()
    {
    }

    #region IPromise Implementation

    public IPromise<T> Always(PromiseAlwaysHandler<T> handler)
    {
      _alwaysHandlers.Enqueue(handler);
      if (IsDone || HasFailed)
      {
        Always(_alwaysArgs);
      }
      return this;
    }

    public IPromise<T> Done(PromiseDoneHandler<T> handler)
    {
      _doneHandlers.Enqueue(handler);
      if (IsDone)
      {
        Done(_doneArgs);
      }
      return this;
    }

    public IPromise<T> Fail(PromiseFailHandler<T> handler)
    {
      _failHandlers.Enqueue(handler);
      if (HasFailed)
      {
        Fail(_failArgs);
      }
      return this;
    }

    #endregion

    /// <summary>Invokes and clears the <see cref="_alwaysHandlers"/></summary>
    private void Always(T args)
    {
      _alwaysArgs = args;
      while (_alwaysHandlers.Any())
      {
        _alwaysHandlers.Dequeue().Invoke(args);
      }
    }

    /// <summary>Invokes and clears the <see cref="_doneHandlers"/></summary>
    protected void Done(T args)
    {
      _doneArgs = args;
      IsDone = true;
      while (_doneHandlers.Any())
      {
        _doneHandlers.Dequeue().Invoke(args);
      }

      Always(args);
    }

    /// <summary>Invokes and clears the <see cref="_failHandlers"/></summary>
    protected void Fail(T args)
    {
      _failArgs = args;
      HasFailed = true;
      while (_failHandlers.Any())
      {
        _failHandlers.Dequeue().Invoke(args);
      }

      Always(args);
    }
  }
}