using System;

namespace Assets.Game.Modules.Content.Level.Scripts.Conditions
{
    [Serializable]
    public abstract class LevelCondition : IEquatable<LevelCondition>
    {
        public abstract bool IsCompleted(PlayerUpgradesInLevel progress);

        public bool Equals(LevelCondition other) => other != null && other.GetType() == GetType();

        public override bool Equals(object obj) => Equals(obj as LevelCondition);

        public override int GetHashCode() => GetType().GetHashCode();
    }
}
