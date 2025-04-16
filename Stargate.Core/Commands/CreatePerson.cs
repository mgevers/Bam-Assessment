using Ardalis.Result;
using MediatR;
using Stargate.Core.Contracts;
using Stargate.Core.Domain;

namespace Stargate.Core.Commands;

public class CreatePersonCommand : IRequest<Result<int>>
{
    public required string Name { get; set; } = string.Empty;
}

public class CreatePersonCommandHandler : IRequestHandler<CreatePersonCommand, Result<int>>
{
    private readonly IRepository<Person> _repository;

    public CreatePersonCommandHandler(IRepository<Person> repository)
    {
        _repository = repository;
    }

    public async Task<Result<int>> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
    {
        var person = new Person(request.Name);

        _repository.Add(person);
        var commitResult = await _repository.CommitTransaction(cancellationToken);

        if (!commitResult.IsSuccess)
        {
            return commitResult.AsTypedError<int>();
        }

        return person.Id;
    }
}
