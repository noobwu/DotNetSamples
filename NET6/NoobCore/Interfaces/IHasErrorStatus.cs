namespace NoobCore;

/// <summary>
/// Used in ApiResult
/// </summary>
public interface IHasErrorStatus
{
    ResponseStatus Error { get; }
}