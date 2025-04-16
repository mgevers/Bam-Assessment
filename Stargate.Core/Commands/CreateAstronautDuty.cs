using Ardalis.Result;
using MediatR;
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
    private readonly IPersonRepository _repository;
    public CreateAstronautDutyCommandHandler(IPersonRepository repository)
    {
        _repository = repository;
    }
    public async Task<Result<int>> Handle(CreateAstronautDutyCommand request, CancellationToken cancellationToken)
    {
        var personResult = await _repository.GetPersonByNameAsync(request.Name, cancellationToken);
        
        if (!personResult.IsSuccess)
        {
            return personResult.AsTypedError<Person, int>();
        }

        var person = personResult.Value;
        var addDutyResult = person.AddAstronautDuty(
            request.Rank,
            request.DutyTitle,
            request.DutyStartDate);

        if (!addDutyResult.IsSuccess)
        {
            return addDutyResult.AsTypedError<int>();
        }

        var commitResult = await _repository.CommitTransaction(cancellationToken);

        if (!commitResult.IsSuccess)
        {
            return commitResult.AsTypedError<int>();
        }

        return Result.Success(person.AstronautDuties.Last().Id);
    }
}
