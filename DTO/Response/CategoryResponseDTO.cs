 
namespace CesiZen_API.DTO.Response  
{  
    public class CategoryResponseDTO  
    {  
        public int Id { get; set; }  
        public string Name { get; set; }  
        public string Emoji { get; set; }  
        public string Duration { get; set; }  
        public bool Status { get; set; }  
    }  

    public class FullCategoryResponseDTO  
    {  
        public int Id { get; set; }  
        public string Name { get; set; }  
        public string Emoji { get; set; }  
        public string Duration { get; set; }  
        public bool Status { get; set; }  
        public string Login { get; set; }  
    }  
}  
