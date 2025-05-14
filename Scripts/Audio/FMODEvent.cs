using UnityEngine;
using FMODUnity;

[CreateAssetMenu(fileName = "New FMOD Event", menuName = "ScriptableObjects/FMOD/Event")]
public class FMODEvent : ScriptableObject
{
    [EventRef]
    [SerializeField]
    private string Event = "";

    public string GetEvent()
    {
        return Event;
    }

    public void SetEvent(string value)
    {
        Event = value;
    }

    public void Play()
    {
        RuntimeManager.PlayOneShot(Event);
    }
}
