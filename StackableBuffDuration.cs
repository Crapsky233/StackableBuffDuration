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

        // 源码如下
        // else if (buffTime[i] < time) {
        // 	buffTime[i] = time;
        // }
        // IL代码如下
        // IL_0050: ldarg.0
        // IL_0051: ldfld int32[] Terraria.Player::buffTime
        // IL_0056: ldloc.1
        // IL_0057: ldelem.i4
        // IL_0058: ldarg.2 (我们的IL指针落在这个位置)
        // IL_0059: bge.s IL_0064
        private void Player_AddBuff_TryUpdatingExistingBuffTime(ILContext il) {
            var c = new ILCursor(il);
            c.GotoNext(MoveType.After, i => i.MatchLdarg(0)); // 0为Player实例
            c.GotoNext(MoveType.After, i => i.MatchLdfld(typeof(Player), nameof(Player.buffTime)));
            c.GotoNext(MoveType.After, i => i.MatchLdloc(1)); // 1即为buffTime[i]中的i
            c.GotoNext(MoveType.After, i => i.MatchLdelemI4());
            c.GotoNext(MoveType.After, i => i.MatchLdarg(2)); // 2是time，药水持续时间
            c.Emit(OpCodes.Ldarg_0); // 推入Player实例
            c.Emit(OpCodes.Ldloc_1); // 推入i
            c.EmitDelegate<Func<int, Player, int, int>>((returnValue, player, i) => {
                player.buffTime[i] += returnValue;
                return 0; // 这里返回0，让判断正常情况下永远返回false，直接干掉原版的重设Buff时间代码
            });
        }
    }
}