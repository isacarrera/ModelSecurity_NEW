using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Entity.DTOs;
using Entity.DTOs.FormModuleDTOs;
using Entity.DTOs.RolFormPermissionDTOs;
using Entity.DTOs.RolUserDTOs;
using Entity.DTOs.UserDTOs;
using Entity.Model;

namespace Business.GeneralMapper
{
    public class GeneralMapper : Profile
    {
        public GeneralMapper()
        {
            CreateMap<Person, PersonDTO>().ReverseMap();

            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<User, UserCreateDTO>().ReverseMap();

            CreateMap<Permission,PermissionDTO>().ReverseMap();

            CreateMap<Rol, RolDTO>().ReverseMap();

            CreateMap<RolFormPermission, RolFormPermissionDTO>().ReverseMap();
            CreateMap<RolFormPermission, RolFormPermissionOptionsDTO>().ReverseMap();

            CreateMap<Form, FormDTO>().ReverseMap();

            CreateMap<FormModule, FormModuleDTO>().ReverseMap();
            CreateMap<FormModule, FormModuleOptionsDTO>().ReverseMap();

            CreateMap<Module, ModuleDTO>().ReverseMap();

            CreateMap<RolUser, RolUserDTO>().ReverseMap();
            CreateMap<RolUser, RolUserOptionsDTO>().ReverseMap();

        }

    }
}
