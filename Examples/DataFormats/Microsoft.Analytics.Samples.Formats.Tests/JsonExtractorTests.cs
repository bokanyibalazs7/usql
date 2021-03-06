﻿using Microsoft.Analytics.Interfaces;
using Microsoft.Analytics.UnitTest;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Analytics.Samples.Formats.Json;
using Newtonsoft.Json;

namespace Microsoft.Analytics.Samples.Formats.Tests
{
    [TestClass]
    public class JsonExtractorTests
    {
        private const int BUFFER_SIZE = 8192;

        public JsonExtractorTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }        

        public IRow SingleColumnRowGenerator<T>()
        {
            var foo = new USqlColumn<T>("Value");
            var columns = new List<IColumn> { foo };
            var schema = new USqlSchema(columns);
            return new USqlRow(schema, null);
        }

        [TestMethod]
        public void JsonExtractor_DatatypeInt_Extracted()
        {
            var data = new List<SingleColumnPoco<int>>
            {
                new SingleColumnPoco<int>() { Value = 1 },
                new SingleColumnPoco<int>() { Value = 0 },
            };

            var result = ExecuteExtract(data);

            Assert.IsTrue(result[0].Get<int>("Value") == 1);
            Assert.IsTrue(result[1].Get<int>("Value") == 0);
        }

        [TestMethod]
        public void JsonExtractor_DatatypeNullableInt_Extracted()
        {
            var data = new List<SingleColumnPoco<int?>>
            {
                new SingleColumnPoco<int?>() { Value = 1 },
                new SingleColumnPoco<int?>() { Value = null }
            };

            var result = ExecuteExtract(data);

            Assert.IsTrue(result[0].Get<int?>("Value") == 1);
            Assert.IsTrue(result[1].Get<int?>("Value") == null);
        }

        [TestMethod]
        public void JsonExtractor_DatatypeBoolean_Extracted()
        {
            var data = new List<SingleColumnPoco<bool>>
            {
                new SingleColumnPoco<bool>() { Value = true },
                new SingleColumnPoco<bool>() { Value = false }
            };

            var result = ExecuteExtract(data);

            Assert.IsTrue(result[0].Get<bool>("Value") == true);
            Assert.IsTrue(result[1].Get<bool>("Value") == false);
        }

        [TestMethod]
        public void JsonExtractor_DatatypeNullableBoolean_Extracted()
        {
            var data = new List<SingleColumnPoco<bool?>>
            {
                new SingleColumnPoco<bool?>() { Value = true },
                new SingleColumnPoco<bool?>() { Value = false },
                new SingleColumnPoco<bool?>() { Value = null }
            };

            var result = ExecuteExtract(data);

            Assert.IsTrue(result[0].Get<bool?>("Value") == true);
            Assert.IsTrue(result[1].Get<bool?>("Value") == false);
            Assert.IsTrue(result[2].Get<bool?>("Value") == null);
        }

        [TestMethod]
        public void JsonExtractor_DatatypeLong_Extracted()
        {
            var data = new List<SingleColumnPoco<long>>
            {
                new SingleColumnPoco<long>() { Value = 9223372036854775807 },
                new SingleColumnPoco<long>() { Value = -9223372036854775807 }
            };

            var result = ExecuteExtract(data);

            Assert.IsTrue(result[0].Get<long>("Value") == 9223372036854775807);
            Assert.IsTrue(result[1].Get<long>("Value") == -9223372036854775807);
        }

        [TestMethod]
        public void JsonExtractor_DatatypeNullableLong_Extracted()
        {
            var data = new List<SingleColumnPoco<long?>>
            {
                new SingleColumnPoco<long?>() { Value = 9223372036854775807 },
                new SingleColumnPoco<long?>() { Value = -9223372036854775807 },
                new SingleColumnPoco<long?>() { Value = null }
            };

            var result = ExecuteExtract(data);

            Assert.IsTrue(result[0].Get<long?>("Value") == 9223372036854775807);
            Assert.IsTrue(result[1].Get<long?>("Value") == -9223372036854775807);
            Assert.IsTrue(result[2].Get<long?>("Value") == null);
        }

