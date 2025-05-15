using System;
using System.Collections.Generic;
using System.Linq;
using ChallengeAccess.Models;
using ChallengeAccess.Services;
using FluentAssertions;
using Xunit;

namespace HotelAvailabilityCli.Tests.Tests
{
    public class SearchServiceTests
    {
        private AvailabilityService MakeSvc(List<Booking> bookings)
        {
            var hotels = new List<Hotel> {
                new Hotel {
                    Id = "H1",
                    Rooms = new List<Room> {
                        new Room { RoomType="SGL", RoomId="101" },
                        new Room { RoomType="SGL", RoomId="102" },
                        new Room { RoomType="DBL", RoomId="201" },
                        new Room { RoomType="DBL", RoomId="202" },
                    }
                }
            };
            return new AvailabilityService(hotels, bookings);
        }

        [Fact]
        public void Search_NoBookings_OneWholeBlock()
        {
            var availSvc = MakeSvc(new List<Booking>());
            var searchSvc = new SearchService(availSvc);

            // Use 3 nights ahead from today
            var ranges = searchSvc.FindRanges("H1", 3, "SGL").ToList();

            ranges.Should().HaveCount(1);
            ranges[0].start.Should().Be(DateTime.Today);
            ranges[0].end.Should().Be(DateTime.Today.AddDays(3));
            ranges[0].avail.Should().Be(2);
        }

        [Fact]
        public void Search_WithBooking_GeneratesTwoBlocks()
        {
            // Create a booking that starts today for one night only
            var today = DateTime.Today;
            var arr = today.ToString("yyyyMMdd");
            var dep = today.AddDays(1).ToString("yyyyMMdd");
            var bookings = new List<Booking> {
                new Booking { HotelId="H1", RoomType="SGL", Arrival=arr, Departure=dep }
            };

            var availSvc = MakeSvc(bookings);
            var searchSvc = new SearchService(availSvc);

            // Ask for 2 nights ahead: night1 has avail=1, night2 has avail=2
            var ranges = searchSvc.FindRanges("H1", 2, "SGL").ToList();

            // Extract just the days and avail
            var actual = ranges
                .Select(r => (r.start.Day, r.end.Day, r.avail))
                .ToList();

            var expected = new List<(int, int, int)> {
                (today.Day,       today.AddDays(1).Day, 1),
                (today.AddDays(1).Day, today.AddDays(2).Day, 2)
            };

            actual.Should().Equal(expected);
        }
    }
}
