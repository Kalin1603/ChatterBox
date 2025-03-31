using ChatterBox.Enums;
using ChatterBox.ViewModels.Profile;

namespace ChatterBox.ViewModels.Home
{
    public class NotificationViewModel
    {
        public int Id { get; set; }

        public string Message { get; set; }

        public NotificationType Type { get; set; }

        public ProfileViewModel SenderProfile { get; set; }

        public int ChatId { get; set; }
    }
}
