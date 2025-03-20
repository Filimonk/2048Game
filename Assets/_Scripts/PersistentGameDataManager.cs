using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace _Scripts
{
    public class PersistentGameDataManager
    {
        private BinaryFormatter formatter;
        private FileStream stream;
        
        public PersistentGameDataManager(string filePath = null)
        {
            filePath ??= Path.Combine(Application.persistentDataPath, "PersistentDataPath.dat");
            formatter = new BinaryFormatter();
            stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        }

        public void Close()
        {
            stream.Close();
        }
        
        public bool LoadPersistentGameData(ref PersistentGameData persistentGameData)
        {
            try
            {
                if (stream.Length > 0)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    persistentGameData = (PersistentGameData) formatter.Deserialize(stream);
                }
                else
                {
                    persistentGameData = new PersistentGameData();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void SavePersistentGameData(PersistentGameData persistentGameData)
        {
            stream.Seek(0, SeekOrigin.Begin);
            formatter.Serialize(stream, persistentGameData);

            // if (Random.value < 0.15f) // иногда выгружаем на диск - профилактика от неожиданного завершения 
            // {
                stream.Flush();
            // }
        }
    }
}