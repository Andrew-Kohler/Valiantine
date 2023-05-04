using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Gem System/Gem Stat Block")]
public class GemStatBlock : ScriptableObject
{
    public float ATKMod;    // The passive modifiers that affect the player's stats when the gem is equipped
    public float DEFMod;
    public float HPMod;
    public float MPMod;
    public float SPDMod;
    public float XPMod;

    public int targetType;  // 1 = Single target, 2 = Multi-target, 3 = Self-target/non-offensive
    public string gemName;

    public float ActiveATKMod;    // The active modifier to attack when the spell is cast

}
