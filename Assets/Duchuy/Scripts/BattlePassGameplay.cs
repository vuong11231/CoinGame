using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePassGameplay : MonoBehaviour {
    public static BattlePassCellData data;

    PlanetController attackPlanet = null;

    void Start() {
        GameManager.sunHp = DataGameSave.dataServer.sunHp;
        GameManager.planetSpeedRatio = 1f;
        GameManager.takeNoDame = false;
        SetUpData();
    }

    public void SetUpData() {
        if (GameManager.isFromRank) {
            InvokeRepeating("TryBossAttack", GameManager.BOSS_DELAY_ATTACK, GameManager.BOSS_ATTACK_RATE);
            GameManager.battlePassAttackMissRange = GameManager.BATTLE_PASS_ATTACK_MISS_RANGE_3;
        } else if (Scenes.LastScene == SceneName.Gameplay) {
            InvokeRepeating("TryBossAttack", GameManager.BOSS_DELAY_ATTACK, GameManager.BOSS_ATTACK_RATE);
            GameManager.battlePassAttackMissRange = 0;
        } else {
            if (data.difficulty == 1) {
                InvokeRepeating("TryBossAttack", GameManager.BOSS_DELAY_ATTACK, GameManager.BATTLE_PASS_ATTACK_RATE_1);
                DataGameSave.dataEnemy.sunHp *= GameManager.BATTLE_PASS_HP_INCREASE_1;
                GameManager.battlePassAttackMissRange = GameManager.BATTLE_PASS_ATTACK_MISS_RANGE_1;
            } else if (data.difficulty == 2) {
                InvokeRepeating("TryBossAttack", GameManager.BOSS_DELAY_ATTACK, GameManager.BATTLE_PASS_ATTACK_RATE_2);
                DataGameSave.dataEnemy.sunHp *= GameManager.BATTLE_PASS_HP_INCREASE_2;
                GameManager.battlePassAttackMissRange = GameManager.BATTLE_PASS_ATTACK_MISS_RANGE_2;
            } else if (data.difficulty == 3) {
                InvokeRepeating("TryBossAttack", GameManager.BOSS_DELAY_ATTACK, GameManager.BATTLE_PASS_ATTACK_RATE_3);
                DataGameSave.dataEnemy.sunHp *= GameManager.BATTLE_PASS_HP_INCREASE_3;
                GameManager.battlePassAttackMissRange = GameManager.BATTLE_PASS_ATTACK_MISS_RANGE_3;
            } else if (data.difficulty == 4) {
                InvokeRepeating("TryBossAttack", GameManager.BOSS_DELAY_ATTACK, GameManager.BATTLE_PASS_ATTACK_RATE_4);
                DataGameSave.dataEnemy.sunHp *= GameManager.BATTLE_PASS_HP_INCREASE_4;
                GameManager.battlePassAttackMissRange = GameManager.BATTLE_PASS_ATTACK_MISS_RANGE_4;
            } else if (data.difficulty == 5) {
                InvokeRepeating("TryBossAttack", GameManager.BOSS_DELAY_ATTACK, GameManager.BATTLE_PASS_ATTACK_RATE_5);
                DataGameSave.dataEnemy.sunHp *= GameManager.BATTLE_PASS_HP_INCREASE_5;
                GameManager.battlePassAttackMissRange = GameManager.BATTLE_PASS_ATTACK_MISS_RANGE_4;
            }
        }
    }

    public void TryBossAttack() {
        attackPlanet = null;

        List<SpaceController> spaces = SpaceEnemyManager.Instance.ListSpace;

        for (int i = 0; i < spaces.Count; i++) {

            spaces[i].Planet.attackDirect = true;

            if (spaces[i].Planet.Type != TypePlanet.Destroy && spaces[i].Planet != null && spaces[i].Planet.IEShoot == null && !spaces[i].Planet.isFlying) {
                attackPlanet = spaces[i].Planet;
                break;
            }
        }

        if (attackPlanet == null || attackPlanet.isFlying) {
            return;
        }

        attackPlanet.attackDirect = true;
        attackPlanet.owner = PlanetController.Owner.Enemy;
        attackPlanet.StartDrag(updateByPointPull: false);
        StartCoroutine(attackPlanet.IEShoot);
        PointPull.Instance.current = null;
    }
}
