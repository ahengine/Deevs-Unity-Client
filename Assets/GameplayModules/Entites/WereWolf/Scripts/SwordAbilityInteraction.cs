using Entities;
using UnityEngine;

public class SwordAbilityInteraction : MonoBehaviour
{
    private const string REACT_STATE_MACHINE = "Base Layer.React.";
    private const string SWORD_ABILITY_STATE_MACHINE = "SwordAbility.";
    private const string SWORD_ABILITY_01_STATE = REACT_STATE_MACHINE + SWORD_ABILITY_STATE_MACHINE + "SwordAbility01";
    private const string SWORD_ABILITY_02_STATE = REACT_STATE_MACHINE + SWORD_ABILITY_STATE_MACHINE + "SwordAbility02";
    private const string SWORD_ABILITY_03_STATE = REACT_STATE_MACHINE + SWORD_ABILITY_STATE_MACHINE + "SwordAbility03";
    private const string JUMP_ATTACK_STATE = REACT_STATE_MACHINE + "JumpAttack";

    private Entity2D entity;

    private void Awake()
    {
        entity = GetComponent<Entity2D>();
    }

    public void SwordAbility01()
    {
        if (!entity.IsDamagable) return;
        entity.DamageState(true);
        entity.DoPlayAnimation(SWORD_ABILITY_01_STATE);
    }

    public void SwordAbility02()
    {
        if (!entity.IsDamagable) return;
        entity.DamageState(true);
        entity.DoPlayAnimation(SWORD_ABILITY_02_STATE);
    }

    public void SwordAbility03()
    {
        if (!entity.IsDamagable) return;
        entity.DamageState(true);
        entity.DoPlayAnimation(SWORD_ABILITY_03_STATE);
    }

    public void JumpAttack()
    {
        if (!entity.IsDamagable) return;
        entity.DamageState(true);
        entity.DoPlayAnimation(JUMP_ATTACK_STATE);
    }
}
