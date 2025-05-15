using ChallengeAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallengeAccess.Services
{
    public class AvailabilityService
    {
        private readonly Dictionary<(string hotel, string type, DateTime day), int> _booked;
        private readonly Dictionary<(string hotel, string type), int> _totalRooms;
        private const string DateFmt = "yyyyMMdd";

        public AvailabilityService(IEnumerable<Hotel> hotels, IEnumerable<Booking> bookings)
        {
            _totalRooms = hotels
              .SelectMany(h => h.Rooms.Select(r => (h.Id, r.RoomType))) //we select the wanted tuple
              .GroupBy(k => k) //we group by the tuple if they are the same
              .ToDictionary(g => g.Key, g => g.Count()); //we set the key as the tuple and the value as the count of the rooms of that type in that hotel

            _booked = new Dictionary<(string, string, DateTime), int>();
            foreach (var b in bookings)
            {
                var a = DateTime.ParseExact(b.Arrival, DateFmt, null);
                var d = DateTime.ParseExact(b.Departure, DateFmt, null);
                for (var day = a; day < d; day = day.AddDays(1))
                {
                    var key = (b.HotelId, b.RoomType, day);
                    _booked[key] = _booked.GetValueOrDefault(key) + 1; //increase the count of the booked rooms
                }
            }
        }

        public int GetAvailability(string hotel, DateTime singleDate, string type)
        {
            var total = _totalRooms.GetValueOrDefault((hotel, type), 0); //we get the total number of rooms of that type in that hotel, if not found we return 0
            var booked = _booked.GetValueOrDefault((hotel, type, singleDate), 0);
            return total - booked;
        }

        public int GetAvailability(string hotel, DateTime start, DateTime end, string type)
        {
            //[start, end)
            var availList = new List<int>();
            for (var day = start; day < end; day = day.AddDays(1))
            {
                var total = _totalRooms.GetValueOrDefault((hotel, type), 0);
                var booked = _booked.GetValueOrDefault((hotel, type, day), 0);
                availList.Add(total - booked);
            }
            return availList.Min();
        }
    }
}
