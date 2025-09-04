using AiryPay.Domain.Common;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace AiryPay.Infrastructure.Data.ValueGenerators;

internal class IdValueGenerator<TId> : ValueGenerator<TId>
    where TId : IId
{
    public override TId Next(EntityEntry entry)
    {
        return (TId) TId.Default();
    }

    public override bool GeneratesTemporaryValues => true;
}