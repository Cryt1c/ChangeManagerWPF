using ChangeManager.Contracts.ChangeManager.CQS;
using ChangeManager.Contracts.ChangeManager.DTOs;
using ChangeManager.Contracts.ChangeManager.Service;
using ChangeManagerWPF.Model;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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

        DispatcherTimer timer;

        public MainWindow(ChangeManagerService changeManagerService, string contractAddress)
        {
            InitializeComponent();
            this.changeManagerService = changeManagerService;
            this.contractAddress = contractAddress;
            changeRequests = new Dictionary<string, ChangeRequest>();

            newVoteEventLog = changeManagerService.ContractHandler.GetEvent<NewVoteEventDTO>();
            newChangeRequestEventLog = changeManagerService.ContractHandler.GetEvent<NewChangeRequestEventDTO>();

            //filterCR = newChangeRequestEventLog.CreateFilterInput();
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(checkEvents);
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
        }

        private async void checkEvents(object sender, EventArgs e)
        {
            NewFilterInput filterCR = newChangeRequestEventLog.CreateFilterInput(BlockParameter.CreateEarliest(), BlockParameter.CreateLatest());
            NewFilterInput filterVote = newVoteEventLog.CreateFilterInput(BlockParameter.CreateEarliest(), BlockParameter.CreateLatest());

            //TODO: Adopt so that blocks are not searched multiple times
            List<EventLog<NewChangeRequestEventDTO>> logCR = await newChangeRequestEventLog.GetAllChanges<NewChangeRequestEventDTO>(filterCR);
            List<EventLog<NewVoteEventDTO>> logVote = await newVoteEventLog.GetAllChanges<NewVoteEventDTO>(filterVote);

            Debug.WriteLine("Checking for CR Events: " + logCR.Count);
            logCR.ForEach(x => Debug.WriteLine("CR created event: " + x.Event.AdditionalInformation));
            Debug.WriteLine("Checking for Vote Events: " + logVote.Count);
            logVote.ForEach(x => Debug.WriteLine("Vote created event: " + x.Event.VoteInfo));
        }

        private async void createChangeRequestAsync(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.changeRequests.ContainsKey(gitHash.Text))
                {
                    MessageBox.Show($"ChangeRequest already exists:  { gitHash.Text }");
                    return;
                }
                ChangeRequest changeRequest = new ChangeRequest(gitHash.Text, additionalInformation.Text, UInt32.Parse(estimation.Text), UInt32.Parse(costs.Text), "Owner");
                await changeRequest.createChangeRequestAsync(changeManagerService, contractAddress);

                managementGitHash.Items.Add(gitHash.Text);
                this.changeRequests.Add(gitHash.Text, changeRequest);
                this.changeRequestsTable.ItemsSource = changeRequests.Values.ToList();

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
                ChangeRequest changeRequest= changeRequests[gitHash];
                List<string> responsibleParties = managementAddresses.Text.Split(',').Select(p => p.Trim()).ToList<string>();

                if (managementAccept.IsChecked == true)
                {
                    await changeRequest.managementVoteAsync(true, responsibleParties, managementInfo.Text);
                    // Remove privateKeys
                    string[] privateKeys = { "ae6ae8e5ccbfb04590405997ee2d52d2b330726137b875053c36d94e974d162f", "0dbbe8e4ae425a6d2687f1a7e3ba17bc98c673636790f1b8ad91193c05875ef1" };
                    responsibleAddress.ItemsSource = privateKeys;
                    responsibleGitHash.Items.Add(gitHash);
                }
                else if (managementReject.IsChecked == true)
                {
                    await changeRequest.managementVoteAsync(false, new List<string>(), managementInfo.Text);
                }

                this.changeRequestsTable.ItemsSource = changeRequests.Values.ToList();

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
                string address = responsibleAddress.SelectedItem.ToString();
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

                this.changeRequestsTable.ItemsSource = changeRequests.Values.ToList();

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
