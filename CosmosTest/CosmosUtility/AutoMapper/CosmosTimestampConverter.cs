using System;
using AutoMapper;

namespace CosmosTest.CosmosUtility.AutoMapper
{
    public class CosmosTimestampConverter : IValueConverter<int?, DateTimeOffset?>, IValueConverter<DateTimeOffset?, int?>
    {
        public DateTimeOffset? Convert(int? sourceMember, ResolutionContext context)
        {
            return DateTimeOffset.FromUnixTimeSeconds(sourceMember ?? 0);
        }

        public int? Convert(DateTimeOffset? sourceMember, ResolutionContext context)
        {
            return (int) (sourceMember?.ToUnixTimeSeconds() ?? 0);
        }
    }
}