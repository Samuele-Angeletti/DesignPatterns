using System;
using System.Collections;
using UnityEngine;

public class TurretNugget : MonoBehaviour, ISubscriber
{
    ObjectPooler<Nugget> nuggetPooler;
    [SerializeField] Nugget nuggetPrefab;
    [SerializeField] float timeShoot;
    [SerializeField] Transform shootPivot;
    [SerializeField] float shootPower;
    [SerializeField] int maxBullets = 10;
    [SerializeField] AudioClip shootSound;

    int _currentBullets;
    private void Awake()
    {
        _currentBullets = maxBullets;
        nuggetPooler = new ObjectPooler<Nugget>(nuggetPrefab);

        Publisher.Subscribe(this, typeof(PauseMessage));
        Publisher.Subscribe(this, typeof(ReloadTurretMessage));
    }

    private void Start()
    {
        StartCoroutine(ShootCoroutine());
    }

    private IEnumerator ShootCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeShoot);
            Shoot();
        }
    }

    private void Shoot()
    {
        if (_currentBullets <= 0)
            return;

        Nugget spawnedNugget = nuggetPooler.Get();
        Publisher.Publish(new PlaySoundMessage(shootSound));

        if (!spawnedNugget.gameObject.activeSelf)
        {
            spawnedNugget.gameObject.SetActive(true);
        }
        else
        {
            spawnedNugget.onCollisionEnter += () =>
            {
                nuggetPooler.Set(spawnedNugget);
            };
        }

        spawnedNugget.transform.SetPositionAndRotation(shootPivot.position, Quaternion.identity);
        spawnedNugget.Body.linearVelocity = Vector3.zero;
        spawnedNugget.Body.AddForce(shootPivot.forward * shootPower, ForceMode.Impulse);

        _currentBullets--;
    }

    public void OnPublish(IPublisherMessage message)
    {
        if (message is PauseMessage pauseMessage)
        {
            if (pauseMessage.GamePaused)
                StopAllCoroutines();
            else
                StartCoroutine(ShootCoroutine());
        }
        else if (message is ReloadTurretMessage)
        {
            _currentBullets = maxBullets;
        }
    }

    public void OnDisableSubscriber()
    {
        Publisher.Unsubscribe(this, typeof(PauseMessage));
        Publisher.Unsubscribe(this, typeof(ReloadTurretMessage));
    }

    // decomissioniamo il subscriber quando l'oggetto viene distrutto
    private void OnDestroy()
    {
        OnDisableSubscriber();
    }
}