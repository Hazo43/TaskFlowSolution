using AutoMapper;
using Domain.Entites;
using Shared.DTOs.TaskModule;

namespace Services.MappingProfile
{
    public class TasksProfile : Profile
    {
        public TasksProfile()
        {
            CreateMap<Tasks, TaskResultDto>()
               .ForMember(dest => dest.AssignedToName, opt => opt.MapFrom
               // null رجعلو null لو ب  src.AssignedTo.DisplayName رجعلو ال null مش ب AssignedTo  لو ال
                                                       (src => src.AssignedTo != null ? src.AssignedTo.DisplayName : null))
               .ForMember(dest => dest.CategoryName, opt => opt.MapFrom
               // null رجعلو null لو ب  src.Category.Name رجعلو ال null مش ب Category  لو ال
                                                        (src => src.Category != null ? src.Category.Name : null))
               .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project.Name))
               // from PriorityDto To Priority
               .ForMember(dest => dest.PriorityDTO, opt => opt.MapFrom(src => src.Priority))
               // from StatusDTO To Status
               .ForMember(dest => dest.StatusDTO, opt => opt.MapFrom(src => src.Status));
        }
    }
}
