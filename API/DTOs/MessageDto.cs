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
}