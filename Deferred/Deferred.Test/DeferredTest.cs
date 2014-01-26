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
    void ThenAlwaysHandler(ref object args);
    void ThenDoneHandler(ref object args);
    void ThenFailHandler(ref object args);
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
      public Mock<IPromiseComsumer> Consumer { get; private set; }

      protected override void EstablishContext()
      {
        base.EstablishContext();
        PromiseUnderTest = SystemUnderTest.Promise;
        Consumer = new Mock<IPromiseComsumer>();
      }

      public abstract class WhenRegisteringAnAlwaysHandler : WhenUsingAPromise
      {
        public object Args { get; private set; }

        protected override void EstablishContext()
        {
          base.EstablishContext();
          Args = new object();
        }

        protected override void Because()
        {
          base.Because();
          PromiseUnderTest.Always(Consumer.Object.AlwaysHandler);
        }

        public abstract class WhenTheDeferredIsResolvedOrRejected : WhenRegisteringAnAlwaysHandler
        {
          [Test]
          public void TheAlwaysHandlerShouldBeInvoked()
          {
            Consumer.Verify(x => x.AlwaysHandler(Args), Times.Once);
          }

          public class WhenTheDeferredIsResolved : WhenTheDeferredIsResolvedOrRejected
          {
            protected override void Because()
            {
              base.Because();
              SystemUnderTest.Resolve(Args);
            }

            public class WhenTheDeferredIsResolvedAgain : WhenTheDeferredIsResolved
            {
              private Exception _exception;

              protected override void Because()
              {
                base.Because();
                try
                {
                  SystemUnderTest.Resolve(Args);
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
                  SystemUnderTest.Reject(Args);
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
              SystemUnderTest.Reject(Args);
            }

            public class WhenTheDeferredIsRejectedAgain : WhenTheDeferredIsRejected
            {
              private Exception _exception;

              protected override void Because()
              {
                base.Because();
                try
                {
                  SystemUnderTest.Resolve(Args);
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
                  SystemUnderTest.Resolve(Args);
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

      public abstract class WhenRegisteringADoneHandler : WhenUsingAPromise
      {
        public object Args { get; private set; }

        protected override void EstablishContext()
        {
          base.EstablishContext();
          Args = new object();
        }

        protected override void Because()
        {
          base.Because();
          PromiseUnderTest.Done(Consumer.Object.DoneHandler);
        }

        public class WhenTheDeferredIsResolved : WhenRegisteringADoneHandler
        {
          protected override void Because()
          {
            base.Because();
            SystemUnderTest.Resolve(Args);
          }

          [Test]
          public void TheDoneHandlerShouldBeInvoked()
          {
            Consumer.Verify(x => x.DoneHandler(Args), Times.Once);
          }

          public class WhenAnotherDoneHandlerIsRegistered : WhenTheDeferredIsResolved
          {
            public Mock<IPromiseComsumer> Consumer2 { get; private set; }

            protected override void EstablishContext()
            {
              base.EstablishContext();
              Consumer2 = new Mock<IPromiseComsumer>();
            }

            protected override void Because()
            {
              base.Because();
              SystemUnderTest.Done(Consumer2.Object.DoneHandler);
            }

            [Test]
            public void TheSecondDoneHandlerShouldBeInvoked()
            {
              Consumer2.Verify(x => x.DoneHandler(Args), Times.Once);
            }

            [Test]
            public void TheFirstDoneHandlerShouldNotBeInvokedTwice()
            {
              Consumer.Verify(x => x.DoneHandler(Args), Times.Once);
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
            Consumer.Verify(x => x.DoneHandler(FailArgs), Times.Never);
          }
        }
      }

      public abstract class WhenRegisteringAFailHandler : WhenUsingAPromise
      {
        public object Args { get; private set; }

        protected override void EstablishContext()
        {
          base.EstablishContext();
          Args = new object();
        }

        protected override void Because()
        {
          base.Because();
          PromiseUnderTest.Fail(Consumer.Object.FailHandler);
        }

        public class WhenTheDeferredIsRejected : WhenRegisteringAFailHandler
        {
          protected override void Because()
          {
            base.Because();
            SystemUnderTest.Reject(Args);
          }

          [Test]
          public void TheFailHandlerShouldBeInvoked()
          {
            Consumer.Verify(x => x.FailHandler(Args), Times.Once);
          }

          public class WhenAnotherFailHandlerIsRegistered : WhenTheDeferredIsRejected
          {
            public Mock<IPromiseComsumer> Consumer2 { get; private set; }

            protected override void EstablishContext()
            {
              base.EstablishContext();
              Consumer2 = new Mock<IPromiseComsumer>();
            }

            protected override void Because()
            {
              base.Because();
              SystemUnderTest.Fail(Consumer2.Object.FailHandler);
            }

            [Test]
            public void TheSecondFailHandlerShouldBeInvoked()
            {
              Consumer2.Verify(x => x.FailHandler(Args), Times.Once);
            }

            [Test]
            public void TheFirstFailHandlerShouldNotBeInvokedTwice()
            {
              Consumer.Verify(x => x.FailHandler(Args), Times.Once);
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
            Consumer.Verify(x => x.FailHandler(DoneArgs), Times.Never);
          }
        }
      }

      public abstract class WhenRegisteringAThenAlwaysHandler : WhenUsingAPromise
      {
        public object Args;

        protected override void EstablishContext()
        {
          base.EstablishContext();
          Args = new object();
        }

        protected override void Because()
        {
          base.Because();
          PromiseUnderTest.ThenAlways(Consumer.Object.ThenAlwaysHandler);
        }

        public abstract class WhenTheDeferredIsResolvedOrRejected : WhenRegisteringAThenAlwaysHandler
        {
          [Test]
          public void TheAlwaysHandlerShouldBeInvoked()
          {
            Consumer.Verify(x => x.ThenAlwaysHandler(ref Args), Times.Once);
          }

          public class WhenTheDeferredIsResolved : WhenTheDeferredIsResolvedOrRejected
          {
            protected override void Because()
            {
              base.Because();
              SystemUnderTest.Resolve(Args);
            }

            public class WhenRegisteringAThenAlwaysHandlerWhenTheDeferredIsResolved : WhenTheDeferredIsResolved
            {
              public Mock<IPromiseComsumer> Consumer2;

              private Exception _exception;

              protected override void EstablishContext()
              {
                base.EstablishContext();
                Consumer2 = new Mock<IPromiseComsumer>();
              }

              protected override void Because()
              {
                base.Because();
                try
                {
                  SystemUnderTest.ThenAlways(Consumer2.Object.ThenAlwaysHandler);
                }
                catch (Exception e)
                {
                  _exception = e;
                }
              }

              [Test]
              public void AResolvedDeferredExceptionShouldBeThrown()
              {
                Assert.IsInstanceOf<ResolvedDeferredException>(_exception);
              }

              [Test]
              public void Consumer2ShouldNotBeInvoked()
              {
                Consumer2.Verify(x => x.ThenAlwaysHandler(ref Args), Times.Never);
              }
            }
          }

          public class WhenTheDeferredIsRejected : WhenTheDeferredIsResolvedOrRejected
          {
            protected override void Because()
            {
              base.Because();
              SystemUnderTest.Reject(Args);
            }

            public class WhenRegisteringAThenAlwaysHandlerWhenTheDeferredIsRejected : WhenTheDeferredIsRejected
            {
              public Mock<IPromiseComsumer> Consumer2;

              private Exception _exception;

              protected override void EstablishContext()
              {
                base.EstablishContext();
                Consumer2 = new Mock<IPromiseComsumer>();
              }

              protected override void Because()
              {
                base.Because();
                try
                {
                  SystemUnderTest.ThenAlways(Consumer2.Object.ThenAlwaysHandler);
                }
                catch (Exception e)
                {
                  _exception = e;
                }
              }

              [Test]
              public void AResolvedDeferredExceptionShouldBeThrown()
              {
                Assert.IsInstanceOf<RejectedDeferredException>(_exception);
              }

              [Test]
              public void Consumer2ShouldNotBeInvoked()
              {
                Consumer2.Verify(x => x.ThenAlwaysHandler(ref Args), Times.Never);
              }
            }
          }
        }
      }

      public abstract class WhenRegisteringAThenDoneHandler : WhenUsingAPromise
      {
        public object Args;

        protected override void EstablishContext()
        {
          base.EstablishContext();
          Args = new object();
        }

        protected override void Because()
        {
          base.Because();
          PromiseUnderTest.ThenDone(Consumer.Object.ThenDoneHandler);
        }

        public abstract class WhenTheDeferredIsResolvedOrRejected : WhenRegisteringAThenDoneHandler
        {
          public class WhenTheDeferredIsResolved : WhenTheDeferredIsResolvedOrRejected
          {
            protected override void Because()
            {
              base.Because();
              SystemUnderTest.Resolve(Args);
            }

            [Test]
            public void TheThenDoneHandlerShouldBeInvoked()
            {
              Consumer.Verify(x => x.ThenDoneHandler(ref Args), Times.Once);
            }

            public class WhenRegisteringAThenAlwaysHandlerWhenTheDeferredIsResolved : WhenTheDeferredIsResolved
            {
              public Mock<IPromiseComsumer> Consumer2;

              private Exception _exception;

              protected override void EstablishContext()
              {
                base.EstablishContext();
                Consumer2 = new Mock<IPromiseComsumer>();
              }

              protected override void Because()
              {
                base.Because();
                try
                {
                  SystemUnderTest.ThenAlways(Consumer2.Object.ThenAlwaysHandler);
                }
                catch (Exception e)
                {
                  _exception = e;
                }
              }

              [Test]
              public void AResolvedDeferredExceptionShouldBeThrown()
              {
                Assert.IsInstanceOf<ResolvedDeferredException>(_exception);
              }

              [Test]
              public void Consumer2ShouldNotBeInvoked()
              {
                Consumer2.Verify(x => x.ThenAlwaysHandler(ref Args), Times.Never);
              }
            }
          }

          public class WhenTheDeferredIsRejected : WhenTheDeferredIsResolvedOrRejected
          {
            protected override void Because()
            {
              base.Because();
              SystemUnderTest.Reject(Args);
            }

            public class WhenRegisteringAThenAlwaysHandlerWhenTheDeferredIsRejected : WhenTheDeferredIsRejected
            {
              public Mock<IPromiseComsumer> Consumer2;

              private Exception _exception;

              protected override void EstablishContext()
              {
                base.EstablishContext();
                Consumer2 = new Mock<IPromiseComsumer>();
              }

              protected override void Because()
              {
                base.Because();
                try
                {
                  SystemUnderTest.ThenAlways(Consumer2.Object.ThenAlwaysHandler);
                }
                catch (Exception e)
                {
                  _exception = e;
                }
              }

              [Test]
              public void AResolvedDeferredExceptionShouldBeThrown()
              {
                Assert.IsInstanceOf<RejectedDeferredException>(_exception);
              }

              [Test]
              public void Consumer2ShouldNotBeInvoked()
              {
                Consumer2.Verify(x => x.ThenAlwaysHandler(ref Args), Times.Never);
              }
            }
          }
        }
      }

      public abstract class WhenRegisteringAThenFailHandler : WhenUsingAPromise
      {
        public object Args;

        protected override void EstablishContext()
        {
          base.EstablishContext();
          Args = new object();
        }

        protected override void Because()
        {
          base.Because();
          PromiseUnderTest.ThenFail(Consumer.Object.ThenFailHandler);
        }

        public abstract class WhenTheDeferredIsResolvedOrRejected : WhenRegisteringAThenFailHandler
        {
          [Test]
          public void TheThenFailHandlerShouldBeInvoked()
          {
            Consumer.Verify(x => x.ThenFailHandler(ref Args), Times.Once);
          }

          public class WhenTheDeferredIsRejected : WhenTheDeferredIsResolvedOrRejected
          {
            protected override void Because()
            {
              base.Because();
              SystemUnderTest.Reject(Args);
            }

            public class WhenRegisteringAThenFailHandlerWhenTheDeferredIsRejected : WhenTheDeferredIsRejected
            {
              public Mock<IPromiseComsumer> Consumer2;

              private Exception _exception;

              protected override void EstablishContext()
              {
                base.EstablishContext();
                Consumer2 = new Mock<IPromiseComsumer>();
              }

              protected override void Because()
              {
                base.Because();
                try
                {
                  SystemUnderTest.ThenFail(Consumer2.Object.ThenFailHandler);
                }
                catch (Exception e)
                {
                  _exception = e;
                }
              }

              [Test]
              public void ARejectedDeferredExceptionShouldBeThrown()
              {
                Assert.IsInstanceOf<RejectedDeferredException>(_exception);
              }

              [Test]
              public void Consumer2ShouldNotBeInvoked()
              {
                Consumer2.Verify(x => x.ThenFailHandler(ref Args), Times.Never);
              }
            }
          }

          public class WhenTheDeferredIsResolved : WhenTheDeferredIsResolvedOrRejected
          {
            protected override void Because()
            {
              base.Because();
              SystemUnderTest.Reject(Args);
            }

            public class WhenRegisteringAThenFailHandlerWhenTheDeferredIsRejected : WhenTheDeferredIsResolved
            {
              public Mock<IPromiseComsumer> Consumer2;

              private Exception _exception;

              protected override void EstablishContext()
              {
                base.EstablishContext();
                Consumer2 = new Mock<IPromiseComsumer>();
              }

              protected override void Because()
              {
                base.Because();
                try
                {
                  SystemUnderTest.ThenFail(Consumer2.Object.ThenFailHandler);
                }
                catch (Exception e)
                {
                  _exception = e;
                }
              }

              [Test]
              public void ARejectedDeferredExceptionShouldBeThrown()
              {
                Assert.IsInstanceOf<RejectedDeferredException>(_exception);
              }

              [Test]
              public void Consumer2ShouldNotBeInvoked()
              {
                Consumer2.Verify(x => x.ThenAlwaysHandler(ref Args), Times.Never);
              }
            }
          }
        }
      }
    }
  }
}
