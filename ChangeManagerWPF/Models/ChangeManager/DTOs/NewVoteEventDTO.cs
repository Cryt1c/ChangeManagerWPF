using System;
using System.Threading.Tasks;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;
namespace ChangeManager.Contracts.ChangeManager.DTOs
{
    [Event("NewVote")]
    public class NewVoteEventDTO
    {
        [Parameter("address", "_voter", 1, false )]
        public string Voter {get; set;}
        [Parameter("bool", "_vote", 2, false )]
        public bool Vote {get; set;}
        [Parameter("uint8", "_currentState", 3, false )]
        public byte CurrentState {get; set;}
        [Parameter("string", "_voteInfo", 4, false )]
        public string VoteInfo {get; set;}
        [Parameter("uint256", "_votesLeft", 5, false )]
        public BigInteger VotesLeft {get; set;}
    }
}
