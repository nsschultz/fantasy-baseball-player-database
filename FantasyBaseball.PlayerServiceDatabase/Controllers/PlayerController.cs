using System.Linq;
using System.Threading.Tasks;
using FantasyBaseball.Common.Exceptions;
using FantasyBaseball.Common.Models;
using FantasyBaseball.PlayerServiceDatabase.Services;
using Microsoft.AspNetCore.Mvc;

namespace FantasyBaseball.PlayerServiceDatabase.Controllers
{
    /// <summary>Endpoint for retrieving player data.</summary>
    [Route("api/player")] [ApiController] public class PlayerController : ControllerBase
    {
        private readonly IBaseballPlayerBuilderService _playerBuilder;
        private readonly IGetPlayersService _getService;
        private readonly IUpsertPlayersService _upsertService;

        /// <summary>Creates a new instance of the controller.</summary>
        /// <param name="context">The player context.</param>
        /// <param name="playerBuilder">Service for converting a PlayerEntity to a BaseballPlayer.</param>
        /// <param name="getService">Service for getting players.</param>
        /// <param name="upsertService">Service for upserting players.</param>
        public PlayerController(IBaseballPlayerBuilderService playerBuilder,
                                IGetPlayersService getService,
                                IUpsertPlayersService upsertService) 
        { 
            _playerBuilder = playerBuilder;
            _getService = getService;
            _upsertService = upsertService;
        }

        /// <summary>Gets all of the players from the source.</summary>
        /// <returns>All of the players from the source.</returns>
        [HttpGet] public async Task<PlayerCollection> GetPlayers() =>
            new PlayerCollection { Players = (await _getService.GetPlayers()).Select(player => _playerBuilder.BuildBaseballPlayer(player)).ToList() }; 

        /// <summary>Upserts all of the players into the source.</summary>
        /// <param name="players">All of the players to upsert into the source.</param>
        [HttpPost] public async Task UpsertPlayers(PlayerCollection players)
        {
            if (players == null || players.Players ==  null) throw new BadRequestException("Players not set");
            await _upsertService.UpsertPlayers(players.Players);
        }
    }
}