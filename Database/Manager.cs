using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using SapphireEngine.Functions;

namespace SapphireEngine.Database
{
    public class Manager
    {
        #region [Define]
        static Manager()
        {
            if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "/Database"))
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "/Database");
        }

        private static HashSet<string> ListCheckedPath { get; } = new HashSet<string>();
        #endregion

        #region [Method] [Method] CheckDirectory
        private static void CheckDirectory(string _directory)
        {
            if (ListCheckedPath.Contains(_directory) == false)
            {
                ListCheckedPath.Add(_directory);
                if (Directory.Exists(_directory) == false)
                    Directory.CreateDirectory(_directory);
            }
        }
        #endregion

        #region [Method] [Method] Save
        public static void Save(IDatabaseObject _database)
        {
            Type type = _database.GetType();
            CheckDirectory(AppDomain.CurrentDomain.BaseDirectory + "/Database/" + type.Name);

            using (BufferWriter writer = new BufferWriter())
            {
                PropertyInfo[] propertyes = type.GetProperties(BindingFlags.CreateInstance | BindingFlags.Instance);
                for (int i = 0; i < propertyes.Length; ++i)
                {
                    switch (propertyes[i].PropertyType.Name)
                    {
                        case "String":
                            writer.String((String)propertyes[i].GetGetMethod(true).Invoke(_database, new object[0]));
                            break;
                        case "Char":
                            writer.Char((Char)propertyes[i].GetGetMethod(true).Invoke(_database, new object[0]));
                            break;
                        case "Double":
                            writer.Double((Double)propertyes[i].GetGetMethod(true).Invoke(_database, new object[0]));
                            break;
                        case "Boolean":
                            writer.Boolean((Boolean)propertyes[i].GetGetMethod(true).Invoke(_database, new object[0]));
                            break;
                        case "Byte":
                            writer.Byte((Byte)propertyes[i].GetGetMethod(true).Invoke(_database, new object[0]));
                            break;
                        case "Single":
                            writer.Float((Single)propertyes[i].GetGetMethod(true).Invoke(_database, new object[0]));
                            break;
                        case "Byte[]":
                            Byte[] buffer = (Byte[]) propertyes[i].GetGetMethod(true).Invoke(_database, new object[0]);
                            writer.Int32(buffer.Length);
                            writer.Bytes(buffer);
                            break;
                        case "Int16":
                            writer.Int16((Int16)propertyes[i].GetGetMethod(true).Invoke(_database, new object[0]));
                            break;
                        case "Int32":
                            writer.Int32((Int32)propertyes[i].GetGetMethod(true).Invoke(_database, new object[0]));
                            break;
                        case "Int64":
                            writer.Int64((Int64)propertyes[i].GetGetMethod(true).Invoke(_database, new object[0]));
                            break;
                        case "UInt16":
                            writer.UInt16((UInt16)propertyes[i].GetGetMethod(true).Invoke(_database, new object[0]));
                            break;
                        case "UInt32":
                            writer.UInt32((UInt32)propertyes[i].GetGetMethod(true).Invoke(_database, new object[0]));
                            break;
                        case "UInt64":
                            writer.UInt64((UInt64)propertyes[i].GetGetMethod(true).Invoke(_database, new object[0]));
                            break;
                    }
                }
                
                File.WriteAllBytes(AppDomain.CurrentDomain.BaseDirectory + "/Database/" + type.Name + "/" + _database.ID + ".sdb", writer.Buffer);
            }
        }
        #endregion

        #region [Method] [Method] LoadAll
        
        public static T[] LoadAll<T>() => (T[])(object)LoadAll(typeof(T));

        public static IDatabaseObject[] LoadAll(Type _type)
        {
            List<IDatabaseObject> databaseObjects = new List<IDatabaseObject>();
            FileInfo[] files = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "/Database/" + _type.Name).GetFiles();
            for (var i = 0; i < files.Length; ++i)
            {
                IDatabaseObject databaseObject = Load(_type, files[i].Name.Substring(0, files[i].Name.IndexOf('.')));
                if (databaseObject != null)
                    databaseObjects.Add(databaseObject);
            }
            return databaseObjects.ToArray();
        }
        
        #endregion

        #region [Method] [Example] Load
        public static T Load<T>(string _uid) => (T)Load(typeof(T), _uid);

        public static IDatabaseObject Load(Type _type, string _uid)
        {
            IDatabaseObject database = null;
            
            CheckDirectory(AppDomain.CurrentDomain.BaseDirectory + "/Database/" + _type.Name);

            try
            {
                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "/Database/" + _type.Name + "/" + _uid + ".sdb"))
                {
                    database = (IDatabaseObject) _type.Assembly.CreateInstance(_type.FullName);
                    database.ID = _uid;

                    byte[] buffer_database = File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + "/Database/" + _type.Name + "/" + _uid + ".sdb");

                    using (BufferReader reader = new BufferReader(buffer_database))
                    {
                        PropertyInfo[] propertyes = database.GetType().GetProperties(BindingFlags.CreateInstance | BindingFlags.Instance);
                        for (int i = 0; i < propertyes.Length; ++i)
                        {
                            switch (propertyes[i].PropertyType.Name)
                            {
                                case "String":
                                    propertyes[i].GetSetMethod(true).Invoke(database, new object[] {reader.String()});
                                    break;
                                case "Char":
                                    propertyes[i].GetSetMethod(true).Invoke(database, new object[] {reader.Char()});
                                    break;
                                case "Double":
                                    propertyes[i].GetSetMethod(true).Invoke(database, new object[] {reader.Double()});
                                    break;
                                case "Boolean":
                                    propertyes[i].GetSetMethod(true).Invoke(database, new object[] {reader.Boolean()});
                                    break;
                                case "Byte":
                                    propertyes[i].GetSetMethod(true).Invoke(database, new object[] {reader.Byte()});
                                    break;
                                case "Single":
                                    propertyes[i].GetSetMethod(true).Invoke(database, new object[] {reader.Float()});
                                    break;
                                case "Byte[]":
                                    int buffer_len = reader.Int32();
                                    propertyes[i].GetSetMethod(true).Invoke(database, new object[] {reader.Bytes(buffer_len)});
                                    break;
                                case "Int16":
                                    propertyes[i].GetSetMethod(true).Invoke(database, new object[] {reader.Int16()});
                                    break;
                                case "Int32":
                                    propertyes[i].GetSetMethod(true).Invoke(database, new object[] {reader.Int32()});
                                    break;
                                case "Int64":
                                    propertyes[i].GetSetMethod(true).Invoke(database, new object[] {reader.Int64()});
                                    break;
                                case "UInt16":
                                    propertyes[i].GetSetMethod(true).Invoke(database, new object[] {reader.UInt16()});
                                    break;
                                case "UInt32":
                                    propertyes[i].GetSetMethod(true).Invoke(database, new object[] {reader.UInt32()});
                                    break;
                                case "UInt64":
                                    propertyes[i].GetSetMethod(true).Invoke(database, new object[] {reader.UInt64()});
                                    break;
                            }
                        }
                    }

                    return database;
                }
            }
            catch (Exception ex)
            {
                ConsoleSystem.LogError($"[Database.Manager]: Exception from load {_type.Name} id {_uid} database: " + ex.Message);   
            }
            return null;
        }
        #endregion
    }
}