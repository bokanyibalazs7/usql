using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Analytics.Interfaces;

namespace Microsoft.Analytics.Samples.Formats.Json
{
    [SqlUserDefinedExtractor(AtomicFileProcessing = false)]
    public class MultiLineJsonExtractor : JsonExtractor
    {

        private readonly byte[] rowdelim;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiLineJsonExtractor"/> class.
        /// </summary>
        /// <param name="rowpath">Selector expression to select a collection of JSON fragments. Each fragment ought to promote one row in the result set. 
        /// Default: the type of the JSON root object determines: collection - this collection will be the fragment collection,
        /// single object - fragment collection containing only the root object (promotes single row)</param>
        /// <param name="byteArrayProjectionMode">Controls how to assign value to byte array columns.</param>
        /// <param name="numOfDocsPerLine">The number of JSON documents per line to parse. Default: the reader will process till the end of the line.</param>
        /// <param name="linedelim">Line delimiting characters. Default: \r\n.</param>
        /// <param name="encoding">Encoding of row delimiter characters in the JSON input. Default: UTF-8. </param>
        /// <param name="skipMalformedObjects">Indicates whether to silently skip malformed JSON objects. Default: false.</param>
        public MultiLineJsonExtractor(string rowpath = null
            , ByteArrayProjectionMode byteArrayProjectionMode = ByteArrayProjectionMode.Normal
            , int? numOfDocsPerLine = null
            , string linedelim = "\r\n"
            , Encoding encoding = null
            , bool skipMalformedObjects = false
           ) 
            : base(rowpath, byteArrayProjectionMode, numOfDocsPerLine, skipMalformedObjects)
        {
            this.rowdelim = (encoding ?? Encoding.UTF8).GetBytes(linedelim);
        }

        public override IEnumerable<IRow> Extract(IUnstructuredReader input, IUpdatableRow output)
        {
            foreach (Stream currentline in input.Split(this.rowdelim))
            {
                foreach (var row in base.ExtractCore(currentline, output))
                    yield return row;
            }
        }
    }
}
