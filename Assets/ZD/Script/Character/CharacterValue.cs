using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZoneDepict;

//腳色數值加成
public class CharacterValue
{
    public CharacterValue() { }

    public CharacterValue(List<float> attackDamage, List<float> skillMana, List<float> skillCD)
    {
        AttackDamage = attackDamage;
        SkillMana = skillMana;
        SkillCD = skillCD;
    }

    public List<float> AttackDamage = new List<float> { 10, 20, 15, 30 };
    public List<float> SkillMana = new List<float> { 5, 20, 30, 60 };
    public List<float> SkillCD = new List<float> { 0.25f, 3f, 6f, 10 };

    
    public float HP {
        get { return ValueSet["HP"]; } 
        set { ValueSet["HP"] = value; }
    } 
    Dictionary<string, float> ValueSet = new Dictionary<string, float>()
    {
            { "HP",100f},
            { "MP",100f},
            { "RegHP",100f},
            { "RegMP",100f},
            { "MaxHP",100f},
            { "MaxMP",100f},

            { "HpRecoverRate",2.5f},     //基礎回血/每秒
            {"MpRecoverRate",2.5f},     //基礎回魔/每秒
            {"HpBuff",1.0f},        //總血量上限加成 ex: 1.3 = 總血量增加30%
            {"MpBuff",1.0f},        //總mp上限加成   ex: 1.3 = 總血量增加30%
            {"CDR",1.0f},           //-CD ex: 0.3 = 減少70%CD
            {"SkillBuff",1.0f},     //技能傷害加成
            {"NormalAttackBuff",1.0f},//普功傷害加成
            {"ReduceManaCost",1.0f}, //0.3 = 魔力消耗-70%
            {"SpeedUp",1.0f},       //speed up
            {"ArmorBuff",1.0f},     //防禦提升  1.0 = 受到100%傷害  0.3 = 只會受到30%傷害 
            {"GriticalRate",0.05f},//普功爆擊率
    };
    //直接設定數值
   /* public void SetValue(string valueName, float value)
    {
        if (ValueSet.ContainsKey(valueName))
        {
            ValueSet[valueName] = value;
        }
    }
    public float GetValue(string valueName, float value)
    {
        if (ValueSet.ContainsKey(valueName))
        {
            return ValueSet[valueName];
        }

        Debug.LogFormat("No {0} value exist", valueName);
        return -1;

    }*/
    /*
   //設定數值為newValue，持續effectTime秒
    public void SetLastValue(string valueName, float newValue, float effectTime)
    {
        if (ValueSet.ContainsKey(valueName))
        {
            StartCoroutine(SetRate(valueName, newValue, effectTime));
        }
    }
    IEnumerator SetRate(string valueName, float newValue, float effectTime)
    {
        Debug.LogFormat("{0} set to {1} : {2}s", valueName, newValue, effectTime);
        float preValue = ValueSet[valueName];
        ValueSet[valueName] = newValue;
        yield return new WaitForSecondsRealtime(effectTime);//等待5s  
        ValueSet[valueName] = preValue;
        Debug.LogFormat("{0} set to {1} end", valueName, newValue);
    }
    */
}
