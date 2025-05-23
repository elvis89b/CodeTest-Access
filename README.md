# CodeTest-Access

Hi, I am Elvis Hardalau and I chose C# for this request of processing the json files, being my most familiar, as well as not stating an exact technology to use. Moreover, if you want something else rather than C#, I am more than excited to invest time on that framework, learning and producing good results under your professional guidance.

To build and run it, you will have to use: dotnet build  -> dotnet run --hotels PathToHotels.json --bookings PathToBookings.json   -> if you prefer vs code.
I personally used vs 2022, where in order to use, you have to right click on the project name, then Properties -> Debug -> Open debug launch profiles UI -> Command line arguments and you have to specify the full path for the json files. As an example I used this: --hotels "C:\Users\elvis\Desktop\DataEngineering\ChallengeAccess\ChallengeAccess\hotels.json" --bookings "C:\Users\elvis\Desktop\DataEngineering\ChallengeAccess\ChallengeAccess\bookings.json"
To get the full path easier, from Solution Explorer from the right, you right click on the json files and select Copy Full Path.

In order to have a greater time efficiency I used a LLM, ChatGPT o4-mini-high, but I also made changes to personalize and work fine on the request.

Here is the prompt I used: 
"Hello, you are an expert in c#, Data Engineering, having the following json files please implement the required classes in Models folder and the functionalities in Services folder:

-Availability Command -> Input: Availability(H1, 20250901, SGL) for single date or Availability(H1, 20250901-20250903, DBL) for a range of dates.    
Output: availability count for the specified hotel, room type, and date or date range

For a single date query like Availability(H1, 20250901, SGL), it shows how many rooms of that type are available on that specific date. For a date range query like Availability(H1, 20250901-20250903, DBL), it shows the minimum availability across all dates in the range, representing how many complete stays could be accommodated.

-Search Command -> Input: Search(H1, 30, SGL) 

Output: a comma separated list of date ranges and availability where the room is available for that range. In this example, 30 is the number of nights ahead to query for availability. If there is no availability the program should return an empty line

The date range format "20250901-20250902" represents an arrival date of 2025/09/01 and a departure date of 2025/09/02

For each booking: The arrival date is inclusive - the room is unavailable for another guest starting on this day, as the guest checks in on this date.
The departure date is exclusive - the room becomes available for another guest on this day, as the previous guest checks out in the morning.

Solve the request by paying attention to all details, but keep it simple please."

If I could do anything better, or if I did something wrong please tell me so it won't happen again (I appreciate costructive feedback), I really hope I didn't overcomplicate the solution, I like having the models separated and the services in other directory. Moreover, I am familiar with Layered Architecture, separating the business, controller and data access logic.

Thanks a lot for the opportunity!


Update: I used the LLM to check for the hints from the specified blog - https://medium.com/guestline-labs/hints-for-our-interview-process-and-code-test-ae647325f400 and besides the testing the hints were acomplished. As a result, I added the testing using xUnit as well!

To use the tests you need to go to HotelAvailabilityCli.Tests and then use dotnet build and dotnet test! Moreover, I added xunit, xunit.runner, and FluentAssertions as nugget packages.