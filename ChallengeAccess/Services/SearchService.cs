using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallengeAccess.Services
{
    public class SearchService
    {
        private readonly AvailabilityService _availSvc;

        public SearchService(AvailabilityService availSvc)
        {
            _availSvc = availSvc;
        }

        public IEnumerable<(DateTime start, DateTime end, int avail)> FindRanges(
            string hotel, int nightsAhead, string type)
        {
            var today = DateTime.Today; //we take as reference the current date
            //[today, today+nightsAhead)
            var byDay = new Dictionary<DateTime, int>();
            for (var i = 0; i < nightsAhead; i++)
            {
                var d = today.AddDays(i);
                byDay[d] = _availSvc.GetAvailability(hotel, d, type);
            }
            // group the dates by availability to create the desired output
            var result = new List<(DateTime, DateTime, int)>();
            var d0 = today;
            while (d0 < today.AddDays(nightsAhead))
            {
                var a = byDay[d0];
                var end = d0.AddDays(1);
                while (end < today.AddDays(nightsAhead) && byDay[end] == a)
                    end = end.AddDays(1);
                result.Add((d0, end, a));
                d0 = end;
            }
            return result;
        }
    }
}
