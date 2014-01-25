namespace Deferred
{
  /// <summary>
  /// Represents a promise that some computing will be completed.
  /// Objects can register callbacks that are invoked when the <see cref="IDeferred{T}"/> is resolved or rejected.
  /// </summary>
  public interface IPromise<out T>
  {
    /// <summary>
    /// Registers a handler that will be called when the <see cref="IDeferred{T}"/> is resolved or rejected.
    /// Handlers registered after resolution or rejection will be called immediately.
    /// </summary>
    IPromise<T> Always(PromiseAlwaysHandler<T> handler);

    /// <summary>
    /// Registers a handler that will be called when the <see cref="IDeferred{T}"/> is resolved.
    /// Handlers registered after resolution will be called immediately.
    /// </summary>
    IPromise<T> Done(PromiseDoneHandler<T> handler);

    /// <summary>
    /// Registers a handler that will be called when the <see cref="IDeferred{T}"/> is rejected.
    /// Handlers registered after rejection will be called immediately.
    /// </summary>
    IPromise<T> Fail(PromiseFailHandler<T> handler);
  }

  public delegate void PromiseAlwaysHandler<in T>(T args);
  public delegate void PromiseDoneHandler<in T>(T args);
  public delegate void PromiseFailHandler<in T>(T args);
}