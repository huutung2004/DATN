using StarterAssets;
using UnityEngine;

public class PlayerStateMachineInitializer : MonoBehaviour
{
    [SerializeField] private ThirdPersonController controller;
    private StateMachine stateMachine;
    private AttackState attacking;
    private LocomotionState locomotion;
    private HandleWeaponState handleWeaponState;
    private UnEquidWeapoState unEquidWeapoState;
    private void Awake()
    {
        if (controller == null)
            controller = GetComponent<ThirdPersonController>();
        stateMachine = new StateMachine();
        handleWeaponState = new HandleWeaponState(controller, stateMachine);
        locomotion = new LocomotionState(controller, stateMachine);
        attacking = new AttackState(controller, stateMachine);
        unEquidWeapoState = new UnEquidWeapoState(controller, stateMachine);
        stateMachine.AddTransition(locomotion, handleWeaponState, new FuncPredicate(() => PlayerEquippedItem.Instance.m_isEquidWeapon));
        stateMachine.AddTransition(unEquidWeapoState, handleWeaponState, new FuncPredicate(() => PlayerEquippedItem.Instance.m_isEquidWeapon));

        stateMachine.AddTransition(handleWeaponState, unEquidWeapoState, new FuncPredicate(() => !PlayerEquippedItem.Instance.m_isEquidWeapon));
        stateMachine.AddTransition(handleWeaponState, attacking, new FuncPredicate(() => Input.GetMouseButtonDown(0) && attacking.IsAttackFinished));
        stateMachine.AddTransition(attacking, handleWeaponState, new FuncPredicate(() => attacking.IsAttackFinished));
        stateMachine.AddTransition(attacking, unEquidWeapoState, new FuncPredicate(() => !PlayerEquippedItem.Instance.m_isEquidWeapon));
        stateMachine.SetState(locomotion);
    }

    private void Update() => stateMachine.Update();

    private void FixedUpdate() => stateMachine.FixedUpdate();
}