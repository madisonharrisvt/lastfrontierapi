namespace LastFrontierApi.Services
{
  public interface IPdfService
  {
    void CreateCharacterSheetById(int id, string src, string des);
  }
}