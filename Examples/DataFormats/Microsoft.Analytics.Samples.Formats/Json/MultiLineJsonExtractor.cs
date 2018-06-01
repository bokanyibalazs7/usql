﻿using System;
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
        /// Initializes a new instance of the MultiLineJsonExtractor class.
        /// </summary>
        /// <param name="rowpath">Selector expression to select a collection of JSON fragments. Each fragment ought to promote one row in the result set. 
        /// Default: the type of the JSON root object determines: collection - this collection will be the fragment collection,
        /// single object - fragment collection containing only the root object (promotes single row)</param>
        /// <param name="compressByteArray">Indicates whether byte array columns hold the corresponding JSON fragment compressed. Deafult: no.</param>
        /// <param name="numOfDocsPerLine">The number of JSON document per line to parse. Default: the reader will process till the end of the line.</param>
        /// <param name="rowdelim">Row delimiting characters. Default: \r\n.</param>
        /// <param name="encoding">Encoding of row delimiter characters in the JSON input. Default: UTF-8. </param>
        public MultiLineJsonExtractor(string rowpath = null
            , bool compressByteArray = false
            , int? numOfDocsPerLine = null
            , string rowdelim = "\r\n"
            , Encoding encoding = null
           ) 
            : base(rowpath, compressByteArray, numOfDocsPerLine)
        {
            this.rowdelim = (encoding ?? Encoding.UTF8).GetBytes(rowdelim);
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
