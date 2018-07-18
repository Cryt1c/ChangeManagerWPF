using System;
using System.Threading.Tasks;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.Contracts.CQS;
using Nethereum.ABI.FunctionEncoding.Attributes;
using ChangeManager.Contracts.ChangeManager.DTOs;
namespace ChangeManager.Contracts.ChangeManager.CQS
{
    [Function("responsibleVote")]
    public class ResponsibleVoteFunction:ContractMessage
    {
        [Parameter("bytes20", "gitHash", 1)]
        public byte[] GitHash {get; set;}
        [Parameter("bool", "acceptChange", 2)]
        public bool AcceptChange {get; set;}
        [Parameter("string", "voteInfo", 3)]
        public string VoteInfo {get; set;}
    }
}
