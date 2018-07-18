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
        
        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            web3 = new Web3(new Account("c87509a1c067bbde78beb793e6fa76530b6382a4c0241e5e4a9ec0a0f44dc0d3"));
            deployment = new ChangeManagerDeployment();
            TransactionReceipt receipt = await ChangeManagerService.DeployContractAndWaitForReceiptAsync(web3, deployment);

            changeManagerService = new ChangeManagerService(web3, receipt.ContractAddress);
            
            MainWindow wnd = new MainWindow(changeManagerService, receipt.ContractAddress);
            wnd.Title = "ChangeManager";
            wnd.Show();
        }

        public async Task InitializeWeb3()
        {

            try
            { 
                //// Read abis and bytecode from files
                //ABIByteCode changerequest = JsonConvert.DeserializeObject<ABIByteCode>(File.ReadAllText(@"..\..\contracts\ChangeRequest.json"));
                //ABIByteCode changemanager = JsonConvert.DeserializeObject<ABIByteCode>(File.ReadAllText(@"..\..\contracts\ChangeManager.json"));
                //var abiCM = JsonConvert.SerializeObject(changemanager.abi, Formatting.Indented);
                //var abiCR = JsonConvert.SerializeObject(changerequest.abi, Formatting.Indented);
                //var byteCodeCM = changemanager.bytecode;

                //// Deploy ChangeManager
                //var transactionHash = await web3.Eth.DeployContract.SendRequestAsync(abiCM, byteCodeCM, address, new HexBigInteger(4700000));
                //var receipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);
                //var contractAddress = receipt.ContractAddress;
                //var contract = web3.Eth.GetContract(abiCM, contractAddress);

                //// Call createNewChangeRequest
                //var createNewChangeRequest = contract.GetFunction("createNewChangeRequest");
                //byte[] result = Enumerable
                //   .Range(1, gitHash.Length / 2 - 1) // Range(1, - to get rid of "0x"  
                //   .Select(i => Convert.ToByte(gitHash.Substring(i * 2, 2), 16))
                //   .ToArray();


                //transactionHash = await createNewChangeRequest.SendTransactionAsync(address, new HexBigInteger(4700000), new HexBigInteger(0), result, additionalInformation, costs, estimation);
                //receipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);


                //// Filter for NewChangeRequest event
                //var newChangeRequestEvent = contract.GetEvent("NewChangeRequest");
                //var filterInput = newChangeRequestEvent.CreateFilterInput(BlockParameter.CreateEarliest(), BlockParameter.CreateLatest());
                //var log = await newChangeRequestEvent.GetAllChanges<NewChangeRequestEvent>(filterInput);
                //Debug.WriteLine("List lenght: " + log.Count);
                //foreach (var logEntry in log)
                //{
                //    Debug.WriteLine("GitHash: 0x" + string.Join("", logEntry.Event.GitHash.Select(b => b.ToString("x2"))));
                //    Debug.WriteLine("AdditionalInformation: " + logEntry.Event.AdditionalInformation);
                //    Debug.WriteLine("Costs: " + logEntry.Event.Costs);
                //    Debug.WriteLine("Estimation: " + logEntry.Event.Estimation);
                //}

                //var manageChangeRequest = crContract.GetFunction("managementVote");

                //string[] voters = { "0xf17f52151EbEF6C7334FAD080c5704D77216b732", "0xC5fdf4076b8F3A5357c5E395ab970B5B54098Fef" };
                //transactionHash = await manageChangeRequest.SendTransactionAsync(address, new HexBigInteger(4700000), new HexBigInteger(0), true, voters, "");
                //receipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);

                //var viewChange = crContract.GetFunction("viewChange");
                //var asyncResult = viewChange.CallDeserializingToObjectAsync<ChangeRequest>();

                // Filter for NewChangeRequest event
                //var newVoteEvent = crContract.GetEvent("NewVote");
                //filterInput = newVoteEvent.CreateFilterInput(BlockParameter.CreateEarliest(), BlockParameter.CreateLatest());
                //var voteLog = await newVoteEvent.GetAllChanges<NewVoteEvent>(filterInput);
                //Debug.WriteLine("List lenght: " + voteLog.Count);
                //Debug.WriteLine(JsonConvert.SerializeObject(voteLog, Formatting.Indented));
            }
            catch (Exception ex)
            {
                Debug.Write(ex);
            }
        }
    }
}
