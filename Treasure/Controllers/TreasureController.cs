using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using System;
using Treasure.Helpers.ApiOutput;
using Treasure.Models.Treasuare;
using Treasure.Services;

namespace Treasure.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TreasureController : ControllerBase
    {
        private readonly ITreasureService _treasureService;
        private readonly IValidator<MatrixRequest> _treasureValidator;

        public TreasureController(ITreasureService treasureService, IValidator<MatrixRequest> treasureValidator)
        {
            _treasureService = treasureService;
            _treasureValidator = treasureValidator;
        }

        /// <summary>
        /// get smallest of fuel
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("onepiece-treasure")]
        public async Task<IActionResult> FindTreasure([FromBody] MatrixRequest request)
        {
            ValidationResult result = await _treasureValidator.ValidateAsync(request);
            if (!result.IsValid)
            {
                var strError = string.Join(',', result.Errors.Select(x => x.ErrorMessage));
                return Ok(ApiOutput.Success(strError));
            }
            var res = await _treasureService.FindMinimumFuel(request);
            return Ok(ApiOutput.Success(res));
        }
    }
}
