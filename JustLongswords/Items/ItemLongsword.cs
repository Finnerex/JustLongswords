using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;

namespace JustLongswords.Items;

public class ItemLongsword : Item
{
    private string[] _attackAnimations;
    
    private int _currentAttackIndex;
    private int CurrentAttackIndex
    {
        get => _currentAttackIndex;
        set
        {
            _currentAttackIndex = value;
            if (value >= _attackAnimations.Length)
                _currentAttackIndex = 0;
        }
    }
    private string CurrentAnimation => _attackAnimations[CurrentAttackIndex];


    public override void OnLoaded(ICoreAPI api)
    {
        base.OnLoaded(api);
        _attackAnimations = Attributes["attackAnimations"].AsArray<string>();
        foreach (var anim in _attackAnimations)
        {
            api.Logger.Event("Has animation: " + anim);
        }
    }

    public override string GetHeldTpHitAnimation(ItemSlot slot, Entity byEntity) => CurrentAnimation;
    
    public override void OnAttackingWith(IWorldAccessor world, Entity byEntity, Entity attackedEntity, ItemSlot itemslot)
    {
        base.OnAttackingWith(world, byEntity, attackedEntity, itemslot);
        api.Logger.Event($"Has attacked with {CurrentAnimation} ({CurrentAttackIndex})");
        
        CurrentAttackIndex++;
    
    }

}