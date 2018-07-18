using System;
using System.Threading.Tasks;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Web3;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.CQS;
using System.Threading;
using ChangeManager.Contracts.ChangeManager.CQS;
using ChangeManager.Contracts.ChangeManager.DTOs;
namespace ChangeManager.Contracts.ChangeManager.Service
{

    public class ChangeManagerService
    {
    
        public static Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(Web3 web3, ChangeManagerDeployment changeManagerDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            return web3.Eth.GetContractDeploymentHandler<ChangeManagerDeployment>().SendRequestAndWaitForReceiptAsync(changeManagerDeployment, cancellationTokenSource);
        }
        public static Task<string> DeployContractAsync(Web3 web3, ChangeManagerDeployment changeManagerDeployment)
        {
            return web3.Eth.GetContractDeploymentHandler<ChangeManagerDeployment>().SendRequestAsync(changeManagerDeployment);
        }
        public static async Task<ChangeManagerService> DeployContractAndGetServiceAsync(Web3 web3, ChangeManagerDeployment changeManagerDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            var receipt = await DeployContractAndWaitForReceiptAsync(web3, changeManagerDeployment, cancellationTokenSource);
            return new ChangeManagerService(web3, receipt.ContractAddress);
        }
    
        protected Web3 Web3{ get; }
        
        protected ContractHandler ContractHandler { get; }
        
        public ChangeManagerService(Web3 web3, string contractAddress)
        {
            Web3 = web3;
            ContractHandler = web3.Eth.GetContractHandler(contractAddress);
        }
    
        public Task<ViewChangeOutputDTO> ViewChangeQueryAsync(ViewChangeFunction viewChangeFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<ViewChangeFunction, ViewChangeOutputDTO>(viewChangeFunction, blockParameter);
        }
        public Task<ViewStateOutputDTO> ViewStateQueryAsync(ViewStateFunction viewStateFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<ViewStateFunction, ViewStateOutputDTO>(viewStateFunction, blockParameter);
        }
        public Task<string> ReleaseChangeRequestAsync(ReleaseChangeFunction releaseChangeFunction)
        {
             return ContractHandler.SendRequestAsync(releaseChangeFunction);
        }
        public Task<TransactionReceipt> ReleaseChangeRequestAndWaitForReceiptAsync(ReleaseChangeFunction releaseChangeFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(releaseChangeFunction, cancellationToken);
        }
        public Task<string> CreateNewChangeRequestRequestAsync(CreateNewChangeRequestFunction createNewChangeRequestFunction)
        {
             return ContractHandler.SendRequestAsync(createNewChangeRequestFunction);
        }
        public Task<TransactionReceipt> CreateNewChangeRequestRequestAndWaitForReceiptAsync(CreateNewChangeRequestFunction createNewChangeRequestFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(createNewChangeRequestFunction, cancellationToken);
        }
        public Task<string> ResponsibleVoteRequestAsync(ResponsibleVoteFunction responsibleVoteFunction)
        {
             return ContractHandler.SendRequestAsync(responsibleVoteFunction);
        }
        public Task<TransactionReceipt> ResponsibleVoteRequestAndWaitForReceiptAsync(ResponsibleVoteFunction responsibleVoteFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(responsibleVoteFunction, cancellationToken);
        }
        public Task<string> ManagementVoteRequestAsync(ManagementVoteFunction managementVoteFunction)
        {
             return ContractHandler.SendRequestAsync(managementVoteFunction);
        }
        public Task<TransactionReceipt> ManagementVoteRequestAndWaitForReceiptAsync(ManagementVoteFunction managementVoteFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(managementVoteFunction, cancellationToken);
        }
    }
}
