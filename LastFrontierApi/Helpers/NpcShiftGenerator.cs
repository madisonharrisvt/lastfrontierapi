using System;
using System.Collections.Generic;
using LastFrontierApi.Models;

namespace LastFrontierApi.Helpers
{
  public static class NpcShiftGenerator
  {
    public static List<NpcShift> GenerateShifts(Event activeEvent, DateTime start, DateTime end, int lengthInMins,
      int minutesBetweenShifts)
    {
      var npcShiftStart = start;
      var npcShiftEnd = start.AddMinutes(lengthInMins);

      var npcShifts = new List<NpcShift>();

      var isEndOfEvent = end - npcShiftEnd;

      while (isEndOfEvent.TotalMinutes >= 0)
      {
        var npcShift = new NpcShift
        {
          EventId = activeEvent.Id,
          Event = activeEvent,
          StartDateTime = npcShiftStart,
          EndDateTime = npcShiftEnd
        };

        npcShifts.Add(npcShift);

        npcShiftStart = npcShiftStart.AddMinutes(minutesBetweenShifts);
        npcShiftEnd = npcShiftStart.AddMinutes(lengthInMins);

        isEndOfEvent = end - npcShiftEnd;
      }

      return npcShifts;
    }
  }
}