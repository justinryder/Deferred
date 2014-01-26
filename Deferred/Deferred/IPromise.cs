namespace Deferred
{
  /// <summary>
  /// Represents a promise that some computing will be completed.
  /// Objects can register callbacks that are invoked when the <see cref="IDeferred{T}"/> is resolved or rejected.
  /// </summary>
  public interface IPromise<T>
  {
    /// <summary>
    /// Registers a handler that will be called when the <see cref="IDeferred{T}"/> is resolved or rejected.
    /// Handlers registered after resolution or rejection will be called immediately.
    /// </summary>
    /// <returns><see langword="this"/>, for method chaining</returns>
    IPromise<T> Always(PromiseAlwaysHandler<T> handler);

    /// <summary>
    /// Registers a handler that will be called when the <see cref="IDeferred{T}"/> is resolved.
    /// Handlers registered after resolution will be called immediately.
    /// </summary>
    /// <returns><see langword="this"/>, for method chaining</returns>
    IPromise<T> Done(PromiseDoneHandler<T> handler);

    /// <summary>
    /// Registers a handler that will be called when the <see cref="IDeferred{T}"/> is rejected.
    /// Handlers registered after rejection will be called immediately.
    /// </summary>
    /// <returns><see langword="this"/>, for method chaining</returns>
    IPromise<T> Fail(PromiseFailHandler<T> handler);
    
    /// <summary>
    /// Registers a handler that will be called when the <see cref="IDeferred{T}"/> is resolved and one for when it's rejected.
    /// <see cref="PromiseThenHandler{T}"/>s are called before other handlers, first-in-first-out, allowing them to modify the arguments before handling resolution/rejection.
    /// </summary>
    /// <param name="doneHandler">A handler with an <see langword="out"/> parameter for modifying the arguments before done handlers are called</param>
    /// <param name="failHandler">A handler with an <see langword="out"/> parameter for modifying the arguments before fail handlers are called</param>
    /// <returns><see langword="this"/>, for method chaining</returns>
    IPromise<T> Then(PromiseThenHandler<T> doneHandler, PromiseThenHandler<T> failHandler);

    /// <summary>Registers a handler that will be called when the <see cref="IDeferred{T}"/> is resolved.</summary>
    /// <returns><see langword="this"/>, for method chaining</returns>
    IPromise<T> ThenDone(PromiseThenHandler<T> handler);

    /// <summary>Registers a handler that will be called when the <see cref="IDeferred{T}"/> is rejected.</summary>
    /// <returns><see langword="this"/>, for method chaining</returns>
    IPromise<T> ThenFail(PromiseThenHandler<T> handler);

    /// <summary>Registers a handler that will be called when the <see cref="IDeferred{T}"/> is resolved or rejected.</summary>
    /// <returns><see langword="this"/>, for method chaining</returns>
    IPromise<T> ThenAlways(PromiseThenHandler<T> handler);
  }

  public delegate void PromiseAlwaysHandler<in T>(T args);
  public delegate void PromiseDoneHandler<in T>(T args);
  public delegate void PromiseFailHandler<in T>(T args);
  public delegate void PromiseThenHandler<T>(ref T args);
}