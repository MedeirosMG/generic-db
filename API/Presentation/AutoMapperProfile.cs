using AutoMapper;
using MyDB.Application.CRUD.Models.Database;
using MyDB.Application.CRUD.Models.Database.Payloads;
using MyDB.Application.CRUD.Models.Database.Responses;
using MyDB.Domain.CRUD.DatabaseService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyDB
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Databases mapper
            CreateMap<Database, FullDatabaseViewModel>();
            CreateMap<Database, DatabaseViewModel>().ForMember(dest => dest.tables, opt => opt.MapFrom(src => src.tablesId));

            // Tables mapper
            CreateMap<Table, TableViewModel>();
            CreateMap<TableAttribute, TableAttributeViewModel>();
            CreateMap<TableAttributePayload, TableAttribute>();
        }
    }
}
