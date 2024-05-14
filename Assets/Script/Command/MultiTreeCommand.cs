using Febucci.UI;
using Febucci.UI.Core;
using Febucci.UI.Core.Parsing;
using Febucci.UI.Effects;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Febucci.UI.Actions;
using System.Linq;

public enum MouseStatus
{
    None,
    Enter,
    Down,
    Drag,
    Up,
    Excute,
    Exit,
}

[RequireComponent(typeof(AnimationHandler))]
public abstract class MultiTreeCommand : MonoBehaviour
{
    [SerializeField] protected AnimationData animationData;

    protected AnimationHandler animationHandler;
    protected ListMenu listMenu;

    protected BoxCollider box;
    protected TextMeshPro text;
    protected RectTransform rect; 
    protected Rigidbody rigi;

    protected Vector2 padding = new Vector2(2f, 0f);

    [Header("�Ҹ�")]
    [SerializeField] private Status status;
    public Status MyStatus { get => status; set => status = value; }

    [Header("��Ī")]

    [SerializeField] private string commandName;
    public string CommandName { get => commandName; private set => commandName = value; }

    [Header("���̱� ����")]

    [SerializeField] private bool isCondition = false;
    public virtual bool IsCondition { get { return isCondition; } set { isCondition = value; } }

    protected virtual void Awake()
    {
        listMenu = GetComponentInChildren<ListMenu>(true);
        initCircle = GetComponentInChildren<SpriteRenderer>(true);

        animationHandler = GetComponent<AnimationHandler>();
        text = GetComponent<TextMeshPro>();
        rect = GetComponent<RectTransform>();
        box = GetComponent<BoxCollider>();
        rigi = GetComponent<Rigidbody>();

        IsFirstAppearance = true;

        if (this is IRootCommand)
            box.isTrigger = false;
        else
            box.isTrigger = true;

        TryInitializing();
    }

    void TryInitializing()
    {
        if (initialized) 
            return;

        text.text = CommandName;
        initialized = true;
        charactersCount = 0;
        characters = new MyCharacterData[0];
        TextUtilities.Initialize();
        behaviors = new AnimationRegion[0];
        appearances = new AnimationRegion[0];
        disappearances = new AnimationRegion[0];

        TextAnimatorSettings.Instance.behaviors.defaultDatabase.ForceBuildRefresh();
        TextAnimatorSettings.Instance.appearances.defaultDatabase.ForceBuildRefresh();
        TextAnimatorSettings.Instance.actions.defaultDatabase.ForceBuildRefresh();

        PositionInitialize();
        HideAllCharactersTime();
    }
    protected virtual void OnEnable()
    {
        SetSize(GetSize());
    }
    public void FixedUpdate()
    {
        if (text.text.Equals(textWithoutTextAnimTags) == false)
        {
            ConvertText(text.text);
        }
        else
            AnimateText(Time.deltaTime);
    }

    protected virtual void OnDisable()
    {
        StopAllCoroutines();
        IsAppearanceStart = false;
        IsDisAppearanceStart = false;
        IsBehaviorStart = false;
    }

    protected void OnMouseEnter()
    {
        initCircle.enabled = false;
    }

    #region Ʈ�� ����
    private ListMenu listmenu;
    public ListMenu MyListMenu
    {
        get
        {
            if (listmenu == null)
            {
                listmenu = GetComponentInChildren<ListMenu>(true);

                if (listmenu == null)
                    Debug.LogError(name + "�߸��� Ʈ������");
            }

            return listmenu;
        }
    }

    private MultiTreeCommand parentCommand;
    public MultiTreeCommand ParentCommand
    {
        get
        {
            if (parentCommand == null)
                if (transform.parent)
                    return transform.parent.GetComponentInParent<MultiTreeCommand>(true);

            return null;
        }
    }