        [TestMethod]
        public void JsonExtractor_DatatypeFloat_Extracted()
        {
            var data = new List<SingleColumnPoco<float>>
            {
                new SingleColumnPoco<float>() { Value = 3.5F},
                new SingleColumnPoco<float>() { Value = 0 }
            };

            var result = ExecuteExtract(data);

            Assert.IsTrue(result[0].Get<float>("Value") == 3.5F);
            Assert.IsTrue(result[1].Get<float>("Value") == 0);
        }

        [TestMethod]
        public void JsonExtractor_DatatypeNullableFloat_Extracted()
        {
            var data = new List<SingleColumnPoco<float?>>
            {
                new SingleColumnPoco<float?>() { Value = 3.5F},
                new SingleColumnPoco<float?>() { Value = 0 },
                new SingleColumnPoco<float?>() { Value = null }
            };

            var result = ExecuteExtract(data);

            Assert.IsTrue(result[0].Get<float?>("Value") == 3.5F);
            Assert.IsTrue(result[1].Get<float?>("Value") == 0);
            Assert.IsTrue(result[2].Get<float?>("Value") == null);
        }

        [TestMethod]
        public void JsonExtractor_DatatypeDouble_Extracted()
        {
            var data = new List<SingleColumnPoco<double>>
            {
                new SingleColumnPoco<double>() { Value = 3D},
                new SingleColumnPoco<double>() { Value = 0 }
            };

            var result = ExecuteExtract(data);

            Assert.IsTrue(result[0].Get<double>("Value") == 3D);
            Assert.IsTrue(result[1].Get<double>("Value") == 0);
        }

        [TestMethod]
        public void JsonExtractor_DatatypeNullableDouble_Extracted()
        {
            var data = new List<SingleColumnPoco<double?>>
            {
                new SingleColumnPoco<double?>() { Value = 3D},
                new SingleColumnPoco<double?>() { Value = 0 },
                new SingleColumnPoco<double?>() { Value = null }
            };

            var result = ExecuteExtract(data);

            Assert.IsTrue(result[0].Get<double?>("Value") == 3D);
            Assert.IsTrue(result[1].Get<double?>("Value") == 0);
            Assert.IsTrue(result[2].Get<double?>("Value") == null);
        }

        [TestMethod]
        public void JsonExtractor_DatatypeByte_Extracted()
        {
            byte[] bytes = { 2, 4, 6 };
            var data = new List<SingleColumnPoco<byte[]>>
            {
                new SingleColumnPoco<byte[]>() { Value = bytes }
            };

            var result = ExecuteExtract(data);

            Assert.IsTrue(result[0].Get<byte[]>("Value")[0] == 2);
        }
     

        [TestMethod]
        public void JsonExtractor_DatatypeNullableByte_Extracted()
        {
            byte[] bytes = { 2, 4, 6 };
            var data = new List<SingleColumnPoco<byte[]>>
            {
                new SingleColumnPoco<byte[]>() { Value = bytes },
                new SingleColumnPoco<byte[]>() { Value = null }
            };

            var result = ExecuteExtract(data);

            Assert.IsTrue(result[0].Get<byte[]>("Value")[0] == 2);
            Assert.IsTrue(result[1].Get<byte[]>("Value") == null);
        }

        [TestMethod]
        public void JsonExtractor_DataTypeSubJson_ByteArray()
        {
            byte[] bytes = { 2, 4, 6 };
            var subdata = new SingleColumnPoco<string> { Value = "foo" };
            var data = new List<SingleColumnPoco<SingleColumnPoco<string>>>
            {
                new SingleColumnPoco<SingleColumnPoco<string>>() { Value= subdata },                
            };

            var result = ExecuteExtract(data, byteArrayProjectionMode:JsonExtractor.ByteArrayProjectionMode.BytesString);
            
            string resultString = Encoding.UTF8.GetString(result[0].Get<byte[]>("Value"));
            SingleColumnPoco<string> resultPoco = JsonConvert.DeserializeObject<SingleColumnPoco<string>>(resultString);
            Assert.IsTrue(subdata.Equals(resultPoco));
        }


