using System;

namespace Deferred
{
  /// <summary>
  /// Represents a promise that some computing will be completed.
  /// Objects can register callbacks that are invoked when the promis is resolved or rejected.
  /// </summary>
  public interface IPromise
  {
    /// <summary>
    /// Registers a handler that will be called when the promise is resolved or rejected.
    /// Handlers registered after resolution or rejection will be called immediately.
    /// </summary>
    IPromise Always(EventHandler handler);

    /// <summary>
    /// Registers a handler that will be called when the promise is resolved.
    /// Handlers registered after resolution will be called immediately.
    /// </summary>
    IPromise Done(EventHandler handler);

    /// <summary>
    /// Registers a handler that will be called when the promise is rejected.
    /// Handlers registered after rejection will be called immediately.
    /// </summary>
    IPromise Fail(EventHandler handler);
  }
}
