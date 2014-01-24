using System;

namespace Deferred
{
  public interface IPromise
  {
    IPromise Always(EventHandler handler);
    IPromise Done(EventHandler handler);
    IPromise Fail(EventHandler handler);
  }
}
