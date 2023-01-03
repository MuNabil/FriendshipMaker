namespace API.Entities;

// This class to Tracking the message groups
public class Group
{
    public Group() // Empty ctor because required by EF (when I create another nonempty one)
    {
    }

    public Group(string name) // To easly scan its properities
    {
        Name = name;
    }

    [Key]
    public string Name { get; set; }
    public ICollection<Connection> Connections { get; set; } = new List<Connection>();

}