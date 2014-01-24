using NUnit.Framework;

namespace Deferred.Test
{
  [TestFixture]
  public abstract class DeferredTest
  {
    public IDeferred SystemUnderTest { get; private set; }

    [TestFixtureSetUp]
    public void Setup()
    {
      SystemUnderTest = new Deferred();
      EstablishContext();
      Because();
    }

    protected virtual void EstablishContext()
    {
    }

    protected abstract void Because();

    public abstract class WhenUsingAPromise : DeferredTest
    {
      public IPromise PromiseUnderTest { get; private set; }

      protected override void EstablishContext()
      {
        base.EstablishContext();
        PromiseUnderTest = SystemUnderTest.Promise;
      }

      public abstract class WhenRegisteringADoneHandler : WhenUsingAPromise
      {
        public int DoneHandlerInvocationCount { get; private set; }

        protected override void EstablishContext()
        {
          base.EstablishContext();
          PromiseUnderTest.Done((sender, args) => { DoneHandlerInvocationCount++; });
        }

        public class WhenTheDeferredIsResolved : WhenRegisteringADoneHandler
        {
          protected override void Because()
          {
            SystemUnderTest.Resolve(null);
          }

          [Test]
          public void TheDoneHandlerShouldBeInvoked()
          {
            Assert.AreEqual(1, DoneHandlerInvocationCount);
          }
        }

        public class WhenTheDeferredIsRejected : WhenRegisteringADoneHandler
        {
          protected override void Because()
          {
            SystemUnderTest.Reject(null);
          }

          [Test]
          public void TheDoneHandlerShouldNotBeInvoked()
          {
            Assert.AreEqual(0, DoneHandlerInvocationCount);
          }
        }
      }

      public abstract class WhenRegisteringAFailHandler : WhenUsingAPromise
      {
        public int FailHandlerInvocationCount { get; private set; }

        protected override void EstablishContext()
        {
          base.EstablishContext();
          PromiseUnderTest.Fail((sender, args) => { FailHandlerInvocationCount++; });
        }

        public class WhenTheDeferredIsResolved : WhenRegisteringAFailHandler
        {
          protected override void Because()
          {
            SystemUnderTest.Resolve(null);
          }

          [Test]
          public void TheFailHandlerShouldNotBeInvoked()
          {
            Assert.AreEqual(0, FailHandlerInvocationCount);
          }
        }

        public class WhenTheDeferredIsRejected : WhenRegisteringAFailHandler
        {
          protected override void Because()
          {
            SystemUnderTest.Reject(null);
          }

          [Test]
          public void TheFailHandlerShouldBeInvoked()
          {
            Assert.AreEqual(1, FailHandlerInvocationCount);
          }
        }
      }
    }
  }
}
