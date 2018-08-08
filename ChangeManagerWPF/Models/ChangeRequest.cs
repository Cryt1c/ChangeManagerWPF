using ChangeManagerWPF.Models.ChangeManager;
using Contracts.Contracts.ChangeManager.CQS;
using Contracts.Contracts.ChangeManager.DTOs;
using Contracts.Contracts.ChangeManager.Service;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace ChangeManagerWPF.Model
{
    class ChangeRequest
    {
        public ChangeManagerService changeManagerService { get; set; }
        private string contractAddress;

        public BigInteger costs { get; private set; }
        public BigInteger estimation { get; private set; }
        public string additionalInformation { get; private set; }

        public State state { get; set; }
        public string voteInfo { get; set; }
        public uint votesLeft { get; set; }
        public string changeOwner { get; set; }
        public string gitHash { get; set; }
        public Byte[] gitHashByte { get; set; }

        public ChangeRequest(ChangeManagerService changeManagerService, string contractAddress)
        {
            this.changeManagerService = changeManagerService;
            this.contractAddress = contractAddress;
        }

        public byte[] HashStringToByteArray(string gitHash)
        {
            return Enumerable.Range(1, gitHash.Length / 2 - 1) // Range(1, - to get rid of "0x"  
                .Select(i => Convert.ToByte(gitHash.Substring(i * 2, 2), 16))
                .ToArray();
        }

        internal async Task createChangeRequestAsync(string gitHash, string additionalInformation, uint estimation, uint costs)
        {
            CreateNewChangeRequestFunction changeRequestFunction = new CreateNewChangeRequestFunction();
            changeRequestFunction.GitHash = HashStringToByteArray(gitHash);
            changeRequestFunction.AdditionalInformation = additionalInformation;
            changeRequestFunction.Estimation = estimation;
            changeRequestFunction.Costs = costs;
            await changeManagerService.CreateNewChangeRequestRequestAsync(changeRequestFunction);
        }

        internal async Task managementVoteAsync(bool accept, List<string> responsibleParties, string voteInfo)
        {
            ManagementVoteFunction managementFunction = new ManagementVoteFunction();
            managementFunction.GitHash = gitHashByte;
            managementFunction.AcceptChange = accept;
            managementFunction.ResponsibleParties = responsibleParties;
            managementFunction.VoteInfo = voteInfo;

            await this.changeManagerService.ManagementVoteRequestAsync(managementFunction);
        }

        internal async Task responsibleVoteAsync(string privateKey, bool accept, string voteInfo)
        {
            ResponsibleVoteFunction responsibleVoteFunction = new ResponsibleVoteFunction();
            responsibleVoteFunction.GitHash = gitHashByte;
            responsibleVoteFunction.FromAddress = Web3.GetAddressFromPrivateKey(privateKey);
            responsibleVoteFunction.AcceptChange = accept;
            responsibleVoteFunction.VoteInfo = voteInfo;

            Web3 web3 = new Web3(new Account(privateKey));

            ChangeManagerService responsibleService = new ChangeManagerService(web3, this.contractAddress);
            responsibleVoteFunction.Gas = 1000000;

            await responsibleService.ResponsibleVoteRequestAsync(responsibleVoteFunction);
        }

        public void updateVotes(State state, String voteInfo, BigInteger votesLeft, String voter)
        {
            this.state = state;
            this.votesLeft = (uint)votesLeft;
        }

        public void updateChangeRequest(String gitHash, String additionalInformation, BigInteger costs, BigInteger estimation)
        {
            this.gitHash = gitHash;
            this.gitHashByte = Converter.HashStringToByteArray(gitHash);
            this.costs = costs;
            this.estimation = estimation;
            this.additionalInformation = additionalInformation;
        }
    }
}
