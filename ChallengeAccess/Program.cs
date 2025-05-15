using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using ChallengeAccess.Models;
using ChallengeAccess.Services;

class Program
{
    static void Main(string[] args)
    {
        //We search for the following command which we added in vs 2022 line argument command, section Debug, project Propoerties with the full path of the json files
        // myapp --hotels hotels.json --bookings bookings.json
        if (args.Length != 4 || args[0] != "--hotels" || args[2] != "--bookings") //we need for certain the 2 flags and the name of json files can be anything
        {
            Console.WriteLine("Usage: myapp --hotels <hotels.json> --bookings <bookings.json>");
            return;
        }

        var hotelsPath = args[1]; //we take the path of the first json file
        var bookingsPath = args[3]; //path of second json file

        //if the path is not valid we return an error
        if (!File.Exists(hotelsPath))
        {
            Console.WriteLine("Hotels json file not found");
            return;
        }
        else if(!File.Exists(bookingsPath))         
        {
            Console.WriteLine("Bookings json file not found");
            return;
        }

        //in json we have all lowercase and in c# we have PascalCase, so we have to set the option to ignore the case in order to match
        var opts = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true  
        };

        //we read the content of the json files
        var hotelContent = File.ReadAllText(hotelsPath);
        var bookingContent = File.ReadAllText(bookingsPath);

        //we take the content of the json files and we deserialize them into a list of coresponding objects
        var hotelList = JsonSerializer.Deserialize<List<Hotel>>(hotelContent,opts);
        if (hotelList is null)
        {
            Console.Error.WriteLine("Could not parse hotels json file");
            return;
        }

        var bookingList = JsonSerializer.Deserialize<List<Booking>>(bookingContent,opts);
        if (bookingList is null)
        {
            Console.Error.WriteLine("Could not parse bookings json file ");
            return;
        }


        var availSvc = new AvailabilityService(hotelList, bookingList);
        var searchSvc = new SearchService(availSvc);

        Console.WriteLine("Ready. Enter commands to process, empty line to exit!");

        while (true)
        {
            Console.Write("> ");
            var line = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(line))
            {
                Console.WriteLine("Goodbye!");
                break;
            }

            //We take even availability as a valid command ignoring the case
            //if we are strict and want to take only "Availability" we can remove the StringComparison.OrdinalIgnoreCase
            if (line.StartsWith("Availability", StringComparison.OrdinalIgnoreCase))
            {
                // Availability(H1,20250901-20250903,DBL)
                var parts = line
                    .Replace("Availability", "")
                    .Trim('(', ')')
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(p => p.Trim()) //accidental whitespace around any piece, this trims each substring
                    .ToArray();

                var hotelId = parts[0];
                var dates = parts[1].Split('-', StringSplitOptions.TrimEntries); //we split the dates, eliminating the spaces
                var start = DateTime.ParseExact(dates[0], "yyyyMMdd", null);
                //if we have an interval we take the second date, if not we take the first one only
                var end = dates.Length == 2
                                ? DateTime.ParseExact(dates[1], "yyyyMMdd", null)
                                : start;

                var roomType = parts[2];
                int result;
                if (end == start)
                {
                    //if we have only one date we take the availability for that date
                    result = availSvc.GetAvailability(hotelId, start, roomType);
                }
                else
                {
                    //if we have an interval we take the availability for that interval
                    result = availSvc.GetAvailability(hotelId, start, end, roomType);
                }
                Console.WriteLine(result);
            }
            else if (line.StartsWith("Search", StringComparison.OrdinalIgnoreCase))
            {
                // Search(H1,30,SGL)
                var parts = line
                    .Replace("Search", "")
                    .Trim('(', ')')
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(p => p.Trim())
                    .ToArray();

                var hotelId = parts[0];
                var nightsAhead = int.Parse(parts[1]);
                var roomType = parts[2];

                var ranges = searchSvc
                    .FindRanges(hotelId, nightsAhead, roomType)
                    .Select(r => $"({r.start:yyyyMMdd}-{r.end:yyyyMMdd}, {r.avail})");

                Console.WriteLine(string.Join(", ", ranges));
            }
            else
            {
                Console.WriteLine("Unknown command. Use Availability(...) or Search(...).");
            }
        }
    }
}
