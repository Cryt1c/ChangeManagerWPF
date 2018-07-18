using System;
using System.Threading.Tasks;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.Contracts.CQS;
using Nethereum.ABI.FunctionEncoding.Attributes;
using ChangeManager.Contracts.ChangeManager.DTOs;
namespace ChangeManager.Contracts.ChangeManager.CQS
{
    [Function("viewState", typeof(ViewStateOutputDTO))]
    public class ViewStateFunction:ContractMessage
    {
        [Parameter("bytes20", "gitHash", 1)]
        public byte[] GitHash {get; set;}
    }
}