        [TestMethod]
        public void JsonExtractor_DatatypeString_Extracted()
        {
            var data = new List<SingleColumnPoco<string>>
            {
                new SingleColumnPoco<string>() { Value = "asdf" },
                new SingleColumnPoco<string>() { Value = "" }
            };

            var result = ExecuteExtract(data);

            Assert.IsTrue(result[0].Get<string>("Value") == "asdf");
            Assert.IsTrue(result[1].Get<string>("Value") == "");
        }

        [TestMethod]
        public void JsonExtractor_DatatypeString_Extracted_C()
        {
            var data = new List<SingleColumnPoco<string>>
            {
                new SingleColumnPoco<string>() { Value = "asdf" },
                new SingleColumnPoco<string>() { Value = "foo" }
            };

            var result = ExecuteExtract(data, byteArrayProjectionMode:JsonExtractor.ByteArrayProjectionMode.BytesStringCompressed);

            Assert.IsTrue(Encoding.UTF8.GetString(DecompressByteArray(result[0].Get<byte[]>("Value"))) == "asdf");
            Assert.IsTrue(Encoding.UTF8.GetString(DecompressByteArray(result[1].Get<byte[]>("Value"))) == "foo");
        }


        [TestMethod]
        public void JsonExtractor_DatatypeString_Extracted_OrphanOpenArrayFront()
        {
            var data = new List<SingleColumnPoco<string>>
            {
                new SingleColumnPoco<string>() { Value = "asdf" },
                new SingleColumnPoco<string>() { Value = "" }
            };

            var result = ExecuteExtract(data, convertJsonString: j=>"["+j);

            Assert.IsTrue(result[0].Get<string>("Value") == "asdf");
            Assert.IsTrue(result[1].Get<string>("Value") == "");
        }

        [TestMethod]
        public void JsonExtractor_DatatypeString_Extracted_ExtraCommaAtEnd()
        {
            var data = new List<SingleColumnPoco<string>>
            {
                new SingleColumnPoco<string>() { Value = "asdf" },
                new SingleColumnPoco<string>() { Value = "" }
            };

            var result = ExecuteExtract(data, convertJsonString: j => j+",", numOfDocs:data.Count);

            Assert.IsTrue(result[0].Get<string>("Value") == "asdf");
            Assert.IsTrue(result[1].Get<string>("Value") == "");
        }

        [TestMethod]
        public void JsonExtractor_DatatypeString_Extracted_OrphanEndArrayAtEnd()
        {
            var data = new List<SingleColumnPoco<string>>
            {
                new SingleColumnPoco<string>() { Value = "asdf" },
                new SingleColumnPoco<string>() { Value = "" }
            };

            var result = ExecuteExtract(data, convertJsonString: j => j + "]", numOfDocs: data.Count);

            Assert.IsTrue(result[0].Get<string>("Value") == "asdf");
            Assert.IsTrue(result[1].Get<string>("Value") == "");
        }

        [TestMethod]
        public void MultiLineJsonExtractor_DatatypeString_Extracted_LineBreakAfterComma()
        {
            var data = new List<SingleColumnPoco<string>>
            {
                new SingleColumnPoco<string>() { Value = "asdf" },
                new SingleColumnPoco<string>() { Value = "foo" },
                new SingleColumnPoco<string>() { Value = "bar" }
            };
            string searchStr = "},";
            var result = ExecuteExtract(data
            , convertJsonString: j => j.Replace(searchStr, searchStr + "\r\n")
            , numOfDocs: 1
            , multiline:true);

            Assert.IsTrue(result[0].Get<string>("Value") == "asdf");
            Assert.IsTrue(result[1].Get<string>("Value") == "foo");
            Assert.IsTrue(result[2].Get<string>("Value") == "bar");
        }

