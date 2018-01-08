// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations;

namespace TestBizLayer.DbForTransactions
{
    public class LogEntry
    {
        public LogEntry()
        {
            WhenCreated = DateTime.Now;
        }

        public LogEntry(string logMessage) : this()
        {
            LogText = logMessage;
        }

        public int LogEntryId { get; set; }

        [Required]
        [MinLength(2)]
        public string LogText { get; set; }

        public DateTime WhenCreated { get; private set; }

        public override string ToString()
        {
            return string.Format("LogEntryId: {0}, LogText: {1}, WhenCreated: {2}", LogEntryId, LogText, WhenCreated);
        }
    }
}