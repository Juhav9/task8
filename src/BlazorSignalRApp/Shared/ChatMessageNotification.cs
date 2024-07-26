namespace BlazorSignalRApp.Shared;

public class ChatMessageNotification : ChatMessage
{
    public Guid Id { get; set; }
    public DateTime MessageTime { get; set; }
}
