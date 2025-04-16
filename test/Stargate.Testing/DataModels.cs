using Stargate.Core.Domain;

namespace Stargate.Testing;

public static class DataModels
{
    public static Person CreatePerson(string? name = null)
    {
        return new Person(name ?? "James Bond");
    }

    public static AstronautDuty CreateAstronautDuty(
        Person person,
        string? rank = null,
        string? dutyTitle = null,
        DateTime? dutyStartDate = null,
        DateTime? dutyEndDate = null)
    {
        return new AstronautDuty(
            person: person,
            rank: rank ?? "Rookie",
            dutyTitle: dutyTitle ?? "Junior Astronaut",
            dutyStartDate: dutyStartDate ?? DateTime.UtcNow,
            dutyEndDate: dutyEndDate);
    }
}
