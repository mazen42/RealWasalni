using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Wasalni_Models;
using Wasalni_Models.DTOs;

namespace Wasalni_Utility
{
    public static class PassengerFunctions
    {
        public static Dictionary<string,bool> putThePassengerinTheApproproiateSeatSingle(this List<Seat> seats)
        {
            var chars = seats.Select(x => x.SeatChar.ToString()).ToHashSet();
            var dict = new Dictionary<string, bool>
            {
                { "A", false },
                { "B", false },
                { "C", false },
                { "D", false },
                { "E", false },
                { "F", false },
                { "G", false },
                { "H", false },
                { "I", false },
                { "L", false },
                { "M", false },
                { "N", false },

            };
            foreach (var letter in chars)
            {
                if(dict.Keys.Contains(letter))
                {
                    dict[letter] = true;
                }
            }
            return dict;
        }
        //public static putThePassengerinTheApproproiateSeatDTO putThePassengerinTheApproproiateSeatGroup(this IEnumerable<Seat> seats, List<SeatChar> seatChar, int count = 1)
        //{
        //    var dto = new putThePassengerinTheApproproiateSeatDTO();
        //    if (seats.Count() - 14 < count)
        //        return null;
        //    //List<char> charList = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J',  'K', 'L'];
        //    foreach (Seat s in seats)
        //    {
        //        foreach (SeatChar schar in seatChar)
        //        {
        //            if (s.SeatChar == schar)
        //                dto.messages.Append($"the seat {schar} is {s.SeatStatus}");
        //        }
        //    }
        //    if (dto.messages.Count() == 0)
        //    {
        //        for (int i = 0; i < count; i++)
        //        {
        //            var seat = new Seat
        //            {
        //                SeatChar = seatChar[i],
        //                SeatStatus = SeatStatus.Booked
        //            };
        //            dto.seats.Append(seat);
        //        }
        //    }
        //    return dto;

        //}
    }
}
