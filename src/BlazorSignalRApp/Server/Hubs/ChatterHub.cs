using Microsoft.AspNetCore.SignalR;
using BlazorSignalRApp.Server.Data;
using BlazorSignalRApp.Shared;

namespace BlazorSignalRApp.Server.Hubs
{
	public class ChatterHub : Hub
	{	
		private readonly AppDataContext _db;
		public ChatterHub(AppDataContext db) { 
			_db = db;
		}

		public async Task SubmitChatMessage(DateTime dateTime, ChatMessage chatMessage)
		{
			ChatMessageNotification notification = new ChatMessageNotification();
			notification.MessageTime = dateTime;
			notification.Message = chatMessage.Message;
			notification.User = chatMessage.User;

			_db.Messages.Add(notification);
			await _db.SaveChangesAsync();

			await Clients.All.SendAsync("ChatMessageArrivedNotification",notification);
		}
	}
}
