// 
// Copyright (c) Microsoft and contributors.  All rights reserved.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// 
// See the License for the specific language governing permissions and
// limitations under the License.
// 

using System.Collections;
using System.IO;
using Microsoft.Analytics.Interfaces;
using Newtonsoft.Json;

namespace Microsoft.Analytics.Samples.Formats.Json
{
    /// <summary>
    /// JsonOutputter (sample)
    ///
    ///     IEnumerable[IRow] =>
    ///         { c1:r1v1, c2:r1v2, ...}
    ///         { c1:r2v2, c2:r2v2, ...} 
    ///         ...
    /// </summary>
    [SqlUserDefinedOutputter(AtomicFileProcessing=false)]
    public class JsonLinesOutputter : IOutputter
    {
        /// <summary/>
        private JsonTextWriter writer;

        /// <summary/>
        public JsonLinesOutputter()
        {
        }
        
        /// <summary/>
        public override void                    Output(IRow row, IUnstructuredWriter output)
        {
            // First Row
            if(this.writer == null)
            {
                // Json.Net (writer)
                this.writer = new JsonTextWriter(new StreamWriter(output.BaseStream));                
            }

            // Row(s)
            WriteRow(row, this.writer);
        }
        
        /// <summary/>
        public override void                    Close()
        {
            if(this.writer != null)
            {
                this.writer.Close();
            }
        }
        
        /// <summary/>
        private static void                     WriteRow(IRow row, JsonTextWriter writer)
        {
            // Row
            //  => { c1:v1, c2:v2, ...}

            // Header
            writer.WriteStartObject();

            // Fields
            var columns = row.Schema;
            for(int i=0; i<columns.Count; i++)
            {
                // Note: We simply delegate to Json.Net for all data conversions
                //  For data conversions beyond what Json.Net supports, do an explicit projection:
                //      ie: SELECT datetime.ToString(...) AS datetime, ...
                object value = row.Get<object>(i);

                // Note: We don't bloat the JSON with sparse (null) properties
                if(value != null)
                {
                    writer.WritePropertyName(columns[i].Name, escape:true);
                    WriteValue(writer, value);
                }
            }

            // Footer
            writer.WriteEndObject();
        }

        private static void WriteValue(JsonTextWriter writer, object value)
        {
            IEnumerable collection = value as IEnumerable;
            if (collection != null && !(collection is string))
            {
                writer.WriteStartArray();
                foreach (var item in collection)
                {
                    WriteValue(writer, item);
                }
                writer.WriteEndArray();
            }
            else
                writer.WriteValue(value);
        }
    }
}
