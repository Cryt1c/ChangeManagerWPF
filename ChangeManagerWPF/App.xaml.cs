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
            web3 = new Web3(new Account("c87509a1c067bbde78beb793e6fa76530b6382a4c0241e5e4a9ec0a0f44dc0d3"));
            deployment = new ChangeManagerDeployment();
            TransactionReceipt receipt = await ChangeManagerService.DeployContractAndWaitForReceiptAsync(web3, deployment);

            changeManagerService = new ChangeManagerService(web3, receipt.ContractAddress);

            MainWindow mWnd = new MainWindow(changeManagerService, receipt.ContractAddress);
            mWnd.Title = "ChangeManager";
            mWnd.Show();
        }

        private void UseChangeManager(string address, string gitProject)
        {
            web3 = new Web3(new Account("c87509a1c067bbde78beb793e6fa76530b6382a4c0241e5e4a9ec0a0f44dc0d3"));

            changeManagerService = new ChangeManagerService(web3, address);

            MainWindow mWnd = new MainWindow(changeManagerService, address);
            mWnd.Title = "ChangeManager";
            mWnd.Show();
        }
    }
}
