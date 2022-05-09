using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace EliteTrading.Extensions {
    public static class DateTimeExtensions {
        public static string TimeSince(this DateTime input) {
            TimeSpan ts = DateTime.Now.Subtract(input);
            StringBuilder b = new StringBuilder();
            int days = (int)Math.Floor(ts.TotalDays);
            int hours = (int)ts.TotalHours - (days * 24);
            int minutes = (int)ts.TotalMinutes - (1440 * days) - (60 * hours);

            if (ts.TotalHours < 1)
                b.AppendFormat("{0}m", (int)ts.TotalMinutes);
            else if (ts.TotalDays < 1) {
                b.AppendFormat("{0}h {1}m", hours, minutes);
            } else if (ts.TotalDays > 1) {
                b.AppendFormat("{0}d {1}h {2}m", days, hours, minutes);
            }
            return b.ToString();
        }    
    }


    public class PeriodOfTimeOutput {
        private readonly DateTime thePast;
        private readonly DateTime reference;
        private DateTime pastToPresentCursor;
        private int yearCounter;
        private int monthCounter;
        private int weekCounter;
        private int dayCounter;

        public PeriodOfTimeOutput(DateTime timeInQuestion)
            : this(timeInQuestion, DateTime.Now) {
        }

        public PeriodOfTimeOutput(DateTime thePast, DateTime reference) {
            this.thePast = thePast;
            pastToPresentCursor = thePast;
            this.reference = reference;
        }

        public override string ToString() {
            advanceTheTimeCursor(
              () => new DateTime(pastToPresentCursor.Year + 1, thePast.Month, thePast.Day),
              () => yearCounter++
            );
            advanceTheTimeCursor(
              () => pastToPresentCursor + monthTimeToAdd(),
              () => monthCounter++
            );
            advanceTheTimeCursor(
              () => pastToPresentCursor + TimeSpan.FromDays(7),
              () => weekCounter++
            );
            advanceTheTimeCursor(
              () => pastToPresentCursor + TimeSpan.FromDays(1),
              () => dayCounter++
            );
            return CreateTheString();
        }

        private string CreateTheString() {
            var parts = new List<string>();

            if (yearCounter > 0)
                parts.Add(string.Format("{0} {1}", yearCounter, yearInflection(yearCounter)));
            if (monthCounter > 0)
                parts.Add(string.Format("{0} {1}", monthCounter, monthInflection(monthCounter)));
            if (weekCounter > 0)
                parts.Add(string.Format("{0} {1}", weekCounter, weekInflection(weekCounter)));
            if (dayCounter > 0)
                parts.Add(string.Format("{0} {1}", dayCounter, dayInflection(dayCounter)));
            return parts.Count > 0 ? string.Join(", ", parts.ToArray()) : "today";

        }


        private void advanceTheTimeCursor(Func<DateTime> nextTime, Action uponSuccessfulAdvancement) {
        loop:
            var t = nextTime();
            if (t > reference) return;
            uponSuccessfulAdvancement();
            pastToPresentCursor = t;
            goto loop;
        }

        private TimeSpan monthTimeToAdd() {
            foreach (var daysInMonth in new[] { 31, 30, 28, 29 }) {
                var theSpan = TimeSpan.FromDays(daysInMonth);
                if ((pastToPresentCursor + theSpan).Day == pastToPresentCursor.Day)
                    return theSpan;
            }
            // can happen when past lies on the last day of a e.g. a 31 day month
            // return a reasonable amount of days
            return TimeSpan.FromDays(30);

        }

        private static string yearInflection(int years) {
            return years > 1 ? "years" : "year";
        }

        private static string monthInflection(int months) {
            return months > 1 ? "months" : "month";
        }

        private static string dayInflection(int days) {
            return days > 1 ? "days" : "day";
        }

        private static string weekInflection(int weeks) {
            return weeks > 1 ? "weeks" : "week";
        }
    }
}