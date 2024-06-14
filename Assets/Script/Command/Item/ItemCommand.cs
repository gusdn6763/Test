using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class ItemCommand : MultiTreeCommand
{
    [SerializeField] private float weight = 1;
    public float Weight { get { return weight; } }
    [SerializeField] private int stackSize = 1;
    public int StackSize { get { return stackSize; } set { stackSize = value; } }


    [SerializeField] private List<MultiTreeCommand> commands = new List<MultiTreeCommand>();
    protected override void Start()
    {
        base.Start();

        for (int i = 0; i < commands.Count; i++)
            commands[i].gameObject.SetActive(false);
    }

    public override void Interaction(MouseStatus mouseStatus)
    {
        if (mouseStatus == MouseStatus.Down)
            Physics.IgnoreCollision(collisionBox, GameManager.instance.bag, true);
        else if (mouseStatus == MouseStatus.Up)
        {
            Collider[] colliders = Physics.OverlapBox(transform.position, collisionBox.size / 2f);

            for(int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i] == GameManager.instance.bag)
                {
                    InventoryArea.instance.SetItem(this, true);
                }
            }
            Physics.IgnoreCollision(collisionBox, GameManager.instance.bag, false);

        }
        base.Interaction(mouseStatus);
    }

    public void Get()
    {

    }

    public void Throw()
    {
        Destroy(gameObject);
    }
}
