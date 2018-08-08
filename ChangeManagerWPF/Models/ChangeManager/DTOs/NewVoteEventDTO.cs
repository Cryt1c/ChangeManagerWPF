using System;
using System.Threading.Tasks;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;
namespace Contracts.Contracts.ChangeManager.DTOs
{
    [Event("NewVote")]
    public class NewVoteEventDTO
    {
        [Parameter("bytes20", "_gitHash", 1, true )]
        public byte[] GitHash {get; set;}
        [Parameter("address", "_voter", 2, false )]
        public string Voter {get; set;}
        [Parameter("bool", "_vote", 3, false )]
        public bool Vote {get; set;}
        [Parameter("uint8", "_currentState", 4, false )]
        public byte CurrentState {get; set;}
        [Parameter("string", "_voteInfo", 5, false )]
        public string VoteInfo {get; set;}
        [Parameter("uint256", "_votesLeft", 6, false )]
        public BigInteger VotesLeft {get; set;}
    }
}
