using System;
using Moq;
using NUnit.Framework;

namespace Deferred.Test
{
  public interface IPromiseComsumer
  {
    void AlwaysHandler(object args);
    void DoneHandler(object args);
    void FailHandler(object args);
  }

  [TestFixture]
  public abstract class DeferredTest : TestBase<IDeferred<object>>
  {
    protected override IDeferred<object> CreateSystemUnderTest()
    {
      return new Deferred<object>();
    }

    public abstract class WhenUsingAPromise : DeferredTest
    {
      public IPromise<object> PromiseUnderTest { get; private set; }

      protected override void EstablishContext()
      {
        base.EstablishContext();
        PromiseUnderTest = SystemUnderTest.Promise;
      }

      public abstract class WhenRegisteringADoneHandler : WhenUsingAPromise
      {
        public Mock<IPromiseComsumer> DoneHandler1 { get; private set; }
        public object DoneArgs { get; private set; }

        protected override void EstablishContext()
        {
          base.EstablishContext();
          DoneHandler1 = new Mock<IPromiseComsumer>();
          DoneArgs = new object();
        }

        protected override void Because()
        {
          base.Because();
          PromiseUnderTest.Done(DoneHandler1.Object.DoneHandler);
        }

        public class WhenTheDeferredIsResolved : WhenRegisteringADoneHandler
        {
          protected override void Because()
          {
            base.Because();
            SystemUnderTest.Resolve(DoneArgs);
          }

          [Test]
          public void TheDoneHandlerShouldBeInvoked()
          {
            DoneHandler1.Verify(x => x.DoneHandler(DoneArgs), Times.Once);
          }

          public class WhenAnotherDoneHandlerIsRegistered : WhenTheDeferredIsResolved
          {
            public Mock<IPromiseComsumer> DoneHandler2 { get; private set; }

            protected override void EstablishContext()
            {
              base.EstablishContext();
              DoneHandler2 = new Mock<IPromiseComsumer>();
            }

            protected override void Because()
            {
              base.Because();
              SystemUnderTest.Done(DoneHandler2.Object.DoneHandler);
            }

            [Test]
            public void TheSecondDoneHandlerShouldBeInvoked()
            {
              DoneHandler2.Verify(x => x.DoneHandler(DoneArgs), Times.Once);
            }

            [Test]
            public void TheFirstDoneHandlerShouldNotBeInvokedTwice()
            {
              DoneHandler1.Verify(x => x.DoneHandler(DoneArgs), Times.Once);
            }
          }
        }

        public class WhenTheDeferredIsRejected : WhenRegisteringADoneHandler
        {
          public object FailArgs { get; private set; }

          protected override void EstablishContext()
          {
            base.EstablishContext();
            FailArgs = new object();
          }

          protected override void Because()
          {
            SystemUnderTest.Reject(FailArgs);
          }

          [Test]
          public void TheDoneHandlerShouldNotBeInvoked()
          {
            DoneHandler1.Verify(x => x.DoneHandler(FailArgs), Times.Never);
          }
        }
      }

      public abstract class WhenRegisteringAFailHandler : WhenUsingAPromise
      {
        public Mock<IPromiseComsumer> FailHandler1 { get; private set; }
        public object FailArgs1 { get; private set; }

        protected override void EstablishContext()
        {
          base.EstablishContext();
          FailHandler1 = new Mock<IPromiseComsumer>();
          FailArgs1 = new object();
        }

        protected override void Because()
        {
          base.Because();
          PromiseUnderTest.Fail(FailHandler1.Object.FailHandler);
        }

        public class WhenTheDeferredIsRejected : WhenRegisteringAFailHandler
        {
          protected override void Because()
          {
            base.Because();
            SystemUnderTest.Reject(FailArgs1);
          }

          [Test]
          public void TheFailHandlerShouldBeInvoked()
          {
            FailHandler1.Verify(x => x.FailHandler(FailArgs1), Times.Once);
          }

          public class WhenAnotherFailHandlerIsRegistered : WhenTheDeferredIsRejected
          {
            public Mock<IPromiseComsumer> FailHandler2 { get; private set; }

            protected override void EstablishContext()
            {
              base.EstablishContext();
              FailHandler2 = new Mock<IPromiseComsumer>();
            }

            protected override void Because()
            {
              base.Because();
              SystemUnderTest.Fail(FailHandler2.Object.FailHandler);
            }

