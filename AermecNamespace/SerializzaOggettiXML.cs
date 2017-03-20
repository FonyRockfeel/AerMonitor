namespace AermecNamespace
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Security.Cryptography;
    using System.Text;
    using System.Xml.Serialization;

    public class SerializzaOggettiXML
    {
        private static byte[] IVConst = new byte[] { 0x11, 0x22, 3, 4, 5, 6, 7, 8, 9, 0x10, 0x11, 0x12, 0x13, 20, 0x15, 0x16 };
        private static byte[] IVConstOld = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0x10, 0x11, 0x12, 0x13, 20, 0x15, 0x16 };
        private static byte[] KeyConst = new byte[] { 0x11, 6, 0x13, 0x52, 0x11, 6, 0x13, 0x52, 0x11, 6, 0x13, 0x52, 15, 14, 13, 14 };

        public static object BinaryLoad(string FilePathName)
        {
            Stream serializationStream = null;
            object obj2 = null;
            try
            {
                IFormatter formatter = new BinaryFormatter();
                serializationStream = new FileStream(FilePathName, FileMode.Open, FileAccess.Read, FileShare.Read);
                obj2 = formatter.Deserialize(serializationStream);
            }
            catch
            {
            }
            finally
            {
                if (serializationStream != null)
                {
                    serializationStream.Close();
                }
            }
            return obj2;
        }

        public static bool BinarySave(object ObjectToSave, string FilePathName)
        {
            Stream serializationStream = null;
            bool flag = false;
            try
            {
                IFormatter formatter = new BinaryFormatter();
                serializationStream = new FileStream(FilePathName, FileMode.Create, FileAccess.Write, FileShare.None);
                formatter.Serialize(serializationStream, ObjectToSave);
                flag = true;
            }
            finally
            {
                if (serializationStream != null)
                {
                    serializationStream.Close();
                }
            }
            return flag;
        }

        public object DeSerializza(MemoryStream daDeSerializzare, object deSerializzato)
        {
            try
            {
                byte[] bytes = new byte[daDeSerializzare.Length];
                bytes = daDeSerializzare.ToArray();
                Encoding.UTF8.GetString(bytes, 0, (int) daDeSerializzare.Length);
                XmlSerializer serializer = new XmlSerializer(deSerializzato.GetType());
                daDeSerializzare.Position = 0L;
                deSerializzato = serializer.Deserialize(daDeSerializzare);
                return deSerializzato;
            }
            catch (Exception exception)
            {
                string message = exception.Message;
                return deSerializzato;
            }
        }

        public static object Load(object ObjectToLoad, string XMLFilePathName)
        {
            return Load(ObjectToLoad, XMLFilePathName, Encoding.Unicode);
        }

        public static object Load(object ObjectToLoad, string XMLFilePathName, bool cryptography)
        {
            FileStream stream = null;
            if (!cryptography)
            {
                return Load(ObjectToLoad, XMLFilePathName, Encoding.Unicode);
            }
            try
            {
                XmlSerializer serializer = new XmlSerializer(ObjectToLoad.GetType());
                stream = File.Open(XMLFilePathName, FileMode.Open);
                RijndaelManaged managed = new RijndaelManaged();
                byte[] rgbKey = (byte[]) KeyConst.Clone();
                byte[] rgbIV = (byte[]) IVConst.Clone();
                CryptoStream stream2 = new CryptoStream(stream, managed.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Read);
                StreamReader textReader = new StreamReader(stream2);
                ObjectToLoad = serializer.Deserialize(textReader);
                textReader.Close();
                stream2.Close();
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
            return ObjectToLoad;
        }

        public static object Load(object ObjectToLoad, string XMLFilePathName, Encoding codifica)
        {
            TextReader textReader = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(ObjectToLoad.GetType());
                textReader = new StreamReader(XMLFilePathName, codifica);
                ObjectToLoad = serializer.Deserialize(textReader);
            }
            finally
            {
                if (textReader != null)
                {
                    textReader.Close();
                }
            }
            return ObjectToLoad;
        }

        public static bool Save(object ObjectToSave, string XMLFilePathName)
        {
            return Save(ObjectToSave, XMLFilePathName, Encoding.Unicode);
        }

        public static bool Save(object ObjectToSave, string XMLFilePathName, bool cryptography)
        {
            FileStream stream = null;
            bool flag = false;
            if (!cryptography)
            {
                return Save(ObjectToSave, XMLFilePathName, Encoding.Unicode);
            }
            try
            {
                XmlSerializer serializer = new XmlSerializer(ObjectToSave.GetType());
                stream = File.Open(XMLFilePathName, FileMode.Create);
                RijndaelManaged managed = new RijndaelManaged();
                byte[] rgbKey = (byte[]) KeyConst.Clone();
                byte[] rgbIV = (byte[]) IVConst.Clone();
                CryptoStream stream2 = new CryptoStream(stream, managed.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                StreamWriter writer = new StreamWriter(stream2);
                serializer.Serialize((TextWriter) writer, ObjectToSave);
                writer.Close();
                stream2.Close();
                flag = true;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
            return flag;
        }

        public static bool Save(object ObjectToSave, string XMLFilePathName, Encoding codifica)
        {
            TextWriter textWriter = null;
            bool flag = false;
            try
            {
                XmlSerializer serializer = new XmlSerializer(ObjectToSave.GetType());
                textWriter = new StreamWriter(XMLFilePathName, false, codifica);
                serializer.Serialize(textWriter, ObjectToSave);
                flag = true;
            }
            finally
            {
                if (textWriter != null)
                {
                    textWriter.Close();
                }
            }
            return flag;
        }

        public MemoryStream Serializza(object daSerializzare)
        {
            Type type = daSerializzare.GetType();
            MemoryStream stream = new MemoryStream();
            new XmlSerializer(type).Serialize((Stream) stream, daSerializzare);
            return stream;
        }
    }
}

