using Domain;

namespace API.DTOs
{
    public class TagFilterParams
    {
        public List<TagStatus> TunnelStatus { get ; set; }
        public List<TagStatus> OfficeStatus { get; set; }
    }
}