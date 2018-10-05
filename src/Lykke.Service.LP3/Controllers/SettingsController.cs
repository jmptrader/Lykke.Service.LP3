using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Common.ApiLibrary.Exceptions;
using Lykke.Service.LP3.Client;
using Lykke.Service.LP3.Client.Models.Settings;
using Lykke.Service.LP3.Domain.Services;
using Lykke.Service.LP3.Domain.Settings;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.LP3.Controllers
{
    [Route("/api/[controller]")]
    public class SettingsController : Controller, ISettingsApi
    {
        private readonly ISettingsService _settingsService;

        public SettingsController(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }
        
        [HttpGet("baseAssetPair")]
        [ProducesResponseType(typeof(BaseAssetPairSettingsModel), (int) HttpStatusCode.OK)]
        public async Task<BaseAssetPairSettingsModel> GetBaseAssetPairSettingsAsync()
        {
            var baseAssetPairSettings = await _settingsService.GetBaseAssetPairSettings();
            
            var model = Mapper.Map<BaseAssetPairSettingsModel>(baseAssetPairSettings);

            return model;
        }
        
        [HttpGet("levels")]
        [ProducesResponseType(typeof(IReadOnlyList<LevelSettingsModel>), (int) HttpStatusCode.OK)]
        public async Task<IReadOnlyList<LevelSettingsModel>> GetLevelsSettingsAsync()
        {
            var levelSettings = await _settingsService.GetLevelSettingsAsync();
            
            var model = Mapper.Map<IReadOnlyList<LevelSettingsModel>>(levelSettings);

            return model;
        }
        
        [HttpPost("levels")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task AddAsync([FromBody] LevelSettingsModel model)
        {
            var levelsSettings = await _settingsService.GetLevelSettingsAsync();
            if (levelsSettings.Any(x => string.Equals(x.Name, model.Name, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ValidationApiException($"A level with name {model.Name} already exist");
            }

            var levelSettings = Mapper.Map<LevelSettings>(model);

            await _settingsService.AddAsync(levelSettings);
        }

        [HttpDelete("levels/{name}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task DeleteAsync(string name)
        {
            await _settingsService.DeleteAsync(name);
        }
        
        
        [HttpPut("levels")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task UpdateAsync([FromBody] LevelSettingsModel model)
        {
            var levelsSettings = await _settingsService.GetLevelSettingsAsync();
            if (levelsSettings.All(x => !string.Equals(x.Name, model.Name, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ValidationApiException($"A level with name {model.Name} doesn't exist");
            }

            var levelSettings = Mapper.Map<LevelSettings>(model);

            await _settingsService.UpdateAsync(levelSettings);
        }
    }
}