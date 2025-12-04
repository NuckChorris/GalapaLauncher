using System;
using Avalonia.Input;
using Avalonia.Threading;
using Microsoft.Extensions.Logging;
using SDL3;

namespace DQXLauncher.Avalonia.Services;

public class GamepadInputService : IDisposable
{
    private readonly ILogger<GamepadInputService> _logger;
    private readonly DispatcherTimer _pollTimer;
    private nint _gamepad = nint.Zero;
    private const float ThumbstickDeadzone = 0.3f;
    private const int PollIntervalMs = 16; // ~60Hz
    private DateTime _lastDirectionTime = DateTime.MinValue;
    private const int DirectionRepeatDelayMs = 150;
    private bool _isInitialized;

    public event EventHandler<Key>? NavigationKeyPressed;

    public GamepadInputService(ILogger<GamepadInputService> logger)
    {
        _logger = logger;
        _pollTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(PollIntervalMs)
        };
        _pollTimer.Tick += OnPollTick;
    }

    public void Start()
    {
        _logger.LogInformation("Starting gamepad input service");

        if (!_isInitialized)
        {
            if (!SDL.Init(SDL.InitFlags.Gamepad))
            {
                _logger.LogError("Failed to initialize SDL gamepad subsystem: {Error}", SDL.GetError());
                return;
            }

            _isInitialized = true;
        }

        FindGamepad();
        _pollTimer.Start();
    }

    public void Stop()
    {
        _logger.LogInformation("Stopping gamepad input service");
        _pollTimer.Stop();
    }

    private void FindGamepad()
    {
        if (_gamepad != nint.Zero)
            return;

        var gamepadIds = SDL.GetGamepads(out var count);

        if (count > 0 && gamepadIds != null)
        {
            var gamepadId = gamepadIds[0];
            _gamepad = SDL.OpenGamepad(gamepadId);

            if (_gamepad != nint.Zero)
            {
                var name = SDL.GetGamepadNameForID(gamepadId);
                _logger.LogInformation("Found and opened gamepad: {Name} (ID: {Id})", name, gamepadId);
            }
            else
            {
                _logger.LogWarning("Failed to open gamepad {Id}: {Error}", gamepadId, SDL.GetError());
            }
        }
    }

    private void OnPollTick(object? sender, EventArgs e)
    {
        if (_gamepad == nint.Zero)
        {
            FindGamepad();
            return;
        }

        // Check if the gamepad is still connected
        if (!SDL.GamepadConnected(_gamepad))
        {
            _logger.LogWarning("Lost connection to gamepad");
            SDL.CloseGamepad(_gamepad);
            _gamepad = nint.Zero;
            return;
        }

        ProcessInput();
    }

    private void ProcessInput()
    {
        var now = DateTime.UtcNow;
        if ((now - _lastDirectionTime).TotalMilliseconds < DirectionRepeatDelayMs) return;

        // Read current button states
        var dpadUp = SDL.GetGamepadButton(_gamepad, SDL.GamepadButton.DPadUp);
        var dpadDown = SDL.GetGamepadButton(_gamepad, SDL.GamepadButton.DPadDown);
        var dpadLeft = SDL.GetGamepadButton(_gamepad, SDL.GamepadButton.DPadLeft);
        var dpadRight = SDL.GetGamepadButton(_gamepad, SDL.GamepadButton.DPadRight);
        var buttonA = SDL.GetGamepadButton(_gamepad, SDL.GamepadButton.South);
        var buttonB = SDL.GetGamepadButton(_gamepad, SDL.GamepadButton.East);

        // Read analog stick values
        var leftStickX = SDL.GetGamepadAxis(_gamepad, SDL.GamepadAxis.LeftX);
        var leftStickY = SDL.GetGamepadAxis(_gamepad, SDL.GamepadAxis.LeftY);

        // Normalize thumbstick values to match XInput range (-1.0 to 1.0)
        var normalizedX = leftStickX / 32767.0f;
        var normalizedY = leftStickY / 32767.0f;

        // Check D-pad Up or left-stick up (negative Y in SDL)
        if (dpadUp || normalizedY < -ThumbstickDeadzone)
        {
            _lastDirectionTime = now;
            NavigationKeyPressed?.Invoke(this, Key.Up);
            return;
        }

        // Check D-pad Down or left stick down (positive Y in SDL)
        if (dpadDown || normalizedY > ThumbstickDeadzone)
        {
            _lastDirectionTime = now;
            NavigationKeyPressed?.Invoke(this, Key.Down);
            return;
        }

        // Check D-pad Left or left-stick left (negative X)
        if (dpadLeft || normalizedX < -ThumbstickDeadzone)
        {
            _lastDirectionTime = now;
            NavigationKeyPressed?.Invoke(this, Key.Left);
            return;
        }

        // Check D-pad Right or left stick right (positive X)
        if (dpadRight || normalizedX > ThumbstickDeadzone)
        {
            _lastDirectionTime = now;
            NavigationKeyPressed?.Invoke(this, Key.Right);
            return;
        }

        // Check A button (South button in SDL - bottom button on Xbox controller)
        if (buttonA)
        {
            _lastDirectionTime = now;
            NavigationKeyPressed?.Invoke(this, Key.Enter);
            return;
        }

        // Check B button (East button in SDL - right button on Xbox controller)
        if (buttonB)
        {
            _lastDirectionTime = now;
            NavigationKeyPressed?.Invoke(this, Key.Escape);
        }
    }

    public void Dispose()
    {
        Stop();
        _pollTimer.Tick -= OnPollTick;

        if (_gamepad != nint.Zero)
        {
            SDL.CloseGamepad(_gamepad);
            _gamepad = nint.Zero;
        }

        if (_isInitialized)
        {
            SDL.Quit();
            _isInitialized = false;
        }
    }
}