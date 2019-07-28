using System.Linq;
using AutoMapper;
using iText.Forms;
using iText.Forms.Fields;
using iText.Kernel.Pdf;
using LastFrontierApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LastFrontierApi.Services
{
  public class PdfService : IPdfService
  {
    private readonly ApplicationDbContext _appDbContext;
    private readonly LfContext _lfContext;
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;

    public PdfService(ApplicationDbContext appDbContext, LfContext lfContext)
    {
      _appDbContext = appDbContext;
      _lfContext = lfContext;
    }

    public void CreateCharacterSheetById(int id, string src, string des)
    {
      var playerName = string.Empty;
      var characterName = string.Empty;
      var speciesName = string.Empty;
      var psyProfileName = string.Empty;
      var healthStatusName = string.Empty;
      var occupationName = string.Empty;
      var sideGigName = string.Empty;
      var cultureName = string.Empty;
      var cloneStatus = string.Empty;
      var availableXP = string.Empty;

      var character = _lfContext.tblCharacter.Include(s => s.Skills).FirstOrDefault(c => c.Id == id);
      if (character != null)
      {
        var player = _appDbContext.tblPlayer.Include(p => p.Identity).FirstOrDefault(p => p.Id == character.PlayerId);
        if (player != null)
        {
          var firstName = string.Empty;
          var lastName = string.Empty;

          if (player.Identity?.FirstName != null) firstName = player.Identity.FirstName;
          if (player.Identity?.LastName != null) lastName = player.Identity.LastName;

          playerName = firstName + " " + lastName;
        }

        if (character.Name != null) characterName = character.Name;

        var species = _lfContext.tblSpecies.FirstOrDefault(s => s.Id == character.Species);
        if (species != null) speciesName = species.Name;

        var occupation = _lfContext.tblOccupation.FirstOrDefault(o => o.Id == character.Occupation);
        if (occupation != null) occupationName = occupation.Name;

        var sideGig = _lfContext.tblOccupation.FirstOrDefault(o => o.Id == character.SideGig);
        if (sideGig != null) sideGigName = sideGig.Name;

        var psyProfile = _lfContext.tblStressResponse.FirstOrDefault(s => s.Id == character.StressResponse);
        if (psyProfile != null) psyProfileName = psyProfile.Name;

        var healthStatus = character.HStatus;
        if (healthStatus != null) healthStatusName = healthStatus;

        var culture = _lfContext.tblCulture.FirstOrDefault(c => c.Id == character.Culture);
        if (culture != null) cultureName = culture.Name;

        if (character.CloneStatus != null && (bool) character.CloneStatus)
          cloneStatus = "Yes";
        else
          cloneStatus = "Off";

        if (character.AvailableXP != null) availableXP = character.AvailableXP.ToString();
      }

      var pdf = new PdfDocument(new PdfReader(src), new PdfWriter(des));
      var form = PdfAcroForm.GetAcroForm(pdf, true);
      var fields = form.GetFormFields();
      PdfFormField toSet;

      fields.TryGetValue("Player Name", out toSet);
      toSet.SetValue(playerName);
      fields.TryGetValue("Character Name", out toSet);
      toSet.SetValue(characterName, false);
      fields.TryGetValue("Species", out toSet);
      toSet.SetValue(speciesName, false);
      fields.TryGetValue("Psycological Profile", out toSet);
      toSet.SetValue(psyProfileName, false);
      fields.TryGetValue("Ailments", out toSet);
      toSet.SetValue(healthStatusName, false);
      fields.TryGetValue("Culture", out toSet);
      toSet.SetValue(cultureName, false);
      fields.TryGetValue("Clone Status", out toSet);
      toSet.SetValue(cloneStatus, false);
      fields.TryGetValue("Occupation", out toSet);
      toSet.SetValue(occupationName, false);
      fields.TryGetValue("Side Gig", out toSet);
      toSet.SetValue(sideGigName, false);
      fields.TryGetValue("Available XP", out toSet);
      toSet.SetValue(availableXP, false);

      var characterSkills = new CharacterSkill[0];
      if (character?.Skills != null) characterSkills = character.Skills.ToArray();
      for (var i = 0; i < 22; i++)
      {
        var skillListName = "Skill " + (i + 1);

        fields.TryGetValue(skillListName, out toSet);
        if (i < characterSkills.Length)
        {
          var skillName = string.Empty;
          var skill = _lfContext.tblSkill.FirstOrDefault(s => s.Id == characterSkills[i].SkillId);
          if (skill != null) skillName = skill.Name;
          toSet.SetValue(skillName, false);
        }
        else
        {
          toSet.SetValue(string.Empty, false);
        }

        var skillLevelListName = "Skill Level " + (i + 1);
        fields.TryGetValue(skillLevelListName, out toSet);

        toSet.SetValue(i < characterSkills.Length ? characterSkills[i].MasteryLevel.ToString() : string.Empty, false);
      }

      PdfAcroForm.GetAcroForm(pdf, true).FlattenFields();
      pdf.Close();
    }
  }
}