using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using TournamentManager.Contexts;
using TournamentManager.DbModels;
using TournamentManager.Requests;
using TournamentManager.Services;

namespace TournamentManager.Controllers
{
    public interface IGenericCommand<T>
    {
        ExecutionToken ExecuteAsync(params object[] parameters);

        public T Execute(params object[] parameters);

        public T WaitResult();
    }

    public abstract class GenericCommand<T> : IGenericCommand<T>
    {
        private readonly Scheduler _scheduler;
        ExecutionToken _token;

        public GenericCommand(Scheduler scheduler)
        {
            _scheduler = scheduler;

            _token = _scheduler.Schedule((token) =>
            {
                token.SetResult(Execute());
            });
        }

        public ExecutionToken ExecuteAsync(params object[] parameters)
        {
            _token = _scheduler.Schedule((token) =>
            {
                token.SetResult(Execute(parameters));
            });
            return _token;
        }

        public abstract T Execute(params object[] parameters);

        public T WaitResult()
        {
            return _token.WaitResult<T>();
        }

        public void Wait()
        {
            _token.Wait();
        }
    }

    public class GetListDivisionCommand : GenericCommand<List<Division>>
    {
        private readonly Scheduler _scheduler;
        private readonly IGenericRepository _repo;
        ExecutionToken token;

        public GetListDivisionCommand(Scheduler scheduler, IGenericRepository repo) : base(scheduler)
        {
            _scheduler = scheduler;
            _repo = repo;
        }

        public override List<Division> Execute(params object[] parameters)
        {
            return _repo.GetAll<Division>().ToList();
        }
    }


    public class GetDivisionById : GenericCommand<Division>
    {
        private readonly Scheduler _scheduler;
        private readonly IGenericRepository _repo;
        ExecutionToken token;

        public GetDivisionById(Scheduler scheduler, IGenericRepository repo) : base(scheduler)
        {
            _scheduler = scheduler;
            _repo = repo;
        }

        public override Division Execute(params object[] parameters)
        {
            return _repo.GetById<Division>((int)parameters[0]);
        }
    }

    public interface ICommandFactory
    {
        T GetCommand<T>();
    }

    public class CommandFactory : ICommandFactory
    {
        private IServiceProvider _collection;
        public CommandFactory(IServiceProvider collection)
        {
            _collection = collection;
        }
        public T GetCommand<T>()
        {
            return _collection.GetRequiredService<T>();
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class DivisionsController(ICommandFactory factory, Scheduler scheduler, IGenericRepository repo) : ControllerBase
    {
        private readonly Scheduler _scheduler = scheduler;
        private readonly IGenericRepository _repo = repo;
        private readonly ICommandFactory _factory = factory;

        [HttpGet]
        public IActionResult ListDivision()
        {

            var command = _factory.GetCommand<GetListDivisionCommand>();

            command.ExecuteAsync();
            
            //var divisions = _scheduler.Schedule((token) =>
            //{
            //    token.SetResult(_repo.GetAll<Division>().ToList());
            //}).WaitResult<List<Division>>();

            return Ok(command.WaitResult());
        }

        [HttpGet("{id}")]
        public IActionResult GetDivision(int id)
        {
            var command = _factory.GetCommand<GetDivisionById>();

            command.ExecuteAsync(id);

            //var division = _scheduler.Schedule((token) =>
            //{
            //    token.SetResult(_repo.GetById<Division>(id));
            //}).WaitResult<Division>();

            var division = command.WaitResult();

            if (division == null)
                return NotFound();

            return Ok(division);
        }

        [HttpGet("{id}/phases")]
        public IActionResult ListPhases(int id)
        {
            var phases = _scheduler.Schedule((token) =>
            {
                var res = _repo.GetAll<Division>().Include(d => d.Phases).FirstOrDefault(d => d.Id == id).Phases.ToList();
                token.SetResult(res);
            }).WaitResult<List<Phase>>();

            return Ok(phases);
        }

        [HttpPost]
        [TypeFilter(typeof(AuthorizationFilterAttribute))]
        public IActionResult AddDivision([FromBody] PostDivisionRequest request)
        {
            var division = _scheduler.Schedule((executionToken) =>
            {
                var division = new Division
                {
                    Name = request.Name
                };

                _repo.Add(division);
                _repo.Save();
                executionToken.SetResult(division);
            }).WaitResult<Division>();

            return Ok(division);
        }

        [HttpPut("{id}")]
        [TypeFilter(typeof(AuthorizationFilterAttribute))]
        public IActionResult UpdateDivision(int id, [FromBody] PostDivisionRequest request)
        {
            var division = _scheduler.Schedule((token) =>
            {
                var division = _repo.GetById<Division>(id);

                if (division == null)
                    return;

                division.Name = request.Name;

                _repo.Save();
                token.SetResult(division);
            }).WaitResult<Division>();

            if (division == null)
                return NotFound();

            return Ok(division);
        }

        [HttpDelete("{id}")]
        [TypeFilter(typeof(AuthorizationFilterAttribute))]
        public IActionResult DeleteDivision(int id)
        {
            _scheduler.Schedule((token) =>
            {
                _repo.DeleteById<Division>(id);

                _repo.Save();
            }).Wait();

            return Ok();
        }
    }
}
