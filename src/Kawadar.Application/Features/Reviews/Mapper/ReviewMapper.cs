using AutoMapper;
using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.Reviews.Dtos;
using Kawadar.Domain.Reviews;

namespace Kawadar.Application.Features.Reviews.Mapper
{
    public class ReviewMapper : Profile
    {
        public ReviewMapper()
        {
            CreateMap<(Review review, UserDto user), ReviewDto>()
                .ForMember(dest => dest.ReviewerUserName, opt => opt.MapFrom(x => x.user.UserName))
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(x => x.review.Rating))
                .ForMember(dest => dest.JobId, opt => opt.MapFrom(x => x.review.JobId))
                .ForMember(dest => dest.Comment, opt => opt.MapFrom(x => x.review.Comment));
        }
    }
}
