using AutoMapper;
using BlogAPI.Models;
using BlogAPI.Dtos;

namespace BlogAPI.Profiles
{
    public class BlogProfile : Profile
    {
        public BlogProfile()
        {
            // Mappning från BlogPost -> PostResponse
            CreateMap<BlogPost, PostResponse>()
                .ForMember(dest => dest.UserName,
                    opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.CategoryName,
                    opt => opt.MapFrom(src => src.Category.Name));

            CreateMap<Comment, CommentResponse>()
            .ForMember(dest => dest.UserName,
                opt => opt.MapFrom(src => src.User.UserName));
        }
    }
}
