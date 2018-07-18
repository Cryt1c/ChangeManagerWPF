using ChangeManager.Contracts.ChangeManager.CQS;
using ChangeManager.Contracts.ChangeManager.Service;
using Nethereum.Contracts;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ChangeManagerWPF.Model
{
    class ChangeRequest
    {
        private State state;
        private uint voteInfo;
        private uint votesLeft;
        public string changeOwner { get; set; }
        public string gitHash { get; set; }
        public CreateNewChangeRequestFunction changeRequestFunction { get; set; }
        public ChangeManagerService changeManagerService { get; set; }
        private string contractAddress;

        public ChangeRequest(string gitHash, string additionalInformation, uint estimation, uint costs, string changeOwner)
        {
            this.changeRequestFunction = new CreateNewChangeRequestFunction();
            this.gitHash = gitHash;
            this.changeRequestFunction.GitHash = this.HashStringToByteArray(gitHash);
            this.changeRequestFunction.AdditionalInformation = additionalInformation;
            this.changeRequestFunction.Estimation = estimation;
            this.changeRequestFunction.Costs = costs;
            this.changeOwner = changeOwner;
        }

        public byte[] HashStringToByteArray(string hash)
        {
            return Enumerable.Range(1, gitHash.Length / 2 - 1) // Range(1, - to get rid of "0x"  
                .Select(i => Convert.ToByte(gitHash.Substring(i * 2, 2), 16))
                .ToArray();
        }

        internal async Task createChangeRequestAsync(ChangeManagerService changeManagerService, string contractAddress)
        {
            this.changeManagerService = changeManagerService;
            this.contractAddress = contractAddress;
            await changeManagerService.CreateNewChangeRequestRequestAsync(this.changeRequestFunction);
        }

        internal async Task managementVoteAsync(bool accept, List<string> responsibleParties, string voteInfo)
        {
            ManagementVoteFunction managementFunction = new ManagementVoteFunction();
            managementFunction.GitHash = this.changeRequestFunction.GitHash;
            managementFunction.AcceptChange = accept;
            managementFunction.ResponsibleParties = responsibleParties;
            managementFunction.VoteInfo = voteInfo;

            await this.changeManagerService.ManagementVoteRequestAsync(managementFunction);
        }

        internal async Task responsibleVoteAsync(string privateKey, bool accept, string voteInfo)
        {
            ResponsibleVoteFunction responsibleVoteFunction = new ResponsibleVoteFunction();
            responsibleVoteFunction.GitHash = this.changeRequestFunction.GitHash;
            responsibleVoteFunction.FromAddress = Web3.GetAddressFromPrivateKey(privateKey);
            responsibleVoteFunction.AcceptChange = accept;
            responsibleVoteFunction.VoteInfo = voteInfo;

            Web3 web3 = new Web3(new Account(privateKey));   
            
            ChangeManagerService responsibleService = new ChangeManagerService(web3, this.contractAddress);
            responsibleVoteFunction.Gas = 1000000;

            await responsibleService.ResponsibleVoteRequestAsync(responsibleVoteFunction);
        }
    }
}
