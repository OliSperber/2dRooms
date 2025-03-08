namespace _2dRooms.Models
{
    public class Object2D
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public int PrefabId { get; set; }

        public float PositionX { get; set; }
        public float PositionY { get; set; }

        public float ScaleX { get; set; } = 1.0f;
        public float ScaleY { get; set; } = 1.0f;

        public float RotationZ { get; set; }

        public int SortingLayer { get; set; }

        public string EnvironmentId { get; set; }
    }
}
