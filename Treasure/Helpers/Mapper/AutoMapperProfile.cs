using AutoMapper;
using Treasure.Data.Entitites;
using Treasure.Models.Treasuare;

namespace Treasure.Helpers.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<MatrixInputHistory, MatrixRequest>()
                .ForMember(x=>x.M, y=>y.MapFrom(map=>map.ColumnMatrix))
                .ForMember(x=>x.N, y=>y.MapFrom(map=>map.RowMatrix))
                .ForMember(x=>x.P, y=>y.MapFrom(map=>map.Treasure))
                .ReverseMap();
        }
    }
}
