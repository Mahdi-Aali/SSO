using System.Diagnostics.CodeAnalysis;

namespace Domain.SeedWork;

public class SysResult<TResult>
{
    private Lazy<List<string>>? _errors;

    public required bool IsSuccessfull { get; set; }

    public TResult? Result { get; set; }

    [SetsRequiredMembers]

    public SysResult(bool isSuccessfull) : this(isSuccessfull, default, null)
    {
        
    }

    [SetsRequiredMembers]

    public SysResult(bool isSuccessfull, TResult? result) : this(isSuccessfull, result, null)
    {
        
    }


    [SetsRequiredMembers]

    public SysResult(bool isSuccessfull, TResult? result, IEnumerable<string>? errorMessages)
    {
        IsSuccessfull = isSuccessfull;

        if (result is not null)
        {
            Result = result;
        }

        if (errorMessages is not null)
        {
            _errors = new Lazy<List<string>>(() => errorMessages.ToList());
        }

        if (!IsSuccessfull && _errors is null)
        {
            _errors = new Lazy<List<string>>();
        }
    }


    public void AddErrorMessage(string errorMessage)
    {
        if (IsSuccessfull)
        {
            IsSuccessfull = !IsSuccessfull;
        }
        _errors?.Value.Add(errorMessage);
    }

    public void AddErrorMessages(IEnumerable<string> errorMessages)
    {
        if (IsSuccessfull)
        {
            IsSuccessfull = !IsSuccessfull;
        }
        _errors?.Value.AddRange(errorMessages);
    }

    public void SetResult(TResult result) => Result = result;
}
