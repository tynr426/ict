using Microsoft.VisualBasic;
using System;
namespace ECF
{
    /// <summary>
    ///   <see cref="ECF.Utils"/>
    /// 日期类型处理单元
    /// Author:  XP
    /// Created: 2011/9/8
    /// </summary>
    public partial class Utils
    {
        #region 格式的日期转换成中文格式日期
        /// <summary>
        /// 把带"-"格式的日期转换成中文格式日期
        /// </summary>
        /// <param name="str">要转换的日期</param>
        /// <param name="shotDate">if set to <c>true</c> [shot date].</param>
        /// <returns></returns>
        public static string DTimeToCN(string str, bool shotDate)
        {
            if (String.IsNullOrEmpty(str)) return "";
            try
            {
                str = str.Replace("/", "-");
                string[] tArray = str.Split('-');
                if (tArray.Length < 2) return "";
                string[] dArray = { "年", "月", "日" };
                string[] timeArray = { "时", "分", "秒" };
                string restr = string.Empty;
                int i = 0;
                foreach (string t in tArray)
                {
                    if (i + 1 == tArray.Length)
                    {
                        if (t.IndexOf(" ") > 0)
                        {
                            if (!shotDate)
                            {
                                restr += t.Substring(0, t.IndexOf(" ")) + dArray[i];
                                string[] times = t.Substring(t.IndexOf(" "), (t.Length - t.IndexOf(" "))).Split(':');
                                for (int j = 0; j < times.Length; j++)
                                {
                                    restr += times[j] + timeArray[j];
                                }
                            }
                            else
                            {
                                restr += t.Substring(0, t.IndexOf(" ")) + dArray[i];
                            }

                        }
                        else
                        {
                            restr += t + dArray[i];
                        }
                    }
                    else
                    {
                        restr += t + dArray[i];
                    }
                    i += 1;
                }
                return restr;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion

        #region 将日期对象转化为格式字符串
        /// <summary>
        /// 将日期对象转化为格式字符串
        /// </summary>
        /// <param name="oDateTime">日期对象</param>
        /// <param name="strFormat">格式：
        /// "SHORTDATE"===短日期
        /// "LONGDATE"==长日期
        /// 其它====自定义格式</param>
        /// <returns>日期字符串</returns>
        public static string DTimeToString(object oDateTime, string strFormat)
        {
            try
            {
                DateTime d = DateTime.Parse(oDateTime.ToString());
                return DTimeToString(d, strFormat);
            }
            catch
            {
                return "";
            }
        }
        /// <summary>
        /// 将日期对象转化为格式字符串
        /// </summary>
        /// <param name="oDateTime">日期对象</param>
        /// <param name="strFormat">格式：
        /// "SHORTDATE"===短日期
        /// "LONGDATE"==长日期
        /// 其它====自定义格式</param>
        /// <returns>日期字符串</returns>
        public static string DTimeToString(DateTime oDateTime, string strFormat)
        {
            string strDate = "";

            try
            {

                switch (strFormat.ToUpper())
                {
                    case "SHORTDATE":
                        strDate = oDateTime.ToShortDateString().Substring(2);
                        break;
                    case "LONGDATE":
                        strDate = oDateTime.ToLongDateString();
                        break;
                    default:
                        strDate = oDateTime.ToString(strFormat);
                        break;
                }
            }
            catch (Exception)
            {
                strDate = oDateTime.ToShortDateString();
            }

            return strDate;
        }
        #endregion

        #region 时间差处理

        /// <summary>
        /// 返回相差的秒数
        /// </summary>
        /// <param name="Time"></param>
        /// <param name="Sec"></param>
        /// <returns></returns>
        public static int StrDateDiffSeconds(string Time, int Sec)
        {
            TimeSpan ts = DateTime.Now - DateTime.Parse(Time).AddSeconds(Sec);
            if (ts.TotalSeconds > int.MaxValue)
            {
                return int.MaxValue;
            }
            else if (ts.TotalSeconds < int.MinValue)
            {
                return int.MinValue;
            }
            return (int)ts.TotalSeconds;
        }

        /// <summary>
        /// 返回相差的分钟数
        /// </summary>
        /// <param name="time"></param>
        /// <param name="minutes"></param>
        /// <returns></returns>
        public static int StrDateDiffMinutes(string time, int minutes)
        {
            if (time == "" || time == null)
                return 1;
            TimeSpan ts = DateTime.Now - DateTime.Parse(time).AddMinutes(minutes);
            if (ts.TotalMinutes > int.MaxValue)
            {
                return int.MaxValue;
            }
            else if (ts.TotalMinutes < int.MinValue)
            {
                return int.MinValue;
            }
            return (int)ts.TotalMinutes;
        }

        /// <summary>
        /// 返回相差的小时数
        /// </summary>
        /// <param name="time"></param>
        /// <param name="hours"></param>
        /// <returns></returns>
        public static int StrDateDiffHours(string time, int hours)
        {
            if (time == "" || time == null)
                return 1;
            TimeSpan ts = DateTime.Now - DateTime.Parse(time).AddHours(hours);
            if (ts.TotalHours > int.MaxValue)
            {
                return int.MaxValue;
            }
            else if (ts.TotalHours < int.MinValue)
            {
                return int.MinValue;
            }
            return (int)ts.TotalHours;
        }

        #endregion

        /// <summary>
        /// 返回标准日期格式string
        /// </summary>
        public static string GetDate()
        {
            return DateTime.Now.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// 日期时间比较
        /// </summary>
        /// <param name="dt1">The DT1.</param>
        /// <param name="dt2">The DT2.</param>
        /// <returns>
        /// System.Int32[]，返回年，月，日，时，分，秒的相差时间
        /// </returns>
        public static int[] DateTimeDiffer(DateTime dt1, DateTime dt2)
        {
            // 对时间进行第一步比较
            if (dt1 < dt2)
            {
                DateTime dt3 = dt1;
                dt1 = dt2;
                dt2 = dt3;
            }

            return new int[]{
                dt1.Year - dt2.Year,
                dt1.Month - dt2.Month,
                dt1.Day - dt2.Day,
                dt1.Hour - dt2.Hour,
                dt1.Minute - dt2.Minute,
                dt1.Second -dt2.Second
            };
        }

        /// <summary>
        /// 返回指定日期格式
        /// </summary>
        public static string GetDate(string datetimestr, string replacestr)
        {
            if (datetimestr == null)
                return replacestr;

            if (datetimestr.Equals(""))
                return replacestr;

            try
            {
                datetimestr = Convert.ToDateTime(datetimestr).ToString("yyyy-MM-dd").Replace("1900-01-01", replacestr);
            }
            catch
            {
                return replacestr;
            }
            return datetimestr;
        }


        /// <summary>
        /// 返回标准时间格式string
        /// </summary>
        public static string GetTime()
        {
            return DateTime.Now.ToString("HH:mm:ss");
        }

        /// <summary>
        /// 返回标准时间格式string
        /// </summary>
        public static string GetDateTime()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 返回相对于当前时间的相对天数
        /// </summary>
        public static string GetDateTime(int relativeday)
        {
            return DateTime.Now.AddDays(relativeday).ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 返回标准时间格式string
        /// </summary>
        public static string GetDateTimeF()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fffffff");
        }



        /// <summary>
        /// Gets the standard date time.
        /// </summary>
        /// <param name="fDateTime">The f date time.</param>
        /// <param name="formatStr">The format STR.</param>
        /// <returns></returns>
        public static string GetStandardDateTime(string fDateTime, string formatStr)
        {
            if (fDateTime == "0000-0-0 0:00:00")
                return fDateTime;
            DateTime time = new DateTime(1900, 1, 1, 0, 0, 0, 0);
            if (DateTime.TryParse(fDateTime, out time))
                return time.ToString(formatStr);
            else
                return "N/A";
        }

        /// <summary>
        /// 返回标准时间 yyyy-MM-dd HH:mm:ss
        /// </summary>
        public static string GetStandardDateTime(string fDateTime)
        {
            return GetStandardDateTime(fDateTime, "yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 返回标准时间 yyyy-MM-dd
        /// </summary>
        public static string GetStandardDate(string fDate)
        {
            return GetStandardDateTime(fDate, "yyyy-MM-dd");
        }

        /// <summary>
        /// 获取当前的时间戳，也就是Unix的时间戳
        /// </summary>
        /// <returns>
        /// System.String
        /// </returns>
        public static long GetTimstamp()
        {
            DateTime timeStamp = new DateTime(1970, 1, 1);  //得到1970年的时间戳
            long ticks = (DateTime.UtcNow.Ticks - timeStamp.Ticks) / 10000000;  //注意这里有时区问题，用now就要减掉8个小时
            return ticks;
        }


        /// <summary>
        /// Unix 时间转标准时间
        /// </summary>
        /// <param name="timeStamp">时间节点.</param>
        /// <param name="length">转换成10位秒还是13位毫秒</param>
        /// <returns>
        /// DateTime
        /// </returns>
        public static DateTime UnixTimeToTime(string timeStamp, int length = 13)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = 0;
            if (length == 13)
            {
                lTime = long.Parse(timeStamp + "0000");
            }
            else
            {              
                lTime = long.Parse(timeStamp + "0000000");               
            }
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }


        /// <summary>
        /// 转Unix时间
        /// </summary>
        /// <param name="time">The time.</param>
        /// <param name="length">转换成10位秒还是13位毫秒</param>
        /// <returns>
        /// System.Int32
        /// </returns>
        public static long ToUnixTime(System.DateTime time,int length=13)
        {
            if (length == 13)
            {
                System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
                long t = (time.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位      
                return t;
            }
            else
            {
                System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
                return (long)(time - startTime).TotalSeconds;
            }
        }


        /// <summary>
        ///  根据日期获取年龄.
        ///  Author :   XP-PC/Shaipe
        ///  Created:  09-30-2014
        /// </summary>
        /// <param name="targetDate">The target date.</param>
        /// <returns>System.Int64.</returns>
        public static long GetYears(DateTime targetDate)
        {
            long BirthDay = DateAndTime.DateDiff(DateInterval.Year, targetDate, DateTime.Now, FirstDayOfWeek.Sunday, FirstWeekOfYear.Jan1);
            if (int.Parse(BirthDay.ToString()) < 0)
            {
                //new ECFException("经鉴定，你是未来人");

            }
            else
            {
                return BirthDay;
            }
            return 0;
        }

        /// <summary>
		/// Converts a nullable date/time value to UTC.
		/// </summary>
		/// <param name="dateTime">The nullable date/time</param>
		/// <returns>The nullable date/time in UTC</returns>
		public static DateTime? ToUniversalTime(DateTime? dateTime)
        {
            return dateTime.HasValue ? dateTime.Value.ToUniversalTime() : (DateTime?)null;
        }

        /// <summary>
        /// Returns a copy of a date/time value with its kind
        /// set to <see cref="DateTimeKind.Utc" /> but does not perform
        /// any time-zone adjustment.
        /// </summary>
        /// <remarks>
        /// This method is useful when obtaining date/time values from sources
        /// that might not correctly set the UTC flag.
        /// </remarks>
        /// <param name="dateTime">The date/time</param>
        /// <returns>The same date/time with the UTC flag set</returns>
        public static DateTime AssumeUniversalTime(DateTime dateTime)
        {
            return new DateTime(dateTime.Ticks, DateTimeKind.Utc);
        }

        /// <summary>
        /// Returns a copy of a nullable date/time value with its kind
        /// set to <see cref="DateTimeKind.Utc" /> but does not perform
        /// any time-zone adjustment.
        /// </summary>
        /// <remarks>
        /// This method is useful when obtaining date/time values from sources
        /// that might not correctly set the UTC flag.
        /// </remarks>
        /// <param name="dateTime">The nullable date/time</param>
        /// <returns>The same nullable date/time with the UTC flag set</returns>
        public static DateTime? AssumeUniversalTime(DateTime? dateTime)
        {
            return dateTime.HasValue ? AssumeUniversalTime(dateTime.Value) : (DateTime?)null;
        }
    }
}
