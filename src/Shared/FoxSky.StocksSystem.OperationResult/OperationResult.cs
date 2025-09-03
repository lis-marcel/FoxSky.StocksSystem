namespace FoxSky.StocksSystem.OperationResult;

public class OperationResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public object? Data { get; set; } = null;

    public OperationResult() { }

    public static OperationResult Succeeded(string message = "Operation succeeded.", object? data = null)
    {
        return new OperationResult { Success = true, Message = message, Data = data };
    }

    public static OperationResult Failed(string message = "Operation failed.", object? data = null)
    {
        return new OperationResult { Success = false, Message = message, Data = data };
    }
}
