using System;
using System.Collections.Generic;

namespace Influencers.Models
{
    public partial class Email
    {
        public int Id { get; set; }
        public string Receiver { get; set; }
        public string Body { get; set; }
        public string Subject { get; set; }
        public string SecurityCode { get; set; }
    }
}