    private List<MultiTreeCommand> childCommands;
    public List<MultiTreeCommand> ChildCommands
    {
        get
        {
            if (childCommands == null)
                RefreshChildCommands();
            return childCommands;
        }
    }
    /// <summary>
    /// �ڽ� �ൿ�� ��˻�
    /// </summary>
    public void RefreshChildCommands()
    {
        childCommands = new List<MultiTreeCommand>();
        if (MyListMenu != null)
        {
            for (int i = 0; i < MyListMenu.transform.childCount; i++)
            {
                Transform childTransform = MyListMenu.transform.GetChild(i);
                MultiTreeCommand childCommand = childTransform.GetComponent<MultiTreeCommand>();
                if (childCommand)
                    childCommands.Add(childCommand);
            }
        }
    }

    /// <summary>
    /// �ֻ��� �ൿ ã��
    /// </summary>
    public MultiTreeCommand RootCommand
    {
        get
        {
            if (ParentCommand == null)
                return this;
            else
                return ParentCommand.RootCommand;
        }
    }

    /// <summary>
    /// ���� Ʈ�������� �ִ� �ൿ ���
    /// </summary>
    /// <param name="Command"></param>
    /// <returns></returns>
    public bool SameCommand(MultiTreeCommand Command)
    {
        if (Command == null)
            return false;

        if (Command == this)
            return true;

        if (IsChildCommand(Command) || IsParentCommand(Command))
            return true;

        return false;
    }

    /// <summary>
    /// ���� Ʈ�������� �ִ� ���� �ൿ ���
    /// </summary>
    /// <param name="Command">���� ���� �ൿ</param>
    /// <returns></returns>
    public bool IsSiblingCommand(MultiTreeCommand command)
    {
        if (command == null)
            return false;

        if (command == this)
            return false;

        if (ParentCommand == null || command.ParentCommand == null)
            return false;

        return ParentCommand == command.ParentCommand;
    }

    /// <summary>
    /// ���� Ʈ�������� �ִ� �ڽ� �ൿ���� üũ
    /// </summary>
    /// <param name="child"></param>
    /// <returns>�ڽ� �ൿ�� ���</returns>
    public bool IsChildCommand(MultiTreeCommand child)
    {
        MultiTreeCommand current = child;
        while (current != null)
        {
            if (current.ParentCommand == this)
                return true;

            current = current.ParentCommand;
        }
        return false;
    }

    /// <summary>
    /// ���� Ʈ�������� �ִ� �θ� �ൿ���� üũ
    /// </summary>
    /// <param name="parent"></param>
    /// <returns>�θ� �ൿ�� ��� true</returns>
    public bool IsParentCommand(MultiTreeCommand parent)
    {
        MultiTreeCommand current = ParentCommand;
        while (current != null)
        {
            if (current == parent)
                return true;
            current = current.ParentCommand;
        }
        return false;
    }

    /// <summary>
    /// �ڽ� �ൿ���� �ڽű��� ���������� ��Ȱ��ȭ.
    /// </summary>
    public void DisableAllCommandFromBottom()
    {
        foreach (MultiTreeCommand childCommand in ChildCommands)
            childCommand.DisableAllCommandFromBottom();

        SetChildrenActive(false);

        gameObject.SetActive(false);
    }
    private void DisableCommandRecursive(MultiTreeCommand command)
    {
        // �ڽ� ������Ʈ���� ���� ��Ȱ��ȭ
        foreach (MultiTreeCommand childCommand in command.ChildCommands)
            DisableCommandRecursive(childCommand);

        command.gameObject.SetActive(false);
    }

    /// <summary>
    /// �ڽ� �ൿ Ȱ��ȭ����
    /// </summary>
    /// <param name="isActive"></param>
    public virtual void SetChildrenActive(bool isActive)
    {
        foreach (MultiTreeCommand childCommand in ChildCommands)
            childCommand.gameObject.SetActive(isActive);
    }
    #endregion

    #region ��ȣ�ۿ�

    public void CommandFreeze(bool isOn)
    {
        if (isOn)
            rigi.constraints = RigidbodyConstraints.FreezeAll;
        else
            rigi.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
    }

