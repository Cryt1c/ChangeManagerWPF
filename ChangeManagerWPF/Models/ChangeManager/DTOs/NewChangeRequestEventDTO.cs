using Nethereum.ABI.FunctionEncoding.Attributes;
using System.Numerics;
namespace ChangeManager.Contracts.ChangeManager.DTOs
{
    [Event("NewChangeRequest")]
    public class NewChangeRequestEventDTO
    {
        [Parameter("bytes20", "_gitHash", 1, true )]
        public byte[] GitHash {get; set;}
        [Parameter("string", "_additionalInformation", 2, false )]
        public string AdditionalInformation {get; set;}
        [Parameter("uint256", "_costs", 3, false )]
        public BigInteger Costs {get; set;}
        [Parameter("uint256", "_estimation", 4, false )]
        public BigInteger Estimation {get; set;}
    }
}
