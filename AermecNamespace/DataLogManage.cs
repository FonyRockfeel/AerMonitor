namespace AermecNamespace
{
    using System;
    using System.Timers;

    internal class DataLogManage
    {
        private BmsProject bmsProject;
        private DataStorage.DataStorageIndex dataIndex;
        private DataStorage dataStorage;
        private DataStorage dataStorageBackup;
        private int memNumberSnapshot = 0;
        private Timer SaveData;
        private const int TIME_AUTOSAVE = 0x493e0;

        public DataLogManage(BmsProject bms)
        {
            this.bmsProject = bms;
            this.dataStorage = new DataStorage(bms);
            this.SaveData = new Timer(300000.0);
            this.SaveData.Elapsed += new ElapsedEventHandler(this.SaveData_Elapsed);
            this.dataIndex = new DataStorage.DataStorageIndex(bms);
        }

        public void DeleteActualDataStorage()
        {
            FilesManage.DeleteUserDataStorage(this.dataIndex);
        }

        private void SaveData_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.UpdateIndexFile();
        }

        public void SaveDataToDisk()
        {
            FilesManage.SaveUserBmsLog(this.dataIndex, this.dataStorageBackup);
        }

        public void StartLogging()
        {
            this.SaveData.Start();
        }

        public void StopLogging()
        {
            this.SaveData.Stop();
            this.UpdateIndexFile();
        }

        public void UpdateData(Supervisor superv)
        {
            this.dataStorage.AddSnapshot(superv);
        }

        public void UpdateIndexFile()
        {
            if (this.dataStorage.DataSnapShots.Count > 0)
            {
                this.dataStorageBackup = this.dataStorage;
                this.memNumberSnapshot = 0;
                this.dataStorage = new DataStorage(this.bmsProject);
                this.memNumberSnapshot = this.dataStorageBackup.DataSnapShots.Count;
                this.dataIndex.AddFileStorage(((DataStorage.DataSnapShot) this.dataStorageBackup.DataSnapShots[0]).Time, "");
                this.dataIndex.lastSave = ((DataStorage.DataSnapShot) this.dataStorageBackup.DataSnapShots[this.dataStorageBackup.DataSnapShots.Count - 1]).Time;
                this.SaveDataToDisk();
            }
        }
    }
}

