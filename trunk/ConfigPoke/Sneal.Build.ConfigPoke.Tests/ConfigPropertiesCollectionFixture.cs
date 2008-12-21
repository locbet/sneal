using NUnit.Framework;
using Rhino.Mocks;

namespace Sneal.Build.ConfigPoke.Tests
{
    [TestFixture]
    public class ConfigPropertiesCollectionFixture
    {
        [Test]
        public void Should_allow_empty_string_as_value()
        {
            var reader = MockRepository.GenerateStub<IConfigPropertiesReader>();
            reader.Stub(r => r.ReadLine()).Return("KeyWithEmptyValue=").Repeat.Once();
            reader.Stub(r => r.ReadLine()).Return(null);

            var properties = new ConfigPropertiesCollection();
            properties.AddPropertiesFromReader(reader);

            Assert.AreEqual("", properties["KeyWithEmptyValue"]);
        }
    }
}
