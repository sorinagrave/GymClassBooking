﻿using System;

namespace ClassBooking.Models
{
    public enum MemberClassStatus
    {
        None = 0,
        BookedClass = 1,
        BookedWaiting = 2,
        EligibleClass = 3,
        EligibleWaiting = 4
    }
}