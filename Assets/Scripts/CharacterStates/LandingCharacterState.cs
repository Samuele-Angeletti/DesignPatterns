using UnityEngine;

public class LandingCharacterState : State
{
    float _delayTime;
    private float _timePassed = 0;
    public LandingCharacterState(Player player, float delayTime)
    {
        _owner = player;
        _delayTime = delayTime;
    }

    public Player _owner { get; }

    public override void OnCollisionEnter()
    {
        throw new System.NotImplementedException();
    }

    public override void OnCollisionExit()
    {
        throw new System.NotImplementedException();
    }

    public override void OnEnd()
    {
        Debug.Log("Sto uscendo da Landing");
    }

    public override void OnFixedUpdate()
    {
    }

    public override void OnStart()
    {
        _timePassed = 0;
    }

    public override void OnTriggerEnter()
    {
        throw new System.NotImplementedException();
    }

    public override void OnTriggerExit()
    {
        throw new System.NotImplementedException();
    }

    public override void OnUpdate()
    {
        if (_owner.MoveRequest)
        {
            _owner.SetState(ECharacterState.Walking);
            return;
        }

        _timePassed += Time.deltaTime;

#if UNITY_EDITOR
        if (_timePassed < _owner.LandingDealyTime)
            return;
#else
        if (_timePassed < _delayTime)
            return;
#endif

        _owner.SetState(ECharacterState.Idle);
    }
}
