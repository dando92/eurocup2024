﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TournamentManager.Contexts;
using TournamentManager.DbModels;
using TournamentManager.Requests;

namespace TournamentManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhasesController(IGenericRepository<Phase> repo) : ControllerBase
    {
        private readonly IGenericRepository<Phase> _repo = repo;

        [HttpGet]
        public IActionResult ListPhase()
        {
            return Ok(_repo.GetAll());
        }

        [HttpPost]
        public IActionResult AddPhase([FromBody] PostPhaseRequest request)
        {
            var phase = new Phase
            {
                DivisionId = request.DivisionId,
                Name = request.Name
            };

            _repo.Add(phase);

            return Ok(phase);
        }

        [HttpPut("{id}")]
        public IActionResult UpdatePhase(int id, [FromBody] PostPhaseRequest request)
        {
            var phase = _repo.GetById(id);

            if (phase == null)
            {
                return NotFound();
            }

            phase.Name = request.Name;

            _repo.Update(phase);

            return Ok(phase);
        }

        [HttpPatch("{id}")]
        public IActionResult UpdatePhasePartial(int id, [FromBody] PostPhaseRequest request)
        {
            var phase = _repo.GetById(id);

            if (phase == null)
            {
                return NotFound();
            }

            phase.Name = request.Name;

            _repo.Update(phase);

            return Ok(phase);
        }

        [HttpDelete("{id}")]
        public IActionResult DeletePhase(int id)
        {
            _repo.DeleteById(id);

            return Ok();
        }
    }
}
