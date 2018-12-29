# ChangeManagerWPF

This repository contains a the ChangeManager smart contract and the C# frontend ChangeManagerWPF application to interact with the blockchain. The code for the ChangeManager is managed here (https://github.com/Cryt1c/ChangeManager).

The smart contract represents a change management workflow where a general manager can setup a new contract and everyone can create new ChangeRequest
from github. Furthermore, the general manager can decide which responsible parties are able to vote on the release of a change. 
As soon as all responsible parties have accepted or one has rejected a ChangeRequest the voting process has finished.
All votes are transparently tracked on the blockchain using events.

# Getting Started

1. Install Ganache (https://github.com/trufflesuite/ganache/releases/) to simulate the Ethereum Blockchain
2. Build solution in Visual Studio 2017
3. Run Ganache and make these settings:
![Settings in Ganache](/image.png)
4. Run ChangeManagerWPF.exe in the Release folder (multiple instances can use one smart contract at the same time)

# Use ChangeManager smart contract
## Create new ChangeManager smart contract
1. Add a Github Project (e.g. ``Cryt1c/ChangeManagerWPF``)
2. Click on ``Create new ContractManager`` (You can leave the address empty, since we are deploying a new Contract)
3. Add the private key of the general manager, which you get from Ganache when you click on the key symbol next to an address
4. A new ChangeManager contract is deployed to the blockchain and you can start creating ChangeRequests

## Use an already existing ChangeManager smart contract
1. Add a Github Project (e.g. ``Cryt1c/ChangeManagerWPF``)
2. Add the address of the existing ChangeManager contract, which you can find in ganache under ``Logs`` if you have already created a ChangeManager before
![Logs in Ganache](/contract_address.png)
3. Click on ``Use existing ContractManager``

# Create a ChangeRequest
1. Choose a git commit you want make make a ChangeRequest of
2. Add your information (costs, estimation, info)
3. Use your private key to sign the new ChangeRequest
4. The ChangeRequest is now in the proposed state

# Manage a ChangeRequest
After a ChangeRequest has been created the general manager, who is the owner of the smart contract, has to manage the ChangeRequest. She can reject the ChangeRequest or accept it and appoint responsible parties (addresses) who can vote on the ChangeRequest.
1. Choose a proposed ChangeRequest from the table and select ``Management Vote`` from the tabs (Tab is only shown, if a proposed ChangeRequest is selected)
2. Add the responsible parties as comma-separated addresses (public keys), which you can find under ``Accounts`` in Ganache
3. Add your information (vote, info)
4. Use your private key (You must use the private key of the general manager to manage a ChangeRequest)
5. The ChangeRequest is now either in the rejected or managed state.

# Responsible vote on a ChangeRequest
The addresses appointed by the general manager to vote on a ChangeRequest are able to accept or reject a ChangeRequest which has been managed.
1. Choose a managed ChangeRequest from the table and select ``Responsible Vote`` from the tabs (Tab is only shown, if a managed ChangeRequest is selected)
2. Add your information (vote, info)
3. Use your private key (You must use one of the appointed keys)
4. As soon as all responsible parties have accepted or one has rejected the ChangeRequest it goes either into the accepted or the rejected state.
