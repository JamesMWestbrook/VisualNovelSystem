﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ActorSlot : MonoBehaviour
{

    public bool IsAI;

    public Image Face;
    public Transform AnimationSpawnPoint;
    public CharacterBase Actor;
    public void SetGraphics()
    {
        Face.sprite = Actor.FacePath.Get();
    }
    public CharacterSO EnemySO;

    public TextMeshProUGUI HP;
    public Image HPForeground;
    public TextMeshProUGUI MP;
    public Image MPForeground;
    public void UpdateStats()
    {
        HP.text = Actor.CurStats.HP.ToString();
        HPForeground.fillAmount = 100 * Actor.CurStats.HP / Actor.MaxStats.HP;

        MP.text = Actor.CurStats.MP.ToString();
        MPForeground.fillAmount = 100 * Actor.CurStats.MP / Actor.MaxStats.MP;
    }

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.BattleManager.Actors.Add(this);
        if (IsAI)
        {
            GameManager.Instance.BattleManager.Enemies.Add(this);
            Actor = EnemySO.Character;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
