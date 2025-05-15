using System;
using System.Collections.Generic;
using ChallengeAccess.Models;
using ChallengeAccess.Services;
using FluentAssertions;
using Xunit;

namespace HotelAvailabilityCli.Tests.Tests
{
    public class AvailabilityServiceTests
    {
        private readonly List<Hotel> _hotels = new()
        {
            new Hotel {
                Id = "H1",
                Rooms = new List<Room> {
                    new Room { RoomType = "SGL", RoomId = "101" },
                    new Room { RoomType = "SGL", RoomId = "102" },
                    new Room { RoomType = "DBL", RoomId = "201" },
                    new Room { RoomType = "DBL", RoomId = "202" },
                }
            }
        };

        [Fact]
        public void SingleDate_NoBookings_ReturnsTotalRooms()
        {
            var svc = new AvailabilityService(_hotels, new List<Booking>());
            // SGL total = 2, DBL total = 2
            svc.GetAvailability("H1", new DateTime(2025, 9, 1), "SGL").Should().Be(2);
            svc.GetAvailability("H1", new DateTime(2025, 9, 1), "DBL").Should().Be(2);
        }

        [Fact]
        public void SingleDate_WithBooking_ReturnsTotalMinusBooked()
        {
            // Book one SGL on 2025-09-01
            var bookings = new List<Booking> {
                new Booking { HotelId = "H1", RoomType = "SGL", Arrival = "20250901", Departure = "20250902" }
            };
            var svc = new AvailabilityService(_hotels, bookings);
            svc.GetAvailability("H1", new DateTime(2025, 9, 1), "SGL")
               .Should().Be(1);
     
            svc.GetAvailability("H1", new DateTime(2025, 9, 1), "DBL")
               .Should().Be(2);
        }

        [Fact]
        public void DateRange_MinimumAcrossDays()
        {
            // Book SGL for 2 nights
            var bookings = new List<Booking> {
                new Booking { HotelId = "H1", RoomType = "SGL", Arrival = "20250901", Departure = "20250903" }
            };
            var svc = new AvailabilityService(_hotels, bookings);
            var avail = svc.GetAvailability("H1",
                                            new DateTime(2025, 9, 1),
                                            new DateTime(2025, 9, 2),
                                            "SGL");
            avail.Should().Be(1);
        }

        [Fact]
        public void Overbooking_CanReturnNegative()
        {
            // Book 3 DBL but only 2 exist
            var bookings = new List<Booking> {
                new Booking { HotelId = "H1", RoomType = "DBL", Arrival = "20250901", Departure = "20250902" },
                new Booking { HotelId = "H1", RoomType = "DBL", Arrival = "20250901", Departure = "20250902" },
                new Booking { HotelId = "H1", RoomType = "DBL", Arrival = "20250901", Departure = "20250902" }
            };
            var svc = new AvailabilityService(_hotels, bookings);
            svc.GetAvailability("H1", new DateTime(2025, 9, 1), "DBL")
               .Should().Be(-1);
        }
    }
}
