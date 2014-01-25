using System;
using System.Runtime.Serialization;

namespace Deferred
{
  public class RejectedDeferredException : Exception
  {
    internal RejectedDeferredException()
    {
    }

    internal RejectedDeferredException(string message)
      :base(message)
    {
    }

    internal RejectedDeferredException(string message, Exception inner)
      : base(message, inner)
    {
    }

    protected RejectedDeferredException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}