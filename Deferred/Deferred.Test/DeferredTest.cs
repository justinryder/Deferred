using System;
using Moq;
using NUnit.Framework;

namespace Deferred.Test
{
  public interface ITestHandlerObject
  {
    void Handler(object sender, EventArgs args);
  }

  [TestFixture]
  public abstract class DeferredTest : TestBase<IDeferred>
  {
    protected override IDeferred CreateSystemUnderTest()
    {
      return new Deferred();
    }

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
        public Mock<ITestHandlerObject> DoneHandler1 { get; private set; }
        public EventArgs DoneEventArgs1 { get; private set; }

        protected override void EstablishContext()
        {
          base.EstablishContext();
          DoneHandler1 = new Mock<ITestHandlerObject>();
          DoneEventArgs1 = new EventArgs();
        }

        protected override void Because()
        {
          base.Because();
          PromiseUnderTest.Done(DoneHandler1.Object.Handler);
        }

        public class WhenTheDeferredIsResolved : WhenRegisteringADoneHandler
        {
          protected override void Because()
          {
            base.Because();
            SystemUnderTest.Resolve(null);
          }

          [Test]
          public void TheDoneHandlerShouldBeInvoked()
          {
            DoneHandler1.Verify(x => x.Handler(PromiseUnderTest, It.IsAny<EventArgs>()), Times.Once);
          }

          public class WhenAnotherDoneHandlerIsRegistered : WhenTheDeferredIsResolved
          {
            public Mock<ITestHandlerObject> DoneHandler2 { get; private set; }
            public EventArgs DoneEventArgs2 { get; private set; }

            protected override void EstablishContext()
            {
              base.EstablishContext();
              DoneHandler2 = new Mock<ITestHandlerObject>();
              DoneEventArgs2 = new EventArgs();
            }

            protected override void Because()
            {
              base.Because();
              SystemUnderTest.Done(DoneHandler2.Object.Handler);
            }

            [Test]
            public void TheSecondDoneHandlerShouldBeInvoked()
            {
              DoneHandler2.Verify(x => x.Handler(PromiseUnderTest, It.IsAny<EventArgs>()), Times.Once);
            }

            [Test]
            public void TheFirstDoneHandlerShouldNotBeInvokedTwice()
            {
              DoneHandler1.Verify(x => x.Handler(PromiseUnderTest, It.IsAny<EventArgs>()), Times.Once);
            }
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
            DoneHandler1.Verify(x => x.Handler(PromiseUnderTest, It.IsAny<EventArgs>()), Times.Never);
          }
        }
      }

      public abstract class WhenRegisteringAFailHandler : WhenUsingAPromise
      {
        public Mock<ITestHandlerObject> FailHandler1 { get; private set; }
        public EventArgs FailEventArgs1 { get; private set; }

        protected override void EstablishContext()
        {
          base.EstablishContext();
          FailHandler1 = new Mock<ITestHandlerObject>();
          FailEventArgs1 = new EventArgs();
        }

        protected override void Because()
        {
          base.Because();
          PromiseUnderTest.Fail(FailHandler1.Object.Handler);
        }

        public class WhenTheDeferredIsRejected : WhenRegisteringAFailHandler
        {
          protected override void Because()
          {
            base.Because();
            SystemUnderTest.Reject(FailEventArgs1);
          }

          [Test]
          public void TheFailHandlerShouldBeInvoked()
          {
            FailHandler1.Verify(x => x.Handler(PromiseUnderTest, It.IsAny<EventArgs>()), Times.Once);
          }

          public class WhenAnotherFailHandlerIsRegistered : WhenTheDeferredIsRejected
          {
            public Mock<ITestHandlerObject> FailHandler2 { get; private set; }
            public EventArgs FailEventArgs2 { get; private set; }

            protected override void EstablishContext()
            {
              base.EstablishContext();
              FailHandler2 = new Mock<ITestHandlerObject>();
              FailEventArgs2 = new EventArgs();
            }

            protected override void Because()
            {
              base.Because();
              SystemUnderTest.Fail(FailHandler2.Object.Handler);
            }

            [Test]
            public void TheSecondFailHandlerShouldBeInvoked()
            {
              FailHandler2.Verify(x => x.Handler(PromiseUnderTest, It.IsAny<EventArgs>()), Times.Once);
            }

            [Test]
            public void TheFirstFailHandlerShouldNotBeInvokedTwice()
            {
              FailHandler1.Verify(x => x.Handler(PromiseUnderTest, FailEventArgs1), Times.Once);
              FailHandler1.Verify(x => x.Handler(PromiseUnderTest, FailEventArgs2), Times.Never);
            }
          }
        }

        public class WhenTheDeferredIsResolved : WhenRegisteringAFailHandler
        {
          protected override void Because()
          {
            SystemUnderTest.Resolve(FailEventArgs1);
          }

          [Test]
          public void TheFailHandlerShouldNotBeInvoked()
          {
            FailHandler1.Verify(x => x.Handler(PromiseUnderTest, FailEventArgs1), Times.Never);
          }
        }
      }

      public abstract class WhenRegisteringAnAlwaysHandler : WhenUsingAPromise
      {
        public Mock<ITestHandlerObject> AlwaysHandler1 { get; private set; }
        public EventArgs AlwaysEventArgs1 { get; private set; }

        protected override void EstablishContext()
        {
          base.EstablishContext();
          AlwaysHandler1 = new Mock<ITestHandlerObject>();
          AlwaysEventArgs1 = new EventArgs();
        }

        protected override void Because()
        {
          base.Because();
          PromiseUnderTest.Always(AlwaysHandler1.Object.Handler);
        }

        public abstract class WhenTheDeferredIsResolvedOrRejected : WhenRegisteringAnAlwaysHandler
        {
          [Test]
          public void TheAlwaysHandlerShouldBeInvoked()
          {
            AlwaysHandler1.Verify(x => x.Handler(PromiseUnderTest, It.IsAny<EventArgs>()), Times.Once);
          }

          public class WhenTheDeferredIsResolved : WhenTheDeferredIsResolvedOrRejected
          {
            protected override void Because()
            {
              base.Because();
              SystemUnderTest.Resolve(AlwaysEventArgs1);
            }

            public class WhenTheDeferredIsResolvedAgain : WhenTheDeferredIsResolved
            {
              protected override void Because()
              {
                base.Because();
                SystemUnderTest.Resolve(AlwaysEventArgs1);
              }
            }

            public class WhenTheDeferredIsAlsoRejected : WhenTheDeferredIsResolved
            {
              protected override void Because()
              {
                base.Because();
                SystemUnderTest.Reject(AlwaysEventArgs1);
              }
            }
          }

          public class WhenTheDeferredIsRejected : WhenTheDeferredIsResolvedOrRejected
          {
            protected override void Because()
            {
              base.Because();
              SystemUnderTest.Reject(AlwaysEventArgs1);
            }

            public class WhenTheDeferredIsRejectedAgain : WhenTheDeferredIsRejected
            {
              protected override void Because()
              {
                base.Because();
                SystemUnderTest.Reject(It.IsAny<EventArgs>());
              }
            }

            public class WhenTheDeferredIsAlsoResolved : WhenTheDeferredIsRejected
            {
              protected override void Because()
              {
                base.Because();
                SystemUnderTest.Resolve(AlwaysEventArgs1);
              }
            }
          }
        }
      }
    }
  }
}
