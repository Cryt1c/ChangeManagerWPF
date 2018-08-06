pragma solidity ^0.4.23;

contract ChangeTracker {
    enum State {
        changeProposed, changeManaged, changeApproved, changeRejected, changeReleased
    }

    struct Change {
        address _changeOwner;
        bytes20 _gitHash;
        string _additionalInformation;
        uint256 _costs;
        uint256 _estimation;
        State _state;
        mapping(address => bool) _allowedToVote;
        uint256 _voteCount;
    }

    event NewChangeRequest(
        bytes20 indexed _gitHash,
        string _additionalInformation,
        uint256 _costs,
        uint256 _estimation
    );

    // This event gets propagated every time a new Vote happens and tracks the _currentChange.state and the _votesLeft
    event NewVote(
        bytes20 indexed _gitHash,
        address _voter,
        bool _vote,
        State _currentState,
        string _voteInfo,
        uint256 _votesLeft
    );
}
