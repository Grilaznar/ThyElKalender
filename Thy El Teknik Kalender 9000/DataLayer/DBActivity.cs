using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thy_El_Teknik_Kalender_9000.ModelLayer;

namespace Thy_El_Teknik_Kalender_9000.DataLayer
{
  public class DBActivity
  {

    private static readonly string azureConnectionString = 
      "Data Source=cleverquizer.database.windows.net;"
      + "Initial Catalog=CleverQuizerDB;"
      + "Integrated Security=False;"
      + "User ID=blackPeople;"
      + "Password=WhitePeople3;"
      + "Connect Timeout=15;"
      + "Encrypt=False;"
      + "TrustServerCertificate=True;"
      + "ApplicationIntent=ReadWrite;"
      + "MultiSubnetFailover=False";

    public int SaveCalendar(Dictionary<Person, List<Activity>> calendarData)
    {
      Dictionary<Person, List<Activity>> existingData = FetchCalendar();

      if(existingData.Count > 0)
      {
        MergeDicts(existingData, calendarData);
      }

      string query = "Insert into ";
      return -1;
    }

    public int SaveCalendarFor(Person person, List<Activity> activities)
    {
      return -1;
    }

    public Dictionary<Person, List<Activity>> FetchCalendar()
    {
      return new Dictionary<Person, List<Activity>>(new PersonComparer());
    }

    private void MergeDicts(Dictionary<Person, List<Activity>> originalDict, Dictionary<Person, List<Activity>> newDict)
    {
      foreach(KeyValuePair<Person, List<Activity>> pair in newDict)
      {
        if (originalDict.ContainsKey(pair.Key))
        {
          Person originalPerson = originalDict.Keys.First(p => p == pair.Key);
          if(originalPerson.Name != pair.Key.Name)
          {
            originalPerson.Name = pair.Key.Name;
          }
        }
        else
        {
          originalDict.Add(pair.Key, pair.Value);
        }
      }
    }
  }
}
