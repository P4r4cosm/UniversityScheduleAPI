namespace LAB1_WEB_API.Interfaces.DataSaver;

public record SaveResult(bool Success, string Message, Exception? Error = null);

public interface IDataSaver<TData>
{
    Task<SaveResult> SaveAsync(TData data);
}