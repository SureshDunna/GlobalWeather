using System.IO;
using System.Threading.Tasks;
using System.Xml;
using GlobalWeatherApi.BusinessLogic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalWeatherApi.Tests.BusinessLogic
{
    [TestClass]
    public class XmlHelperTest
    {
        private readonly IXmlHelper _xmlHelper = new XmlHelper();

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public async Task DeserializeRoot_Must_Throw_Exception_If_SerializedString_Is_Empty()
        {
            await _xmlHelper.DeserializeRoot<object>(string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(XmlException))]
        public async Task DeserializeRoot_Must_Throw_Exception_If_SerializedString_Does_Not_Have_Root()
        {
            await _xmlHelper.DeserializeRoot<object>("Invalid Xml");
        }

        [TestMethod]
        public async Task DeserializeRoot_Must_Return_Response_Object()
        {
            var response = await _xmlHelper.DeserializeRoot<NewDataSet>("<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<string xmlns=\"http://www.webserviceX.NET\">&lt;NewDataSet&gt;\r\n  &lt;Table&gt;\r\n    &lt;Country&gt;India&lt;/Country&gt;\r\n    &lt;City&gt;Ahmadabad&lt;/City&gt;\r\n  &lt;/Table&gt;\r\n&lt;/NewDataSet&gt;</string>");

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Table);
            Assert.AreEqual(response.Table.Length, 1);
            Assert.AreEqual(response.Table[0].Country, "India");
            Assert.AreEqual(response.Table[0].City, "Ahmadabad");
        }
    }
}
