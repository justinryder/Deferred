using System.Collections.Generic;
using System.Linq;

namespace Deferred
{
  public class Promise<T> : IPromise<T>
  {
    private readonly Queue<PromiseAlwaysHandler<T>> _alwaysHandlers = new Queue<PromiseAlwaysHandler<T>>();
    private readonly Queue<PromiseDoneHandler<T>> _doneHandlers = new Queue<PromiseDoneHandler<T>>();
    private readonly Queue<PromiseFailHandler<T>> _failHandlers = new Queue<PromiseFailHandler<T>>();
    private readonly Queue<PromiseThenHandler<T>> _thenDoneHandlers = new Queue<PromiseThenHandler<T>>();
    private readonly Queue<PromiseThenHandler<T>> _thenFailHandlers = new Queue<PromiseThenHandler<T>>();
    private readonly Queue<PromiseThenHandler<T>> _thenAlwaysHandlers = new Queue<PromiseThenHandler<T>>();

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

    public IPromise<T> Then(PromiseThenHandler<T> doneHandler, PromiseThenHandler<T> failHandler)
    {
      ThenDone(doneHandler);
      ThenFail(failHandler);
      return this;
    }

    public IPromise<T> ThenDone(PromiseThenHandler<T> handler)
    {
      ThenAfterResolveOrRejectContract();
      _thenDoneHandlers.Enqueue(handler);
      return this;
    }

    public IPromise<T> ThenFail(PromiseThenHandler<T> handler)
    {
      ThenAfterResolveOrRejectContract();
      _thenFailHandlers.Enqueue(handler);
      return this;
    }

    public IPromise<T> ThenAlways(PromiseThenHandler<T> handler)
    {
      ThenAfterResolveOrRejectContract();
      _thenAlwaysHandlers.Enqueue(handler);
      return this;
    }

    #endregion

    /// <summary>Invokes and clears the <see cref="_alwaysHandlers"/> and sets the <see cref="_alwaysArgs"/></summary>
    private void Always(T args)
    {
      _alwaysArgs = args;
      while (_alwaysHandlers.Any())
      {
        _alwaysHandlers.Dequeue().Invoke(args);
      }
    }

    /// <summary>Invokes and clears the <see cref="_doneHandlers"/> and sets the <see cref="_doneArgs"/></summary>
    protected void Done(T args)
    {
      ThenDone(ref args);
      _doneArgs = args;
      IsDone = true;
      while (_doneHandlers.Any())
      {
        _doneHandlers.Dequeue().Invoke(args);
      }

      Always(args);
    }

    /// <summary>Invokes and clears the <see cref="_failHandlers"/> and sets the <see cref="_failArgs"/></summary>
    protected void Fail(T args)
    {
      ThenFail(ref args);
      _failArgs = args;
      HasFailed = true;
      while (_failHandlers.Any())
      {
        _failHandlers.Dequeue().Invoke(args);
      }

      Always(args);
    }

    private void ThenDone(ref T args)
    {
      while (_thenDoneHandlers.Any())
      {
        _thenDoneHandlers.Dequeue().Invoke(ref args);
      }
      ThenAlways(ref args);
    }

    private void ThenFail(ref T args)
    {
      while (_thenFailHandlers.Any())
      {
        _thenFailHandlers.Dequeue().Invoke(ref args);
      }
      ThenAlways(ref args);
    }

    private void ThenAlways(ref T args)
    {
      while (_thenAlwaysHandlers.Any())
      {
        _thenAlwaysHandlers.Dequeue().Invoke(ref args);
      }
    }

    private void ThenAfterResolveOrRejectContract()
    {
      if (IsDone)
      {
        throw new ResolvedDeferredException("The promise has already been resolved.");
      }
      if (HasFailed)
      {
        throw  new RejectedDeferredException("The promise has already been rejected.");
      }
    }
  }
}