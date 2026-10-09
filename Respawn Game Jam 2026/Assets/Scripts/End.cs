using UnityEngine;
using UnityEngine.UI;

public class End : MonoBehaviour
{
    [SerializeField] GameObject endScreen;
    [SerializeField] AudioSource audioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.root.CompareTag("Player"))
        {
            endScreen.SetActive(true);
            Time.timeScale = 0f;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            audioSource.Play();
        }
    }
}
