using DesignPatterns.Generics;
using UnityEngine;

public class UIManager : Singleton<UIManager>, IObserver
{
    [SerializeField] GameObject pauseMenuPanel;

    // --------------------------------------------
    // perché è importante? 
    // perché se non eseguo il detach e questo oggetto viene distrutto,
    // il GameManager contiene ancora il suo riferimento e quindi ricevo NullReferenceException
    private void OnEnable()
    {
        GameManager.Instance.Attach(this);
    }
    private void OnDisable()
    {
        GameManager.Instance.Detach(this);
    }
    // --------------------------------------------

    public void ObserverUpdate(ISubject subject)
    {
        // usiamo l'observer sul game manager per sapere quando il gioco viene messo in pausa

        var gameManager = subject as GameManager; // ci assicuriamo che il subject sia un game manager
        if (gameManager != null)
        {
            /*
            if (gameManager.IsPaused)
            {
                // mostriamo il menu di pausa
                pauseMenuPanel.SetActive(true);
            }
            else
            {
                // nascondiamo il menu di pausa
                pauseMenuPanel.SetActive(false);
            }
            */

            pauseMenuPanel.SetActive(gameManager.IsPaused);
        }
    }
}
