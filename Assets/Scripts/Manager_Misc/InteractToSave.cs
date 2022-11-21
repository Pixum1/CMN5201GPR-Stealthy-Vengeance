using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using static UnityEngine.InputSystem.InputAction;

public class InteractToSave : MonoBehaviour
{

    [SerializeField] private VisualEffect[] m_VFX;
    [SerializeField] private VisualEffect m_ShockWave;
    [SerializeField] private Gradient m_ActivatedFlameColor;
    [SerializeField] private float m_Distance = 3f;
    [SerializeField] private GameObject m_Text;
    private void Start()
    {
        GameManager.Instance.PlayerInput.currentActionMap.FindAction("Interact").performed += OnInteract;
    }
    private void Update()
    {
        if ((transform.position - PlayerController.Instance.transform.position).sqrMagnitude <= Mathf.Pow(m_Distance, 2))
            m_Text.SetActive(true);
        else
            m_Text.SetActive(false);
    }
    public void OnInteract(CallbackContext _ctx)
    {
        if ((transform.position - PlayerController.Instance.transform.position).sqrMagnitude <= Mathf.Pow(m_Distance, 2))
        {
            GameManager.Instance.OnSaveGame();
            for (int i = 0; i < m_VFX.Length; i++)
            {
                m_VFX[i].SetGradient("Gradient", m_ActivatedFlameColor);
            }
            m_ShockWave.Play();
        }
    }
}
