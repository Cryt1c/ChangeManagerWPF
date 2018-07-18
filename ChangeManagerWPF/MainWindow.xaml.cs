using ChangeManager.Contracts.ChangeManager.CQS;
using ChangeManager.Contracts.ChangeManager.DTOs;
using ChangeManager.Contracts.ChangeManager.Service;
using ChangeManagerWPF.Model;
using Nethereum.Web3;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

        public MainWindow(ChangeManagerService changeManagerService, string contractAddress)
        {
            InitializeComponent();
            this.changeManagerService = changeManagerService;
            this.contractAddress = contractAddress;
            changeRequests = new Dictionary<string, ChangeRequest>();
        }

        private async void createChangeRequestAsync(object sender, RoutedEventArgs e)
        {
            try
            {

                ChangeRequest changeRequest = new ChangeRequest(gitHash.Text, additionalInformation.Text, UInt32.Parse(estimation.Text), UInt32.Parse(costs.Text), "Owner");
                await changeRequest.createChangeRequestAsync(changeManagerService, contractAddress);

                // Needs to be removed; Does not work on public blockchain because we don't wait for block to be mined
                ViewChangeFunction viewChangeFunction = new ViewChangeFunction();
                viewChangeFunction.GitHash = changeRequest.changeRequestFunction.GitHash;

                ViewChangeOutputDTO viewChangeRequest = await changeManagerService.ViewChangeQueryAsync(viewChangeFunction);

                managementGitHash.Items.Add(gitHash.Text);
                this.changeRequests.Add(gitHash.Text, changeRequest);

                MessageBox.Show($"Created ChangeRequest:  { gitHash.Text }\n" +
                    $"Additional Information: {viewChangeRequest.AdditionalInformation}\n" +
                    $"Costs (Euro): {viewChangeRequest.Costs}\n" +
                    $"Estimation (Hours): {viewChangeRequest.Estimation}");
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
                List<string> responsibleParties = managementAddresses.Text.Split(',').Select(p => p.Trim()).ToList<string>();

                if (managementAccept.IsChecked == true)
                {
                    await changeRequests[gitHash].managementVoteAsync(true, responsibleParties, managementInfo.Text);
                }
                else if (managementReject.IsChecked == true)
                {
                    await changeRequests[gitHash].managementVoteAsync(false, new List<string>(), managementInfo.Text);
                }

                ViewStateFunction viewStateFunction = new ViewStateFunction();
                viewStateFunction.GitHash = changeRequests[gitHash].changeRequestFunction.GitHash;
                ViewStateOutputDTO viewState = await changeManagerService.ViewStateQueryAsync(viewStateFunction);

                // Remove privateKeys
                string[] privateKeys = { "ae6ae8e5ccbfb04590405997ee2d52d2b330726137b875053c36d94e974d162f", "0dbbe8e4ae425a6d2687f1a7e3ba17bc98c673636790f1b8ad91193c05875ef1" };
                responsibleAddress.ItemsSource = privateKeys;
                responsibleGitHash.Items.Add(gitHash);

                MessageBox.Show($"ChangeRequest:  { gitHash }\n" +
                    $"Vote Information: {viewState.VoteInfo}\n" +
                    $"Votes Left: {viewState.VoteCount}\n" +
                    $"State: {viewState.State}");
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

                if (responsibleAccept.IsChecked == true)
                {
                    await changeRequests[gitHash].responsibleVoteAsync(address, true, responsibleInfo.Text);
                }
                else if (responsibleAccept.IsChecked == true)
                {
                    await changeRequests[gitHash].responsibleVoteAsync(address, false, responsibleInfo.Text);
                }

                ViewStateFunction viewStateFunction = new ViewStateFunction();
                viewStateFunction.GitHash = changeRequests[gitHash].changeRequestFunction.GitHash;
                ViewStateOutputDTO viewState = await changeManagerService.ViewStateQueryAsync(viewStateFunction);

                MessageBox.Show($"ChangeRequest:  { gitHash }\n" +
                    $"Vote Information: {viewState.VoteInfo}\n" +
                    $"Votes Left: {viewState.VoteCount}\n" +
                    $"State: {viewState.State}");
            }
            catch (Exception ex)
            {
                Debug.Write(ex);
            }
        }
    }
}
