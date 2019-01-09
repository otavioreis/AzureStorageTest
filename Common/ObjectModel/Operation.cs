using System;
using Services.Table.ObjectModel;

namespace Common.ObjectModel
{
    public class Operation : TableEntityBase
    {
        public Operation()
        {
        }

        public Operation(string messageId, DateTimeOffset? nextVisibleTime, string messageContent, string popReceipt) 
            : base(Guid.Parse(messageId), nextVisibleTime?.DateTime)
        {
            this.MessageContent = messageContent;
            this.Success = false;
            this.Processed = false;
            this.TryCount = 0;
            this.NextVisibleTime = nextVisibleTime;
            this.PopReceipt = popReceipt;
        }

        public string MessageContent { get; set; }
        public bool Success { get; set; }
        public bool Processed { get; set; }
        public DateTimeOffset? NextVisibleTime { get; set; }
        public int TryCount { get; set; }
        public string PopReceipt { get; set; }

    }
}
