using DesignPatterns.Generics;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : Singleton<GameManager>, ISubject
{
    [SerializeField] Player player;
    public Transform FoodSpawnPoint;
    public IFactory FoodFactory { get; private set; }

    InputSystem_Actions _input;

    private List<IObserver> _attachedObservers = new List<IObserver>();
    public bool IsPaused { get; private set; }

    public override void Awake()
    {
        base.Awake();
        FoodFactory = new Factory();

        var spawnPoint = FindFirstObjectByType<SpawnPoint>();
        if (spawnPoint != null)
        {
            FoodSpawnPoint = spawnPoint.transform;
        }

        _input = new InputSystem_Actions();

        _input.Player.Move.performed += Move_performed;
        _input.Player.Move.canceled += Move_canceled;

        _input.Player.Jump.performed += Jump_performed;

        _input.Player.Attack.performed += Attack_performed;

        _input.General.PauseGame.performed += PauseGame_performed;

        _input.Enable();
    }

    private void PauseGame_performed(InputAction.CallbackContext context)
    {
        IsPaused = !IsPaused;
        Notify();
        Publisher.Publish(new PauseMessage(IsPaused));
        Time.timeScale = IsPaused ? 0 : 1;
    }

    private void Attack_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        player.AttackRequest();
    }

    private void Jump_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        player.JumpRequest();
    }

    private void Move_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        player.MoveDirectionRequest(Vector2.zero);
    }

    private void Move_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        player.MoveDirectionRequest(obj.ReadValue<Vector2>());
    }

    public void Attach(IObserver observer)
    {
        _attachedObservers.Add(observer);
    }

    public void Detach(IObserver observer)
    {
        _attachedObservers.Remove(observer);
    }

    public void Notify()
    {
        _attachedObservers.ForEach(observer => observer.ObserverUpdate(this));
    }
}
 