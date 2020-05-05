﻿using System.Threading.Tasks;
using W3ChampionsStatisticService.PadEvents;
using W3ChampionsStatisticService.Ports;
using W3ChampionsStatisticService.ReadModelBase;

namespace W3ChampionsStatisticService.PlayerProfiles
{
    public class PlayerModelHandler : IReadModelHandler
    {
        private readonly IPlayerRepository _playerRepository;

        public PlayerModelHandler(
            IPlayerRepository playerRepository
            )
        {
            _playerRepository = playerRepository;
        }

        public async Task Update(MatchFinishedEvent nextEvent)
        {
            foreach (var playerRaw in nextEvent.match.players)
            {
                var player = await _playerRepository.Load(playerRaw.id)
                             ?? PlayerProfile.Create(playerRaw.id, playerRaw.battleTag);
                player.RecordWin(
                    (Race) playerRaw.race,
                    nextEvent.match.gameMode,
                    playerRaw.won);
                player.UpdateRank(
                    nextEvent.match.gameMode,
                    (int?) playerRaw.updatedMmr?.rating ?? (int?) playerRaw.mmr?.rating ?? 0,
                    (int?) playerRaw.updatedRanking?.rp ?? (int?) playerRaw.ranking?.rp ?? 0);
                await _playerRepository.UpsertPlayer(player);
            }
        }
    }
}