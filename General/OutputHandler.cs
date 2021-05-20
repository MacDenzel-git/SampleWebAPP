using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projectWebApplication.General
{
    public class OutputHandler
    {
        public OutputHandler()
        {
            this.IsErrorOccured = false;
        }
        public bool IsErrorOccured { get; set; }
        public string Message { get; set; }
        public object Result { get; set; }
        public bool IsErrorKnown { get; set; }
    }
}
