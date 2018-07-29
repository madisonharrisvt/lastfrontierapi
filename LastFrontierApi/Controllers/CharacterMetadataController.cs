using System;
using System.Collections.Generic;
using System.Linq;
using LastFrontierApi.Models;
using LastFrontierApi.Models.Metadata;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LastFrontierApi.Controllers
{
    [Authorize(Policy = "ApiUser", Roles = "Admin")]
    [Route("api/[controller]")]
    public class CharacterMetadataController : Controller
    {
        private readonly LfContext _context;

        public CharacterMetadataController(LfContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public CharacterMetadata GetMetadata(int id)
        {
            var characterMetadata = new CharacterMetadata()
            {
                Occupations = _context.tblOccupation.ToArray(),
                SideGigs = AvailableSideGigs(id),
                Skills = _context.tblSkill.ToArray(),
                Species = _context.tblSpecies.ToArray(),
                Statuses = _context.tblStatus.ToArray(),
                StressResponses = _context.tblStressResponse.ToArray()
            };

            return characterMetadata;
        }

        [HttpPost]
        public IActionResult GetSideGigMetadata([FromBody] int chosenOccupation)
        {
            try
            {
                var availableSideGigs = AvailableSideGigs(chosenOccupation);

                return Ok(availableSideGigs);
            }
            catch (Exception e)
            {
                return StatusCode(500);
            }
            
        }

        private SideGig[] AvailableSideGigs(int chosenOccupation)
        {
            var occupations = _context.tblOccupation.ToArray();

            var orderedOccupations = new List<Occupation>()
            {
                occupations.FirstOrDefault(x => x.Id == 3), // Soldier
                occupations.FirstOrDefault(x => x.Id == 1), // Doctor
                occupations.FirstOrDefault(x => x.Id == 5), // Prospector
                occupations.FirstOrDefault(x => x.Id == 2), // Socialite
                occupations.FirstOrDefault(x => x.Id == 4)  // Engineer
            };

            var orderedOccupationsList = new LinkedList<Occupation>(orderedOccupations);

            var chosenOccupationInList = orderedOccupationsList.FirstOrDefault(o => o.Id == chosenOccupation);

            var currentOccupation = orderedOccupationsList.Find(chosenOccupationInList);

            if (currentOccupation == null)
            {
                return null;
            }

            var previousOccupation = currentOccupation.Previous ?? orderedOccupationsList.Last;
            var nextOccupation = currentOccupation.Next ?? orderedOccupationsList.First;

            var previousSideGig = new SideGig()
            {
                Id = previousOccupation.Value.Id,
                Name = previousOccupation.Value.Name
            };

            var nextSideGig = new SideGig()
            {
                Id = nextOccupation.Value.Id,
                Name = nextOccupation.Value.Name
            };


            var availableSideGigs = new SideGig[]
            {
                previousSideGig,
                nextSideGig
            };

            return availableSideGigs;
        }
    }
}
