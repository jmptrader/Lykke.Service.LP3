using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Common;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Common.ApiLibrary.Exceptions;
using Lykke.Service.LP3.Client;
using Lykke.Service.LP3.Client.Models.Settings;
using Lykke.Service.LP3.Domain;
using Lykke.Service.LP3.Domain.Services;
using Lykke.Service.LP3.Domain.Settings;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.LP3.Controllers
{
    [Route("/api/[controller]")]
    public class SettingsController : Controller, ISettingsApi
    {
        private readonly ISettingsService _settingsService;
        private readonly ILevelsService _levelsService;

        public SettingsController(ISettingsService settingsService,
            ILevelsService levelsService)
        {
            _settingsService = settingsService;
            _levelsService = levelsService;
        }
        
        [HttpGet("baseAssetPair")]
        [ProducesResponseType(typeof(BaseAssetPairSettingsModel), (int) HttpStatusCode.OK)]
        public async Task<BaseAssetPairSettingsModel> GetBaseAssetPairSettingsAsync()
        {
            var baseAssetPairSettings = await _settingsService.GetBaseAssetPairSettings();
            
            var model = Mapper.Map<BaseAssetPairSettingsModel>(baseAssetPairSettings);

            return model;
        }
        
        [HttpPost("baseAssetPair")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task SaveBaseAssetPairSettingsAsync([FromBody] BaseAssetPairSettingsModel model)
        {
            var settings = Mapper.Map<BaseAssetPairSettings>(model);
            await _settingsService.SaveBaseAssetPairSettings(settings);
        }
        
        [HttpGet("levels")]
        [ProducesResponseType(typeof(IReadOnlyList<LevelSettingsModel>), (int) HttpStatusCode.OK)]
        public async Task<IReadOnlyList<LevelSettingsModel>> GetLevelsSettingsAsync()
        {
            var levels = _levelsService.GetLevels();
            
            var model = Mapper.Map<IReadOnlyList<LevelSettingsModel>>(levels);

            return model;
        }
        
        [HttpPost("levels")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task AddAsync([FromBody] LevelSettingsModel model)
        {
            var levels = _levelsService.GetLevels();
            if (levels.Any(x => string.Equals(x.Name, model.Name, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ValidationApiException($"A level with name {model.Name} already exist");
            }

            var level = Mapper.Map<Level>(model);

            await _levelsService.AddAsync(level);
        }

        [HttpDelete("levels/{name}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task DeleteAsync(string name)
        {
            await _levelsService.DeleteAsync(name);
        }
        
        
        [HttpPut("levels")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task UpdateAsync([FromBody] LevelSettingsModel model)
        {
            var level = _levelsService.GetLevels();
            if (level.All(x => !string.Equals(x.Name, model.Name, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ValidationApiException($"A level with name {model.Name} doesn't exist");
            }

            await _levelsService.UpdateAsync(model.Name, model.Delta, model.Volume);
        }
    }
}
