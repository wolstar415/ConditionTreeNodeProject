
// 예제: 체력이 50 이상이고 침묵 상태가 아님
// Example: HP is greater than or equal to 50 AND not silenced
string cond1 = "HP:>=50 && !IsSilence";
var node1 = ConditionTreeNode<BattleContext>.Parse(cond1, BattleConditionUtil.LeafFactory);

var ctx1_1 = new BattleContext { HP = 80, IsSilence = false }; // ✅
var ctx1_2 = new BattleContext { HP = 80, IsSilence = true };  // ❌
var ctx1_3 = new BattleContext { HP = 30, IsSilence = false }; // ❌
bool r1_1 = node1.IsMatch(ctx1_1);
bool r1_2 = node1.IsMatch(ctx1_2);
bool r1_3 = node1.IsMatch(ctx1_3);

// 예제: 스턴 상태이거나 공격 버프가 있음
// Example: Is stunned OR has "buff_attack" state
string cond2 = "IsStun || HasState:buff_attack";
var node2 = ConditionTreeNode<BattleContext>.Parse(cond2, BattleConditionUtil.LeafFactory);

var ctx2_1 = new BattleContext { IsStun = true }; // ✅
var ctx2_2 = new BattleContext { States = new() { "buff_attack" } }; // ✅
var ctx2_3 = new BattleContext { }; // ❌
bool r2_1 = node2.IsMatch(ctx2_1);
bool r2_2 = node2.IsMatch(ctx2_2);
bool r2_3 = node2.IsMatch(ctx2_3);

// 예제: (체력이 30 미만이고 공황 상태) 또는 (스턴 상태이면서 위기 상태)
// Example: (HP < 30 AND has "panic") OR (IsStun AND has "critical")
string cond3 = "(HP:<30 && HasState:panic) || (IsStun && HasState:critical)";
var node3 = ConditionTreeNode<BattleContext>.Parse(cond3, BattleConditionUtil.LeafFactory);

var ctx3_1 = new BattleContext { HP = 20, States = new() { "panic" } }; // ✅
var ctx3_2 = new BattleContext { IsStun = true, States = new() { "critical" } }; // ✅
var ctx3_3 = new BattleContext { HP = 50, IsStun = false }; // ❌
bool r3_1 = node3.IsMatch(ctx3_1);
bool r3_2 = node3.IsMatch(ctx3_2);
bool r3_3 = node3.IsMatch(ctx3_3);

// 예제: 공격 버프가 있고, 스턴이 아니며, 체력이 20 이상
// Example: Has "buff_attack" AND not stunned AND HP >= 20
string cond4 = "HasState:buff_attack && !IsStun && HP:>=20";
var node4 = ConditionTreeNode<BattleContext>.Parse(cond4, BattleConditionUtil.LeafFactory);

var ctx4_1 = new BattleContext { HP = 25, States = new() { "buff_attack" }, IsStun = false }; // ✅
var ctx4_2 = new BattleContext { HP = 25, States = new() { "buff_attack" }, IsStun = true };  // ❌
var ctx4_3 = new BattleContext { HP = 25, IsStun = false }; // ❌
var ctx4_4 = new BattleContext { HP = 10, States = new() { "buff_attack" }, IsStun = false }; // ❌
bool r4_1 = node4.IsMatch(ctx4_1);
bool r4_2 = node4.IsMatch(ctx4_2);
bool r4_3 = node4.IsMatch(ctx4_3);
bool r4_4 = node4.IsMatch(ctx4_4);

// 예제: (스턴 상태이거나 침묵 상태)이며 동시에 약화 상태
// Example: (IsStun OR IsSilence) AND has "weak"
string cond5 = "(IsStun || IsSilence) && HasState:weak";
var node5 = ConditionTreeNode<BattleContext>.Parse(cond5, BattleConditionUtil.LeafFactory);

var ctx5_1 = new BattleContext { IsStun = true, States = new() { "weak" } };     // ✅
var ctx5_2 = new BattleContext { IsSilence = true, States = new() { "weak" } }; // ✅
var ctx5_3 = new BattleContext { IsSilence = true }; // ❌
var ctx5_4 = new BattleContext { IsStun = true, States = new() { "panic" } }; // ❌
bool r5_1 = node5.IsMatch(ctx5_1);
bool r5_2 = node5.IsMatch(ctx5_2);
bool r5_3 = node5.IsMatch(ctx5_3);
bool r5_4 = node5.IsMatch(ctx5_4);

// 예제: 독 상태가 아니며, (스턴 상태여도 체력이 충분해야 함)
// Example: NOT (Has poison OR (IsStun AND HP < 20))
string cond6 = "!(HasState:poison || (IsStun && HP:<20))";
var node6 = ConditionTreeNode<BattleContext>.Parse(cond6, BattleConditionUtil.LeafFactory);

var ctx6_1 = new BattleContext { HP = 100 }; // ✅
var ctx6_2 = new BattleContext { IsStun = true, HP = 50 }; // ✅
var ctx6_3 = new BattleContext { States = new() { "poison" } }; // ❌
var ctx6_4 = new BattleContext { IsStun = true, HP = 10 }; // ❌
bool r6_1 = node6.IsMatch(ctx6_1);
bool r6_2 = node6.IsMatch(ctx6_2);
bool r6_3 = node6.IsMatch(ctx6_3);
bool r6_4 = node6.IsMatch(ctx6_4);

// 예제: (HP >= 100 AND 준비 상태) 또는 (스턴이 아니고 가속 상태)
// Example: (HP >= 100 AND has "ready") OR (NOT IsStun AND has "haste")
string cond7 = "(HP:>=100 && HasState:ready) || (!IsStun && HasState:haste)";
var node7 = ConditionTreeNode<BattleContext>.Parse(cond7, BattleConditionUtil.LeafFactory);

var ctx7_1 = new BattleContext { HP = 100, States = new() { "ready" } }; // ✅
var ctx7_2 = new BattleContext { IsStun = false, States = new() { "haste" } }; // ✅
var ctx7_3 = new BattleContext { HP = 90, IsStun = true, States = new() { "haste" } }; // ❌
bool r7_1 = node7.IsMatch(ctx7_1);
bool r7_2 = node7.IsMatch(ctx7_2);
bool r7_3 = node7.IsMatch(ctx7_3);






