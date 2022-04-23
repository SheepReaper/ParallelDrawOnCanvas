using System.Linq;

namespace BlitApp;

public partial class MainPage : ContentPage
{
    private readonly Timer frameRateTimer;
    private readonly Timer eventTimer;

	readonly RandomLinesDrawable drawable1 = new();

	public MainPage()
	{
		InitializeComponent();

		gView1.Drawable = drawable1;

        frameRateTimer = new(OnFrameRateTimerTick, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1d / 60d));
        eventTimer = new(OnEventTimerTick, null, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1d / 24d));
	}

	private void OnEventTimerTick(object sender)
	{
		drawable1.NextStep();
	}

    private void OnFrameRateTimerTick(object sender)
    {
		gView1.Invalidate();
	}
}

