using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GridManager))]
public class SlotManager : MonoBehaviour
{
    public float delay;

    private List<SettingSlots> slots;

    private int currentSlotIndex = 0;

    public bool startPlay;

    private bool canPlay;

    private GridManager gridManager;

    public TMPro.TextMeshProUGUI SlotText;

    private void Awake()
    {
        if(slots == null)
            slots = new List<SettingSlots>();

        gridManager = GetComponent<GridManager>();

        currentSlotIndex = 0;

        canPlay = true;

        startPlay = false;
    }

    public void AddSlot(SettingSlots slot)
    {
        slots.Add(slot);
    }

    public void PlaySlot(int index)
    {
        if(index >= slots.Count || index < 0)
        {
            Debug.LogError("Index out of bound: " + index);
        }

        SettingSlots slot = slots[index];

        if (slot.type != SettingSlots.NoteType.silence && slot.type != SettingSlots.NoteType.length)
        {
            gridManager.PlayGrid(slot.ids.ToArray(), delay);
        }

        canPlay = false;

        if (slot.type != SettingSlots.NoteType.silence)
        {
            StartCoroutine(PlayDelay(delay));
        }
        else
        {
            StartCoroutine(PlayDelay(slot.ids[0]));
        }
    }
    
    private void Update()
    {
        if (startPlay && canPlay)
        {
            PlaySlot(currentSlotIndex);
            currentSlotIndex = (currentSlotIndex + 1) % slots.Count;
        }        
    }

    private void LateUpdate()
    {
        // minus one to get current Playing Slot Index        
        int index = (currentSlotIndex - 1) == -1 ? slots.Count - 1 : (currentSlotIndex - 1);
        SettingSlots slot = slots[index];

        SlotText.text = string.Format("Now Playing: {0} / {1} / {2}", index, SettingSlots.GetNameByType(slot.type), slot.name);
    }

    void StopAllSlots()
    {
        for(int i = 0; i < gridManager.GeneratedGrid.Count; ++i)
        {
            AudioSource audioSource = gridManager.GeneratedGrid[i].GetComponent<AudioSource>();
            if(audioSource && audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }

    IEnumerator PlayDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        //float timer = 0.0f;
        //while (timer < delay)
        //{
        //    timer += Time.deltaTime;
        //    print(timer);
        //    yield return null;
        //}

        StopAllSlots();

        canPlay = true;
    }

    public void InvokeStartPlay()
    {
        startPlay = !startPlay;
    }

}
