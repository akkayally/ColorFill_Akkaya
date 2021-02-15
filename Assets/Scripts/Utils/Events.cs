using UnityEngine.Events;
using System;
public class Events
{
    [Serializable] public class EventGameState : UnityEvent<GameManager.GameState, GameManager.GameState> { }
}
