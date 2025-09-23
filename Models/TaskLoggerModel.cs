using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskLoggerDotNet.Models
{
    public class TaskLoggerModel
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public string DateDone { get; set; }
        public string TotalHours { get; set; }


    }
}