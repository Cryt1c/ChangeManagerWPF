using System;
using System.Threading.Tasks;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.Contracts.CQS;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Contracts.Contracts.ChangeManager.DTOs;
namespace Contracts.Contracts.ChangeManager.CQS
{
    [Function("createNewChangeRequest")]
    public class CreateNewChangeRequestFunction:ContractMessage
    {
        [Parameter("bytes20", "gitHash", 1)]
        public byte[] GitHash {get; set;}
        [Parameter("string", "additionalInformation", 2)]
        public string AdditionalInformation {get; set;}
        [Parameter("uint256", "costs", 3)]
        public BigInteger Costs {get; set;}
        [Parameter("uint256", "estimation", 4)]
        public BigInteger Estimation {get; set;}
    }
}
