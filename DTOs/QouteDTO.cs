using projectWebApplication.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projectWebApplication.DTOs
{
    public class QouteDTO
    {
        public long QouteId { get; set; }
        public string QouteText { get; set; }
        public string QouteImg { get; set; }
        public string FileName { get; set; }
        public bool IsPublished { get; set; }
        public bool IsFeaturedOnHomePage { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public string StorageSize { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public byte[] ImgBytes { get; set; }

        public OutputHandler OutputHandler { get; set; }
    }
}
