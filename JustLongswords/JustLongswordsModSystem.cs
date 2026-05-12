using JustLongswords.Items;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.GameContent;

namespace JustLongswords;

public class JustLongswordsModSystem : ModSystem
{
    
    private Harmony _harmony;

    // Called on server and client
    // Useful for registering block/entity classes on both sides
    public override void Start(ICoreAPI api)
    {
        _harmony = new Harmony(Mod.Info.ModID);
        _harmony.PatchAll();
        api.RegisterItemClass(Mod.Info.ModID + ".longsword", typeof(ItemLongsword));
    }


    // this is so the modifiers dont apply twice (i wish i knew a better way of doing this)
    // current impl is to sum halves, which is an average for this case of 2 elements
    [HarmonyPatch(typeof(CollectibleBehaviorBuffable), nameof(CollectibleBehaviorBuffable.applyQuenchableBuffs))]
    public class ModifyApplyQuenchableBuffs
    {
        
        public static bool Prefix(ItemStack stack, ItemStack takeBuffsFromStack)
        {
            if (takeBuffsFromStack.Attributes.HasAttribute("tmp-powervalue") ||
                takeBuffsFromStack.Attributes.HasAttribute("tmp-durationbonus") ||
                (!takeBuffsFromStack.Attributes.HasAttribute("powervalue") &&
                 !takeBuffsFromStack.Attributes.HasAttribute("durationbonus")) ||
                !(stack.Item.Class == "justlongswords.longsword")) // lmao
            {
                return true;
            }
            
            float power = takeBuffsFromStack.Attributes.GetFloat("powervalue");
            
            takeBuffsFromStack.Attributes.SetFloat("tmp-powervalue", power);
            takeBuffsFromStack.Attributes.SetFloat("powervalue", power * 0.5f);
            
            float duration = takeBuffsFromStack.Attributes.GetFloat("durationbonus");
            
            takeBuffsFromStack.Attributes.SetFloat("tmp-durationbonus", duration);
            takeBuffsFromStack.Attributes.SetFloat("durationbonus", duration * 0.5f);

            return true;
        }
        
        public static void Postfix(ItemStack stack, ItemStack takeBuffsFromStack)
        {
            if (!takeBuffsFromStack.Attributes.HasAttribute("tmp-powervalue") ||
                !takeBuffsFromStack.Attributes.HasAttribute("tmp-durationbonus") ||
                (!takeBuffsFromStack.Attributes.HasAttribute("powervalue") &&
                 !takeBuffsFromStack.Attributes.HasAttribute("durationbonus")) ||
                !(stack.Item.Class == "justlongswords.longsword"))
            {
                return;
            }

            // undo it i guess idk if it works like that
            takeBuffsFromStack.Attributes.SetFloat("powervalue", takeBuffsFromStack.Attributes.GetFloat("tmp-powervalue"));
            takeBuffsFromStack.Attributes.SetFloat("durationbonus", takeBuffsFromStack.Attributes.GetFloat("tmp-durationbonus"));
            
            takeBuffsFromStack.Attributes.RemoveAttribute("tmp-powervalue");
            takeBuffsFromStack.Attributes.RemoveAttribute("tmp-durationbonus");

        }
        
    }
    
}