        [TestMethod]
        public void MultiLineJsonExtractor_DatatypeString_Extracted_LineBreakAfterComma_RowDelim()
        {
            var data = new List<SingleColumnPoco<string>>
            {
                new SingleColumnPoco<string>() { Value = "asdf" },
                new SingleColumnPoco<string>() { Value = "foo" },
                new SingleColumnPoco<string>() { Value = "bar" }
            };
            string searchStr = "},";
            var result = ExecuteExtract(data
            , convertJsonString: j => j.Replace(searchStr, searchStr + "\r\n")
            , numOfDocs: 1
            , multiline: true
            , rowdelim: ",\r\n");

            Assert.IsTrue(result[0].Get<string>("Value") == "asdf");
            Assert.IsTrue(result[1].Get<string>("Value") == "foo");
            Assert.IsTrue(result[2].Get<string>("Value") == "bar");
        }        

        [TestMethod]
        public void MultiLineJsonExtractor_DatatypeString_Extracted_ArrayLineBreaks()
        {
            var data = new List<SingleColumnPoco<string>>
            {
                new SingleColumnPoco<string>() { Value = "asdf" },
                new SingleColumnPoco<string>() { Value = "foo" },
                new SingleColumnPoco<string>() { Value = "bar" }
            };           
            var result = ExecuteExtract(data
            , convertJsonString: j => j.Replace("[","[\r\n").Replace("]","\r\n]").Replace("},", "},\r\n")
            , numOfDocs: 1
            , multiline: true
            , skipMalformedObjects:true);

            Assert.IsTrue(result[0].Get<string>("Value") == "asdf");
            Assert.IsTrue(result[1].Get<string>("Value") == "foo");
            Assert.IsTrue(result[2].Get<string>("Value") == "bar");
        }

        [TestMethod]
        public void JsonExtractor_DatatypeNullableString_Extracted()
        {
            var data = new List<SingleColumnPoco<string>>
            {
                new SingleColumnPoco<string>() { Value = "asdf" },
                new SingleColumnPoco<string>() { Value = null }
            };

            var result = ExecuteExtract(data);

            Assert.IsTrue(result[0].Get<string>("Value") == "asdf");
            Assert.IsTrue(result[1].Get<string>("Value") == null);
        }

        [TestMethod]
        public void JsonExtractor_EmptyFile_ReturnNoRow()
        {
            var data = new List<SingleColumnPoco<string>>();

            var result = ExecuteExtract(data);

            Assert.IsTrue(result.Count == 0);
        }


        private IList<IRow> ExecuteExtract<T>(List<SingleColumnPoco<T>> data
            , JsonExtractor.ByteArrayProjectionMode byteArrayProjectionMode = JsonExtractor.ByteArrayProjectionMode.Normal
            , Func<string,string> convertJsonString = null
            , int? numOfDocs=null
            , bool multiline = false
            , string rowdelim = "\r\n"
            , bool skipMalformedObjects = false)
        {
            var output = (byteArrayProjectionMode == JsonExtractor.ByteArrayProjectionMode.Normal 
                ? SingleColumnRowGenerator<T>() 
                : SingleColumnRowGenerator<byte[]>())
                .AsUpdatable();

            var jsonString = JsonConvert.SerializeObject(data);
            if (convertJsonString != null)
                jsonString = convertJsonString(jsonString);
            using (var dataStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
            {
                var reader = new USqlStreamReader(dataStream);
                var extractor = 
                    multiline ? new MultiLineJsonExtractor(linedelim: rowdelim, byteArrayProjectionMode: byteArrayProjectionMode, numOfDocsPerLine: numOfDocs, skipMalformedObjects: skipMalformedObjects)
                    : new JsonExtractor(byteArrayProjectionMode: byteArrayProjectionMode, numOfDocs:numOfDocs, skipMalformedObjects : skipMalformedObjects);
                return extractor.Extract(reader, output).ToList();
            }
        }

        private static byte[] DecompressByteArray(byte[] input)
        {
            using (var compressedMs = new MemoryStream(input))
            using (var decompressedMs = new MemoryStream())
            using (var gzs = new GZipStream(compressedMs, CompressionMode.Decompress))
            {
                gzs.CopyTo(decompressedMs);
                return decompressedMs.ToArray();
            }
        }



    }
}