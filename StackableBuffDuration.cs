using Terraria.ModLoader;
using MonoMod.Cil;
using Terraria;
using Mono.Cecil.Cil;
using System;

namespace StackableBuffDuration
{
    public class StackableBuffDuration : Mod
    {
        public override void Load() {
            base.Load();
            IL.Terraria.Player.AddBuff_TryUpdatingExistingBuffTime += Player_AddBuff_TryUpdatingExistingBuffTime;
        }

        // Դ������
        // else if (buffTime[i] < time) {
        // 	buffTime[i] = time;
        // }
        // IL��������
        // IL_0050: ldarg.0
        // IL_0051: ldfld int32[] Terraria.Player::buffTime
        // IL_0056: ldloc.1
        // IL_0057: ldelem.i4
        // IL_0058: ldarg.2 (���ǵ�ILָ���������λ��)
        // IL_0059: bge.s IL_0064
        private void Player_AddBuff_TryUpdatingExistingBuffTime(ILContext il) {
            var c = new ILCursor(il);
            c.GotoNext(MoveType.After, i => i.MatchLdarg(0)); // 0ΪPlayerʵ��
            c.GotoNext(MoveType.After, i => i.MatchLdfld(typeof(Player), nameof(Player.buffTime)));
            c.GotoNext(MoveType.After, i => i.MatchLdloc(1)); // 1��ΪbuffTime[i]�е�i
            c.GotoNext(MoveType.After, i => i.MatchLdelemI4());
            c.GotoNext(MoveType.After, i => i.MatchLdarg(2)); // 2��time��ҩˮ����ʱ��
            c.Emit(OpCodes.Ldarg_0); // ����Playerʵ��
            c.Emit(OpCodes.Ldloc_1); // ����i
            c.EmitDelegate<Func<int, Player, int, int>>((returnValue, player, i) => {
                player.buffTime[i] += returnValue;
                return 0; // ���ﷵ��0�����ж������������Զ����false��ֱ�Ӹɵ�ԭ�������Buffʱ�����
            });
        }
    }
}