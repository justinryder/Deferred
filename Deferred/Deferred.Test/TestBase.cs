using NUnit.Framework;

namespace Deferred.Test
{
  [TestFixture]
  public abstract class TestBase<SUT>
  {
    public SUT SystemUnderTest { get; private set; }

    [TestFixtureSetUp]
    public void Setup()
    {
      SystemUnderTest = CreateSystemUnderTest();
      EstablishContext();
      Because();
    }

    protected abstract SUT CreateSystemUnderTest();

    protected virtual void EstablishContext()
    {
    }

    protected virtual void Because()
    {
    }
  }
}