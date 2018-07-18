using System;
using System.Threading.Tasks;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;
namespace ChangeManager.Contracts.ChangeManager.DTOs
{
    [FunctionOutput]
    public class ViewStateOutputDTO
    {
        [Parameter("uint8", "state", 1)]
        public byte State {get; set;}
        [Parameter("uint256", "voteCount", 2)]
        public BigInteger VoteCount {get; set;}
        [Parameter("string", "voteInfo", 3)]
        public string VoteInfo {get; set;}
    }
}
