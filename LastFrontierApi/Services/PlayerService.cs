using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LastFrontierApi.Helpers;
using LastFrontierApi.Models;
using Microsoft.AspNetCore.Identity;

namespace LastFrontierApi.Services
{

    public class PlayerService : IPlayerService
    {
        private readonly ApplicationDbContext _appDbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly LfContext _lfContext;
        private readonly IMapper _mapper;

        public PlayerService(UserManager<AppUser> userManager, IMapper mapper,
            ApplicationDbContext appDbContext, LfContext lfContext)
        {
            _userManager = userManager;
            _mapper = mapper;
            _appDbContext = appDbContext;
            _lfContext = lfContext;
        }

        public async Task<Player> CreatePlayerFromEmail(string email)
        {
            var temporaryPassword = RandomString.GetRandomString(16);

            var newUser = new Registration()
            {
                FirstName = null,
                LastName = null,
                Email = email,
                Password = temporaryPassword
            };

            var userIdentity = _mapper.Map<AppUser>(newUser);

            var playerResult = await _userManager.CreateAsync(userIdentity, temporaryPassword);

            if (!playerResult.Succeeded)
            {
                throw new Exception("Make this exception better :(");
            }

            await _userManager.AddToRoleAsync(userIdentity, "User");

            await _appDbContext.tblPlayer.AddAsync(new Player { IdentityId = userIdentity.Id });
            await _appDbContext.SaveChangesAsync();

            var player = _appDbContext.tblPlayer.FirstOrDefault(p => p.IdentityId == userIdentity.Id);

            return player;
        }

        public async void DeletePlayer(Player player)
        {
            _appDbContext.tblPlayer.Remove(player);

            var charactersToDelete = _lfContext.tblCharacter.Where(c => c.PlayerId == player.Id);
            _lfContext.tblCharacter.RemoveRange(charactersToDelete);
            _lfContext.SaveChanges();

            var deletePlayerResult = await _userManager.DeleteAsync(player.Identity);

            if (!deletePlayerResult.Succeeded)
            {
                throw new Exception("Make this exception better :(");
            }
        }
    }
}
