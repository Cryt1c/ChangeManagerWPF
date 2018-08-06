using ChangeManager.Contracts.ChangeManager.CQS;
using ChangeManager.Contracts.ChangeManager.DTOs;
using ChangeManager.Contracts.ChangeManager.Service;
using ChangeManagerWPF.Model;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
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

        private async void NewChangeManager(string gitProject)
        {
            web3 = new Web3(new Account("7b0ce3ddd31b4bba4b8f116217b8db976b8537fae717fd5ef92c4233f83e7b36"));
            deployment = new ChangeManagerDeployment();
            TransactionReceipt receipt = await ChangeManagerService.DeployContractAndWaitForReceiptAsync(web3, deployment);

            changeManagerService = new ChangeManagerService(web3, receipt.ContractAddress);

            MainWindow mWnd = new MainWindow(changeManagerService, receipt.ContractAddress);
            mWnd.Title = "ChangeManager";
            mWnd.Show();
        }

        private void UseChangeManager(string address, string gitProject)
        {
            web3 = new Web3(new Account("7b0ce3ddd31b4bba4b8f116217b8db976b8537fae717fd5ef92c4233f83e7b36"));

            changeManagerService = new ChangeManagerService(web3, address);

            MainWindow mWnd = new MainWindow(changeManagerService, address);
            mWnd.Title = "ChangeManager";
            mWnd.Show();
        }
    }
}
