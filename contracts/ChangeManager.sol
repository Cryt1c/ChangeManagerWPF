pragma solidity ^0.4.24;

import {ChangeTracker} from "./ChangeTracker.sol";

// This contracts serves as a ChangeRequest factory
contract ChangeManager is ChangeTracker {
    address private _constructionManager;
    mapping(bytes20 => Change) private _changes;

    constructor() public {
        _constructionManager = msg.sender;
    }

    // Creates a new ChangeRequest contract and saves the address in _changes
    function createNewChangeRequest(
        bytes20 gitHash,
        string additionalInformation,
        uint256 costs,
        uint256 estimation
    )
    public
    {
        Change memory change;

        change._gitHash = gitHash;
        change._additionalInformation = additionalInformation;
        change._costs = costs;
        change._estimation = estimation;
        change._changeOwner = msg.sender;
        change._state = State.changeProposed;

        _changes[gitHash] = change;

        emit NewChangeRequest(gitHash, additionalInformation, costs, estimation);
    }

    // Function can only be run by the owner of the ChangeManager contract (construction manager). The construction
    // manager does the first review of the ChangeRequest, can reject it or employ the responsible parties who are
    // allowed to vote on the change.
    function managementVote(
        bytes20 gitHash,
        bool acceptChange,
        address[] responsibleParties,
        string voteInfo
    )
    public
    {
        Change storage change = _changes[gitHash];
        require(msg.sender == _constructionManager);
        require(change._state == State.changeProposed);

        if (acceptChange) {
            change._voteCount = responsibleParties.length;
            for (uint i = 0; i < responsibleParties.length; i++) {
                change._allowedToVote[responsibleParties[i]] = true;
            }
            change._state = State.changeManaged;
            emit NewVote(gitHash, msg.sender, acceptChange, change._state, voteInfo, change._voteCount);
        }
        else {
            change._state = State.changeRejected;
            emit NewVote(gitHash, msg.sender, acceptChange, change._state, voteInfo, 0);
        }
    }

    // The allowed parties can vote. As soon as everyone has voted the ChangeRequest is either accepted or rejected
    // TODO: If it's rejected, the changeOwner can amend the ChangeRequest and restart the voting process.
    function responsibleVote(
        bytes20 gitHash,
        bool acceptChange,
        string voteInfo
    )
    public
    {
        Change storage change = _changes[gitHash];
        require(change._state == State.changeManaged);
        require(change._allowedToVote[msg.sender]);
        change._allowedToVote[msg.sender] = false;
        if (!acceptChange) {
            change._state = State.changeRejected;
            change._voteCount = 0;
            emit NewVote(gitHash, msg.sender, acceptChange, change._state, voteInfo, 0);
        }
        else {
            change._voteCount = change._voteCount - 1;
            if (change._voteCount == 0) {
                change._state = State.changeApproved;
                emit NewVote(gitHash, msg.sender, acceptChange, change._state, voteInfo, change._voteCount);
            }
            else {
                emit NewVote(gitHash, msg.sender, acceptChange, change._state, voteInfo, change._voteCount);
            }
        }
    }

    // The ChangeRequest has been accepted and can be released.
    function releaseChange(bytes20 gitHash) public {
        require(_changes[gitHash]._state == State.changeApproved);
        _changes[gitHash]._state = State.changeReleased;
        emit NewVote(gitHash, msg.sender, true, _changes[gitHash]._state, "Released", 0);
    }

    // Returns the address of a change
    function viewChange(bytes20 gitHash)
    public
    view
    returns (string additionalInformation, uint256 costs, uint256 estimation)
    {
        return (
        _changes[gitHash]._additionalInformation,
        _changes[gitHash]._costs,
        _changes[gitHash]._estimation
        );
    }

    // Returns the state of a change
    function viewState(bytes20 gitHash)
    public
    view
    returns (State state, uint256 voteCount)
    {
        return (
        _changes[gitHash]._state,
        _changes[gitHash]._voteCount
        );
    }
}
