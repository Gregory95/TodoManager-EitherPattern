using AutoMapper;
using GKTodoManager.Domain.Dtos;
using GKTodoManager.Domain.Entities;

namespace GKTodoManager.Domain;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ApplicationUser, RegisterUserResponseDto>()
            .ForMember(dest => dest.FullName,
                        opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
            .ReverseMap();

        CreateMap<Entities.Task, TaskResponseDto>()
            .ReverseMap();

        CreateMap<CreateTaskRequestDto, TaskResponseDto>()
            .ReverseMap();

        CreateMap<TaskGroup, TaskGroupResponseDto>()
            .ReverseMap();

        CreateMap<CreateTaskGroupRequestDto, TaskGroupResponseDto>()
            .ReverseMap();
    }
}
