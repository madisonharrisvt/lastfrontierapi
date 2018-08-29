using System.Threading.Tasks;
using LastFrontierApi.Models;

namespace LastFrontierApi.Services
{
    public interface IPlayerService
    {
        Task<Player> CreatePlayerFromEmail(string email);
    }
}
