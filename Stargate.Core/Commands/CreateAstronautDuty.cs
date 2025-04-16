using Ardalis.Result;
using MediatR;
using Microsoft.Extensions.Logging;
using Stargate.Core.Contracts;
using Stargate.Core.Domain;

namespace Stargate.Core.Commands;

public class CreateAstronautDutyCommand : IRequest<Result<int>>
{
    public required string Name { get; set; }

    public required string Rank { get; set; }

    public required string DutyTitle { get; set; }

    public DateTime DutyStartDate { get; set; }
}

public class CreateAstronautDutyCommandHandler : IRequestHandler<CreateAstronautDutyCommand, Result<int>>
{
    private readonly ILogger<CreateAstronautDutyCommandHandler> _logger;
    private readonly IPersonRepository _repository;

    public CreateAstronautDutyCommandHandler(
        ILogger<CreateAstronautDutyCommandHandler> logger,
        IPersonRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<Result<int>> Handle(CreateAstronautDutyCommand request, CancellationToken cancellationToken)
    {
        var personResult = await _repository.GetPersonByNameAsync(request.Name, cancellationToken);

        if (!personResult.IsSuccess)
        {
            _logger.LogError(
                "Failed to commit transaction for creating person {Name}: {Error}",
                request.Name,
                string.Concat(personResult.Errors, ","));

            return personResult.AsTypedError<Person, int>();
        }

        var person = personResult.Value;
        var addDutyResult = person.AddAstronautDuty(
            request.Rank,
            request.DutyTitle,
            request.DutyStartDate);

        if (!addDutyResult.IsSuccess)
        {
            _logger.LogError(
                "Failed to commit transaction for creating person {Name}: {Error}",
                request.Name,
                string.Concat(personResult.Errors, ","));
            return addDutyResult.AsTypedError<int>();
        }

        var commitResult = await _repository.CommitTransaction(cancellationToken);

        if (!commitResult.IsSuccess)
        {
            _logger.LogError(
                "Failed to commit transaction for creating person {Name}: {Error}",
                request.Name,
                string.Concat(personResult.Errors, ","));
            return commitResult.AsTypedError<int>();
        }

        _logger.LogInformation("Created astronaut duty {duty}", person.AstronautDuties.Last());
        return Result.Success(person.AstronautDuties.Last().Id);
    }
}
