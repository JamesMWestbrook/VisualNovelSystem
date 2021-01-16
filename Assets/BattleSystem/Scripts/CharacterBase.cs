﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Sirenix.Serialization;

[System.Serializable]
public class CharacterBase
{
    public string Name = "Empty";
    public string NickName = "EmptyNN";
    public int Lvl = 1;


    public CharStats MaxStats = new CharStats();
    public CharStats CurStats = new CharStats();
    [OdinSerialize] public List<Skills> Skills = new List<Skills>();
    public ResourcePathAsset<Sprite> ShowTest;
    public ResourcePathAsset<Sprite> FacePath;
    public string SpriteGUI;
    public ResourcePathAsset<Sprite> BattleOutfitPath;
    public ResourcePathAsset<Sprite> BattleFacePath;
    
    //[SerializeField] public Dictionary<int, BaseBattleActions> BaseBattleActions;

    //public Weapon WeaponOne;

    //public EquipmentBase Head;
    //public EquipmentBase Body;
    //public EquipmentBase AccOne;
    //public EquipmentBase AccTwo;

    public CharacterBase DeepClone(){
        CharacterBase cb = new CharacterBase();
        DeepClone(cb);
        return cb;
    }
    public virtual void DeepClone(CharacterBase dst){
        dst.Name = Name;
        dst.ShowTest = ShowTest;
    }

    public virtual void Test(string test)
    {
        Debug.Log(this.Name);
    }

}

public static class CharacterExtensions
{
    public static CharacterBase Clone<T>(this CharacterBase data, T other)
    {

        using (MemoryStream ms = new MemoryStream())
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(ms, other);
            ms.Position = 0;
            return (CharacterBase)formatter.Deserialize(ms);
        }
    }

}