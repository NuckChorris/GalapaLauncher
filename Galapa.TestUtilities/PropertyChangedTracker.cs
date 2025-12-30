using System.ComponentModel;

namespace Galapa.TestUtilities;

/// <summary>
///     Helper class to track PropertyChanged events on INotifyPropertyChanged objects during tests.
/// </summary>
public class PropertyChangedTracker : IDisposable
{
    private readonly INotifyPropertyChanged _target;
    private readonly List<string> _changedProperties = new();

    public PropertyChangedTracker(INotifyPropertyChanged target)
    {
        this._target = target;
        this._target.PropertyChanged += this.OnPropertyChanged;
    }

    /// <summary>
    ///     Gets the list of property names that raised PropertyChanged events.
    /// </summary>
    public IReadOnlyList<string> ChangedProperties => this._changedProperties.AsReadOnly();

    /// <summary>
    ///     Checks if a specific property raised a PropertyChanged event.
    /// </summary>
    public bool WasPropertyChanged(string propertyName)
    {
        return this._changedProperties.Contains(propertyName);
    }

    /// <summary>
    ///     Gets the number of times a specific property raised a PropertyChanged event.
    /// </summary>
    public int GetPropertyChangedCount(string propertyName)
    {
        return this._changedProperties.Count(p => p == propertyName);
    }

    /// <summary>
    ///     Clears the tracked property changes.
    /// </summary>
    public void Clear()
    {
        this._changedProperties.Clear();
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (!string.IsNullOrEmpty(e.PropertyName)) this._changedProperties.Add(e.PropertyName);
    }

    public void Dispose()
    {
        this._target.PropertyChanged -= this.OnPropertyChanged;
    }
}