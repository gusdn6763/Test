using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MultiTreeData", menuName = "Data/MultiTreeData", order = 1)]
public class MultiTreeData : ScriptableObject
{
    [Header("이름")]
    public string commandName;

    [Header("소모값")]
    public Status status;

    [Header("보여주기 조건")]
    public bool isCondition;
}
