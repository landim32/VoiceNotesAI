using DevNote.ViewModels;

namespace DevNote.Pages;

public partial class RecordingPage : ContentPage
{
    private readonly RecordingViewModel _viewModel;
    private CancellationTokenSource? _animationCts;

    public RecordingPage(RecordingViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.StartRecordingCommand.ExecuteAsync(null);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        StopPulseAnimation();
    }

    private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(RecordingViewModel.IsRecording))
        {
            if (_viewModel.IsRecording)
                StartPulseAnimation();
            else
                StopPulseAnimation();
        }
    }

    private void StartPulseAnimation()
    {
        _animationCts?.Cancel();
        _animationCts = new CancellationTokenSource();
        var token = _animationCts.Token;

        // Reset rings
        PulseRing1.Scale = 0.6;
        PulseRing1.Opacity = 0;
        PulseRing2.Scale = 0.6;
        PulseRing2.Opacity = 0;

        _ = RunPulseLoopAsync(token);
    }

    private async Task RunPulseLoopAsync(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                // Mic frame gentle pulse
                var micPulse = Task.WhenAll(
                    MicFrame.ScaleTo(1.08, 600, Easing.SinInOut),
                    MicFrame.FadeTo(0.85, 600, Easing.SinInOut)
                );

                // Ring 1: expand and fade out
                PulseRing1.Scale = 0.7;
                PulseRing1.Opacity = 0.4;
                var ring1 = Task.WhenAll(
                    PulseRing1.ScaleTo(1.2, 1200, Easing.CubicOut),
                    PulseRing1.FadeTo(0, 1200, Easing.CubicIn)
                );

                await Task.Delay(400, token);

                // Ring 2: expand and fade out (staggered)
                PulseRing2.Scale = 0.7;
                PulseRing2.Opacity = 0.3;
                var ring2 = Task.WhenAll(
                    PulseRing2.ScaleTo(1.15, 1200, Easing.CubicOut),
                    PulseRing2.FadeTo(0, 1200, Easing.CubicIn)
                );

                await micPulse;

                // Mic frame back to normal
                await Task.WhenAll(
                    MicFrame.ScaleTo(1.0, 600, Easing.SinInOut),
                    MicFrame.FadeTo(1.0, 600, Easing.SinInOut)
                );

                await Task.WhenAll(ring1, ring2);

                if (token.IsCancellationRequested) break;
                await Task.Delay(100, token);
            }
        }
        catch (TaskCanceledException)
        {
            // Animation cancelled, expected
        }
    }

    private void StopPulseAnimation()
    {
        _animationCts?.Cancel();
        _animationCts = null;

        // Reset visual state
        MicFrame.CancelAnimations();
        PulseRing1.CancelAnimations();
        PulseRing2.CancelAnimations();

        MicFrame.Scale = 1.0;
        MicFrame.Opacity = 1.0;
        PulseRing1.Opacity = 0;
        PulseRing2.Opacity = 0;
    }
}
