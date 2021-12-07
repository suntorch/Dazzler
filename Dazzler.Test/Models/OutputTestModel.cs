using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Dazzler.Test.Models
{
    public class OutputTestModel
    {
        /// <summary>
        ///  input parameter.
        /// </summary>
        public string input { get; set; }

        /// <summary>
        /// output parameter.
        /// </summary>
        [Bind(ParameterDirection.Output, 200)]
        public string output { get; set; }
    }
}
