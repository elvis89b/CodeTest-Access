using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallengeAccess.Services
{
    public static class CommandParser
    {
        private const string AvailPrefix = "Availability";
        private const string SearchPrefix = "Search";

        public static (string hotel, DateTime start, DateTime end, string type)? TryParseAvailability(string line)
        {
            if (!line.StartsWith(AvailPrefix, StringComparison.OrdinalIgnoreCase))
                return null;

            // strip prefix, parens, split on commas
            var parts = line
                .Substring(AvailPrefix.Length)      // remove "Availability"
                .Trim('(', ')')
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .ToArray();

            if (parts.Length != 3)
                return null;

            var hotelId = parts[0];

            var dates = parts[1]
                .Split('-', StringSplitOptions.TrimEntries);
            var start = DateTime.ParseExact(dates[0], "yyyyMMdd", null);
            var end = dates.Length == 2
                        ? DateTime.ParseExact(dates[1], "yyyyMMdd", null)
                        : start;

            var roomType = parts[2];
            return (hotelId, start, end, roomType);
        }

        public static (string hotel, int nights, string type)? TryParseSearch(string line)
        {
            if (!line.StartsWith(SearchPrefix, StringComparison.OrdinalIgnoreCase))
                return null;

            var parts = line
                .Substring(SearchPrefix.Length)
                .Trim('(', ')')
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .ToArray();

            if (parts.Length != 3 || !int.TryParse(parts[1], out var nights))
                return null;

            return (parts[0], nights, parts[2]);
        }
    }
}
