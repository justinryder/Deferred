using System;

namespace Deferred
{
  public class Promise : IPromise
  {
    private event EventHandler OnAlways;
    private event EventHandler OnDone;
    private event EventHandler OnFail;

    private EventArgs _alwaysArgs;
    private bool _isResolved;
    private EventArgs _resolvedArgs;
    private bool _isRejected;
    private EventArgs _rejectedArgs;

    /// <summary>Internal to enforce initialization as a <see cref="Deferred"/></summary>
    internal Promise()
    {
    }

    public IPromise Always(EventHandler handler)
    {
      OnAlways += handler;
      if (_isResolved || _isRejected)
      {
        Always(_alwaysArgs);
      }
      return this;
    }

    public IPromise Done(EventHandler handler)
    {
      OnDone += handler;
      if (_isResolved)
      {
        Done(_resolvedArgs);
      }
      return this;
    }

    public IPromise Fail(EventHandler handler)
    {
      OnFail += handler;
      if (_isRejected)
      {
        Fail(_rejectedArgs);
      }
      return this;
    }

    protected void Always(EventArgs args)
    {
      _alwaysArgs = args;
      if (OnAlways != null)
      {
        OnAlways(this, args);
      }
      OnAlways = null;
    }

    protected void Done(EventArgs args)
    {
      _resolvedArgs = args;
      _isResolved = true;
      if (OnDone != null)
      {
        OnDone(this, args);
      }
      OnDone = null;
    }

    protected void Fail(EventArgs args)
    {
      _rejectedArgs = args;
      _isRejected = true;
      if (OnFail != null)
      {
        OnFail(this, args);
      }
      OnFail = null;
    }
  }
}
