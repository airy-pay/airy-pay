using AiryPayNew.Domain.Common;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace AiryPayNew.Infrastructure.Data.ValueGenerators;

internal class IdValueGenerator<TId> : ValueGenerator<TId>
    where TId : IId
{
    public override TId Next(EntityEntry entry)
    {
        return (TId) TId.Default();
    }

    public override bool GeneratesTemporaryValues => true;
}