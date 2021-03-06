﻿using System.Numerics;

namespace ChangeManagerWPF.Models
{
    class Vote
    {
        public string voter { get; set; }
        public string voteInfo { get; set; }
        public bool acceptChange { get; set; }
        public BigInteger timestamp { get; set; }
    }
}
