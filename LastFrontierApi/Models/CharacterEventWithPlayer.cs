using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LastFrontierApi.Models
{
  public class CharacterEventWithPlayer
  {
    public CharacterEvent CharacterEvent { get; set; }
    public Player Player { get; set; }
  }
}
