namespace AermecNamespace
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Windows.Forms;

    public class FilesManage
    {
        private static string extensionFileDatabase = ".Adb";
        private static string aermecDatabaseFilePath = (Application.StartupPath + @"\AermecDatabase" + extensionFileDatabase);
        private static string BmsPath = (Application.StartupPath + @"\BMS\");
        private static bool cryptography = true;
        private static string extensionFileIndex = ".xml";
        private static string extensionFileLog = ".bin";
        private static string extensionFileXml = ".xml";
        private static string fileIndex = "index";
        private static string fileLog = "log";
        private static string LogPath = (BmsPath + @"Log\");
        private static bool saveToXml = false;

        public static void CreateZipFile(string pathZipFile, string[] filesToZip)
        {
        }

        public static void DeleteAllBmsLogs(string name)
        {
            if (Directory.Exists(LogPath + name))
            {
                Directory.Delete(LogPath + name, true);
            }
        }

        public static void DeleteBmsLog(string name)
        {
            if (Directory.Exists(LogPath + name))
            {
                Directory.Delete(LogPath + name, true);
            }
        }

        public static void DeleteUserBmsFile(string name)
        {
            if (File.Exists(BmsPath + name + extensionFileDatabase))
            {
                try
                {
                    File.Delete(BmsPath + name + extensionFileDatabase);
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        public static void DeleteUserDataStorage(DataStorage.DataStorageIndex dataIndex)
        {
            string str = LogPath + dataIndex.bmsProject.Name + @"\";
            DataStorage.DataStorageIndex objectToLoad = new DataStorage.DataStorageIndex();
            if (dataIndex.Count() != 0)
            {
                string path = "";
                for (path = GetNameIndexFile(dataIndex); File.Exists(path); path = GetNameIndexFile(dataIndex))
                {
                    objectToLoad = (DataStorage.DataStorageIndex) SerializzaOggettiXML.Load(objectToLoad, GetNameIndexFile(dataIndex));
                    if (objectToLoad.fileTimeSnaphot[0].Equals(dataIndex.fileTimeSnaphot[0]))
                    {
                        try
                        {
                            for (int i = 0; i < dataIndex.fileDataStorageName.Length; i++)
                            {
                                File.Delete(str + dataIndex.fileDataStorageName[i]);
                            }
                        }
                        catch (Exception exception)
                        {
                            MessageBox.Show(exception.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                        try
                        {
                            File.Delete(path);
                            break;
                        }
                        catch (Exception exception2)
                        {
                            MessageBox.Show(exception2.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            break;
                        }
                    }
                    dataIndex.logNumber++;
                }
            }
        }

        public static void DeleteUserDataStorage(string data)
        {
        }

        public static DataStorage.DataStorageIndex[] GetAllDataLogIndex()
        {
            DataStorage.DataStorageIndex[] indexArray = new DataStorage.DataStorageIndex[1];
            string str = "";
            string[] directories = Directory.GetDirectories(LogPath);
            for (int i = 0; i < directories.Length; i++)
            {
                directories[i] = directories[i].Substring(directories[i].LastIndexOf(@"\") + 1);
            }
            Directory.GetFiles(LogPath + str, "index*");
            return indexArray;
        }

        public static DataStorage.DataStorageIndex[] GetDataLogIndex(string bmsName)
        {
            string[] files;
            int num2;
            int num = 0;
            DataStorage.DataStorageIndex[] c = new DataStorage.DataStorageIndex[0];
            Directory.GetDirectories(LogPath);
            ArrayList list = new ArrayList();
            try
            {
                files = Directory.GetFiles(LogPath + bmsName, "index*");
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message + "\n" + LogPath + bmsName, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return c;
            }
            c = new DataStorage.DataStorageIndex[files.Length];
            for (num2 = 0; num2 < c.Length; num2++)
            {
                c[num2] = new DataStorage.DataStorageIndex();
            }
            for (num2 = 0; num2 < c.Length; num2++)
            {
                try
                {
                    c[num2] = (DataStorage.DataStorageIndex) SerializzaOggettiXML.Load(c[num2], files[num2]);
                    num++;
                }
                catch (Exception)
                {
                    c[num2] = null;
                }
            }
            if (num < files.Length)
            {
                DataStorage.DataStorageIndex[] indexArray2 = new DataStorage.DataStorageIndex[num];
                int num3 = 0;
                for (num2 = 0; num2 < c.Length; num2++)
                {
                    if (c[num2] != null)
                    {
                        indexArray2[num3++] = c[num2];
                    }
                }
                c = indexArray2;
                indexArray2 = null;
            }
            list.AddRange(c);
            list.Sort(new DataStorage.DataStorageIndex.DataStorageIndexComparer());
            list.CopyTo(c);
            return c;
        }

        public static string[] GetDataLogProjectNames()
        {
            string[] directories;
            try
            {
                directories = Directory.GetDirectories(LogPath);
            }
            catch (Exception)
            {
                return new string[0];
            }
            for (int i = 0; i < directories.Length; i++)
            {
                directories[i] = directories[i].Substring(directories[i].LastIndexOf(@"\") + 1);
            }
            return directories;
        }

        private static string GetNameIndexFile(DataStorage.DataStorageIndex dataIndex)
        {
            string str = LogPath + dataIndex.bmsProject.Name + @"\";
            return (str + fileIndex + dataIndex.logNumber.ToString() + extensionFileIndex);
        }

        public static BmsProject LoadAermecDatabaseFromFile()
        {
            BmsProject objectToLoad = new BmsProject();
            try
            {
                objectToLoad = (BmsProject) SerializzaOggettiXML.Load(objectToLoad, aermecDatabaseFilePath, true);
            }
            catch (Exception)
            {
            }
            return objectToLoad;
        }

        public static BmsProject LoadAermecDatabaseFromFile(string pathFile)
        {
            BmsProject objectToLoad = new BmsProject();
            try
            {
                objectToLoad = (BmsProject) SerializzaOggettiXML.Load(objectToLoad, pathFile, true);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
            return objectToLoad;
        }

        public static BmsProject[] LoadUserBmsDatabaseFromFile()
        {
            BmsProject objectToLoad = new BmsProject();
            BmsProject[] projectArray = new BmsProject[0];
            int index = 0;
            if (Directory.Exists(BmsPath))
            {
                string[] files = Directory.GetFiles(BmsPath);
                projectArray = new BmsProject[files.Length];
                foreach (string str in files)
                {
                    try
                    {
                        projectArray[index] = (BmsProject) SerializzaOggettiXML.Load(objectToLoad, str, true);
                        index++;
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            return projectArray;
        }

        public static BmsProject LoadUserBmsFromFile(string name)
        {
            BmsProject objectToLoad = new BmsProject();
            try
            {
                objectToLoad = (BmsProject) SerializzaOggettiXML.Load(objectToLoad, BmsPath + name + extensionFileDatabase, true);
            }
            catch (Exception)
            {
            }
            return objectToLoad;
        }

        public static DataStorage LoadUserDataStorage(DataStorage.DataStorageIndex dataIndex)
        {
            string str = LogPath + dataIndex.bmsProject.Name + @"\";
            int num = 0;
            DataStorage.DataStorageIndex objectToLoad = new DataStorage.DataStorageIndex();
            DataStorage storage = new DataStorage();
            DataStorage storage2 = new DataStorage();
            string path = "";
            for (path = GetNameIndexFile(dataIndex); File.Exists(path); path = GetNameIndexFile(dataIndex))
            {
                objectToLoad = (DataStorage.DataStorageIndex) SerializzaOggettiXML.Load(objectToLoad, GetNameIndexFile(dataIndex));
                if (objectToLoad.fileTimeSnaphot[0].Equals(dataIndex.fileTimeSnaphot[0]))
                {
                    for (num = 0; num < objectToLoad.fileDataStorageName.Length; num++)
                    {
                        if (File.Exists(str + objectToLoad.fileDataStorageName[num]))
                        {
                            storage = (DataStorage) SerializzaOggettiXML.BinaryLoad(str + objectToLoad.fileDataStorageName[num]);
                            storage2.DataSnapShots.AddRange(storage.DataSnapShots);
                        }
                    }
                    return storage2;
                }
                dataIndex.logNumber++;
            }
            return storage2;
        }

        public static DataStorage LoadUserDataStorage(DataStorage.DataStorageIndex dataIndex, DateTime start, DateTime stop)
        {
            string str = LogPath + dataIndex.bmsProject.Name + @"\";
            int num = 0;
            DataStorage.DataStorageIndex objectToLoad = new DataStorage.DataStorageIndex();
            DataStorage storage = new DataStorage();
            DataStorage storage2 = new DataStorage();
            string path = "";
            for (path = GetNameIndexFile(dataIndex); File.Exists(path); path = GetNameIndexFile(dataIndex))
            {
                objectToLoad = (DataStorage.DataStorageIndex) SerializzaOggettiXML.Load(objectToLoad, GetNameIndexFile(dataIndex));
                if (objectToLoad.fileTimeSnaphot[0].Equals(dataIndex.fileTimeSnaphot[0]))
                {
                    for (num = 0; num < objectToLoad.fileDataStorageName.Length; num++)
                    {
                        if (((objectToLoad.fileTimeSnaphot[num] >= start) && (objectToLoad.fileTimeSnaphot[num] <= stop)) && File.Exists(str + objectToLoad.fileDataStorageName[num]))
                        {
                            if ((storage2.DataSnapShots.Count == 0) && (num > 0))
                            {
                                storage = (DataStorage) SerializzaOggettiXML.BinaryLoad(str + objectToLoad.fileDataStorageName[num - 1]);
                                storage2.DataSnapShots.AddRange(storage.DataSnapShots);
                            }
                            storage = (DataStorage) SerializzaOggettiXML.BinaryLoad(str + objectToLoad.fileDataStorageName[num]);
                            if (storage == null)
                            {
                                return null;
                            }
                            storage2.DataSnapShots.AddRange(storage.DataSnapShots);
                        }
                    }
                    return storage2;
                }
                dataIndex.logNumber++;
            }
            return storage2;
        }

        public static void SaveAermecDatabase(BmsProject aermecDatabase)
        {
            if (saveToXml)
            {
                SerializzaOggettiXML.Save(aermecDatabase, aermecDatabaseFilePath + extensionFileXml, false);
            }
            SerializzaOggettiXML.Save(aermecDatabase, aermecDatabaseFilePath, cryptography);
        }

        public static void SaveAermecDatabase(BmsProject aermecDatabase, string pathFile)
        {
            SerializzaOggettiXML.Save(aermecDatabase, pathFile, cryptography);
        }

        public static void SaveUserBmsFile(BmsProject userBms)
        {
            if (!Directory.Exists(BmsPath))
            {
                Directory.CreateDirectory(BmsPath);
            }
            SerializzaOggettiXML.Save(userBms, BmsPath + userBms.Name + extensionFileDatabase, cryptography);
            if (saveToXml)
            {
                SerializzaOggettiXML.Save(userBms, BmsPath + userBms.Name + extensionFileDatabase + extensionFileXml, false);
            }
        }

        public static void SaveUserBmsLog(DataStorage.DataStorageIndex dataIndex, DataStorage dataStorage)
        {
            string path = LogPath + dataIndex.bmsProject.Name + @"\";
            int index = 0;
            string str2 = "";
            string nameIndexFile = "";
            new DataStorage.DataStorageIndex();
            if (!dataIndex.logging)
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                for (nameIndexFile = GetNameIndexFile(dataIndex); File.Exists(nameIndexFile); nameIndexFile = GetNameIndexFile(dataIndex))
                {
                    index++;
                    dataIndex.logNumber = index;
                }
                dataIndex.logging = true;
            }
            str2 = dataIndex.logNumber.ToString();
            for (index = 0; index < dataIndex.fileDataStorageName.Length; index++)
            {
                if (dataIndex.fileDataStorageName[index] == "")
                {
                    dataIndex.fileDataStorageName[index] = fileLog + str2 + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + extensionFileLog;
                }
            }
            nameIndexFile = GetNameIndexFile(dataIndex);
            SerializzaOggettiXML.Save(dataIndex, nameIndexFile);
            if (saveToXml)
            {
                SerializzaOggettiXML.Save(dataStorage, path + dataIndex.fileDataStorageName[dataIndex.fileDataStorageName.Length - 1] + extensionFileXml);
            }
            SerializzaOggettiXML.BinarySave(dataStorage, path + dataIndex.fileDataStorageName[dataIndex.fileDataStorageName.Length - 1]);
        }

        public static void UpdateUserDataIndex(DataStorage.DataStorageIndex dataIndex)
        {
            string path = LogPath + dataIndex.bmsProject.Name + @"\";
            string xMLFilePathName = "";
            new DataStorage.DataStorageIndex();
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            xMLFilePathName = GetNameIndexFile(dataIndex);
            SerializzaOggettiXML.Save(dataIndex, xMLFilePathName);
        }
    }
}

