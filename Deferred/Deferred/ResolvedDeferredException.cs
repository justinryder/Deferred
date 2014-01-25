using System;
using System.Runtime.Serialization;

namespace Deferred
{
  public class ResolvedDeferredException : Exception
  {
    internal ResolvedDeferredException()
    {
    }

    internal ResolvedDeferredException(string message)
      :base(message)
    {
    }

    internal ResolvedDeferredException(string message, Exception inner)
      : base(message, inner)
    {
    }

    protected ResolvedDeferredException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}