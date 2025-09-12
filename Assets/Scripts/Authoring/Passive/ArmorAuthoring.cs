using Unity.Entities;
using UnityEngine;

public class ArmorAuthoring : MonoBehaviour
{
    public int ID;
    public int baseArmorVaule;

    public int currentLevel;
    public int increment;

    public class Baker : Baker<ArmorAuthoring>
    {
        public override void Bake(ArmorAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new ArmorComponent
            {
                baseArmorVaule = authoring.baseArmorVaule,
                armorValue = authoring.baseArmorVaule,

                increment = authoring.increment,
            });

            AddComponent(entity, new PassiveComponent
            {
                PassiveType = PassiveType.Armor,
                ID = authoring.ID,
                Level = authoring.currentLevel,
                MaxLevel = 5,
                DisplayName = "Armor",
                Description = "Reduces incoming damage.",
            });
        }
    }
}

public struct ArmorComponent : IComponentData
{
    public int armorValue;
    public int baseArmorVaule;
    public int increment;
}