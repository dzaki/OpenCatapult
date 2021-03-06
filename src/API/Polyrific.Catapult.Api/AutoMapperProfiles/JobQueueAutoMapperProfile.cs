﻿// Copyright (c) Polyrific, Inc 2018. All rights reserved.

using System.Collections.Generic;
using AutoMapper;
using Newtonsoft.Json;
using Polyrific.Catapult.Api.Core.Entities;
using Polyrific.Catapult.Shared.Dto.JobQueue;

namespace Polyrific.Catapult.Api.AutoMapperProfiles
{
    public class JobQueueAutoMapperProfile : Profile
    {
        public JobQueueAutoMapperProfile()
        {
            CreateMap<JobQueue, JobDto>()
                .ForMember(dest => dest.JobTasksStatus, opt => opt.MapFrom(src => JsonConvert.DeserializeObject<List<JobTaskStatusDto>>(src.JobTasksStatus)))
                .ForMember(dest => dest.OutputValues, opt => opt.MapFrom(src => JsonConvert.DeserializeObject<Dictionary<string, string>>(src.OutputValues)))
                .ForMember(dest => dest.JobDefinitionName, opt => opt.MapFrom(src => src.JobDefinition.Name))
                .ForMember(dest => dest.IsDeletion, opt => opt.MapFrom(src => src.JobDefinition.IsDeletion))
                .ForMember(dest => dest.ProjectStatus, opt => opt.MapFrom(src => src.Project.Status));

            CreateMap<NewJobDto, JobDto>();

            CreateMap<UpdateJobDto, JobQueue>()
                .ForMember(dest => dest.JobTasksStatus, opt => opt.MapFrom(src => JsonConvert.SerializeObject(src.JobTasksStatus)))
                .ForMember(dest => dest.OutputValues, opt => opt.MapFrom(src => JsonConvert.SerializeObject(src.OutputValues)));
        }
    }
}
