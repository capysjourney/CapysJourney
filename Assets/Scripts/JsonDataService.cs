using Newtonsoft.Json;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

// https://www.youtube.com/watch?v=mntS45g8OK4
public class JsonDataService : IDataService
{
    private const string KEY = "T33avRCdG13nu2NCRTMh3QjrFTt03KCFyuScfwiAe4k=";
    private const string IV = "ZkeHbE8PtmNZZOtR5YD3Ew==";

    public bool SaveData<T>(string RelativePath, T Data, bool Encrypted=true)
    {
        string path = Path.Combine(Application.persistentDataPath, RelativePath);

        try
        {
            if (File.Exists(path))
            {
                //Debug.Log("Data exists. Deleting old file and writing a new one!");
                File.Delete(path);
            }
            else
            {
                //Debug.Log("Writing file for the first time!");
            }
            using FileStream stream = File.Create(path);
            if (Encrypted)
            {
                WriteEncryptedData(Data, stream);
            }
            else
            {
                stream.Close();
                File.WriteAllText(path, JsonConvert.SerializeObject(Data));
            }
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Unable to save data due to: {e.Message} {e.StackTrace}");
            return false;
        }
    }

    private void WriteEncryptedData<T>(T Data, FileStream Stream)
    {
        using Aes aesProvider = Aes.Create();
        aesProvider.Key = Convert.FromBase64String(KEY);
        aesProvider.IV = Convert.FromBase64String(IV);
        using ICryptoTransform cryptoTransform = aesProvider.CreateEncryptor();
        using CryptoStream cryptoStream = new(
            Stream,
            cryptoTransform,
            CryptoStreamMode.Write
        );

        // You can uncomment the below to see a generated value for the IV & key.
        // You can also generate your own if you wish
        //Debug.Log($"Initialization Vector: {Convert.ToBase64String(aesProvider.IV)}");
        //Debug.Log($"Key: {Convert.ToBase64String(aesProvider.Key)}");
        string s = JsonConvert.SerializeObject(Data);
        //Debug.Log($"writing: {s}");
        cryptoStream.Write(Encoding.ASCII.GetBytes(s));
    }


    public T LoadData<T>(string RelativePath, bool Encrypted=true)
    {
        string path = Path.Combine(Application.persistentDataPath, RelativePath);

        if (!File.Exists(path))
        {
            Debug.LogError($"Cannot load file at {path}. File does not exist!");
            throw new FileNotFoundException($"{path} does not exist!");
        }

        try
        {
            T data;
            if (Encrypted)
            {
                data = ReadEncryptedData<T>(path);
            }
            else
            {
                data = JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
            }
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load data due to: {e.Message} {e.StackTrace}");
            throw e;
        }
    }

    private T ReadEncryptedData<T>(string Path)
    {
        byte[] fileBytes = File.ReadAllBytes(Path);
        using Aes aesProvider = Aes.Create();

        aesProvider.Key = Convert.FromBase64String(KEY);
        aesProvider.IV = Convert.FromBase64String(IV);

        using ICryptoTransform cryptoTransform = aesProvider.CreateDecryptor(
            aesProvider.Key,
            aesProvider.IV
        );
        using MemoryStream decryptionStream = new(fileBytes);
        using CryptoStream cryptoStream = new(
            decryptionStream,
            cryptoTransform,
            CryptoStreamMode.Read
        );
        using StreamReader reader = new(cryptoStream);

        string result = reader.ReadToEnd();

        // if not legible, probably wrong key or iv
        //Debug.Log($"Decrypted result: {result}");
        return JsonConvert.DeserializeObject<T>(result);
    }
}