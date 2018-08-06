using System;
using System.Threading.Tasks;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.Contracts.CQS;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Contracts.Contracts.ChangeManager.DTOs;
namespace Contracts.Contracts.ChangeManager.CQS
{
    [Function("managementVote")]
    public class ManagementVoteFunction:ContractMessage
    {
        [Parameter("bytes20", "gitHash", 1)]
        public byte[] GitHash {get; set;}
        [Parameter("bool", "acceptChange", 2)]
        public bool AcceptChange {get; set;}
        [Parameter("address[]", "responsibleParties", 3)]
        public List<string> ResponsibleParties {get; set;}
        [Parameter("string", "voteInfo", 4)]
        public string VoteInfo {get; set;}
    }
}
