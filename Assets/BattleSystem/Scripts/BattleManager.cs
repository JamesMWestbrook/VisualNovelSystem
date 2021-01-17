﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public ActorSlot CurrentActor;
    public int TurnCounter;
    [Header(header: "Current actor's turn")]
    public int ActorCounter;

    public TextMeshProUGUI MoveText;
    public GameObject DamagePopupPrefab;
    public List<ActorSlot> Party;

    public List<ActorSlot> Actors;
    public List<ActorSlot> Enemies;
    public Transform EffectTrans;
    public void UpdateMove(string moveName)
    {
        MoveText.text = moveName;
        MoveText.maxVisibleCharacters = 0;
        LeanTween.value(MoveText.gameObject, (float x) => MoveText.maxVisibleCharacters = (int)x, 0, MoveText.text.Length, 2f);
    }
    // Start is called before the first frame update

    public UnityEvent StartOfBattle;
    public UnityEvent StartOfTurn;
    public UnityEvent EndOfTurn;
    public UnityEvent EndOfBattle;

    public bool AllowInput;

    [Header("UI")]
    public GameObject ChooseActionPanel;


    void Start()
    {
        StartOfBattle.Invoke();
        StartCoroutine(SlightDelay());
    }
    public IEnumerator SlightDelay()
    {
        yield return new WaitForSeconds(0.01f);
        CurrentActor = Actors[0];
        if (!DelayedStart) StartActor();
    }
    void DimText()
    {

    }
    public bool DelayedStart;
    public void StartBattle()
    {
        //start Enemy if AI is first

        //NextActor for the player?



    }


    public void NextActor()
    {
        ActorCounter++;
        if (ActorCounter >= Actors.Count)
        {
            ActorCounter = 0;
            StartCoroutine(DelayTurn());
        }
        else CurrentActor = Actors[ActorCounter];
        StartCoroutine(DelayActor());
    }
    public bool DelayNextTurn;

    public IEnumerator DelayTurn()
    {
        EndOfTurn.Invoke();
        do
        {
            yield return null;
        } while (DelayNextTurn);
        NextTurn();
    }

    public Button AttackButton;
    public Button SkillsButton;

    public void StartActor()
    {
        if (!CurrentActor.IsAI)
        {
            //Sprite
            CharacterSprite charSprite = CutsceneManager.Instance.rightCharacter.GetComponent<CharacterSprite>();
            charSprite.Face.enabled = true;
            charSprite.Outfit.enabled = true;
            charSprite.Face.sprite = CurrentActor.Actor.BattleFacePath.Get();
            charSprite.Outfit.sprite = CurrentActor.Actor.BattleOutfitPath.Get();

            Transform dest = CutsceneManager.Instance.RightSpot;
            Image rightImage = CutsceneManager.Instance.rightCharacter;

            Vector3 _beginPoint = new Vector3(dest.position.x + 100, rightImage.transform.position.y, rightImage.transform.position.z);
            Vector3 _endPoint = new Vector3(CutsceneManager.Instance.RightSpot.position.x, _beginPoint.y, _beginPoint.z);

            Image _face = charSprite.Face;
            Image _outfit = charSprite.Outfit;
            rightImage.transform.position = _beginPoint;
            LeanTween.move(rightImage.gameObject, _endPoint, SpriteTime);

            MovementNode.ColorChange(_outfit.gameObject, new Color(0.7f, 0.7f, 0.7f, 1), new Color(1, 1, 1, 1), SpriteTime);
            MovementNode.ColorChange(_face.gameObject, new Color(0.7f, 0.7f, 0.7f, 1), new Color(1, 1, 1, 1), SpriteTime);

            //UI Buttons
            StartOptions();
        }
        else
        {
            CurrentActor.GetComponent<IEnemy>().AI();
            HideSprite();
            // hide buttons
        }
    }
    public void StartOptions()
    {
        buttonState = ButtonState.First;

        ChooseActionPanel.SetActive(true);
        OpenTargetsButton TargetButton = AttackButton.GetComponent<OpenTargetsButton>();
        TargetButton.AssignedSkill = NormalAttack;
    }
    public void SkillMenu()
    {
        InTargetMenu = true;
        buttonState = ButtonState.Skills;
        for (int i = 0; i < CurrentActor.Actor.Skills.Count; i++)
        {
            //fill each of the target buttons
            SkillButtons[i].gameObject.SetActive(true);
            SkillButtons[i].GetComponent<OpenTargetsButton>().AssignedSkill = CurrentActor.Actor.Skills[i];
            SkillButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = SkillButtons[i].GetComponent<OpenTargetsButton>().AssignedSkill.Name;
        }
    }


    public void TargetMenu(Button self)
    {
        InTargetMenu = true;
        //close appropriate window
        switch (buttonState)
        {
            case (ButtonState.First):
                ChooseActionPanel.SetActive(false);
                break;
            case (ButtonState.Skills):
                foreach (Button button in SkillButtons)
                {

                    button.gameObject.SetActive(false);
                }
                break;
        }
        Skills skill = self.GetComponent<OpenTargetsButton>().AssignedSkill;
        List<ActorSlot> targets = new List<ActorSlot>();

        //check if friendly
        if (skill.Friendly) targets = Party;
        else targets = Enemies;
        List<GameObject> targetsGO = new List<GameObject>();
        foreach (ActorSlot actor in targets)
        {
            targetsGO.Add(actor.gameObject);
        }
        //check if single or multi target
        //Put target and skill on button. 
        //When you click this the skill will affect its target.
        if (skill.targetCount == Skills.TargetCount.Multiple)
        {
            //assign the entire list to a button
            TargetButtons[0].gameObject.SetActive(true);
            TargetButtons[0].GetComponentInChildren<TextMeshProUGUI>().text = "All";
            TargetButtons[0].GetComponent<SkillButton>().AssignedSkill = skill;
            TargetButtons[0].GetComponent<SkillButton>().Targets = targetsGO;
            TargetButtons[0].GetComponent<SkillButton>().user = CurrentActor.gameObject;
        }
        else
        {//Single target
            //Opens a button for each enemy/ally target to hit
            for (int i = 0; i < targets.Count; i++)
            {
                TargetButtons[i].gameObject.SetActive(true);
                TargetButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = targets[i].GetComponent<ActorSlot>().Actor.Name;
                TargetButtons[i].GetComponent<SkillButton>().AssignedSkill = skill;


                List<GameObject> singleTarget = new List<GameObject>();
                singleTarget.Add(targetsGO[i]);
                TargetButtons[i].GetComponent<SkillButton>().Targets = singleTarget;
                //TargetButtons[i].GetComponent<SkillButton>().Targets = targetsGO;
                TargetButtons[i].GetComponent<SkillButton>().user = CurrentActor.gameObject;
            }
        }
    }
    void FillTargetButton(Button button, List<ActorSlot> targets, Skills skill)
    {
        button.gameObject.SetActive(true);
        SkillButton skillButton = button.GetComponent<SkillButton>();
        //set button name to skill's name
        skillButton.AssignedSkill = skill;
    }
    public void OpenWindow(GameObject go)
    {
        go.SetActive(true);
    }
    public void CloseWindow(GameObject go)
    {
        go.SetActive(false);
    }
    public void CloseTargets()
    {
        foreach (Button go in TargetButtons)
        {
            go.gameObject.SetActive(false);
        }
    }

    public bool InTargetMenu;
    public void CancelMenu()
    {

        switch (buttonState)
        {
            case (ButtonState.First):

                if (InTargetMenu)
                {
                    CloseTargets();

                    //close target menu and open first menu
                }

                //                InTargetMenu = false;

                break;
            case (ButtonState.Skills):
                if (InTargetMenu)
                {
                    //close target menu and open skill menu
                    CloseTargets();
                    SkillMenu();
                }

                //                InTargetMenu = false;
                break;
        }
    }
    public void RemoveUI()
    {

    }
    public void EnableUI()
    {
        foreach (ActorSlot actor in Party)
        {
            actor.gameObject.SetActive(true);
            RectTransform actorTrans = actor.GetComponent<RectTransform>();
            actorTrans.LeanScale(new Vector3(0, 1, 1), 0);
            Vector3 endPos = actorTrans.position;
            //  actorTrans.position = new Vector3(-500, actorTrans.position.y, actorTrans.position.z);
            // actorTrans.LeanMove(endPos, 0.7);
            // LeanTween.move(actorTrans.gameObject, endPos, 0.7f);
            LeanTween.scale(actorTrans, Vector3.one, 0.7f);
        }
    }
    public List<Button> TargetButtons;
    public List<Button> SkillButtons;
    public ButtonState buttonState;
    public enum ButtonState
    {
        First,
        Skills,
        Items,
    }
    public IEnumerator DelayActor()
    {
        do
        {
            yield return null;
        } while (DelayNextTurn);
        StartActor();
    }
    public void NextTurn()
    {
        CurrentActor = Actors[ActorCounter];
        TurnCounter++;
        StartOfTurn.Invoke();
    }
    public void PostSkill(float waitTime)
    {
        //this is used after every skill is done
        StartCoroutine(DelayAction(NextActor, waitTime));
        foreach (Button button in TargetButtons)
        {
            button.gameObject.SetActive(false);
        }
    }
    public IEnumerator DelayAction(Action action, float secondsToWait)
    {
        yield return new WaitForSeconds(secondsToWait);
        action();
    }
    [NonSerialized] public float SpriteTime = 0.3f;
    public void HideSprite()
    {
        CharacterSprite charSprite = CutsceneManager.Instance.rightCharacter.GetComponent<CharacterSprite>();


        Transform dest = CutsceneManager.Instance.RightSpot;
        Image rightImage = CutsceneManager.Instance.rightCharacter;

        Vector3 _endPoint = new Vector3(CutsceneManager.Instance.RightSpot.position.x + 100, charSprite.transform.position.y, charSprite.transform.position.z);

        Image _face = charSprite.Face;
        Image _outfit = charSprite.Outfit;
        LeanTween.move(rightImage.gameObject, _endPoint, SpriteTime);

        MovementNode.ColorChange(_outfit.gameObject, new Color(1f, 1f, 1f, 1), new Color(0.0f, 0.0f, 0.0f, 0.0f), SpriteTime);
        MovementNode.ColorChange(_face.gameObject, new Color(1, 1, 1, 1), new Color(0.0f, 0.0f, 0.0f, 0.0f), SpriteTime);
        StartCoroutine(DisableSprite(charSprite));
    }
    private IEnumerator DisableSprite(CharacterSprite charSprite)
    {
        yield return new WaitForSeconds(SpriteTime);
        charSprite.Face.enabled = false;
        charSprite.Outfit.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void CloseSkills()
    {
        for (int i = 0; i < SkillButtons.Count; i++)
        {
            SkillButtons[i].gameObject.SetActive(false);
        }
    }
    public Skills NormalAttack;
    public void SpawnGO(GameObject go, Transform dest, float time)
    {

        go = Instantiate<GameObject>(go, dest.Find("EffectTrans"));
        go.GetComponent<DestroyThis>().Timer = time;
    }
public void StartSpawn(float DestructTimer, int popupText, ActorSlot Defender, bool Healing = false){
    StartCoroutine(DelayedSpawn(DestructTimer, popupText, Defender, Healing));
}
    public IEnumerator DelayedSpawn(float DestructTimer, int popupText, ActorSlot Defender, bool Healing = false){
        yield return new WaitForSeconds(DestructTimer);
        SpawnDamage(popupText, Defender, Healing);
    }
    public void SpawnDamage(int Damage, ActorSlot actor, bool Healing = false)
    {
        //spawn
        //set parent
        //set local transform
        GameObject go = Instantiate<GameObject>(DamagePopupPrefab, actor.transform);

        if (actor.IsAI)
        {

        }
        else
        {
            go.GetComponent<RectTransform>().localPosition = new Vector3(135, 87, 0);
        }
        TextMeshProUGUI text = go.GetComponent<TextMeshProUGUI>();
        text.text = Damage.ToString();
        if(Healing || Damage > 0){
            text.text = "+ " + text.text;
            text.color = Color.green;
        }
    }
}
