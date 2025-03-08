namespace _2dRooms.Models;

public class Environment2D
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;

    public float MaxHeight { get; set; }
    public float MaxWidth { get; set; }

    public string UserId { get; set; }
}

