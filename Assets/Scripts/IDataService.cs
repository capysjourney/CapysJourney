public interface IDataService
{
    bool SaveData<T>(string RelativePath, T Data, bool Encrypted = true);

    T LoadData<T>(string RelativePath, bool Encrypted = true);
}