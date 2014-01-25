using System;

namespace Deferred
{
  /// <summary>
  /// Messenger that represents some computation that will happen at an unknown time.
  /// Can be used to issue a promise that other objects can register callbacks with for when the computation completes.
  /// </summary>
  public interface IDeferred : IPromise
  {
    /// <summary>A promise that can be used to register callbacks for when the computation completes.</summary>
    /// <remarks>Expose this to consumers to prevent them from resolving or rejecting it.</remarks>
    IPromise Promise { get; }

    /// <summary>Invoke when the computation has completed successfully.</summary>
    void Resolve(EventArgs args);

    /// <summary>Invoke when the computation has completed unsuccessfully.</summary>
    void Reject(EventArgs args);
  }
}
