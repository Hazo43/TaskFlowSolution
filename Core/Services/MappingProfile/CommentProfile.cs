using AutoMapper;
using Domain.Entites;
using Shared.DTOs.CommentModule;

namespace Services.MappingProfile
{
    public class CommentProfile : Profile
    {
        public CommentProfile()
        {
            CreateMap<Comment, CommentResultDTO>()
                .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author.DisplayName))
                .ForMember(dest => dest.TaskTitle, opt => opt.MapFrom(src => src.Task.Title));
        }
    }
}
