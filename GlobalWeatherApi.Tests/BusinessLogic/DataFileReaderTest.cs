using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using GlobalWeatherApi.BusinessLogic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalWeatherApi.Tests.BusinessLogic
{
    [TestClass]
    public class DataFileReaderTest
    {
        private readonly IDataFileReader _dataFileReader = new DataFileReader();

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public async Task DeserializeObjectJsonDataFile_Must_Throw_Exception_If_FileName_Is_Empty_Or_Null()
        {
            await _dataFileReader.DeserializeObjectJsonDataFile<object>(string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public async Task DeserializeObjectJsonDataFile_Must_Throw_Exception_If_FileName_Is_NotFound()
        {
            await _dataFileReader.DeserializeObjectJsonDataFile<object>("invalidFileName.json");
        }

        [TestMethod]
        public async Task DeserializeObjectJsonDataFile_Must_Return_Countries_Object_If_FileName_Is_Valid()
        {
            var response = await _dataFileReader.DeserializeObjectJsonDataFile<List<SampleClass>>(Path.Combine(Directory.GetCurrentDirectory(), @"BusinessLogic\sample.json"));

            Assert.IsNotNull(response);
            Assert.AreEqual(response.Count, 2);
            Assert.AreEqual(response[0].Key, "keyName1");
            Assert.AreEqual(response[0].Value, "value1");
            Assert.AreEqual(response[1].Key, "keyName2");
            Assert.AreEqual(response[1].Value, "value2");
        }
    }

    public class SampleClass
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