    #region ������ ����
    public void TotalSizeCacluate()
    {
        float maxX = 0;
        float maxY = 0;
        Vector2 size;
        foreach (MultiTreeCommand childCommand in ChildCommands)
        {
            size = childCommand.GetSize();
            maxX = Mathf.Max(size.x, maxX);
            maxY = Mathf.Max(size.y, maxY);
        }
        size = new Vector2(maxX, maxY);
        foreach (MultiTreeCommand childCommand in ChildCommands)
            SetSize(size);
    }
    public Vector2 GetSize()
    {
        return new Vector2(text.preferredWidth, text.preferredHeight);
    }
    public void SetSize(Vector2 size)
    {
        rect.sizeDelta = new Vector2(size.x + padding.x, size.y + padding.y);
        box.size = new Vector3(size.x + padding.x, size.y + padding.y, 1);
    }

    #endregion
    #endregion

    #region ���콺 ��ȣ�ۿ�
    public Action<MouseStatus> onMouseEvent;
    public virtual void Interaction(MouseStatus mouseStatus)
    {
        onMouseEvent?.Invoke(mouseStatus);
        Player.instance.ShowPreviewStatus(status);
        StartCoroutine(animationHandler.AnimaionCoroutine(this, mouseStatus));
    }

    #endregion

    #region �ִϸ��̼�
    public Action<MouseStatus> onAnimationEndEvent;
    public bool IsAppearanceStart { get; set; }           //��������
    public bool IsDisAppearanceStart { get; set; }        //���������
    public bool IsBehaviorStart { get; set; }             //�ൿ�ִϸ��̼� ����
    public bool IsFirstAppearance { get; set; } = true;
    public bool IsLoop { get => currentBehavior.isLoop; }
    public float FontSize { get => text.fontSize; }

    private int charactersCount;
    public int CharactersCount { get => charactersCount; }

    private MyCharacterData[] characters;
    public MyCharacterData[] Characters { get => characters; }

    protected AppearanceAnimationScriptible currentAppearance;
    protected BehaviorAnimationScriptible currentBehavior;
    protected DisAppearanceAnimationScriptible currentDisAppearance;

    AnimationRegion[] behaviors;
    AnimationRegion[] appearances;
    AnimationRegion[] disappearances;

    private Dictionary<RectTransform, Vector3> childData = new Dictionary<RectTransform, Vector3>();

    private string textWithoutTextAnimTags = string.Empty;
    private bool initialized;                           //�ʱ� ������ ����
    private bool useDynamicScaling = true;              //��Ʈ ũ�⿡ ���� �ִϸ��̼� ��������
    private float referenceFontSize = 10;

    #region �ִϸ��̼� ���� �κ�
    public void Appearance()
    {
        gameObject.SetActive(true);
        StartCoroutine(AppearanceCoroutine());
    }
    IEnumerator AppearanceCoroutine()
    {
        IsAppearanceStart = true;

        currentAppearance = GetAppearanceTags();
        ConvertText(text.text);

        IsFirstAppearance = false;
       
        HideAllCharactersTime();
        PasteMeshToSource(characters);

        for (int i = 0; i < charactersCount; i++)
        {
            SetVisibilityChar(i, true);

            if (currentAppearance.waitForNormalChars != 0)
                yield return new WaitForSeconds(currentAppearance.waitForNormalChars);
        }

        for (int i = 0; i < charactersCount; i++)
        {
            if (characters[i].passedTime < characters[i].appearancesMaxDuration)
                i = 0;

            yield return null;
        }
        IsAppearanceStart = false;
        Behavior();
    }
    public void DisAppearance()
    {
        StartCoroutine(DisAppearanceCoroutine());
    }
    IEnumerator DisAppearanceCoroutine()
    {
        IsDisAppearanceStart = true;

        currentDisAppearance = GetDisAppearanceTags();
        ConvertText(text.text);

        PasteMeshToSource(characters);

        for (int i = 0; i < charactersCount; i++)
        {
            characters[i].passedTime = characters[i].disappearancesMaxDuration;
            SetVisibilityChar(i, false);
            if (currentAppearance.waitForNormalChars != 0)
                yield return new WaitForSeconds(currentDisAppearance.waitForNormalChars);
        }


        for (int i = 0; i < charactersCount; i++)
        {
            yield return new WaitUntil(() => characters[i].passedTime <= 0);
        }

        yield return new WaitUntil(() => ChildCommands.All(cmd => cmd.IsDisAppearanceStart == false));

        IsDisAppearanceStart = false;
    }

