using System;

namespace Deferred
{
  public interface IDeferred : IPromise
  {
    IPromise Promise { get; }

    void Resolve(EventArgs args);
    void Reject(EventArgs args);
  }
}
