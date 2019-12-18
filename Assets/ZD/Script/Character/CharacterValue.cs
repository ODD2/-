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

    
    public float HP { get; set; } = 100f;
    public float MP { get; set; } = 100f;
    public float RegHP { get; set; } = 100f;
    public float RegMP { get; set; } = 100f;
    public float MaxHP { get; set; } = 100f;
    public float MaxMP { get; set; } = 100f;

    public float HpRecoverRate { get; set; } = 2.5f;     //基礎回血/每秒
    public float MpRecoverRate { get; set; } = 2.5f;     //基礎回魔/每秒
    public float HpBuff { get; set; } = 1.0f;        //總血量上限加成 ex: 1.3 { get; set; } =  總血量增加30%
    public float MpBuff { get; set; } = 1.0f;        //總mp上限加成   ex: 1.3 { get; set; } =  總血量增加30%
    public float CDR { get; set; } = 1.0f;           //-CD ex: 0.3 { get; set; } =  減少70%CD
    public float SkillBuff { get; set; } = 1.0f;     //技能傷害加成
    public float NormalAttackBuff { get; set; } = 1.0f;//普功傷害加成
    public float ReduceManaCost { get; set; } = 1.0f; //0.3 { get; set; } =  魔力消耗-70%
    public float SpeedUp { get; set; } = 1.0f;       //speed up
    public float ArmorBuff { get; set; } = 1.0f;     //防禦提升  1.0 { get; set; } =  受到100%傷害  0.3 { get; set; } =  只會受到30%傷害 
    public float GriticalRate { get; set; } = 0.05f;//普功爆擊率
}
