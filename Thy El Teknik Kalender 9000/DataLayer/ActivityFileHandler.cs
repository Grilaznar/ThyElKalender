﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Thy_El_Teknik_Kalender_9000.ModelLayer;
using Thy_El_Teknik_Kalender_9000.Properties;
using ProtoBuf;

namespace Thy_El_Teknik_Kalender_9000.DataLayer
{
  public static class ActivityFileHandler
  {
    private static readonly string filePath = AppDomain.CurrentDomain.BaseDirectory;
    private static readonly string fileName = "ThyData.dta";

    public static string userDefinedSavePath = Settings.Default.CustomSavePath;

    public static string CurrentSavePath
    {
      get
      {
        if (userDefinedSavePath != null && userDefinedSavePath != "")
          return userDefinedSavePath;
        else
          return filePath ; 
      }
      set
      {
        userDefinedSavePath = Settings.Default.CustomSavePath = value;
      }
    }

    public static void SaveData(List<Person> activityData)
    {
      List<DataChunk> data = new List<DataChunk>();
      string path = CurrentSavePath + fileName;

      int numberOfDays = 0;

      foreach (Person entry in activityData)
      {
        data.Add(new DataChunk(entry.Name, entry.Department, entry.ActivityList));
        numberOfDays += entry.ActivityList.Count;
      }

      Console.WriteLine("Total number of days saved: " + numberOfDays);

      //WriteToBinaryFile(path, data);
      WriteProtoFile(path, activityData);
    }

    public static List<Person> ReadData()
    {
      List<Person> outputList = new List<Person>();
      string path = CurrentSavePath + fileName;

      if (File.Exists(path))
      {
        outputList = ReadProtoFile(path);
      }

      return outputList;
    }

    private static void WriteProtoFile(string path, List<Person> data)
    {
      string directory = path.Substring(0, path.LastIndexOf('\\') + 1);
      if (!Directory.Exists(directory))
        Directory.CreateDirectory(directory);
      using (Stream stream = File.Open(path, FileMode.Create))
      {
        Serializer.Serialize(stream, data);
      }
    }

    private static List<Person> ReadProtoFile(string path)
    {
      List<Person> list = new List<Person>();
      using (Stream stream = File.OpenRead(path))
      {
        list = Serializer.Deserialize<List<Person>>(stream);
      }
      return list;
    }

    /// <summary>
    /// Writes the given object instance to a binary file.
    /// <para>Object type (and all child types) must be decorated with the [Serializable] attribute.</para>
    /// <para>To prevent a variable from being serialized, decorate it with the [NonSerialized] attribute; cannot be applied to properties.</para>
    /// </summary>
    /// <typeparam name="T">The type of object being written to the XML file.</typeparam>
    /// <param name="filePath">The file path to write the object instance to.</param>
    /// <param name="objectToWrite">The object instance to write to the XML file.</param>
    /// <param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param>
    private static void WriteToBinaryFile<T>(string path, T objectToWrite, bool append = false)
    {
      using (Stream stream = File.Open(path, append ? FileMode.Append : FileMode.Create))
      {
        var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        binaryFormatter.Serialize(stream, objectToWrite);
      }
    }

    /// <summary>
    /// Reads an object instance from a binary file.
    /// </summary>
    /// <typeparam name="T">The type of object to read from the XML.</typeparam>
    /// <param name="filePath">The file path to read the object instance from.</param>
    /// <returns>Returns a new instance of the object read from the binary file.</returns>
    private static T ReadFromBinaryFile<T>(string path)
    {
      using (Stream stream = File.Open(path, FileMode.Open))
      {
        var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        return (T)binaryFormatter.Deserialize(stream);
      }
    }

    private static List<Activity> MakeUseableList(List<WriteableActivity> list)
    {
      List<Activity> useableList = new List<Activity>();

      foreach (WriteableActivity wAct in list)
      {
        useableList.Add(new Activity(wAct.Date, (Activity.activityType)wAct.ActivityCode));
      }

      return useableList;
    }

    private static List<WriteableActivity> MakeWriteableList(List<Activity> list)
    {
      List<WriteableActivity> writeableList = new List<WriteableActivity>();

      foreach (Activity act in list)
      {
        writeableList.Add(new WriteableActivity(act));
      }

      return writeableList;
    }

    [Serializable]
    [ProtoContract]
    private class DataChunk
    {
      [ProtoMember(1)]
      public string Name { get; set; }
      [ProtoMember(2)]
      public string Department { get; set; }
      [ProtoMember(3)]
      public List<WriteableActivity> Activities { get; set; }

      public DataChunk()
      {
        Activities = new List<WriteableActivity>();
      }

      public DataChunk(string name, string department, List<Activity> activities)
      {
        Name = name;
        Department = department;
        Activities = MakeWriteableList(activities);
      }
    }

    [Serializable]
    [ProtoContract]
    private class WriteableActivity
    {
      [ProtoMember(4)]
      public DateTime Date { get; set; }
      [ProtoMember(5)]
      public int ActivityCode { get; set; }

      public WriteableActivity() { }
      public WriteableActivity(Activity input)
      {
        Date = input.Date;
        ActivityCode = (int)input.ActivityCode;
      }
    }
  }
}
