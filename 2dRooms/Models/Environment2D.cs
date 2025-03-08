namespace _2dRooms.Models
{
    public class Environment2D
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public float MaxHeight { get; set; }
        public float MaxWidth { get; set; }
        public Guid UserId { get; set; }
        
    }
}
