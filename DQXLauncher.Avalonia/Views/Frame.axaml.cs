using System;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;

namespace DQXLauncher.Avalonia.Views;

/// <summary>
///     A simple navigation frame component similar to WinUI 3's Frame.
///     Displays pages with optional transition animations and supports data-binding for easy integration with tabs.
/// </summary>
public partial class Frame : UserControl
{
    /// <summary>
    ///     Bindable property for the currently displayed page.
    ///     Can be bound to a ViewModel property or set directly.
    /// </summary>
    public static readonly StyledProperty<object?> CurrentPageProperty =
        AvaloniaProperty.Register<Frame, object?>(nameof(CurrentPage));

    /// <summary>
    ///     Bindable property for the page transition animation.
    ///     Set this to define how pages transition in/out (e.g., PageSlide, CrossFade).
    /// </summary>
    public static readonly StyledProperty<IPageTransition?> PageTransitionProperty =
        AvaloniaProperty.Register<Frame, IPageTransition?>(nameof(PageTransition));

    private readonly IServiceProvider? _serviceProvider;

    public Frame()
    {
        InitializeComponent();
    }

    /// <summary>
    ///     Creates a Frame with dependency injection support.
    ///     Pages will be resolved from the service provider when using Navigate(Type).
    /// </summary>
    public Frame(IServiceProvider serviceProvider) : this()
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    ///     Gets or sets the currently displayed page.
    ///     This property is bindable and will trigger transition animations when changed.
    /// </summary>
    public object? CurrentPage
    {
        get => GetValue(CurrentPageProperty);
        set => SetValue(CurrentPageProperty, value);
    }

    /// <summary>
    ///     Gets or sets the page transition animation.
    ///     Example: new PageSlide(TimeSpan.FromSeconds(0.3), PageSlide.SlideAxis.Horizontal)
    /// </summary>
    public IPageTransition? PageTransition
    {
        get => GetValue(PageTransitionProperty);
        set => SetValue(PageTransitionProperty, value);
    }

    /// <summary>
    ///     Handles property changes and updates the UI accordingly.
    /// </summary>
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == CurrentPageProperty)
            UpdateContent(change.NewValue);
        else if (change.Property == PageTransitionProperty) UpdateTransition(change.NewValue as IPageTransition);
    }

    /// <summary>
    ///     Navigates to a page by its Type.
    ///     The page will be created using dependency injection (if available) or Activator.CreateInstance.
    /// </summary>
    /// <param name="pageType">The Type of the page to navigate to (must be a Control)</param>
    /// <param name="parameter">Optional parameter to pass as the page's DataContext</param>
    public virtual void Navigate(Type pageType, object? parameter = null)
    {
        if (pageType == null)
            throw new ArgumentNullException(nameof(pageType));

        if (!typeof(Control).IsAssignableFrom(pageType))
            throw new ArgumentException($"Type {pageType.Name} must derive from Control", nameof(pageType));

        var page = CreatePage(pageType);
        if (page != null)
        {
            // Set the parameter as DataContext if provided and DataContext isn't already set
            if (parameter != null && page.DataContext == null) page.DataContext = parameter;

            CurrentPage = page;
        }
    }

    /// <summary>
    ///     Navigates to an already-instantiated page.
    ///     Use this when you want to manually create and configure the page before navigating.
    /// </summary>
    /// <param name="page">The page instance to navigate to</param>
    public virtual void Navigate(object page)
    {
        if (page == null)
            throw new ArgumentNullException(nameof(page));

        CurrentPage = page;
    }

    /// <summary>
    ///     Creates a new instance of the specified page type.
    ///     This method first tries to resolve the page from the dependency injection container (if one was provided),
    ///     and falls back to using Activator.CreateInstance if DI is not available or doesn't have the type registered.
    /// </summary>
    /// <param name="pageType">The Type of page to create</param>
    /// <returns>A new instance of the page, or null if creation failed</returns>
    /// <example>
    ///     With DI: If HomePage is registered in the service provider, it will be resolved with all its dependencies injected.
    ///     Without DI: Creates a new instance using the parameterless constructor via
    ///     Activator.CreateInstance(typeof(HomePage)).
    /// </example>
    protected virtual Control? CreatePage(Type pageType)
    {
        try
        {
            // First, try to get the page from dependency injection if a service provider was provided
            if (_serviceProvider != null)
            {
                var page = _serviceProvider.GetService(pageType);
                if (page != null)
                    return (Control)page;
            }

            // If DI isn't available or didn't return the page, fall back to creating it directly
            // This requires the page type to have a parameterless constructor
            return (Control?)Activator.CreateInstance(pageType);
        }
        catch
        {
            // Return null if creation fails (e.g., no parameterless constructor, type is abstract, etc.)
            return null;
        }
    }

    /// <summary>
    ///     Updates the content displayed in the frame's TransitioningContentControl.
    /// </summary>
    private void UpdateContent(object? newContent)
    {
        if (ContentPresenter != null) ContentPresenter.Content = newContent;
    }

    /// <summary>
    ///     Updates the transition animation used when changing pages.
    /// </summary>
    private void UpdateTransition(IPageTransition? transition)
    {
        if (ContentPresenter != null) ContentPresenter.PageTransition = transition;
    }
}