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
            CreateMap<Form, FormDTO>().ReverseMap();

            CreateMap<Module, ModuleDTO>().ReverseMap();

            CreateMap<Permission,PermissionDTO>().ReverseMap();

            CreateMap<Person, PersonDTO>().ReverseMap();

            CreateMap<Rol, RolDTO>().ReverseMap();

            CreateMap<RolFormPermission, RolFormPermissionDTO>().ReverseMap();
            CreateMap<RolFormPermission, RolFormPermissionOptionsDTO>().ReverseMap();

            CreateMap<FormModule, FormModuleDTO>().ReverseMap();
            CreateMap<FormModule, FormModuleOptionsDTO>().ReverseMap();

            CreateMap<RolUser, RolUserDTO>().ReverseMap();
            CreateMap<RolUser, RolUserOptionsDTO>().ReverseMap();

            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<User, UserCreateDTO>().ReverseMap();
        }
    }
}
