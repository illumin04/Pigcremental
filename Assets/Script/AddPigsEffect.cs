using System;
using UnityEngine;

[CreateAssetMenu(fileName = "AddPigs", menuName = "Pigcremental/Effects/Add Pigs")]
public sealed class AddPigsEffect : EventEffect
{
    [Min(1)] public int amount = 1;
    public override string Execute(GameManager game)
    {
        if (amount < 1 || amount > int.MaxValue - game.pigs)
            throw new InvalidOperationException("Invalid pig reward.");
        for (int i = 0; i < amount; i++) game.FeedPig();
        return "+" + amount + " Pigs";
    }
}
