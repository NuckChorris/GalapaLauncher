using CommunityToolkit.Mvvm.ComponentModel;

namespace Galapa.Launcher.ViewModels.OnboardingFrame;

public abstract class OnboardingPageViewModel : ObservableValidator
{
    public abstract string Title { get; }
    public virtual bool CanContinue => true;
}