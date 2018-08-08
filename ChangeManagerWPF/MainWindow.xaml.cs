using ChangeManagerWPF.Model;
using ChangeManagerWPF.Models.ChangeManager;
using Contracts.Contracts.ChangeManager.DTOs;
using Contracts.Contracts.ChangeManager.Service;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Windows;
using System.Windows.Threading;

namespace ChangeManagerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ChangeManagerService changeManagerService;
        private string contractAddress;
        private Dictionary<string, ChangeRequest> changeRequests;
        Event newChangeRequestEventLog;
        Event newVoteEventLog;
        BlockParameter lastBlock;
        BlockParameter firstBlock;
        DispatcherTimer timer;

        public MainWindow(ChangeManagerService changeManagerService, string contractAddress)
        {
            InitializeComponent();
            this.changeManagerService = changeManagerService;
            this.contractAddress = contractAddress;
            changeRequests = new Dictionary<string, ChangeRequest>();

            newVoteEventLog = changeManagerService.ContractHandler.GetEvent<NewVoteEventDTO>();
            newChangeRequestEventLog = changeManagerService.ContractHandler.GetEvent<NewChangeRequestEventDTO>();
            firstBlock = BlockParameter.CreateEarliest();

            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(checkEvents);
            timer.Interval = new TimeSpan(0, 0, 2);
            timer.Start();
        }

        private async void checkEvents(object sender, EventArgs e)
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
                if (changeRequests.ContainsKey(gitHash))
                {
                    changeRequest = changeRequests[gitHash];
                }
                else
                {
                    changeRequest = new ChangeRequest(this.changeManagerService, this.contractAddress);
                    changeRequest.updateChangeRequest(gitHash, additionalInformation.Text, UInt32.Parse(costs.Text), UInt32.Parse(estimation.Text));
                    changeRequests.Add(gitHash, changeRequest);
                }
                changeRequest.updateChangeRequest(gitHash, cr.Event.AdditionalInformation, cr.Event.Costs, cr.Event.Estimation);
                if (!managementGitHash.Items.Contains(gitHash))
                {
                    managementGitHash.Items.Add(gitHash);
                }
            }

            // Get vote confirmation. Then update State of the vote.
            Debug.WriteLine("Checking for Vote Events: " + logVote.Count);
            foreach (EventLog<NewVoteEventDTO> vote in logVote)
            {
                String gitHash = Converter.ByteArrayToBinHex(vote.Event.GitHash);
                ChangeRequest changeRequest = changeRequests[gitHash];
                changeRequest.updateVotes((State)vote.Event.CurrentState, vote.Event.VoteInfo, vote.Event.VotesLeft, vote.Event.Voter);

                if (changeRequest.state == State.changeManaged)
                {
                    if(!responsibleGitHash.Items.Contains(gitHash))
                    {
                        responsibleGitHash.Items.Add(gitHash);
                        managementGitHash.Items.Remove(gitHash);
                    }
                }
                else if (changeRequest.state == State.changeApproved)
                {

                }
            }
            this.changeRequestsTable.ItemsSource = changeRequests.Values.ToList();
        }

        private async void createChangeRequestAsync(object sender, RoutedEventArgs e)
        {
            try
            {
                // Prevents creation of CR duplicates
                if (this.changeRequests.ContainsKey(gitHash.Text))
                {
                    MessageBox.Show($"ChangeRequest already exists:  { gitHash.Text }");
                    return;
                }
                ChangeRequest changeRequest = new ChangeRequest(changeManagerService, this.contractAddress);
                await changeRequest.createChangeRequestAsync(gitHash.Text, additionalInformation.Text, UInt32.Parse(estimation.Text), UInt32.Parse(costs.Text));
                this.changeRequests.Add(gitHash.Text, changeRequest);

                MessageBox.Show($"Created ChangeRequest:  { gitHash.Text }");
            }
            catch (Exception ex)
            {
                Debug.Write(ex);
            }
        }

        private async void managementVote(object sender, RoutedEventArgs e)
        {
            try
            {
                string gitHash = managementGitHash.SelectedItem.ToString();
                ChangeRequest changeRequest = changeRequests[gitHash];
                List<string> responsibleParties = managementAddresses.Text.Split(',').Select(p => p.Trim()).ToList<string>();

                if (managementAccept.IsChecked == true)
                {
                    await changeRequest.managementVoteAsync(true, responsibleParties, managementInfo.Text);
                }
                else if (managementReject.IsChecked == true)
                {
                    await changeRequest.managementVoteAsync(false, new List<string>(), managementInfo.Text);
                }

                MessageBox.Show($"ChangeRequest managed:  { gitHash }\n");
            }
            catch (Exception ex)
            {
                Debug.Write(ex);
            }
        }

        private async void responsibleVote(object sender, RoutedEventArgs e)
        {
            try
            {
                string address = responsibleAddress.Text;
                string gitHash = responsibleGitHash.SelectedItem.ToString();
                ChangeRequest changeRequest = changeRequests[gitHash];

                if (responsibleAccept.IsChecked == true)
                {
                    await changeRequest.responsibleVoteAsync(address, true, responsibleInfo.Text);
                }
                else if (responsibleReject.IsChecked == true)
                {
                    await changeRequest.responsibleVoteAsync(address, false, responsibleInfo.Text);
                }

                MessageBox.Show($"Voted on ChangeRequest:  { gitHash }");
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
    }
}
