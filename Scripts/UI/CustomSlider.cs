using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public class CustomSlider : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    
    // Keys
    private const string MusicVolumeKey = "MusicVolume";
    private const string SFXVolumeKey = "SFXVolume";

    // References
    private UIReferences uiReferences;
    
    private void Awake()
    {
        uiReferences = FindObjectOfType<UIReferences>();
    }

    private void Start()
    {
        Slider musicSlider = uiReferences.MusicSlider;
        Slider sfxSlider = uiReferences.SFXSlider;
        
        ConfigureSlider(musicSlider);
        AddSliderValueChange(musicSlider, audioMixer, MusicVolumeKey);
        
        ConfigureSlider(sfxSlider);
        AddSliderValueChange(sfxSlider, audioMixer, SFXVolumeKey);
    }

    private void ConfigureSlider(Slider m_Slider)
    {
        VisualElement m_Dragger = m_Slider.Q<VisualElement>(UIConstants.SliderDragger);
        
        // Fill
        VisualElement m_Fill = new VisualElement();
        m_Dragger.Add(m_Fill);

        m_Fill.name = "Fill";
        m_Fill.AddToClassList(UIConstants.SliderFillClass);
        
        // Dragger
        VisualElement m_NewDragger = new VisualElement();
        m_Slider.Add(m_NewDragger);
        m_NewDragger.AddToClassList(UIConstants.NewDraggerClass);
        m_NewDragger.pickingMode = PickingMode.Ignore;
        
        CreateOnSliderValueChange(m_Slider, m_Dragger, m_NewDragger);
    }

    private void CreateOnSliderValueChange(Slider m_Slider, VisualElement m_Dragger, VisualElement m_NewDragger)
    {
        void OnSliderValueChanged(ChangeEvent<float> value)
        {
            Vector2 dist = new Vector2((m_NewDragger.layout.width - m_Dragger.layout.width) / 2, (m_NewDragger.layout.height - m_Dragger.layout.height) / 2);
            Vector2 pos = m_Dragger.parent.LocalToWorld(m_Dragger.transform.position);
            m_NewDragger.transform.position = m_NewDragger.parent.WorldToLocal(pos-dist);
        }
        
        void OnSliderInit(GeometryChangedEvent evt)
        {
            OnSliderValueChanged(null);
        }
        
        m_Slider.RegisterCallback<ChangeEvent<float>>(OnSliderValueChanged);
        m_Slider.RegisterCallback<GeometryChangedEvent>(OnSliderInit);
    }
    
    private void AddSliderValueChange(Slider m_Slider, AudioMixer audioMixer, string volumeKey)
    {
        void OnSliderValueChanged(ChangeEvent<float> value)
        {
            float volume = Mathf.Log10(value.newValue + Mathf.Epsilon) * 20;
            audioMixer.SetFloat(volumeKey, volume);
        }
        
        m_Slider.RegisterCallback<ChangeEvent<float>>(OnSliderValueChanged);
        
        // Set slider value to current volume
        audioMixer.GetFloat(volumeKey, out float volume);
        m_Slider.value = Mathf.Pow(10, volume / 20);
    }
}
