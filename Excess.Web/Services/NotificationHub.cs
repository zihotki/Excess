using Excess.RuntimeProject;
using Microsoft.AspNet.SignalR;

namespace Excess.Web.Services
{
    public class NotificationHub : Hub
    {
    }

    public class HubNotifier : INotifier
    {
        private readonly string _connection;
        private readonly IHubContext _ctx;

        public HubNotifier(string connection)
        {
            _connection = connection;
            _ctx = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
        }


        public void notify(Notification notification)
        {
            _ctx.Clients.Client(_connection).notify(notification);
        }
    }
}