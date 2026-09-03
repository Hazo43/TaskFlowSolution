using AutoMapper;
using Domain.Entites;
using Shared.DTOs.Project;

namespace Services.MappingProfile
{
    public class ProjectsProfile : Profile
    {
        public ProjectsProfile()
        {
            // Project → ProjectResultDto
            CreateMap<Project, ProjectResultDto>();

            // CreateProjectDto → Project
            CreateMap<CreateProjectDto, Project>();

            // UpdateProjectDto → Project
            CreateMap<UpdateProjectDto, Project>();
        }
    }
}
