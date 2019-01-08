using ChangeManagerWPF.Models;
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
        private string contractAddress;

        public string gitHash { get; set; }
        public Byte[] gitHashByte { get; set; }
        public BigInteger costs { get; private set; }
        public BigInteger estimation { get; private set; }
        public string additionalInformation { get; private set; }
        public string changeOwner { get; set; }
        public BigInteger timestamp { get; set; }

        public State state { get; set; }
        public uint votesLeft { get; set; }
        public Dictionary<string, Vote> votes;


        public ChangeRequest(string contractAddress)
        {
            this.contractAddress = contractAddress;
            this.votes = new Dictionary<string, Vote>();
        }

        internal async Task createChangeRequestAsync(string privateKey, string gitHash, string additionalInformation, uint estimation, uint costs)
        {
            CreateNewChangeRequestFunction changeRequestFunction = new CreateNewChangeRequestFunction();
            changeRequestFunction.GitHash = Converter.HashStringToByteArray(gitHash);
            changeRequestFunction.AdditionalInformation = additionalInformation;
            changeRequestFunction.Estimation = estimation;
            changeRequestFunction.Costs = costs;

            Web3 web3 = new Web3(new Account(privateKey));

            ChangeManagerService createService = new ChangeManagerService(web3, this.contractAddress);
            changeRequestFunction.Gas = 500000;

            await createService.CreateNewChangeRequestRequestAsync(changeRequestFunction);
        }

        internal async Task managementVoteAsync(string privateKey, bool accept, List<string> responsibleParties, string voteInfo)
        {
            ManagementVoteFunction managementFunction = new ManagementVoteFunction();
            managementFunction.GitHash = gitHashByte;
            managementFunction.AcceptChange = accept;
            managementFunction.ResponsibleParties = responsibleParties;
            managementFunction.VoteInfo = voteInfo;

            Web3 web3 = new Web3(new Account(privateKey));

            ChangeManagerService managementService = new ChangeManagerService(web3, this.contractAddress);

            await managementService.ManagementVoteRequestAsync(managementFunction);
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
            responsibleVoteFunction.Gas = 500000;

            await responsibleService.ResponsibleVoteRequestAsync(responsibleVoteFunction);
        }

        public void updateVotes(State state, String voteInfo, BigInteger votesLeft, String voter, bool acceptChange, BigInteger timestamp)
        {
            this.state = state;
            this.votesLeft = (uint)votesLeft;
            Vote vote = new Vote();
            vote.acceptChange = acceptChange;
            vote.voteInfo = voteInfo;
            vote.voter = voter;
            vote.timestamp = timestamp;
            votes.Add(voter, vote);
        }

        public void updateChangeRequest(String gitHash, String additionalInformation, BigInteger costs, BigInteger estimation, BigInteger timestamp)
        {
            this.gitHash = gitHash;
            this.gitHashByte = Converter.HashStringToByteArray(gitHash);
            this.costs = costs;
            this.estimation = estimation;
            this.additionalInformation = additionalInformation;
            this.timestamp = timestamp;
        }
    }
}
