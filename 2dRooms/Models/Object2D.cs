namespace _2dRooms.Models
{
    public class Object2D
    {
        public Guid Id { get; set; }
        public int PrefabId { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float ScaleX { get; set; }
        public float ScaleY { get; set; }
        public float RotationY { get; set; }
        public int SortingLayer { get; set; }
        public Guid EnvironmentId { get; set; }
    }
}
