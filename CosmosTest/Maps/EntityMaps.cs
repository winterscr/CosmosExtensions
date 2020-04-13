using System;
using AutoMapper;
using CosmosTest.CosmosUtility.AutoMapper;
using CosmosTest.DomainModels;
using CosmosTest.EntityModels;

namespace CosmosTest.Maps
{
    /// <summary>
    ///     Maps between entities and domain models
    /// </summary>
    public class EntityMaps : Profile
    {
        public EntityMaps()
        {
            // Have to provide explicit maps, cannot use attribute based mapping [AutoMap]
            // or ReverseMap() due to custom value conversion and name mapping.
            // For simpler scenarios, consider using [AutoMap] on the entity

            CreateMap<User, UserEntity>()
                .ForMember(i => i.Timestamp, opt =>
                {
                    opt.MapFrom(s => s.LastUpdated);
                    opt.ConvertUsing(new CosmosTimestampConverter(), s => s.LastUpdated);
                });

            CreateMap<UserEntity, User>()
                .ForMember(i => i.LastUpdated, opt =>
                {
                    opt.MapFrom(s => s.Timestamp);
                    opt.ConvertUsing(new CosmosTimestampConverter(), s => s.Timestamp);
                });
        }
    }
}