    private Coroutine behaviorCoroutine;
    public void Behavior()
    {
        if (behaviorCoroutine != null)
            StopCoroutine(behaviorCoroutine);

        behaviorCoroutine = StartCoroutine(BehaviorCoroutine());
    }
    IEnumerator BehaviorCoroutine()
    {
        IsBehaviorStart = true;

        for (int i = 0; i < charactersCount; i++)
            characters[i].SaveBeforePositions();

        currentBehavior = GetBehaviorTags();
        ConvertText(text.text);

        if (currentBehavior.isLoop)
        {
            while (true)
                yield return null;
        }
        else
        {
            for (int i = 0; i < charactersCount; i++)
            {
                if (characters[i].passedTime < characters[i].behaviorMaxDuration)
                    i = 0;

                yield return null;
            }
        }

        IsBehaviorStart = false;
    }
    public T GetAnimationTags<T>(List<T> animationList, Func<T, bool> predicate) where T : new()
    {
        ResetTimeAllCharacter();

        foreach (T animation in animationList)
        {
            if (predicate(animation))
                return animation;
        }

        return new T();
    }
    public virtual AppearanceAnimationScriptible GetAppearanceTags()
    {
        return GetAnimationTags(animationData.appearanceAnimation, animation =>
        {
            if (IsFirstAppearance)
                return animation.appearanceAnimationType == AppearanceAnimationType.First;
            else
                return animation.appearanceAnimationType == AppearanceAnimationType.Default;
        });
    }
    public virtual BehaviorAnimationScriptible GetBehaviorTags()
    {
        return GetAnimationTags(animationData.behaviorAnimation, animation => true);
    }
    public virtual DisAppearanceAnimationScriptible GetDisAppearanceTags()
    {
        return GetAnimationTags(animationData.disAppearanceAnimation, animation =>
        {
            return animation.disAppearanceAnimationType == DisAppearanceAnimationType.Default;
        });
    }

    public void Show(bool isOn)
    {
        text.enabled = isOn ? true : false;
    }
    #endregion

