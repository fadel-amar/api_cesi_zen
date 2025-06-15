namespace CesiZen_API.DTO.Response
{

    public class ActivityResponseDTO
    {
        public required int Id { get; set; }
        public required string Title { get; set; }
        public required string ImagePresentation { get; set; }
        public required string Type { get; set; }
        public required Boolean Status { get; set; }
        public required string Category { get; set; }
    }
    public class FullActivityResponseDTO
    {
        public required int Id { get; set; }
        public required string Title { get; set; }
        public required string ImagePresentation { get; set; }
        public required string Description { get; set; }
        public required string Url { get; set; }
        public required Boolean Status { get; set; }
        public required string Type { get; set; }
        public required int DurationMin { get; set; }
        public required string Category { get; set; }
        public required int User { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required DateTime UpdatedAt { get; set; }
    }
    public class ListActivityResponseDTO
    {
        public IEnumerable<ActivityResponseDTO> Activities { get; set; }
    }

}
