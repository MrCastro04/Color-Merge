using System.Threading;
using Assets.Game.Modules.Content.Item.Scripts;
using Cysharp.Threading.Tasks;

namespace Assets.Game.Modules.Core.Infrastructure.Tutorial.Scripts
{
    public interface ILevelTutorial
    {
        UniTask PlayAsync(ItemChainConfig chain, CancellationToken ct = default);
    }

    public class EmptyLevelTutorial : ILevelTutorial
    {
        public UniTask PlayAsync(ItemChainConfig chain, CancellationToken ct = default) => UniTask.CompletedTask;
    }
}
