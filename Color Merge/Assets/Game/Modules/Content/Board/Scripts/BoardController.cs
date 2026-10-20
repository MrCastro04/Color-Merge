using System;
using System.Collections.Generic;
using Assets.Game.Modules.Content.Item.Scripts;
using Assets.Game.Modules.Content.Item.Scripts.Interfaces;
using Assets.Game.Modules.Content.Level.Scripts;
using Assets.Game.Modules.Content.Slot.Scripts;
using Assets.Game.Modules.Content.Slot.Scripts.Interfaces;
using Assets.Game.Modules.Core.Utils.Scripts.Extensions;
using Cysharp.Threading.Tasks;

namespace Assets.Game.Modules.Content.Board.Scripts
{
    // Модель доски и решение по дропу: move / swap / merge.
    // Кто где лежит, знает только _items — слоты в процессе анимации не опрашиваются.
    public class BoardController : ISlotDropHandler, IDisposable
    {
        private readonly BoardView _boardView;
        private readonly IItemFactory _itemFactory;
        private readonly ISlotFactory _slotFactory;

        private readonly List<SlotController> _slots = new();
        private readonly HashSet<int> _busySlots = new();
        private ItemController[] _items = Array.Empty<ItemController>();
        private ItemChainConfig _itemChain;
        // Собрали этот предмет — цель уровня выполнена (LevelConfig.GoalItem).
        private ItemConfig _goalItem;
        private bool _isInteractable = true;

        public event Action<ItemController> OnItemSpawned;
        public event Action<ItemController> OnItemPicked;
        public event Action<ItemController> OnItemMoved;
        public event Action<ItemController> OnItemReturned;
        public event Action<ItemController, ItemController> OnItemsSwapped;
        public event Action<ItemController> OnItemMerged;
        public event Action<ItemController> OnGoalItemCreated;

        public IReadOnlyList<SlotController> Slots => _slots;
        public bool HasFreeSlot => GetFreeSlotIds().Count > 0;

        public BoardController(BoardView boardView, IItemFactory itemFactory, ISlotFactory slotFactory)
        {
            _boardView = boardView;
            _itemFactory = itemFactory;
            _slotFactory = slotFactory;
        }

        public void Dispose()
        {
            Clear();
        }

        // Фабрики должны быть прогреты (WarmUpAsync) до вызова.
        public void Build(LevelConfig levelConfig)
        {
            Clear();

            _itemChain = levelConfig.ItemChain;
            _goalItem = levelConfig.GoalItem;
            _items = new ItemController[levelConfig.SlotsCount];

            _boardView.Init(levelConfig.Columns, levelConfig.Rows);

            for (int id = 0; id < levelConfig.SlotsCount; id++)
            {
                var slot = _slotFactory.Create(id, _boardView.SlotsContainer, this);
                _slots.Add(slot);
            }

            foreach (var blockedId in levelConfig.BlockedSlots)
            {
                if (IsValidId(blockedId))
                    _slots[blockedId].View.SetBlocked(true);
            }

            _boardView.RebuildLayout();

            foreach (var initialItem in levelConfig.InitialItems)
            {
                if (!IsValidId(initialItem.SlotID) || _slots[initialItem.SlotID].IsBlocked)
                    continue;

                CreateAt(initialItem.SlotID, _itemChain.GetByLevel(initialItem.ItemStartLevel));
            }

            _isInteractable = true;
        }

        public bool TrySpawnAtRandomFreeSlot(ItemConfig config)
        {
            var freeIds = GetFreeSlotIds();

            if (freeIds.Count == 0)
                return false;

            int slotId = freeIds[UnityEngine.Random.Range(0, freeIds.Count)];
            var item = CreateAt(slotId, config);

            item.View.PlaySpawnAsync().Forget();
            OnItemSpawned?.Invoke(item);

            return true;
        }

        public void SetInteractable(bool isInteractable)
        {
            _isInteractable = isInteractable;

            foreach (var item in _items)
            {
                if (item.IsNotNull())
                    item.View.SetInteractable(isInteractable);
            }
        }

        // Есть ли на доске пара, которую можно смерджить.
        public bool HasAvailableMerge()
        {
            var seen = new HashSet<ItemConfig>();

            foreach (var item in _items)
            {
                if (item.IsNull() || !_itemChain.TryGetNext(item.Config, out _))
                    continue;

                if (!seen.Add(item.Config))
                    return true;
            }

            return false;
        }

        public IReadOnlyList<ItemController> GetItems()
        {
            var result = new List<ItemController>();

            foreach (var item in _items)
            {
                if (item.IsNotNull())
                    result.Add(item);
            }

            return result;
        }

