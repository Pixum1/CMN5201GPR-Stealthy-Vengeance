using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MenuLight : MonoBehaviour
{
    [SerializeField] private GameObject m_Lights;
    [SerializeField] private Light2D m_RealLight;

    [SerializeField] private float m_FlickerIntensity;
    [SerializeField] private float m_MaxTime = 25f;
    [SerializeField] private float m_MinSize;
    [SerializeField] private float m_MaxSize;
    private float offtimer;

    private void Start()
    {
        offtimer = Random.Range(0, m_MaxTime);
    }

    private void Update()
    {
        if (offtimer <= 0)
        {
            m_Lights.transform.localScale = Vector3.one * Random.Range(m_MinSize, m_MaxSize);
            m_RealLight.intensity = Random.Range(m_MinSize/m_FlickerIntensity, m_MaxSize * m_FlickerIntensity) * 10;
            offtimer = Random.Range(0, m_MaxTime);
        }

        offtimer -= Time.deltaTime;
    }
}
