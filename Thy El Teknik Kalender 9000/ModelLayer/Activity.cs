using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace Thy_El_Teknik_Kalender_9000.ModelLayer
{
  [ProtoContract]
  public class Activity
  {
    public enum activityType {
      None = 0,
      Offday,
      Counterbalance,
      Course,
      Project}

    [ProtoMember(10)]
    public DateTime Date { get; set; }
    [ProtoMember(11)]
    public activityType ActivityCode { get; set; }
    [ProtoMember(12)]
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
