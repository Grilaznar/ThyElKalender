using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Thy_El_Teknik_Kalender_9000
{
  /// <summary>
  /// Contains <see cref="DateTime"/> utility methods.
  /// </summary>
  public static class HolidayCalculator
  {
    public static List<DateTime> HolidayColumnsInPeriod(DateTime start, DateTime end)
    {
      bool keepGoing = true;
      List<DateTime> list = new List<DateTime>();
      //List<int> years = new List<int>();
      for (int i = start.Year; i <= end.Year; i++)
      {
        DateTime easter = CalculateEasterSunday(i);
        TryAdd(Nytaardag(i), start, end, ref list, out keepGoing);
        if (!keepGoing && list.Count > 0) { break; }

        //Dage der kan være svære at pladsere i forhold til Påske
        TryAdd(Grundlovsdag(i), start, end, ref list, out keepGoing);
        TryAdd(FirstMay(i), start, end, ref list, out keepGoing);
        TryAdd(Fastelavn(i), start, end, ref list, out keepGoing);

        TryAdd(easter.AddDays(-7), start, end, ref list, out keepGoing);
        if (!keepGoing && list.Count > 0) { break; }
        
        TryAdd(easter.AddDays(-3), start, end, ref list, out keepGoing);
        if (!keepGoing && list.Count > 0) { break; }
        
        TryAdd(easter.AddDays(-2), start, end, ref list, out keepGoing);
        if (!keepGoing && list.Count > 0) { break; }

        TryAdd(easter, start, end, ref list, out keepGoing);
        if (!keepGoing && list.Count > 0) { break; }
        
        TryAdd(easter.AddDays(1), start, end, ref list, out keepGoing);
        if (!keepGoing && list.Count > 0) { break; }
        
        TryAdd(easter.AddDays(26), start, end, ref list, out keepGoing);
        if (!keepGoing && list.Count > 0) { break; }
        
        TryAdd(easter.AddDays(39), start, end, ref list, out keepGoing);
        if (!keepGoing && list.Count > 0) { break; }
        
        TryAdd(easter.AddDays(49), start, end, ref list, out keepGoing);
        if (!keepGoing && list.Count > 0) { break; }
        
        TryAdd(easter.AddDays(50), start, end, ref list, out keepGoing);
        if (!keepGoing && list.Count > 0) { break; }
        
        TryAdd(Juleaften(i), start, end, ref list, out keepGoing);
        if (!keepGoing && list.Count > 0) { break; }
        
        TryAdd(Juledag(i), start, end, ref list, out keepGoing);
        if (!keepGoing && list.Count > 0) { break; }
        
        TryAdd(AndenJuledag(i), start, end, ref list, out keepGoing);
        if (!keepGoing && list.Count > 0) { break; }
        
        TryAdd(Nytaarsaften(i), start, end, ref list, out keepGoing);
        if (!keepGoing && list.Count > 0) { break; }
      }
      return list;
    }

    private static void TryAdd(
        DateTime toAdd, 
        DateTime startCheck, 
        DateTime endCheck, 
        ref List<DateTime> list, 
        out bool success)
    {
      if(list == null) { list = new List<DateTime>(); }
      if(startCheck <= toAdd && toAdd <= endCheck)
      {
        list.Add(toAdd);
        success = true;
      }
      else
      {
        success = false;
      }
      //return list;
    }

    //private void TryAdd(DateTime date, out List<DateTime> list)
    //{
    //  if()
    //  return null;
    //}

    public static bool IsHoliday(DateTime date)
    {
      date = date.Date;
      int year = date.Year;
      if(date == Grundlovsdag(year) 
        || date == Juleaften(year)
        || date == Juledag(year) 
        || date == AndenJuledag(year)
        || date == Nytaarsaften(year)
        || date == Nytaardag(year)
        || date == FirstMay(year)
        || date == PalmSunday(year)
        || date == MaundyThursday(year)
        || date == GoodFriday(year)
        || date == CalculateEasterSunday(year)
        || date == EasterMonday(year)
        || date == GreatPrayerDay(year)
        || date == AscensionDay(year)
        || date == WhitSunday(year)
        || date == WhitMonday(year))
      {
        return true;
      }
      return false;
    }

    #region -- Fixed holidays --
    //Faste danske helligdage

      public static DateTime FirstMay(int year)
    {
      if (year > DateTime.MaxValue.Year)
      {
        throw new ArgumentException("Invalid year value");
      }

      int day = 1;
      int month = 5;
      return new DateTime(year, month, day);
    }

    public static DateTime Grundlovsdag(int year)
    {
      if (year > DateTime.MaxValue.Year)
      {
        throw new ArgumentException("Invalid year value");
      }

      int day = 5;
      int month = 6;
      return new DateTime(year, month, day);
    }

    public static DateTime Juleaften(int year)
    {
      if (year > DateTime.MaxValue.Year)
      {
        throw new ArgumentException("Invalid year value");
      }

      int day = 24;
      int month = 12;
      return new DateTime(year, month, day);
    }
    
    public static DateTime Juledag(int year)
    {
      if (year > DateTime.MaxValue.Year)
      {
        throw new ArgumentException("Invalid year value");
      }

      int day = 25;
      int month = 12;
      return new DateTime(year, month, day);
    }

    public static DateTime AndenJuledag(int year)
    {
      if (year > DateTime.MaxValue.Year)
      {
        throw new ArgumentException("Invalid year value");
      }

      int day = 26;
      int month = 12;
      return new DateTime(year, month, day);
    }

    public static DateTime Nytaarsaften(int year)
    {
      if (year > DateTime.MaxValue.Year)
      {
        throw new ArgumentException("Invalid year value");
      }

      int day = 31;
      int month = 12;
      return new DateTime(year, month, day);
    }

    public static DateTime Nytaardag(int year)
    {
      if (year > DateTime.MaxValue.Year)
      {
        throw new ArgumentException("Invalid year value");
      }

      int day = 1;
      int month = 1;
      return new DateTime(year, month, day);
    }
    #endregion

    #region -- Moving holydays --

    /// <summary>
    /// Gets the palm sunday of the specified <paramref name="year"/>.
    /// </summary>
    /// <param name="year">The year.</param>
    /// <returns></returns>
    /// <remarks>Sunday before Easter.</remarks>
    public static DateTime Fastelavn(int year)
    {
      if (year > DateTime.MaxValue.Year)
      {
        throw new ArgumentException("Invalid year value");
      }
      return CalculateEasterSunday(year).AddDays(-(7 * 7));
    }

    public static DateTime PalmSunday(int year)
    {
      if (year > DateTime.MaxValue.Year)
      {
        throw new ArgumentException("Invalid year value");
      }
      return CalculateEasterSunday(year).AddDays(-7);
    }

    /// <summary>
    /// Gets the maundy thursday of the specified <paramref name="year"/>.
    /// </summary>
    /// <param name="year">The year.</param>
    /// <returns></returns>
    /// <remarks>The Thursday before Easter; commemorates the Last Supper.</remarks>
    public static DateTime MaundyThursday(int year)
    {
      if (year > DateTime.MaxValue.Year)
      {
        throw new ArgumentException("Invalid year value");
      }
      return CalculateEasterSunday(year).AddDays(-3);
    }

    /// <summary>
    /// Gets the good friday of the specified <paramref name="year"/>. 
    /// </summary>
    /// <param name="year">The year.</param>
    /// <returns></returns>
    /// <remarks>Friday before Easter.</remarks>
    public static DateTime GoodFriday(int year)
    {
      if (year > DateTime.MaxValue.Year)
      {
        throw new ArgumentException("Invalid year value");
      }
      return MaundyThursday(year).AddDays(1);
    }

    /// <summary>
    /// Gets the easter monday of the specified <paramref name="year"/>.
    /// </summary>
    /// <param name="year">The year.</param>
    /// <returns></returns>
    /// <remarks></remarks>
    public static DateTime EasterMonday(int year)
    {
      if (year > DateTime.MaxValue.Year)
      {
        throw new ArgumentException("Invalid year value");
      }
      return CalculateEasterSunday(year).AddDays(1);
    }

    /// <summary>
    /// Gets the great prayer day of the specified <paramref name="year"/>.
    /// </summary>
    /// <param name="year">The year.</param>
    /// <returns></returns>
    /// <remarks>This is a specific danish holyday.</remarks>
    public static DateTime GreatPrayerDay(int year)
    {
      if (year > DateTime.MaxValue.Year)
      {
        throw new ArgumentException("Invalid year value");
      }
      //fourth friday after easter.
      return CalculateEasterSunday(year).AddDays(5 + 3 * 7);
    }

    /// <summary>
    /// Gets the ascension day of the specified <paramref name="year"/>.
    /// </summary>
    /// <param name="year">The year.</param>
    /// <returns></returns>
    /// <remarks>Celebration of the Ascension of Christ into heaven; observed on the 40th day after Easter</remarks>
    public static DateTime AscensionDay(int year)
    {
      if (year > DateTime.MaxValue.Year)
      {
        throw new ArgumentException("Invalid year value");
      }
      //sixth thursday after easter.
      return CalculateEasterSunday(year).AddDays(39);
    }

    /// <summary>
    /// Gets the whit sunday of the specified <paramref name="year"/>.
    /// </summary>
    /// <param name="year">The year.</param>
    /// <returns></returns>
    /// <remarks>Seventh Sunday after Easter; commemorates the emanation of the Holy Spirit to the Apostles; a quarter day in Scotland.</remarks>
    public static DateTime WhitSunday(int year)
    {
      if (year > DateTime.MaxValue.Year)
      {
        throw new ArgumentException("Invalid year value");
      }
      return CalculateEasterSunday(year).AddDays(7 * 7);
    }

    /// <summary>
    /// Gets the whit monday of the specified <paramref name="year"/>.
    /// </summary>
    /// <param name="year">The year.</param>
    /// <returns></returns>
    /// <remarks>The day after Whitsunday; a legal holiday in England and Wales and Ireland.</remarks>
    public static DateTime WhitMonday(int year)
    {
      if (year > DateTime.MaxValue.Year)
      {
        throw new ArgumentException("Invalid year value");
      }
      return WhitSunday(year).AddDays(1);
    }

    /// <summary>
    /// Calculates easter sunday for the specified <paramref name="year"/>.
    /// </summary>
    /// <param name="year">The year.</param>
    /// <returns>The <see cref="DateTime">date</see> of easter sunday.</returns>
    /// <remarks>This method uses the algorithm specified in the wikipedia article: <a href="http://en.wikipedia.org/wiki/Computus">Computus</a>.</remarks>
    public static DateTime CalculateEasterSunday(int year)
    {
      if (year > DateTime.MaxValue.Year)
      {
        throw new ArgumentException("Invalid year value");
      }
      int a = year % 19;
      int b = year / 100;
      int c = year % 100;
      int d = b / 4;
      int e = b % 4;
      int f = (b + 8) / 25;
      int g = (b - f + 1) / 3;
      int h = (19 * a + b - d - g + 15) % 30;
      int i = c / 4;
      int k = c % 4;
      int l = (32 + 2 * e + 2 * i - h - k) % 7;
      int m = (a + 11 * h + 22 * l) / 451;
      int month = (h + l - 7 * m + 114) / 31;
      int day = ((h + l - 7 * m + 114) % 31) + 1;
      return new DateTime(year, month, day).Date;
    }
    #endregion

    #region -- Weeknumber --
    /// <summary>
    /// Gets the week number of the specified date.
    /// </summary>
    /// <param path="year">The year.</param>
    /// <param path="month">The month.</param>
    /// <param path="day">The day.</param>
    /// <returns></returns>
    [SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow")]
    public static int WeekNumber(int year, int month, int day)
    {
      //hrmpf
      // System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("sv-SE", false);
      // int week = culture.Calendar.GetWeekOfYear(DateTime.Now, System.Globalization.CalendarWeekRule.FirstFourDayWeek, System.DayOfWeek.Monday);
      //
      if (year > DateTime.MaxValue.Year)
      {
        throw new ArgumentException("Invalid year value");
      }
      if (month > DateTime.MaxValue.Month)
      {
        throw new ArgumentException("Invalid month value");
      }
      if (day > DateTime.MaxValue.Day)
      {
        throw new ArgumentException("Invalid day value");
      }
      int a = (14 - month) / 12;
      int y = year + 4800 - a;
      int m = month + 12 * a - 3;
      int JD = day + (153 * m + 2) / 5 + 365 * y + y / 4 - y / 100 + y / 400 - 32045;
      int d4 = (((JD + 31741 - JD % 7) % 146097) % 36524) % 1461;
      int L = d4 / 1460;
      int d1 = ((d4 - L) % 365) + L;
      return d1 / 7 + 1;
    }

    /// <summary>
    /// Gets the week number of the specified date.
    /// </summary>
    /// <param path="date">The date.</param>
    /// <returns></returns>
    public static int WeekNumber(DateTime date)
    {
      DateTime dt = date.Date;
      return WeekNumber(dt.Year, dt.Month, dt.Day);
    }

    #endregion
  }
}

