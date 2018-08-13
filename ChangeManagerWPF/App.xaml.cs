using Contracts.Contracts.ChangeManager.CQS;
using Contracts.Contracts.ChangeManager.Service;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows;


namespace ChangeManagerWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        ChangeManagerDeployment deployment;
        Web3 web3;
        ChangeManagerService changeManagerService;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            InitializationWindow iWnd = new InitializationWindow(NewChangeManager, UseChangeManager);
            iWnd.Show();

        }

        private async void NewChangeManager(string gitProject, string privateKey)
        {
            web3 = new Web3(new Account(privateKey));
            deployment = new ChangeManagerDeployment();
            TransactionReceipt receipt = await ChangeManagerService.DeployContractAndWaitForReceiptAsync(web3, deployment);

            changeManagerService = new ChangeManagerService(web3, receipt.ContractAddress);

            Web3.GetAddressFromPrivateKey(privateKey);
            // TODO: Delete Testcode
            gitProject = "Cryt1c/ChangeManager";
            MainWindow mWnd = new MainWindow(receipt.ContractAddress, gitProject);
            mWnd.Title = "ChangeManager for https://github.com/" + gitProject + " Managed by: " + Web3.GetAddressFromPrivateKey(privateKey); ;
            mWnd.Show();
        }

        private void UseChangeManager(string address, string gitProject)
        {
            MainWindow mWnd = new MainWindow(address, gitProject);
            mWnd.Title = "ChangeManager for https://github.com/" + gitProject;
            mWnd.Show();
        }
    }
}
