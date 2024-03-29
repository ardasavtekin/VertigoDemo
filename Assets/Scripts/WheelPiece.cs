﻿using UnityEngine;

namespace UI.FortuneWheel
{
    [System.Serializable]
    public class WheelPiece
    {
        public Sprite icon;
        public GiftType giftType;
        [Tooltip("Reward amount")] public int Amount;

        [Tooltip("Probability in %")] [Range(0f, 100f)]
        public float Chance = 100f;

        [HideInInspector] public int Index;
        [HideInInspector] public double _weight = 0f;
    }
}

public enum GiftType
{
    Normal,
    Currency,
    Bomb
}