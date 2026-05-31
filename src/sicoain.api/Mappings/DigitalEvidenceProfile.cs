using AutoMapper;
using sicoain.shared.DTOs.DigitalEvidences;
using sicoain.shared.Entities;

namespace sicoain.api.Mappings
{
    public class DigitalEvidenceProfile : Profile
    {
        public DigitalEvidenceProfile()
        {
            // Entity -> DTO
            CreateMap<DigitalEvidence, DigitalEvidenceDto>();

            // Create Request -> Entity
            // Note: FilePath, FileHash, and FileSize are set by the service
            CreateMap<CreateDigitalEvidenceRequest, DigitalEvidence>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.FilePath, opt => opt.Ignore())
                .ForMember(dest => dest.FileSize, opt => opt.Ignore())
                .ForMember(dest => dest.FileHash, opt => opt.Ignore())
                .ForMember(dest => dest.Accident, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false));

            // Update Request -> Entity
            CreateMap<UpdateDigitalEvidenceRequest, DigitalEvidence>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.FilePath, opt => opt.Ignore())
                .ForMember(dest => dest.FileSize, opt => opt.Ignore())
                .ForMember(dest => dest.MimeType, opt => opt.Ignore())
                .ForMember(dest => dest.FileHash, opt => opt.Ignore())
                .ForMember(dest => dest.AccidentId, opt => opt.Ignore())
                .ForMember(dest => dest.Accident, opt => opt.Ignore())
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
