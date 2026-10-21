using System.Threading;
using Assets.Game.Modules.Content.Item.Scripts;
using Cysharp.Threading.Tasks;

namespace Assets.Game.Modules.Core.Infrastructure.Tutorial.Scripts
{
    // Тутор, который LevelController проигрывает перед игрой. На уровнях без тутора — EmptyLevelTutorial.
    public interface ILevelTutorial
    {
        UniTask PlayAsync(ItemChainConfig chain, CancellationToken ct = default);
    }

    public class EmptyLevelTutorial : ILevelTutorial
    {
        public UniTask PlayAsync(ItemChainConfig chain, CancellationToken ct = default) => UniTask.CompletedTask;
    }
}
