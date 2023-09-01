using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SampleWebApiAspNetCore.Dtos;
using SampleWebApiAspNetCore.Entities;
using SampleWebApiAspNetCore.Helpers;
using SampleWebApiAspNetCore.Services;
using SampleWebApiAspNetCore.Models;
using SampleWebApiAspNetCore.Repositories;
using System.Text.Json;

namespace SampleWebApiAspNetCore.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class PlayersController : ControllerBase
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IMapper _mapper;
        private readonly ILinkService<PlayersController> _linkService;

        public PlayersController(
            IPlayerRepository playerRepository,
            IMapper mapper,
            ILinkService<PlayersController> linkService)
        {
            _playerRepository = playerRepository;
            _mapper = mapper;
            _linkService = linkService;
        }

        [HttpGet(Name = nameof(GetAllPlayers))]
        public ActionResult GetAllPlayers(ApiVersion version, [FromQuery] QueryParameters queryParameters)
        {
            List<PlayerEntity> playerEntity = _playerRepository.GetAll(queryParameters).ToList();

            var allItemCount = _playerRepository.Count();

            var paginationMetadata = new
            {
                totalCount = allItemCount,
                pageSize = queryParameters.PageCount,
                currentPage = queryParameters.Page,
                totalPages = queryParameters.GetTotalPages(allItemCount)
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            var links = _linkService.CreateLinksForCollection(queryParameters, allItemCount, version);
            var toReturn = playerEntity.Select(x => _linkService.ExpandSinglePlayerEntity(x, x.Id, version));

            return Ok(new
            {
                value = toReturn,
                links = links
            });
        }

        [HttpGet]
        [Route("{id:int}", Name = nameof(GetSinglePlayer))]
        public ActionResult GetSinglePlayer(ApiVersion version, int id)
        {
            PlayerEntity playerEntity = _playerRepository.GetSingle(id);

            if (playerEntity == null)
            {
                return NotFound();
            }

            PlayerDto item = _mapper.Map<PlayerDto>(playerEntity);

            return Ok(_linkService.ExpandSinglePlayerEntity(item, item.Id, version));
        }

        [HttpPost(Name = nameof(AddPlayer))]
        public ActionResult<PlayerDto> AddPlayer(ApiVersion version, [FromBody] PlayerCreateDto playerCreateDto)
        {
            if (playerCreateDto == null)
            {
                return BadRequest();
            }

            PlayerEntity toAdd = _mapper.Map<PlayerEntity>(playerCreateDto);

            _playerRepository.Add(toAdd);

            if (!_playerRepository.Save())
            {
                throw new Exception("Creating a playerEntity failed on save.");
            }

            PlayerEntity newPlayerEntity = _playerRepository.GetSingle(toAdd.Id);
            PlayerDto playerDto = _mapper.Map<PlayerDto>(newPlayerEntity);

            return CreatedAtRoute(nameof(GetSinglePlayer),
                new { version = version.ToString(), id = newPlayerEntity.Id },
                _linkService.ExpandSinglePlayerEntity(playerDto, playerDto.Id, version));
        }

        [HttpPatch("{id:int}", Name = nameof(PartiallyUpdatePlayer))]
        public ActionResult<PlayerDto> PartiallyUpdatePlayer(ApiVersion version, int id, [FromBody] JsonPatchDocument<PlayerUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            PlayerEntity existingEntity = _playerRepository.GetSingle(id);

            if (existingEntity == null)
            {
                return NotFound();
            }

            PlayerUpdateDto playerUpdateDto = _mapper.Map<PlayerUpdateDto>(existingEntity);
            patchDoc.ApplyTo(playerUpdateDto);

            TryValidateModel(playerUpdateDto);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(playerUpdateDto, existingEntity);
            PlayerEntity updated = _playerRepository.Update(id, existingEntity);

            if (!_playerRepository.Save())
            {
                throw new Exception("Updating a playerEntity failed on save.");
            }

            PlayerDto playerDto = _mapper.Map<PlayerDto>(updated);

            return Ok(_linkService.ExpandSinglePlayerEntity(playerDto, playerDto.Id, version));
        }

        [HttpDelete]
        [Route("{id:int}", Name = nameof(RemovePlayer))]
        public ActionResult RemovePlayer(int id)
        {
            PlayerEntity playerEntity = _playerRepository.GetSingle(id);

            if (playerEntity == null)
            {
                return NotFound();
            }

            _playerRepository.Delete(id);

            if (!_playerRepository.Save())
            {
                throw new Exception("Deleting a playerEntity failed on save.");
            }

            return NoContent();
        }

        [HttpPut]
        [Route("{id:int}", Name = nameof(UpdatePlayer))]
        public ActionResult<PlayerDto> UpdatePlayer(ApiVersion version, int id, [FromBody] PlayerUpdateDto playerUpdateDto)
        {
            if (playerUpdateDto == null)
            {
                return BadRequest();
            }

            var existingPlayerEntity = _playerRepository.GetSingle(id);

            if (existingPlayerEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(playerUpdateDto, existingPlayerEntity);

            _playerRepository.Update(id, existingPlayerEntity);

            if (!_playerRepository.Save())
            {
                throw new Exception("Updating a playerEntity failed on save.");
            }

            PlayerDto playerDto = _mapper.Map<PlayerDto>(existingPlayerEntity);

            return Ok(_linkService.ExpandSinglePlayerEntity(playerDto, playerDto.Id, version));
        }

        [HttpGet("GetRandomPlayer", Name = nameof(GetRandomPlayer))]
        public ActionResult GetRandomPlayer()
        {
            ICollection<PlayerEntity> playerEntities = _playerRepository.GetRandomPlayer();

            IEnumerable<PlayerDto> dtos = playerEntities.Select(x => _mapper.Map<PlayerDto>(x));

            var links = new List<LinkDto>();

            // self 
            links.Add(new LinkDto(Url.Link(nameof(GetRandomPlayer), null), "self", "GET"));

            return Ok(new
            {
                value = dtos,
                links = links
            });
        }
    }
}
