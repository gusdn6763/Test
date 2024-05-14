using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MultiTreeData", menuName = "Data/MultiTreeData", order = 1)]
public class MultiTreeData : ScriptableObject
{
    [Header("�̸�")]
    public string commandName;

    [Header("�Ҹ�")]
    public Status status;

    [Header("�����ֱ� ����")]
    public bool isCondition;
}
