using System;

public static class DimensionManager
{
    // Przyk³adowe wykorzystanie z inngo skryptu:
    // np. if (DimensionManager.CurrentState == PlayerColorState.Red)
    public static PlayerColorState CurrentState { get; private set; } = PlayerColorState.White;

    public static event Action<PlayerColorState> OnDimensionChanged;

    public static void ChangeDimension(PlayerColorState newState)
    {
        if (CurrentState == newState) return;

        CurrentState = newState;
        OnDimensionChanged?.Invoke(newState);
    }
}