            [Test]
            public void TheSecondFailHandlerShouldBeInvoked()
            {
              FailHandler2.Verify(x => x.FailHandler(FailArgs1), Times.Once);
            }

            [Test]
            public void TheFirstFailHandlerShouldNotBeInvokedTwice()
            {
              FailHandler1.Verify(x => x.FailHandler(FailArgs1), Times.Once);
            }
          }
        }

        public class WhenTheDeferredIsResolved : WhenRegisteringAFailHandler
        {
          public object DoneArgs { get; private set; }

          protected override void EstablishContext()
          {
            base.EstablishContext();
            DoneArgs = new object();
          }

          protected override void Because()
          {
            SystemUnderTest.Resolve(DoneArgs);
          }

          [Test]
          public void TheFailHandlerShouldNotBeInvoked()
          {
            FailHandler1.Verify(x => x.FailHandler(DoneArgs), Times.Never);
          }
        }
      }

      public abstract class WhenRegisteringAnAlwaysHandler : WhenUsingAPromise
      {
        public Mock<IPromiseComsumer> AlwaysHandler1 { get; private set; }
        public object AlwaysArgs { get; private set; }

        protected override void EstablishContext()
        {
          base.EstablishContext();
          AlwaysHandler1 = new Mock<IPromiseComsumer>();
          AlwaysArgs = new object();
        }

        protected override void Because()
        {
          base.Because();
          PromiseUnderTest.Always(AlwaysHandler1.Object.AlwaysHandler);
        }

        public abstract class WhenTheDeferredIsResolvedOrRejected : WhenRegisteringAnAlwaysHandler
        {
          [Test]
          public void TheAlwaysHandlerShouldBeInvoked()
          {
            AlwaysHandler1.Verify(x => x.AlwaysHandler(AlwaysArgs), Times.Once);
          }

          public class WhenTheDeferredIsResolved : WhenTheDeferredIsResolvedOrRejected
          {
            protected override void Because()
            {
              base.Because();
              SystemUnderTest.Resolve(AlwaysArgs);
            }

            public class WhenTheDeferredIsResolvedAgain : WhenTheDeferredIsResolved
            {
              private Exception _exception;

              protected override void Because()
              {
                base.Because();
                try
                {
                  SystemUnderTest.Resolve(AlwaysArgs);
                }
                catch (Exception e)
                {
                  _exception = e;
                }
              }

              [Test]
              public void ARejectedPromiseExceptionShouldBeThrown()
              {
                Assert.IsInstanceOf<ResolvedDeferredException>(_exception);
              }
            }

            public class WhenTheDeferredIsAlsoRejected : WhenTheDeferredIsResolved
            {
              private Exception _exception;

              protected override void Because()
              {
                base.Because();
                try
                {
                  SystemUnderTest.Reject(AlwaysArgs);
                }
                catch (Exception e)
                {
                  _exception = e;
                }
              }

              [Test]
              public void AResolvedPromiseExceptionShouldBeThrown()
              {
                Assert.IsInstanceOf<ResolvedDeferredException>(_exception);
              }
            }
          }

          public class WhenTheDeferredIsRejected : WhenTheDeferredIsResolvedOrRejected
          {
            protected override void Because()
            {
              base.Because();
              SystemUnderTest.Reject(AlwaysArgs);
            }

            public class WhenTheDeferredIsRejectedAgain : WhenTheDeferredIsRejected
            {
              private Exception _exception;

              protected override void Because()
              {
                base.Because();
                try
                {
                  SystemUnderTest.Resolve(AlwaysArgs);
                }
                catch (Exception e)
                {
                  _exception = e;
                }
              }

              [Test]
              public void ARejectedPromiseExceptionShouldBeThrown()
              {
                Assert.IsInstanceOf<RejectedDeferredException>(_exception);
              }
            }

            public class WhenTheDeferredIsAlsoResolved : WhenTheDeferredIsRejected
            {
              private Exception _exception;

              protected override void Because()
              {
                base.Because();
                try
                {
                  SystemUnderTest.Resolve(AlwaysArgs);
                }
                catch (Exception e)
                {
                  _exception = e;
                }
              }

              [Test]
              public void ARejectedPromiseExceptionShouldBeThrown()
              {
                Assert.IsInstanceOf<RejectedDeferredException>(_exception);
              }
            }
          }
        }
      }
    }
  }
}
