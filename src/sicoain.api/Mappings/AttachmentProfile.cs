using AutoMapper;
using sicoain.shared.DTOs.Attachments;
using sicoain.shared.Entities;

namespace sicoain.api.Mappings
{
    public class AttachmentProfile : Profile
    {
        public AttachmentProfile()
        {
            // Entity -> DTO
            CreateMap<Attachment, AttachmentDto>();

            // Create Request -> Entity
            // Note: FilePath, FileHash, and FileSize are set by the service
            CreateMap<CreateAttachmentRequest, Attachment>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.FilePath, opt => opt.Ignore())
                .ForMember(dest => dest.FileSize, opt => opt.Ignore())
                .ForMember(dest => dest.FileHash, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false));

            // Update Request -> Entity
            CreateMap<UpdateAttachmentRequest, Attachment>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.FileName, opt => opt.Ignore())
                .ForMember(dest => dest.FilePath, opt => opt.Ignore())
                .ForMember(dest => dest.FileSize, opt => opt.Ignore())
                .ForMember(dest => dest.MimeType, opt => opt.Ignore())
                .ForMember(dest => dest.FileHash, opt => opt.Ignore())
                .ForMember(dest => dest.EntityType, opt => opt.Ignore())
                .ForMember(dest => dest.EntityId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
