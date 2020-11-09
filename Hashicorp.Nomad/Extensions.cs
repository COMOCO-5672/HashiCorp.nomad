using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Hashicorp.Nomad
{
    internal static class Extensions
    {
        internal static readonly Dictionary<string, double> UnitMap = new Dictionary<string, double>() { { "ns", 1E-06 }, { "us", 0.001 }, { "µs", 0.001 }, { "μs", 0.001 }, { "ms", 1.0 }, { "s", 1000.0 }, { "m", 60000.0 }, { "h", 3600000.0 } };
        internal const double Nanosecond = 1E-06;
        internal const double Microsecond = 0.001;
        internal const double Millisecond = 1.0;
        internal const double Second = 1000.0;
        internal const double Minute = 60000.0;
        internal const double Hour = 3600000.0;

        internal static string ToGoDuration(this TimeSpan ts)
        {
            if (ts == TimeSpan.Zero)
                return "0";
            if (ts.TotalSeconds < 1.0)
                return ts.TotalMilliseconds.ToString("#ms");
            StringBuilder stringBuilder = new StringBuilder();
            if ((int)ts.TotalHours > 0)
                stringBuilder.Append(ts.TotalHours.ToString("#h"));
            if (ts.Minutes > 0)
                stringBuilder.Append(ts.ToString("%m'm'"));
            if (ts.Seconds > 0)
                stringBuilder.Append(ts.ToString("%s"));
            if (ts.Milliseconds > 0)
            {
                stringBuilder.Append(".");
                stringBuilder.Append(ts.ToString("fff"));
            }
            if (ts.Seconds > 0)
                stringBuilder.Append("s");
            return stringBuilder.ToString();
        }

        internal static TimeSpan FromGoDuration(string value)
        {
            if (string.IsNullOrEmpty(value) || value == "0")
                return TimeSpan.Zero;
            ulong result1;
            if (ulong.TryParse(value, out result1))
                return TimeSpan.FromTicks((long)(result1 / 100UL));
            MatchCollection matchCollection = Regex.Matches(value, "([0-9]*(?:\\.[0-9]*)?)([a-z]+)");
            if (matchCollection.Count == 0)
                throw new ArgumentException("Invalid duration", value);
            double num = 0.0;
            foreach (Match match in matchCollection)
            {
                double result2;
                if (!double.TryParse(match.Groups[1].Value, out result2))
                    throw new ArgumentException("Invalid duration number", value);
                if (!Extensions.UnitMap.ContainsKey(match.Groups[2].Value))
                    throw new ArgumentException("Invalid duration unit", value);
                num += result2 * Extensions.UnitMap[match.Groups[2].Value];
            }
            TimeSpan timeSpan = TimeSpan.FromMilliseconds(num);
            if (value[0] != '-')
                return timeSpan;
            return timeSpan.Negate();
        }
    }
}
