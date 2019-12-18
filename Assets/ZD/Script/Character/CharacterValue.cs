using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZoneDepict;
using ExitGames.Client.Photon;
//腳色數值加成
public class CharacterValue
{
    /*
    public byte Id { get; set; }
    public static object Deserialize(byte[] data)
    {
        var result = new CharacterValue();
        result.Id = data[0];
        return result;
    }
    public static byte[] Serialize(object customType)
    {
        var c = (CharacterValue)customType;
        return new byte[] { c.Id };
    }

    internal static void Register()
    {
        PhotonPeer.RegisterType(typeof(CharacterValue), 123, Serialize, Deserialize);
    }
    */
    public float HP { get; set; } = 100f;
    public float MP { get; set; } = 100f;
    public float RegHP { get; set; } = 0.2f;
    public float RegMP { get; set; } = 2.9f;
    public float MaxHP { get; set; } = 100f;
    public float MaxMP { get; set; } = 100f;

    public float CDR { get; set; } = 1.0f;              //Attack speed
    public float AttackBuff { get; set; } = 1.0f;       //傷害加成
    public float ReduceManaCost { get; set; } = 1.0f;   //0.3 { get; set; } =  魔力消耗-70%
}
