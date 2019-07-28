using System;
using System.IO;
using System.Linq;
using System.Reflection;
using AutoMapper;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using LastFrontierApi.Models;
using LastFrontierApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LastFrontierApi.Controllers
{
  [Authorize(Policy = "ApiUser", Roles = "Admin")]
  [Route("api/[controller]")]
  public class PrintCharacterSheetController : Controller
  {
    private readonly ApplicationDbContext _appDbContext;
    private readonly UserManager<AppUser> _userManager;
    private readonly IMapper _mapper;
    private readonly IPlayerService _playerService;
    private readonly LfContext _lfContext;
    private readonly IPdfService _pdfService;

    public PrintCharacterSheetController(UserManager<AppUser> userManager, IMapper mapper,
      ApplicationDbContext appDbContext, RoleManager<IdentityRole> roleManager,
      IPlayerService playerService, LfContext lfContext, IPdfService pdfService)
    {
      _userManager = userManager;
      _mapper = mapper;
      _appDbContext = appDbContext;
      _playerService = playerService;
      _lfContext = lfContext;
      _pdfService = pdfService;
    }

    [HttpGet("{id}")]
    public IActionResult GetCharacterSheetById(int id)
    {
      try
      {
        const string src = @"Assets\InputPdfs\EmptyPassport.pdf";
        var des = $@"Assets\OutputPdfs\FilledPassport{id}.pdf";

        var oldFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), src);
        var newFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), des);

        _pdfService.CreateCharacterSheetById(id, oldFile, newFile);

        var fileBytes = System.IO.File.ReadAllBytes(newFile);

        return File(fileBytes, "application/pdf", "CharacterSheet.pdf");
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }


    [HttpGet]
    public IActionResult GetAllCharactersByIds()
    {
      var characters = _lfContext.tblCharacter.ToList();

      foreach (var character in characters)
      {
        const string src = @"Assets\InputPdfs\EmptyPassport.pdf";
        var des = $@"Assets\OutputPdfs\FilledPassport{character.Id}.pdf";

        var oldFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), src);
        var newFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), des);

        _pdfService.CreateCharacterSheetById(character.Id, oldFile, newFile);
      }

      var folderPath = @"Assets\OutputPdfs";
      var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), folderPath);

      var combinedPathx = @"Assets\CombinedPdfs\Combined.pdf";
      var combinedPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), combinedPathx);

      var d = new DirectoryInfo(path);

      var pdf = new PdfDocument(new PdfWriter(combinedPath));
      var merger = new PdfMerger(pdf);

      var pages = 2;
      foreach (var file in d.GetFiles("*.pdf"))
      {
        var firstSourcePdf = new PdfDocument(new PdfReader(file.FullName));
        var subPages = firstSourcePdf.GetNumberOfPages();
        merger.Merge(firstSourcePdf, 1, subPages);
        firstSourcePdf.Close();

        pages += subPages;
      }

      pdf.Close();

      //converting Pdf file into bytes array  
      /*
      var dataBytes = System.IO.File.ReadAllBytes(oldFile);
      var base64 = Convert.ToBase64String(dataBytes);
      var pdfObj = new JObject { { "Pdf", base64 } };
      */

      var fileBytes = System.IO.File.ReadAllBytes(combinedPath);

      return File(fileBytes, "application/pdf", "CharacterSheet.pdf");
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteCharacterById(int id)
    {
      return Ok();
    }
  }
}