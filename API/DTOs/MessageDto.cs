using System.Text.Json.Serialization;

namespace API.DTOs;

public class MessageDto
{
    public int Id { get; set; }

    public string SenderUsername { get; set; }
    public int SenderId { get; set; }
    public string SenderPhotoUrl { get; set; }

    public string RecipientUsername { get; set; }
    public int RecipientId { get; set; }
    public string RecipientPhotoUrl { get; set; }

    public string Content { get; set; }
    public DateTime SendAt { get; set; }
    public DateTime? ReadAt { get; set; }

    [JsonIgnore] // So I could user them here but when I send them to the client Json will ignore them
    public bool SenderDeleted { get; set; }
    [JsonIgnore]
    public bool RecipientDeleted { get; set; }
}