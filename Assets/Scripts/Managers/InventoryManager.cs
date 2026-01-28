using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class InventoryManager
{
    private static Tier? _lastBasketTier = null;
    /// <summary>
    /// The last accessory obtained from a basket purchase. 
    /// Null if no basket has been purchased in the current session.
    /// </summary>
    private static Accessory _lastAccessoryObtained = null;

    private static readonly Dictionary<Tier, int> TierCosts = new()
        {
            { Tier.Common, 50 },
            { Tier.Rare, 100 },
            { Tier.Epic, 250 },
            { Tier.Legendary, 500 }
        };
    public static bool BuyBasket(Tier basketTier) {

        bool bought = false;
        HashSet<Accessory> lockedAccessories = new();

        DataManager.WithStats(stats =>
        {
            if (stats.NumCarrots < TierCosts[basketTier])
            {
                Debug.Log("Not enough carrots to buy basket");
                return;
            }
            foreach (Accessory acc in Accessory.AllAccesories)
            {
                if (!stats.AccessoriesOwned.Contains(acc))
                {
                    lockedAccessories.Add(acc);
                }
            }
            if (lockedAccessories.Count == 0)
            {
                Debug.Log("No locked accessories left to obtain");
                return;
            }
            bool CommonAvailable = lockedAccessories.Any(a => a.Tier == Tier.Common);
            bool RareAvailable = lockedAccessories.Any(a => a.Tier == Tier.Rare);
            bool EpicAvailable = lockedAccessories.Any(a => a.Tier == Tier.Epic);
            bool LegendaryAvailable = lockedAccessories.Any(a => a.Tier == Tier.Legendary);
            System.Random random = new();
            double randomNum = random.NextDouble();
            Tier accessoryTier;
            double commonWeight;
            double rareWeight;
            double epicWeight;
            double legendaryWeight;
            switch (basketTier)
            {
                case Tier.Common:
                    commonWeight = CommonAvailable ? 0.9 : 0;
                    rareWeight = RareAvailable ? 0.095 : 0;
                    epicWeight = EpicAvailable ? 0.005 : 0;
                    //legendaryWeight = 0.00001;
                    legendaryWeight = 0;
                    break;
                case Tier.Rare:
                    commonWeight = CommonAvailable ? 0.00001 : 0;
                    rareWeight = RareAvailable ? 0.8 : 0;
                    epicWeight = EpicAvailable ? 0.2 : 0;
                    //legendaryWeight = LegendaryAvailable ? 0.0000001 : 0;
                    legendaryWeight = 0;
                    break;
                case Tier.Epic:
                    commonWeight = CommonAvailable ? 0.00001 : 0;
                    rareWeight = RareAvailable ? 0.4 : 0;
                    epicWeight = EpicAvailable ? 0.5 : 0;
                    //legendaryWeight = LegendaryAvailable ? 0.1 : 0;
                    legendaryWeight = 0;
                    break;
                case Tier.Legendary:
                    commonWeight = CommonAvailable ? 0.00001 : 0;
                    rareWeight = RareAvailable ? 0.000001 : 0;
                    epicWeight = EpicAvailable ? 0.3 : 0;
                    //legendaryWeight = LegendaryAvailable ? 0.7 : 0;
                    legendaryWeight = 0;
                    break;
                default:
                    throw new Exception("Invalid tier");
            }
            double totalWeight = commonWeight + rareWeight + epicWeight + legendaryWeight;
            if (totalWeight == 0)
            {
                // todo - behavior?
                Debug.Log("No accessories of any tier available to obtain");
                return;
            }
            commonWeight /= totalWeight;
            rareWeight /= totalWeight;
            epicWeight /= totalWeight;
            legendaryWeight /= totalWeight;
            if (randomNum <= commonWeight)
            {
                accessoryTier = Tier.Common;
            }
            else if (randomNum <= commonWeight + rareWeight)
            {
                accessoryTier = Tier.Rare;
            }
            else if (randomNum <= commonWeight + rareWeight + epicWeight)
            {
                accessoryTier = Tier.Epic;
            }
            else
            {
                accessoryTier = Tier.Legendary;
            }
            stats.IncreaseCarrots(-TierCosts[basketTier], GameManager.HandleBadgesEarned);
            List<Accessory> accessoriesOfTier = lockedAccessories.Where(a => a.Tier == accessoryTier).ToList();
            Accessory accessory = accessoriesOfTier[random.Next(accessoriesOfTier.Count)];
            _lastBasketTier = basketTier;
            _lastAccessoryObtained = accessory;
            stats.AddAccessory(accessory, GameManager.HandleBadgesEarned);
            bought = true;
        }, true);

        // Track basket purchase with PostHog
        if (bought)
        {
            PostHogManager.Instance.Capture("basket_purchased", new Dictionary<string, object>
            {
                { "basket_tier", basketTier.ToString() },
                { "basket_cost", TierCosts[basketTier] },
                { "accessory_tier", _lastAccessoryObtained.Tier.ToString() },
                { "accessory_name", _lastAccessoryObtained.Name },
                { "accessory_type", _lastAccessoryObtained.Type.ToString() },
                { "remaining_carrots", CarrotManager.GetNumCarrots() }
            });
        }

        return bought;
    }

    public static List<Accessory> GetOwnedAccessoriesOfType(AccessoryType type)
    {
        List<Accessory> result = new();
        DataManager.WithStats(stats =>
        {
            result = stats.AccessoriesOwned.Where(a => a.Type == type).ToList();
        }, false);
        return result;
    }

    public static void UseAccessory(Accessory accessory)
    {
        DataManager.WithStats(stats => stats.UseAccessory(accessory, GameManager.HandleBadgesEarned), true);
    }


    public static void StopUsingAccessory(AccessoryType type)
    {
        DataManager.WithStats(stats => stats.StopUsingAccessory(type), true);
    }
    public static int NumTotalAccessoriesOfType(AccessoryType type)
    {
        return Accessory.AllAccesories.Count(a => a.Type == type);
    }



    /// <summary>
    /// Gets the currently used accessory of the given type. 
    /// Returns null if no accessory of that type is currently being used.
    /// </summary>
    public static Accessory GetCurrentAccessoryOfType(AccessoryType type)
    {
        Accessory result = null;
        DataManager.WithStats(stats =>
        {
            result = type switch
            {
                AccessoryType.Hat => stats.CurrHat,
                AccessoryType.Neckwear => stats.CurrNeckwear,
                AccessoryType.Clothing => stats.CurrClothing,
                AccessoryType.Facewear => stats.CurrFacewear,
                AccessoryType.Pet => stats.CurrPet,
                _ => null
            };
        }, false);
        return result;
    }

    public static (Tier?, Accessory) GetLastBasketInfo() => (_lastBasketTier, _lastAccessoryObtained);

}