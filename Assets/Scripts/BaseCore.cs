using System;
using UnityEngine;

/// <summary>
/// A singleton that controls the base of the player.
/// Used a singleton to give access to the enemies to track that.
/// </summary>
public class BaseCore : Building
{
    public int CurrentHealth { get; private set; }
}