using System.Collections.Generic;
using System.Threading.Tasks;
using LastFrontierApi.Models;

namespace LastFrontierApi.Services
{
    public interface IPlayerService
    {
        Task<Player> CreatePlayerFromEmail(string email);
        Task DeletePlayer(Player player);
        List<Player> GetPlayersFromNpcShift(int shiftId);
    }
}