    void ConvertText(string textToParse)
    {
        if (textToParse is null) // prevents error along the method if text is passed as null
            textToParse = string.Empty;

        TextAnimatorSettings settings = TextAnimatorSettings.Instance;
        if (!settings)
        {
            charactersCount = 0;
            Debug.LogError("Text Animator Settings not found. Skipping setting the text to Text Animator.");
            return;
        }

        ActionDatabase databaseActions = TextAnimatorSettings.Instance.actions.defaultDatabase;
        AnimationsDatabase databaseBehaviors = TextAnimatorSettings.Instance.behaviors.defaultDatabase;
        AnimationsDatabase databaseAppearances = TextAnimatorSettings.Instance.appearances.defaultDatabase;

        var ruleBehavior = new AnimationParser<AnimationScriptableBase>(settings.behaviors.openingSymbol, '/', settings.behaviors.closingSymbol, VisibilityMode.Persistent, databaseBehaviors);
        var ruleAppearance = new AnimationParser<AnimationScriptableBase>(settings.appearances.openingSymbol, '/', settings.appearances.closingSymbol, VisibilityMode.OnVisible, databaseAppearances);
        var ruleDisappearance = new AnimationParser<AnimationScriptableBase>(settings.appearances.openingSymbol, '/', '#', settings.appearances.closingSymbol, VisibilityMode.OnHiding, databaseAppearances);
        ActionParser ruleActions = new ActionParser(settings.actions.openingSymbol, '/', settings.actions.closingSymbol, databaseActions);
        EventParser ruleEvents = new EventParser('<', '/', '>');

        //TODO optimize
        var parsers = new System.Collections.Generic.List<TagParserBase>()
            {
                ruleBehavior,
                ruleAppearance,
                ruleDisappearance,
                ruleActions,
                ruleEvents
            };

        foreach (var extraParser in new TagParserBase[1] { new TMPTagParser(text.richText, '<', '/', '>') })
        {
            parsers.Add(extraParser);
        }

        //Convert text in tags, mesh etc.
        textWithoutTextAnimTags = TextParser.ParseText(textToParse, parsers.ToArray());
        text.text = textWithoutTextAnimTags;
        //Set converted text to source
        text.ForceMeshUpdate(true);
        text.renderMode = TextRenderFlags.DontRender;

        charactersCount = text.textInfo.characterCount;

        //Assigns results
        behaviors = ruleBehavior.results;
        appearances = ruleAppearance.results;
        disappearances = ruleDisappearance.results;

        //Adds fallback effects to characters that have no effect assigned
        AddFallbackEffectsFor(ref behaviors, VisibilityMode.Persistent, databaseBehaviors, currentBehavior.animationString);
        AddFallbackEffectsFor(ref appearances, VisibilityMode.OnVisible, databaseAppearances, currentAppearance.animationString);
        AddFallbackEffectsFor(ref disappearances, VisibilityMode.OnHiding, databaseAppearances, currentDisAppearance.animationString);

        //Initializes only animations that are being used
        foreach (var behavior in behaviors) behavior.animation.InitializeOnce();
        foreach (var appearance in appearances) appearance.animation.InitializeOnce();
        foreach (var disappearance in disappearances) disappearance.animation.InitializeOnce();

        //Prepares Characters
        PopulateCharacters(true);
        CopyMeshFromSource(ref characters); //�ʱ� ������ ����
    }
    void AnimateText(float deltaTime)
    {
        if (IsAppearanceStart || IsDisAppearanceStart || IsBehaviorStart)
        {
            UpdateUniformIntensity();

            for (int i = 0; i < characters.Length; i++)
            {
                characters[i].ResetAnimation();

                if (characters[i].isVisible)
                    characters[i].passedTime += deltaTime;
                else
                    characters[i].passedTime -= deltaTime;

                if (characters[i].passedTime <= 0) // "<=" to force hiding characters when TimeScale = 0 
                {
                    characters[i].passedTime = 0;
                    characters[i].Hide();
                }
            }

            if (IsAppearanceStart)
                ProcessAnimationRegions(appearances);

            if (IsDisAppearanceStart)
                ProcessAnimationRegions(disappearances);

            if (IsBehaviorStart)
            {
                ProcessAnimationRegions(behaviors);

                if(this is IRootCommand)
                    ChildObjectMove();
            }

            //updates source -> �޽������� ������Ʈ�� ����
            PasteMeshToSource(characters);
        }
    }

