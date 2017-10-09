using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;
using System.Xml.Serialization;

namespace Thy_El_Teknik_Kalender_9000.ModelLayer
{
  [XmlType]
  //[ProtoContract]
  public class Activity
  {
    public enum activityType
    {
      None = 0,
      Fridag,
      Afspadsering,
      Kursus,
      Projekt,
      Selvvalgt1,
      Selvvalgt2,
      Selvvalgt3
    }

    [XmlElement(Order = 20)]
    //[ProtoMember(10)]
    public DateTime Date { get; set; }
    [XmlElement(Order = 21)]
    //[ProtoMember(11)]
    public activityType ActivityCode { get; set; }
    [XmlElement(Order = 22)]
    //[ProtoMember(12)]
    public string Description { get; set; }

    public Activity() { }
    public Activity(DateTime date, activityType activity, string descritpion = "")
    {
      Date = date;
      ActivityCode = activity;
      Description = descritpion;
    }

    public override string ToString()
    {
      return Date.ToString() + "-" + ActivityCode;
    }
  }
}
