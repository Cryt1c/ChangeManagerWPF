using System;
using System.Threading.Tasks;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;
namespace ChangeManager.Contracts.ChangeManager.DTOs
{
    [FunctionOutput]
    public class ViewChangeOutputDTO
    {
        [Parameter("string", "additionalInformation", 1)]
        public string AdditionalInformation {get; set;}
        [Parameter("uint256", "costs", 2)]
        public BigInteger Costs {get; set;}
        [Parameter("uint256", "estimation", 3)]
        public BigInteger Estimation {get; set;}
    }
}
