namespace EmailClient.Common;

internal enum SMTPResponseCode
{
    ServerConnectionError = 101,

    //  200 series: Positive Completion reply
    SystemStatus = 211,
    HelpMessage = 214,
    ServiceReady = 220,
    ServiceClosingTransmissionChannel = 221,
    AuthenticationSuccessful = 235,
    OK = 250,
    UserNotLocalWillForward = 251,
    CannotVerifyUserWillAttemptDelivery = 252,
    RequestedSecurityMechanismAccepted = 334,
    StartMailInput = 354,

    //  400 series: Transient Negative Completion reply
    ServiceNotAvailable = 421,
    RecipientStorageLimitExceeded = 422,
    FileOverload = 431,
    NoResponseFromRecipientServer = 441,
    ConnectionDropped = 442,
    PublicLoopOccurred = 446,
    MailboxBusyOrBlocked = 450,
    LocalErrorInProcessing = 451,
    InsufficientSystemStorage = 452,
    TLSNotAvailable = 454,
    ParametersCannotBeAccommodated = 455,
    LocalSpamFilterError = 471,

    //  500 series: Permanent Negative Completion reply
    SyntaxError = 500,
    SyntaxErrorInArguments = 501,
    CommandNotImplemented = 502,
    BadCommandSequence = 503,
    CommandParameterNotImplemented = 504,
    InvalidEmailAddress = 510,
    DNSError = 512,
    RecipientServerLimitExceeded = 523,
    AuthenticationError = 530,
    AuthenticationFailed = 535,
    EncryptionRequired = 538,
    RejectedBySpamFilter = 541,
    MailboxUnavailable = 550,
    UserNotLocalTryAlternatePath = 551,
    ExceededStorageAllocation = 552,
    MailboxNameNotAllowed = 553,
    TransactionFailed = 554,
    ParameterNotRecognized = 555,

    // Custom codes: used only inside this project
    ExceptionOccurred = 1000
}
