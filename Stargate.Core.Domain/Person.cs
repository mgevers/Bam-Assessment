﻿using Ardalis.Result;

namespace Stargate.Core.Domain;

public class Person
{
    private Person() { }

    public Person(string name)
    {
        Name = name;
    }

    public int Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public virtual AstronautDetail? AstronautDetail { get; private set; }

    public virtual IReadOnlyList<AstronautDuty> AstronautDuties { get; private set; } = new List<AstronautDuty>();

    public Result AddAstronautDuty(string rank, string dutyTitle, DateTime dutyStartDate)
    {
        var lastDuty = AstronautDuties.LastOrDefault();

        if (lastDuty != null)
        {
            if (lastDuty.DutyTitle == AstronautDuty.Retired)
            {
                return Result.Error("Cannot add a new duty after retirement.");
            }

            lastDuty.DutyEndDate = dutyStartDate;
        }

        var newDuty = new AstronautDuty(
            person: this,
            rank,
            dutyTitle,
            dutyStartDate,
            dutyEndDate: null);

        AstronautDuties = [.. AstronautDuties, newDuty];

        if (AstronautDetail == null)
        {
            AstronautDetail = new AstronautDetail(
                person: this,
                rank,
                dutyTitle,
                dutyStartDate);
        }
        else
        {
            AstronautDetail.CurrentRank = rank;
            AstronautDetail.CurrentDutyTitle = dutyTitle;
            if (dutyTitle == AstronautDuty.Retired)
            {
                AstronautDetail.CareerEndDate = dutyStartDate.AddDays(-1).Date;
            }
        }

        return Result.Success();
    }
}
