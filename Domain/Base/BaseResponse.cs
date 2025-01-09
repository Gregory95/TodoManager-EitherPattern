using System.Runtime.Serialization;

public class Either<TSuccess, TError>
{
    [DataMember(Order = 1)]
    public TSuccess Success { get; }

    [DataMember(Order = 2)]
    public TError Error { get; }

    [DataMember(Order = 3)]
    public bool IsSuccess { get; }

    private Either(TSuccess success, TError error, bool isSuccess)
    {
        Success = success;
        Error = error;
        IsSuccess = isSuccess;
    }

    public static Either<TSuccess, TError> Succeeded(TSuccess success) =>
        new Either<TSuccess, TError>(success, default, true);

    public static Either<TSuccess, TError> Failure(TError error) =>
        new Either<TSuccess, TError>(default, error, false);
}