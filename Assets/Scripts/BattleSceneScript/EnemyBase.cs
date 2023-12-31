using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnemyBase : ScriptableObject
{
    [SerializeField] private string enemyName;

    [TextArea]
    [SerializeField] private string description;

    // モデル
    [SerializeField] private GameObject enemyPrefab;

    // ステータス
    [SerializeField] private int maxHp;

    [SerializeField] private int atk;// 力
    [SerializeField] private int magicPower;// 魔
    [SerializeField] private int def;// 耐
    [SerializeField] private int agi;// 速
    [SerializeField] private int luck; // 運

    // 覚える技一覧
    [SerializeField] private List<LearnableSkill> learnableEnemySkills;

    public int MaxHp { get => maxHp; }
    public int Agi { get => agi; }
    public int Atk { get => atk; }
    public int Def { get => def; }
    public int MagicPower { get => magicPower; }
    public int Luck { get => luck; }
    public List<LearnableSkill> LearableEnemySkills { get => learnableEnemySkills; }
    public string EnemyName { get => enemyName; }
    public string Description { get => description; }
    public GameObject EnemyPrefab { get => enemyPrefab; }
}

// 覚える技：どのレベルで何を覚えるのか
[Serializable]
public class LearnableSkill
{
    [SerializeField] private SkillBase skillBase;
    [SerializeField] private int level;

    public SkillBase SkillBase { get => skillBase; }
    public int Level { get => level; }
}