using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;
using System.Xml.Serialization;

namespace Thy_El_Teknik_Kalender_9000.ModelLayer
{
  [XmlType]
  //[ProtoContract]
  public class Person
  {

    [XmlElement(Order = 24)]
    public int ID { get; private set; }
    [XmlElement(Order = 25)]
    //[ProtoMember(13)]
    public string Name { get; set; }
    [XmlElement(Order = 26)]
    //[ProtoMember(14)]
    public string Department { get; set; }
    [XmlElement(Order = 27)]
    public List<Activity> activityList { get; set; }

    public Person()
    {
      ID = -1;
      activityList = new List<Activity>();
    }
    public Person(string name)
    {
      Name = name;
      ID = -1;
      activityList = new List<Activity>();
    }
    public Person(string name, string department)
    {
      Name = name;
      Department = department;
      ID = -1;
      activityList = new List<Activity>();
    }
    public Person(int id, string name, string department)
    {
      ID = ID;
      Name = name;
      Department = department;
      activityList = new List<Activity>();
    }

    public override string ToString()
    {
      return Name;
    }
  }

  public class PersonComparer : IEqualityComparer<Person>
  {
    public bool Equals(Person x, Person y)
    {
      if (x.ID != -1 && y.ID != -1)
      {
        return x.ID == y.ID;
      }
      return x.Name == y.Name;
    }

    public int GetHashCode(Person x)
    {
      if (x.ID != -1)
      {
        return x.ID.GetHashCode();
      }
      return x.Name.GetHashCode();
    }
  }
}
