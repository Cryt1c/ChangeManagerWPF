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
    }
}
