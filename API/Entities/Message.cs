namespace API.Entities;

// (join/intermediary) table between two users that messageing each other
public class Message
{
    public int Id { get; set; }

    // To configure the sender relationship
    public string SenderUsername { get; set; }
    public int SenderId { get; set; }
    public ApplicationUser Sender { get; set; }

    // To configure the recipient relationship
    public string RecipientUsername { get; set; }
    public int RecipientId { get; set; }
    public ApplicationUser Recipient { get; set; }

    // Actual message properties
    public string Content { get; set; }
    public DateTime SendAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReadAt { get; set; }  // nullable to be null when the recipient doesn't read it.

    // When sender delete the message will be deleted in his view only.
    public bool SenderDeleted { get; set; }
    // When recipient delete the message will be deleted in his view only.
    public bool RecipientDeleted { get; set; }
}