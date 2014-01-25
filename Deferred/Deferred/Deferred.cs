namespace Deferred
{
  public class Deferred<T> : Promise<T>, IDeferred<T>
  {
    public IPromise<T> Promise
    {
      get { return this; }
    }

    public void Resolve(T args)
    {
      SingleResolveOrRejectContract();

      Done(args);
    }

    public void Reject(T args)
    {
      SingleResolveOrRejectContract();

      Fail(args);
    }

    /// <summary>Enforces the constact that a promise can only be resolved or rejected once</summary>
    private void SingleResolveOrRejectContract()
    {
      if (IsDone)
      {
        throw new ResolvedDeferredException("The deferred has already been resolved.");
      }
      if (HasFailed)
      {
        throw new RejectedDeferredException("The deferred has already been resolved.");
      }
    }
  }
}