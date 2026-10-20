using Godot;

namespace ROMA2.Client.Logic.Views;

public partial class StartView : Node2D
{
    [Export] private Core.Client _client;
    [Export] private Button _button;

    public void Init()
    {
        _button.Pressed += _client.OnConnecting;
        _client.SuccessfulConnection += QueueFree;
    }

    public override void _Notification(int what)
    {
        if (what == NotificationPredelete)
        {
            _button.Pressed -= _client.OnConnecting;
            _client.SuccessfulConnection -= QueueFree;
        }
    }
}
