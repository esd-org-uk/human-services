﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ServiceLoader.JsonMappingObjects;

namespace ServiceLoader
{
    internal static class ScheduleBuilder
    {
        private static readonly string[] ValidDays = new [] { "SU", "MO", "TU", "WE", "TH", "FR", "SA" };
        private static readonly string[] MonthlyWords = new [] { "1st", "2nd", "3rd", "4th", "first", "second", "third", "fourth", "last" };
        private static readonly Dictionary<string, string> IntervalWords = new Dictionary<string, string>
        {
            {"fortnightly","2" },
            {"alternate", "2" }
        };
        private static readonly Dictionary<string, string> DayPrefixWords = new Dictionary<string, string>
        {
            {"1st","1" },
            {"first","1" },
            {"2nd", "2" },
            {"second", "2" },
            {"3rd", "3" },
            {"third", "3" },
            {"4th", "4" },
            {"fourth", "4" },
            {"last", "-1" }
        };
        private static readonly Dictionary<string, string> WordReplacements = new Dictionary<string, string>
        {
            {"&", " and "},
            {",", " and "},
            {"mondays", "MO"},
            {"tuesdays", "TU"},
            {"wednesdays", "WE"},
            {"thursdays", "TH"},
            {"fridays", "FR"},
            {"saturdays", "SA"},
            {"sundays", "SU"},
            {"monday", "MO"},
            {"tuesday", "TU"},
            {"wednesday", "WE"},
            {"thursday", "TH"},
            {"friday", "FR"},
            {"saturday", "SA"},
            {"sunday", "SU"},
            {"mon", "MO"},
            {"tue", "TU"},
            {"wed", "WE"},
            {"thu", "TH"},
            {"fri", "FR"},
            {"sat", "SA"},
            {"sun", "SU"},
            {" - ", "-"},
            {" midday ", "12:00"},
            {" noon ", "12:00"},
            {" midnight ", "00:00"}
        };

        /// <summary>
        /// Generates schedule information from Days and Frequency
        /// </summary>
        /// <param name="result">The record representing a service</param>
        /// <returns>Schedule information which is not guaranteed to be completely accurate</returns>
        public static IEnumerable<Schedule> Build(Result result)
        {
            if (string.IsNullOrEmpty(result.Frequency) && !result.Days.Any()) yield break;

            var text = result.Frequency?.ToLowerInvariant() ?? string.Empty;
            foreach (var wordReplacement in WordReplacements)
            {
                text = text.Replace(wordReplacement.Key, wordReplacement.Value);
            }

            var parts = text.Split(new[] {"and", " "}, StringSplitOptions.RemoveEmptyEntries).Select(part => part.Trim());
            var weeklyMonthly = parts.Intersect(MonthlyWords, StringComparer.OrdinalIgnoreCase).Any() ? "MONTHLY" : "WEEKLY";
            var times = ParseTimes(parts);
            var days = result.Days.Any() ? result.Days.Distinct() : ParseDays(parts);
            var interval = ParseInterval(parts);
            var dayPrefix = ParseDayPrefix(parts);
            
            foreach (var day in days)
            {
                if (day.Length < 2) continue;
                var dayAbbrv = day.Substring(0, 2).ToUpperInvariant();
                if (!ValidDays.Contains(dayAbbrv, StringComparer.Ordinal)) continue;

                yield return new Schedule($"{dayPrefix}{dayAbbrv}", result.Frequency, weeklyMonthly, times?.StartTime, times?.EndTime, interval);
            }

            if (!days.Any()) yield return new Schedule(string.Empty, result.Frequency, weeklyMonthly, times?.StartTime, times?.EndTime, interval);
        }

        private static string ParseInterval(IEnumerable<string> parts)
        {
            var intervals = new List<string>();
            foreach (var part in parts)
            {
                if (IntervalWords.TryGetValue(part, out var interval)) intervals.Add(interval);
            }

            //better to write no information than partial information - we are targetting the simple records
            return intervals.Count == 1 ? intervals.First() : string.Empty;
        }

        private static string ParseDayPrefix(IEnumerable<string> parts)
        {
            var prefixes = new List<string>();
            foreach (var part in parts)
            {
                if (DayPrefixWords.TryGetValue(part, out var prefix)) prefixes.Add(prefix);
            }

            //better to write no information than partial information - we are targetting the simple records
            return prefixes.Count == 1 ? prefixes.First() : string.Empty;
        }

        private static IEnumerable<string> ParseDays(IEnumerable<string> parts)
        {
            return parts.Where(part => ValidDays.Contains(part, StringComparer.OrdinalIgnoreCase)).Distinct();
        }

        private static Times ParseTimes(IEnumerable<string> parts)
        {
            var times = new List<Times>();
            var formats = new string[]{ "hh\\:mm", "h\\:mm" };

            foreach (var part in parts)
            {
                if (char.IsDigit(part, 0))
                {
                    var timeParts = part.Split('-').Select(timePart => timePart.Trim()).ToArray();
                    if (timeParts.Length > 2) continue;
                    if (!TimeSpan.TryParseExact(timeParts[0], formats, null, out var startTime)) continue;
                    var time = new Times { StartTime = startTime };
                    if (timeParts.Length > 1 && TimeSpan.TryParseExact(timeParts[1], formats, null, out var endTime)) time.EndTime = endTime;
                    times.Add(time);
                }
            }

            //better to write no information than partial information - we are targetting the simple records
            return times.Count == 1 ? times.First() : null;
        }

        private class Times
        {
            public TimeSpan StartTime { get; set; }
            public TimeSpan? EndTime { get; set; }
        }
    }
}
