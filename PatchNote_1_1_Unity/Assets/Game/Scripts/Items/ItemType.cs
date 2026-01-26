using System;
using UnityEngine;

public enum ItemType
{
    None = 0,
    
    [InspectorName("Vegetable/Tomato")]
    Tomato = 1,
    [InspectorName("Vegetable/Leek")]
    Leek = 2,
    [InspectorName("Vegetable/Swede")]
    Swede = 3,
    [InspectorName("Vegetable/Potato")]
    Potato = 4,
    [InspectorName("Vegetable/Carrot")]
    Carrot = 5,
    [InspectorName("Vegetable/Broccoli")]
    Broccoli = 6,
    
    [InspectorName("Animal/Beef")]
    Beef = 101,
    [InspectorName("Animal/Egg")]
    Egg = 102,
    [InspectorName("Animal/Cheese")]
    Cheese = 103,
    [InspectorName("Animal/Milk")]
    Milk = 104,
    [InspectorName("Animal/Sausage")]
    Sausage = 105,
    [InspectorName("Animal/Fish")]
    Fish = 106,
    
    [InspectorName("Processed/Can")]
    Can = 201,
    [InspectorName("Processed/Oil")]
    Oil = 202,
    [InspectorName("Processed/Pasta")]
    Pasta = 203,
    [InspectorName("Processed/Bottle")]
    Bottle = 204,
    [InspectorName("Processed/Cereal")]
    Cereal = 205,
    [InspectorName("Processed/Bread")]
    Bread = 206,
    
    [InspectorName("Other/Gold")]
    Gold = 301,
    [InspectorName("Other/Crisps")]
    Crisps = 302,
    [InspectorName("Other/Jam")]
    Jam = 303,
    [InspectorName("Other/Ham")]
    Ham = 304,
    [InspectorName("Other/Melon")]
    Melon = 305,
    [InspectorName("Other/Gem")]
    Gem = 306
}

public enum ItemGroup
{
    None = 0,
    Normal = 1,
    Cold = 2,
    Fresh = 3,
    Rare = 4,
    NonRare = 5,
}

public static class ItemGroupDefinition
{
    public static ItemType[] GetByGroup(ItemGroup group)
    {
        return group switch
        {
            ItemGroup.Normal => NormalItems,
            ItemGroup.Cold => ColdItems,
            ItemGroup.Fresh => FreshItems,
            ItemGroup.Rare => RareItems,
            ItemGroup.NonRare => NonRareItems,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
    private static readonly ItemType[] NormalItems =
    {
        ItemType.Bottle,
        ItemType.Bread,
        ItemType.Can,
        ItemType.Cereal,
        ItemType.Jam,
        ItemType.Oil,
        ItemType.Pasta
    };
    
    private static readonly ItemType[] ColdItems =
    {
        ItemType.Beef,
        ItemType.Fish,
        ItemType.Egg,
        ItemType.Milk,
        ItemType.Sausage
    };
    
    private static readonly ItemType[] FreshItems =
    {
        ItemType.Broccoli,
        ItemType.Carrot,
        ItemType.Leek,
        ItemType.Potato,
        ItemType.Swede,
        ItemType.Tomato
    };

    private static readonly ItemType[] RareItems =
    {
        ItemType.Crisps,
        ItemType.Ham,
        ItemType.Melon
    };

    private static readonly ItemType[] NonRareItems =
    {
        ItemType.Bottle,
        ItemType.Bread,
        ItemType.Can,
        ItemType.Cereal,
        ItemType.Jam,
        ItemType.Oil,
        ItemType.Pasta,
        ItemType.Beef,
        ItemType.Fish,
        ItemType.Egg,
        ItemType.Milk,
        ItemType.Sausage,
        ItemType.Broccoli,
        ItemType.Carrot,
        ItemType.Leek,
        ItemType.Potato,
        ItemType.Swede,
        ItemType.Tomato
    };
}