        public void HandleDrop(SlotController targetSlot, ItemView itemView)
        {
            if (!_isInteractable || itemView.IsNull() || itemView.Controller.IsNull())
                return;

            int sourceId = IndexOf(itemView.Controller);
            int targetId = targetSlot.ID;

            // Не подтверждаем дроп — ItemView сам вернётся в прежний слот.
            if (sourceId < 0 || targetSlot.IsBlocked || _busySlots.Contains(sourceId) || _busySlots.Contains(targetId))
                return;

            if (sourceId == targetId)
            {
                MoveAsync(sourceId, targetId).Forget();
                return;
            }

            var dragged = _items[sourceId];
            var target = _items[targetId];

            if (target.IsNull())
            {
                MoveAsync(sourceId, targetId).Forget();
                return;
            }

            if (dragged.IsSameAs(target) && _itemChain.TryGetNext(dragged.Config, out var next))
            {
                MergeAsync(sourceId, targetId, next).Forget();
                return;
            }

            SwapAsync(sourceId, targetId).Forget();
        }

        // Все *Async ниже до первого await синхронно вызывают SlotController.Attach → ItemView.ConfirmDrop.
        private async UniTaskVoid MoveAsync(int sourceId, int targetId)
        {
            var dragged = _items[sourceId];

            _items[sourceId] = null;
            _items[targetId] = dragged;

            Lock(targetId);
            OnItemMoved?.Invoke(dragged);
            await _slots[targetId].Attach(dragged.View);
            Unlock(targetId);
        }

        private async UniTaskVoid SwapAsync(int sourceId, int targetId)
        {
            var dragged = _items[sourceId];
            var target = _items[targetId];

            _items[sourceId] = target;
            _items[targetId] = dragged;

            Lock(sourceId);
            Lock(targetId);
            target.View.CanDrag = false;

            var attachTasks = UniTask.WhenAll(
                _slots[targetId].Attach(dragged.View),
                _slots[sourceId].Attach(target.View));

            OnItemsSwapped?.Invoke(dragged, target);

            await attachTasks;

            target.View.CanDrag = _isInteractable;
            Unlock(sourceId);
            Unlock(targetId);
        }

        private async UniTaskVoid MergeAsync(int sourceId, int targetId, ItemConfig nextConfig)
        {
            var dragged = _items[sourceId];
            var target = _items[targetId];

            _items[sourceId] = null;
            _items[targetId] = null;

            Lock(targetId);
            dragged.View.CanDrag = false;
            target.View.CanDrag = false;

            await _slots[targetId].Attach(dragged.View);

            DestroyItem(dragged);
            DestroyItem(target);

            var merged = CreateAt(targetId, nextConfig);
            merged.View.CanDrag = false;

            OnItemMerged?.Invoke(merged);

            bool isGoal = nextConfig == _goalItem;

            if (isGoal)
                OnGoalItemCreated?.Invoke(merged);

            await merged.View.PlayMergeAsync();

            if (merged.View.IsNotNull())
                merged.View.CanDrag = _isInteractable;

            Unlock(targetId);
        }

        private ItemController CreateAt(int slotId, ItemConfig config)
        {
            var item = _itemFactory.Create(config, _boardView.SlotsContainer);

            _slots[slotId].Place(item.View);
            _items[slotId] = item;

            item.View.SetInteractable(_isInteractable);
            item.View.OnDragStarted += HandleOnItemDragStarted;
            item.View.OnReturned += HandleOnItemReturned;

            return item;
        }

        private void DestroyItem(ItemController item)
        {
            if (item.IsNull())
                return;

            if (item.View.IsNotNull())
            {
                item.View.OnDragStarted -= HandleOnItemDragStarted;
                item.View.OnReturned -= HandleOnItemReturned;
            }

            _itemFactory.Destroy(item);
        }

        private void HandleOnItemDragStarted(ItemView itemView)
        {
            OnItemPicked?.Invoke(itemView.Controller);
        }

        private void HandleOnItemReturned(ItemView itemView)
        {
            OnItemReturned?.Invoke(itemView.Controller);
        }

        private List<int> GetFreeSlotIds()
        {
            var result = new List<int>();

            for (int id = 0; id < _items.Length; id++)
            {
                if (_items[id].IsNull() && !_slots[id].IsBlocked && !_busySlots.Contains(id))
                    result.Add(id);
            }

            return result;
        }

        private int IndexOf(ItemController item)
        {
            return Array.IndexOf(_items, item);
        }

        private bool IsValidId(int id) => id >= 0 && id < _slots.Count;

        private void Lock(int id) => _busySlots.Add(id);
        private void Unlock(int id) => _busySlots.Remove(id);

        private void Clear()
        {
            foreach (var item in _items)
                DestroyItem(item);

            foreach (var slot in _slots)
                _slotFactory.Destroy(slot);

            _items = Array.Empty<ItemController>();
            _slots.Clear();
            _busySlots.Clear();
        }
    }
}
