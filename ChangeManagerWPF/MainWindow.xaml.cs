﻿using ChangeManagerWPF.Model;
using ChangeManagerWPF.Models;
using ChangeManagerWPF.Models.ChangeManager;
using Contracts.Contracts.ChangeManager.DTOs;
using Contracts.Contracts.ChangeManager.Service;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ChangeManagerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string contractAddress;
        private Dictionary<string, ChangeRequest> changeRequests;
        Event newChangeRequestEventLog;
        Event newVoteEventLog;
        BlockParameter lastBlock;
        BlockParameter firstBlock;
        DispatcherTimer timer;
        Web3 web3;
        string gitProject;


        public MainWindow(string contractAddress, string gitProject)
        {
            InitializeComponent();
            tabControl.Items.Remove(responsibleTab);
            tabControl.Items.Remove(managementTab);

            this.contractAddress = contractAddress;
            changeRequests = new Dictionary<string, ChangeRequest>();
            web3 = new Web3();
            ChangeManagerService changeManagerService = new ChangeManagerService(web3, contractAddress);
            newVoteEventLog = changeManagerService.ContractHandler.GetEvent<NewVoteEventDTO>();
            newChangeRequestEventLog = changeManagerService.ContractHandler.GetEvent<NewChangeRequestEventDTO>();
            firstBlock = BlockParameter.CreateEarliest();

            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(checkEventsAndUpdateTable);
            timer.Interval = new TimeSpan(0, 0, 2);
            timer.Start();
            this.gitProject = gitProject;

            getGitCommits(gitProject);
        }

        // Helper class for the Github request
        private class Commit
        {
            public string sha { get; set; }
        }
        private async void getGitCommits(string gitProject)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add(
                "User-Agent",
                "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36");
            string url = "https://api.github.com/repos/" + gitProject + "/commits";
            var responseString = await client.GetStringAsync(url);
            List<Commit> commit = JsonConvert.DeserializeObject<List<Commit>>(responseString);
            commit.ForEach(x => createGitHash.Items.Add("0x" + x.sha));
        }

        // This method will check all blocks for new Events. 
        private async Task<Dictionary<string, ChangeRequest>> checkCREvents(string contractAdress)
        {
            ChangeManagerService changeManagerService = new ChangeManagerService(new Web3(), contractAddress);
            Event newVoteEventLog = changeManagerService.ContractHandler.GetEvent<NewVoteEventDTO>();
            Event newChangeRequestEventLog = changeManagerService.ContractHandler.GetEvent<NewChangeRequestEventDTO>();

            Dictionary<string, ChangeRequest> result = new Dictionary<string, ChangeRequest>();

            BlockParameter firstBlock = BlockParameter.CreateEarliest();
            BlockParameter lastBlock = BlockParameter.CreateLatest();
            NewFilterInput filterCR = newChangeRequestEventLog.CreateFilterInput(firstBlock, lastBlock);
            NewFilterInput filterVote = newVoteEventLog.CreateFilterInput(firstBlock, lastBlock);

            List<EventLog<NewChangeRequestEventDTO>> logCR = await newChangeRequestEventLog.GetAllChanges<NewChangeRequestEventDTO>(filterCR);
            List<EventLog<NewVoteEventDTO>> logVote = await newVoteEventLog.GetAllChanges<NewVoteEventDTO>(filterVote);

            // Get confirmation for CR creation. Then add CR to table and combobox for management vote.
            Debug.WriteLine("Checking for CR Events: " + logCR.Count);
            foreach (EventLog<NewChangeRequestEventDTO> cr in logCR)
            {
                ChangeRequest changeRequest;
                String gitHash = Converter.ByteArrayToBinHex(cr.Event.GitHash);

                HexBigInteger blocknumber = cr.Log.BlockNumber;
                Block block = await web3.Eth.Blocks.GetBlockWithTransactionsHashesByNumber.SendRequestAsync(blocknumber);

                if (result.ContainsKey(gitHash))
                {
                    changeRequest = result[gitHash];
                }
                else
                {
                    changeRequest = new ChangeRequest(this.contractAddress);
                    result.Add(gitHash, changeRequest);
                }
                changeRequest.updateChangeRequest(gitHash, cr.Event.AdditionalInformation, cr.Event.Costs, cr.Event.Estimation, block.Timestamp.Value);
            }

            // Get vote confirmation. Then update State of the vote.
            Debug.WriteLine("Checking for Vote Events: " + logVote.Count);
            foreach (EventLog<NewVoteEventDTO> vote in logVote)
            {
                String gitHash = Converter.ByteArrayToBinHex(vote.Event.GitHash);
                ChangeRequest changeRequest = result[gitHash];

                HexBigInteger blocknumber = vote.Log.BlockNumber;
                Block block = await web3.Eth.Blocks.GetBlockWithTransactionsHashesByNumber.SendRequestAsync(blocknumber);

                if (!changeRequest.votes.ContainsKey(vote.Event.Voter))
                {
                    changeRequest.updateVotes((State)vote.Event.CurrentState, vote.Event.VoteInfo, vote.Event.VotesLeft, vote.Event.Voter, vote.Event.Vote, block.Timestamp.Value);
                }
            }

            return result;
        }

        // This method only checks the yet unchecked blocks for new Events and updates the CR and vote tables.
        private async void checkEventsAndUpdateTable(object sender, EventArgs e)
        {
            lastBlock = BlockParameter.CreateLatest();
            NewFilterInput filterCR = newChangeRequestEventLog.CreateFilterInput(firstBlock, lastBlock);
            NewFilterInput filterVote = newVoteEventLog.CreateFilterInput(firstBlock, lastBlock);
            firstBlock = lastBlock;

            List<EventLog<NewChangeRequestEventDTO>> logCR = await newChangeRequestEventLog.GetAllChanges<NewChangeRequestEventDTO>(filterCR);
            List<EventLog<NewVoteEventDTO>> logVote = await newVoteEventLog.GetAllChanges<NewVoteEventDTO>(filterVote);

            // Get confirmation for CR creation. Then add CR to table and combobox for management vote.
            Debug.WriteLine("Checking for CR Events: " + logCR.Count);
            foreach (EventLog<NewChangeRequestEventDTO> cr in logCR)
            {
                ChangeRequest changeRequest;
                String gitHash = Converter.ByteArrayToBinHex(cr.Event.GitHash);

                HexBigInteger blocknumber = cr.Log.BlockNumber;
                Block block = await web3.Eth.Blocks.GetBlockWithTransactionsHashesByNumber.SendRequestAsync(blocknumber);

                if (changeRequests.ContainsKey(gitHash))
                {
                    changeRequest = changeRequests[gitHash];
                }
                else
                {
                    changeRequest = new ChangeRequest(this.contractAddress);
                    changeRequests.Add(gitHash, changeRequest);
                }
                changeRequest.updateChangeRequest(gitHash, cr.Event.AdditionalInformation, cr.Event.Costs, cr.Event.Estimation, block.Timestamp.Value);
            }

            // Get vote confirmation. Then update State of the vote.
            Debug.WriteLine("Checking for Vote Events: " + logVote.Count);
            foreach (EventLog<NewVoteEventDTO> vote in logVote)
            {
                String gitHash = Converter.ByteArrayToBinHex(vote.Event.GitHash);
                ChangeRequest changeRequest = changeRequests[gitHash];

                HexBigInteger blocknumber = vote.Log.BlockNumber;
                Block block = await web3.Eth.Blocks.GetBlockWithTransactionsHashesByNumber.SendRequestAsync(blocknumber);

                if (!changeRequest.votes.ContainsKey(vote.Event.Voter))
                {
                    changeRequest.updateVotes((State)vote.Event.CurrentState, vote.Event.VoteInfo, vote.Event.VotesLeft, vote.Event.Voter, vote.Event.Vote, block.Timestamp.Value);
                }
            }
            updateVoteTableAndTabs();
            this.changeRequestsTable.ItemsSource = changeRequests.Values.ToList();
        }

        private async void createChangeRequestAsync(object sender, RoutedEventArgs e)
        {
            try
            {
                // Prevents creation of CR duplicates; Could make sense if a ChangeRequest has been rejected before to revisit it
                if (this.changeRequests.ContainsKey(createGitHash.Text))
                {
                    MessageBox.Show($"ChangeRequest already exists:  { createGitHash.Text }");
                    return;
                }
                string privateKey = createKey.Password;

                ChangeRequest changeRequest = new ChangeRequest(this.contractAddress);
                await changeRequest.createChangeRequestAsync(privateKey, createGitHash.Text, createAdditionalInformation.Text, UInt32.Parse(createEstimation.Text), UInt32.Parse(createCosts.Text));
                this.changeRequests.Add(createGitHash.Text, changeRequest);

                MessageBox.Show($"Created ChangeRequest:  { createGitHash.Text }");
            }
            catch (Exception ex)
            {
                Debug.Write(ex);
            }
        }

        // Manage a ChangeRequest
        private async void managementVote(object sender, RoutedEventArgs e)
        {
            try
            {
                string privateKey = managementKey.Password;

                ChangeRequest changeRequest = (ChangeRequest)changeRequestsTable.SelectedItem;

                List<string> responsibleParties = managementAddresses.Text.Split(',').Select(p => p.Trim()).ToList<string>();

                if (managementAccept.IsChecked == true)
                {
                    await changeRequest.managementVoteAsync(privateKey, true, responsibleParties, managementInfo.Text);
                }
                else if (managementReject.IsChecked == true)
                {
                    await changeRequest.managementVoteAsync(privateKey, false, new List<string>(), managementInfo.Text);
                }

                MessageBox.Show($"ChangeRequest managed:  { changeRequest.gitHash }\n");
            }
            catch (Exception ex)
            {
                Debug.Write(ex);
            }
        }

        // Let the responsible parties vote on the ChangeRequest
        private async void responsibleVote(object sender, RoutedEventArgs e)
        {
            try
            {
                string privateKey = responsibleKey.Password;

                ChangeRequest changeRequest = (ChangeRequest)changeRequestsTable.SelectedItem;

                if (responsibleAccept.IsChecked == true)
                {
                    await changeRequest.responsibleVoteAsync(privateKey, true, responsibleInfo.Text);
                }
                else if (responsibleReject.IsChecked == true)
                {
                    await changeRequest.responsibleVoteAsync(privateKey, false, responsibleInfo.Text);
                }

                MessageBox.Show($"Voted on ChangeRequest:  { changeRequest.gitHash }");
            }
            catch (Exception ex)
            {
                Debug.Write(ex);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            timer.Stop();
        }

        // Update the table and show correct tabs for selected ChangeRequest
        private void changeRequestSelected(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ChangeRequest selection = (ChangeRequest)changeRequestsTable.SelectedItem;

            Uri commitURI = new Uri("https://github.com/" + this.gitProject + "/commit/" + selection.gitHash.Substring(2));
            managementLink.NavigateUri = commitURI;
            responsibleLink.NavigateUri = commitURI;

            updateVoteTableAndTabs();
        }

        // Show votes and correct tabs for selected ChangeRequest
        private void updateVoteTableAndTabs()
        {
            ChangeRequest selection = (ChangeRequest)changeRequestsTable.SelectedItem;

            if (changeRequestsTable.SelectedItem != null)
            {
                selection = (ChangeRequest)changeRequestsTable.SelectedItem;
                votesTable.ItemsSource = selection.votes.Values.ToList();

                if (selection.state == State.changeProposed)
                {
                    if (!tabControl.Items.Contains(managementTab)) tabControl.Items.Add(managementTab);
                    if (tabControl.Items.Contains(responsibleTab)) tabControl.Items.Remove(responsibleTab);
                }
                else if (selection.state == State.changeManaged)
                {
                    if (!tabControl.Items.Contains(responsibleTab)) tabControl.Items.Add(responsibleTab);
                    if (tabControl.Items.Contains(managementTab)) tabControl.Items.Remove(managementTab);
                }
                else if (selection.state == State.changeApproved || selection.state == State.changeRejected)
                {
                    if (tabControl.Items.Contains(responsibleTab)) tabControl.Items.Remove(responsibleTab);
                    if (tabControl.Items.Contains(managementTab)) tabControl.Items.Remove(managementTab);
                }
            }
        }

        private void Link_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
