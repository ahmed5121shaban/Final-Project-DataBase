﻿using Azure.Identity;
using FinalApi;
using Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using ModelView;
using System.Security.Claims;

namespace FinalApi
{
    
    public class NotificationsHub:Hub
    {
        private readonly NotificationManager notificationManager;

        public NotificationsHub(NotificationManager _notificationManager)
        {
            notificationManager = _notificationManager;
        }

        public override async Task OnConnectedAsync()
        {
            string userID = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userID))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, userID);
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            string userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId != null)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
