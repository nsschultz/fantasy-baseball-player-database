using System;
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
        private readonly IPlayerUpdateService _updateService;
        private readonly IUpsertPlayersService _upsertService;

        /// <summary>Creates a new instance of the controller.</summary>
        /// <param name="context">The player context.</param>
        /// <param name="playerBuilder">Service for converting a PlayerEntity to a BaseballPlayer.</param>
        /// <param name="getService">Service for getting players.</param>
        /// <param name="updateService">Service for updating a player.</param>
        /// <param name="upsertService">Service for upserting players.</param>
        public PlayerController(IBaseballPlayerBuilderService playerBuilder,
                                IGetPlayersService getService,
                                IPlayerUpdateService updateService,
                                IUpsertPlayersService upsertService) 
        { 
            _playerBuilder = playerBuilder;
            _getService = getService;
            _updateService = updateService;
            _upsertService = upsertService;
        }

        /// <summary>Gets all of the players from the source.</summary>
        /// <returns>All of the players from the source.</returns>
        [HttpGet] public async Task<PlayerCollection> GetPlayers() =>
            new PlayerCollection { Players = (await _getService.GetPlayers()).Select(player => _playerBuilder.BuildBaseballPlayer(player)).ToList() }; 

        /// <summary>Gets all of the players from the source.</summary>
        /// <param name="player">The object containing all of the player's data (non-changed data must be included as well).</param>
        [HttpPut("{id}")] public async Task UpdatePlayer([FromRoute] Guid id, [FromBody] BaseballPlayer player) 
        {
            if (player == null) throw new BadRequestException("Player not set");
            if (id == default) throw new BadRequestException("Invalid player id used");
            if (id != player.Id) throw new BadRequestException("The ids must match");
            await _updateService.UpdatePlayer(player);
        } 

        /// <summary>Upserts all of the players into the source.</summary>
        /// <param name="players">All of the players to upsert into the source.</param>
        [HttpPost] public async Task UpsertPlayers([FromBody] PlayerCollection players)
        {
            if (players == null || players.Players ==  null) throw new BadRequestException("Players not set");
            await _upsertService.UpsertPlayers(players.Players);
        }
    }
}