    #region ���� �Լ�
    public void PositionInitialize()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            RectTransform child = transform.GetChild(i).GetComponent<RectTransform>();
            if (child != null)
                childData[child] = child.anchoredPosition;
        }
    }
    void UpdateUniformIntensity()
    {
        if (useDynamicScaling)
        {
            for (int i = 0; i < characters.Length; i++)
            {
                // multiplies by current character size, which could be modified by "size" tags and so
                // be different than the basic tmp font size value 
                characters[i].UpdateIntensity(referenceFontSize);
            }
        }
        else
        {
            for (int i = 0; i < characters.Length; i++)
            {
                characters[i].uniformIntensity = 1;
            }
        }
    }
    void ProcessAnimationRegions(AnimationRegion[] regions)
    {
        foreach (var region in regions)
        {
            foreach (var range in region.ranges)
            {
                region.SetupContextFor(range.modifiers); //TODO index instead of passing modifier by value

                for (int i = range.indexes.x; i < range.indexes.y && i < charactersCount; i++)
                {
                    if (characters[i].passedTime < 0)
                        continue;
                    

                    if (!region.IsVisibilityPolicySatisfied(characters[i].isVisible)) 
                        continue;

                    if (region.animation.CanApplyEffectTo(characters[i], rect))
                    {
                        if (region.animation.IsBehaviorDuration())
                            region.animation.ApplyEffectToBehaviorDuration(ref characters[i], rect);
                        else
                            region.animation.ApplyEffectTo(ref characters[i], rect);
                    }
                }
            }
        }
    }
    protected void CopyMeshFromSource(ref MyCharacterData[] characters)
    {
        TMP_CharacterInfo currentCharInfo;

        //Updates the characters sources
        for (int i = 0; i < characters.Length; i++)
        {
            currentCharInfo = text.textInfo.characterInfo[i];
            characters[i].isVisible = currentCharInfo.isVisible;
            characters[i].info.character = currentCharInfo.character;
            //Updates TMP char info
            //characters[i].current.tmp_CharInfo = textInfo.characterInfo[i];

            //Copies source data from the mesh info only if the character is valid, otherwise its vertices array will be null and tAnim will start throw errors
            if (currentCharInfo.isVisible == false) 
                continue;

            characters[i].info.pointSize = currentCharInfo.pointSize;

            //Updates vertices
            for (byte k = 0; k < TextUtilities.verticesPerChar; k++)
            {
                characters[i].source.positions[k] = text.textInfo.meshInfo[currentCharInfo.materialReferenceIndex].vertices[currentCharInfo.vertexIndex + k];
            }

            //Updates colors
            for (byte k = 0; k < TextUtilities.verticesPerChar; k++)
            {
                characters[i].source.colors[k] = text.textInfo.meshInfo[currentCharInfo.materialReferenceIndex].colors32[currentCharInfo.vertexIndex + k];
            
            }
        }
    }
    void PasteMeshToSource(MyCharacterData[] characters)
    {
        TMP_CharacterInfo currentCharInfo;

        //Updates the mesh
        for (int i = 0; i < charactersCount; i++)
        {
            currentCharInfo = text.textInfo.characterInfo[i];
            //Avoids updating if we're on an invisible character, like a spacebar
            //Do not switch this with "i<visibleCharacters", since the plugin has to update not yet visible characters
            if (!currentCharInfo.isVisible) 
                continue;

            //Updates TMP char info
            //textInfo.characterInfo[i] = characters[i].data.tmp_CharInfo;

            //Updates vertices
            for (byte k = 0; k < TextUtilities.verticesPerChar; k++)
            {
                text.textInfo.meshInfo[currentCharInfo.materialReferenceIndex].vertices[currentCharInfo.vertexIndex + k] = characters[i].current.positions[k];
            }

            //Updates colors
            for (byte k = 0; k < TextUtilities.verticesPerChar; k++)
            {
                text.textInfo.meshInfo[currentCharInfo.materialReferenceIndex].colors32[currentCharInfo.vertexIndex + k] = characters[i].current.colors[k];
            }
        }
        text.UpdateVertexData();
    }
    void HideAllCharactersTime()    //��� �ؽ�Ʈ ����
    {
        for (int i = 0; i < charactersCount; i++)
        {
            var c = characters[i];
            c.isVisible = false;
            c.passedTime = 0;
            c.Hide();
            characters[i] = c;
        }
    }
    void ResetTimeAllCharacter()
    {
        for (int i = 0; i < charactersCount; i++)
        {
            characters[i].passedTime = 0;
        }
    }
    void PopulateCharacters(bool resetVisibility)
    {
        if (characters.Length < charactersCount)
            Array.Resize(ref characters, charactersCount);

        for (int i = 0; i < charactersCount; i++)
        {
            //--Resets info--
            characters[i].ResetInfo(i, resetVisibility);

            //--Assigns effect times--
            float CalculateRegionMaxDuration(AnimationRegion[] tags)
            {
                float maxDuration = 0;
                float currentDuration;

                foreach (var tag in tags)
                {
                    foreach (var range in tag.ranges)
                    {
                        if (i >= range.indexes.x && i < range.indexes.y)
                        {
                            tag.SetupContextFor(range.modifiers);

                            currentDuration = tag.animation.GetMaxDuration();

                            if (currentDuration > maxDuration)
                            {
                                //Assigns the new max
                                maxDuration = currentDuration;
                            }
                        }
                    }
                }
                return maxDuration;
            }

            characters[i].disappearancesMaxDuration = CalculateRegionMaxDuration(disappearances);
            characters[i].appearancesMaxDuration = CalculateRegionMaxDuration(appearances);
            characters[i].behaviorMaxDuration = CalculateRegionMaxDuration(behaviors);
        }
    }
    bool IsCharacterInsideAnyEffect(int charIndex, AnimationRegion[] regions)
    {
        foreach (var region in regions)
        {
            foreach (var range in region.ranges)
            {
                if (charIndex >= range.indexes.x && (range.indexes.y == int.MaxValue || charIndex < range.indexes.y))
                {
                    return true;
                }
            }
        }

        return false;
    }
    void AddFallbackEffectsFor<T>(ref AnimationRegion[] currentEffects, VisibilityMode visibilityMode, Database<T> database, string[] defaultEffectsTags) where T : AnimationScriptableBase
    {
        if (!database) return;

        if (defaultEffectsTags == null || defaultEffectsTags.Length == 0)
        {
            return;
        }

        //create list of default regions that should be added
        var defaultRegions = new System.Collections.Generic.List<DefaultRegion>();
        string[] tagWords;
        string tagName;
        foreach (var tag in defaultEffectsTags)
        {
            if (string.IsNullOrEmpty(tag))
            {
                if (Application.isPlaying)
                    Debug.LogError($"Empty tag as default effect in database {database.name}. Skipping.", gameObject);
                continue;
            }

            tagWords = tag.Split(' ');
            tagName = tagWords[0];

            if (!database.ContainsKey(tagName))
            {
                if (Application.isPlaying)
                    Debug.LogError($"Fallback effect with tag '{tagName}' not found in database {database.name}. Skipping.", gameObject);
                continue;
            }

            defaultRegions.Add(new DefaultRegion(tagName, visibilityMode, database[tagName], tagWords));
        }

        //if there are no current effects, directly adds the default effects
        if (currentEffects.Length == 0)
        {
            foreach (var element in defaultRegions)
            {
                element.region.OpenNewRange(0, element.tagWords);
            }
        }
        else
        {
            //for every character in the text
            for (int startIndex = 0; startIndex < charactersCount; startIndex++)
            {
                //if the character has no effect of this category assigned
                if (!IsCharacterInsideAnyEffect(startIndex, currentEffects))
                {
                    //opens new range for default effects
                    foreach (var element in defaultRegions)
                    {
                        //add the default effect to the character
                        //TODO performance can be improved by caching modifiers
                        element.region.OpenNewRange(startIndex, element.tagWords);
                    }

                    //until there are characters that are not inside this category
                    int endIndex = startIndex + 1;
                    for (; endIndex < charactersCount; endIndex++)
                    {
                        if (IsCharacterInsideAnyEffect(endIndex, currentEffects))
                        {
                            break;
                        }
                    }

                    //closes new range for default effects
                    foreach (var element in defaultRegions)
                    {
                        element.region.TryClosingRange(endIndex);
                    }

                    startIndex = endIndex;
                }
            }
        }

        //adds the default regions to the current effects
        int prevCount = currentEffects.Length;
        System.Array.Resize(ref currentEffects, currentEffects.Length + defaultRegions.Count);
        for (int i = 0; i < defaultRegions.Count; i++)
        {
            currentEffects[prevCount + i] = defaultRegions[i].region;
        }
    }
    public void ChildObjectMove()
    {
        int count = 0;
        Vector3 middlePos = Vector3.zero;
        for (int i = 0; i < charactersCount && i < characters.Length; i++)
        {
            if (characters[i].passedTime <= 0)
            {
                characters[i].passedTime = 0;
                characters[i].Hide();
                continue;
            }

            middlePos += characters[i].current.positions.GetMiddlePos();
            count++;
        }
        middlePos /= count;

        if (count != 0)
        {
            foreach (var kvp in childData)
                kvp.Key.anchoredPosition = kvp.Value + new Vector3(0, middlePos.y, 0);

            box.center = new Vector3(box.center.x, middlePos.y, box.center.z);
        }
    }
    public void SetVisibilityChar(int index, bool isVisible)
    {
        if (index < 0 || index >= charactersCount) 
            return;
        characters[index].isVisible = isVisible;
    }
    #endregion

    #endregion

    #region ������ ��
    public SpriteRenderer initCircle { get; set; }
    public bool IsFirstShow { get; set; } = true;
    public bool IsInitCircleEnabled
    {
        set
        {
            initCircle.enabled = value;
            if (ParentCommand != null)
                ParentCommand.IsInitCircleEnabled = value;
        }
    }
    #endregion
}