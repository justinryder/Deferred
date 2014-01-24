using System;

namespace Deferred
{
  public class Deferred : Promise, IDeferred
  {
    public IPromise Promise
    {
      get { return this; }
    }

    public void Resolve(EventArgs args)
    {
      Done(args);
      Always(args);
    }

    public void Reject(EventArgs args)
    {
      Fail(args);
      Always(args);
    }
  }
}
