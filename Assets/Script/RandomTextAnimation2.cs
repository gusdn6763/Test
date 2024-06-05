using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RandomTextAnimation2 : MonoBehaviour
{
    [SerializeField] private TextSeparator2 textSeparator;

    private List<TextSeparator2> separators = new List<TextSeparator2>();

    public bool IsAnimation { get; set; }

    private string divid;
    private float fontSize;
    private float moveSpeed;
    private float duration;
    private float durationReduce;
    private float xMax;
    private float yMax;

    public void Init(string str, float size, float speed, float dur, float durReduce, float x, float y)
    {
        divid = Seperate(str);
        fontSize = size;
        moveSpeed = speed;
        duration = dur;
        durationReduce = durReduce;
        xMax = x;
        yMax = y;
    }


    public void Active(bool isOn)
    {
        if (isOn)
        {
            IsAnimation = true;
            for (int i = 0; i < divid.Length; i++)
            {
                TextSeparator2 tmp = Instantiate(textSeparator, transform);
                tmp.Init(divid[i].ToString(), fontSize, moveSpeed, duration, durationReduce, xMax, yMax);
                tmp.IsOn = true;

                separators.Add(tmp);
            }
        }
        else
        {
            for (int i = 0; i < separators.Count; i++)
            {
                separators[i].IsOn = false;
                StartCoroutine(separators[i].ComeBack());
            }

            StartCoroutine(ComeBackCoroutine());
        }
    }
    private IEnumerator ComeBackCoroutine()
    {
        while (true)
        {
            if (transform.childCount == 0)
                break;

            yield return null;
        }
        IsAnimation = false;
    }

    public string Seperate(string data)
    {
        int a, b, c;//�ڼҹ��� �ʼ��߼�������
        string result = " ";//�и������ ����Ǵ� ���ڿ�
        int cnt;
        //�ѱ��� �����ڵ�
        // �� �� �� �� �� �� �� �� �� �� �� �� �� �� �� �� �� �� ��
        int[] ChoSung = { 0x3131, 0x3132, 0x3134, 0x3137, 0x3138, 0x3139, 0x3141, 0x3142, 0x3143, 0x3145, 0x3146, 0x3147, 0x3148, 0x3149, 0x314a, 0x314b, 0x314c, 0x314d, 0x314e };
        // �� �� �� �� �� �� �� �� �� �� �� �� �� �� �� �� �� �� �� �� ��
        int[] JwungSung = { 0x314f, 0x3150, 0x3151, 0x3152, 0x3153, 0x3154, 0x3155, 0x3156, 0x3157, 0x3158, 0x3159, 0x315a, 0x315b, 0x315c, 0x315d, 0x315e, 0x315f, 0x3160, 0x3161, 0x3162, 0x3163 };
        // �� �� �� �� �� �� �� �� �� �� �� �� �� �� �� �� �� �� �� �� �� �� �� �� �� �� ��
        int[] JongSung = { 0, 0x3131, 0x3132, 0x3133, 0x3134, 0x3135, 0x3136, 0x3137, 0x3139, 0x313a, 0x313b, 0x313c, 0x313d, 0x313e, 0x313f, 0x3140, 0x3141, 0x3142, 0x3144, 0x3145, 0x3146, 0x3147, 0x3148, 0x314a, 0x314b, 0x314c, 0x314d, 0x314e };

        int x;

        for (cnt = 0; cnt < data.Length; cnt++)
        {
            x = (int)data[cnt];

            //�ѱ��� ��츸 �и� ����
            if (x >= 0xAC00 && x <= 0xD7A3)
            {
                c = x - 0xAC00;
                a = c / (21 * 28);
                c = c % (21 * 28);
                b = c / 28;
                c = c % 28;
                /*
                a = (int)a;
                b = (int)b;
                c = (int)c;
                */
                result += string.Format("{0}{1}", (char)ChoSung[a], (char)JwungSung[b]);
                // $c�� 0�̸�, �� ��ħ�� �������
                if (c != 0)
                    result += string.Format("{0}", (char)JongSung[c]);
            }
            else
            {
                result += string.Format("{0}", (char)x);
            }
        }
        return result;
    }
}
