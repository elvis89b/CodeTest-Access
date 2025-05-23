﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallengeAccess.Models
{
    public class Booking
    {
        public string HotelId { get; set; }

        //Both Arrival and Departure are in the format YYYYMMDD
        public string Arrival { get; set; }   
        public string Departure { get; set; }  
        public string RoomType { get; set; }
        public string RoomRate { get; set; }
    }
}
