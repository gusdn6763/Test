using Febucci.UI;
using Febucci.UI.Core;
using Febucci.UI.Core.Parsing;
using Febucci.UI.Effects;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Febucci.UI.Core.TAnimCore;
using Febucci.UI.Actions;
using UnityEngine.UIElements;
using static Unity.Burst.Intrinsics.X86.Avx;
using System.Threading;

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

public abstract class MultiTreeCommand : MonoBehaviour
{
    [SerializeField] protected MultiTreeData multiTreeData;
    [SerializeField] protected AnimationData animationData;

    protected ListMenu listMenu;

    protected BoxCollider box;
    protected TextMeshPro text;
    protected RectTransform rect; 
    protected Rigidbody rigi;

    protected Vector2 padding = new Vector2(2f, 0f);
    public string CommandName { get; private set; }
    public Status MyStatus { get; set; }
    protected bool isCondition = false;
    public virtual bool IsCondition { get { return isCondition; } set { isCondition = value; } }

    protected virtual void Awake()
    {
        listMenu = GetComponentInChildren<ListMenu>();

        text = GetComponent<TextMeshPro>();
        rect = GetComponent<RectTransform>();
        box = GetComponent<BoxCollider>();
        rigi = GetComponent<Rigidbody>();

        IsFirstAppearance = true;

        ScritibleInit();
        TryInitializing();
    }
    protected virtual void ScritibleInit()
    {
        CommandName = multiTreeData.commandName;
        MyStatus = multiTreeData.status;
        isCondition = multiTreeData.isCondition;
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
    }

    private void OnEnable()
    {
        SetSize(GetSize());
    }

    private void Update()
    {
        if (text.text.Equals(textWithoutTextAnimTags) == false)
        {
            ConvertText(text.text);
            if (appearanceStart)
                HideAllCharactersTime();
        }
        else
            AnimateText(Time.deltaTime);
    }


    #region 트리 관련
    private ListMenu listmenu;
    public ListMenu MyListMenu
    {
        get
        {
            if (listmenu == null)
            {
                listmenu = GetComponentInChildren<ListMenu>(true);

                if (listmenu == null)
                    Debug.LogError(name + "잘못된 트리구조");
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
    /// 자식 행동들 재검색
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
    /// 최상위 행동 찾기
    /// </summary>
    public MultiTreeCommand GetRootCommand
    {
        get
        {
            if (ParentCommand == null)
                return this;
            else
                return ParentCommand.GetRootCommand;
        }
    }

    /// <summary>
    /// 같은 트리구조에 있는 행동 경우
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
    /// 같은 트리구조에 있는 형제 행동 경우
    /// </summary>
    /// <param name="Command">같은 형제 행동</param>
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
    /// 같은 트리구조에 있는 자식 행동인지 체크
    /// </summary>
    /// <param name="child"></param>
    /// <returns>자식 행동인 경우</returns>
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
    /// 같은 트리구조에 있는 부모 행동인지 체크
    /// </summary>
    /// <param name="parent"></param>
    /// <returns>부모 행동인 경우 true</returns>
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
    /// 자식 행동부터 자신까지 순차적으로 비활성화.
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
        // 자식 오브젝트들을 먼저 비활성화
        foreach (MultiTreeCommand childCommand in command.ChildCommands)
            DisableCommandRecursive(childCommand);

        command.gameObject.SetActive(false);
    }

    /// <summary>
    /// 자식 행동 활성화여부
    /// </summary>
    /// <param name="isActive"></param>
    public virtual void SetChildrenActive(bool isActive)
    {
        foreach (MultiTreeCommand childCommand in ChildCommands)
            childCommand.gameObject.SetActive(isActive);
    }
    #endregion

    #region 사이즈 조절
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

    #region 마우스 상호작용
    public abstract void Excute();

    public Action<MouseStatus> onMouseEvent;
    public abstract void Interaction(MouseStatus mouseStatus);
    #endregion

    #region 애니메이션
    private Dictionary<RectTransform, Vector3> childData = new Dictionary<RectTransform, Vector3>();

    [Header("생성애니메이션 시작")]
    [SerializeField] protected bool appearanceStart;

    private bool initialized;                           //초기 데이터 설정
    private bool useDynamicScaling = true;              //폰트 크기에 따른 애니메이션 동적변경
    private float referenceFontSize = 10;

    public bool IsAppearanceStart { get; set; }           //생성시작
    public bool IsDisAppearanceStart { get; set; }        //사라짐시작
    public bool IsBehaviorStart { get; set; }             //행동애니메이션 시작

    string textWithoutTextAnimTags = string.Empty;
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

    string[] defaultAppearancesTags;
    string[] defaultBehaviorsTags;
    string[] defaultDisappearancesTags;

    public bool IsFirstAppearance { get; set; } = true;

    public void Appearance()
    {
        gameObject.SetActive(true);
        StartCoroutine(AppearanceCoroutine());
    }
    IEnumerator AppearanceCoroutine()
    {
        IsAppearanceStart = true;
        GetTagsData();
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
            if (characters[i].appearanceTime < characters[i].appearancesMaxDuration)
                i = 0;

            yield return null;
        }
        IsAppearanceStart = false;
    }
    public void DisAppearance()
    {
        gameObject.SetActive(true);
        StartCoroutine(DisAppearanceCoroutine());
    }
    IEnumerator DisAppearanceCoroutine()
    {
        IsDisAppearanceStart = true;

        GetTagsData();
        ConvertText(text.text);

        PasteMeshToSource(characters);

        for (int i = 0; i < charactersCount; i++)
        {
            characters[i].appearanceTime = characters[i].disappearancesMaxDuration;
            SetVisibilityChar(i, false);
            if (currentAppearance.waitForNormalChars != 0)
                yield return new WaitForSeconds(currentDisAppearance.waitForNormalChars);
        }


        for (int i = 0; i < charactersCount; i++)
        {
            if (characters[i].appearanceTime > 0)
                i = 0;

            yield return null;
        }
        
        IsDisAppearanceStart = false;
        gameObject.SetActive(false);
    }
    public void Behavior()
    {
        StartCoroutine(BehaviorCoroutine());
    }
    IEnumerator BehaviorCoroutine()
    {
        IsBehaviorStart = true;

        GetTagsData();
        ConvertText(text.text);

        for (int i = 0; i < charactersCount; i++)
        {
            if (characters[i].passedTime > characters[i].behaviorMaxDuration)
                i = 0;

            yield return null;
        }

        IsBehaviorStart = false;
    }
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
        AddFallbackEffectsFor(ref behaviors, VisibilityMode.Persistent, databaseBehaviors, defaultBehaviorsTags);
        AddFallbackEffectsFor(ref appearances, VisibilityMode.OnVisible, databaseAppearances, defaultAppearancesTags);
        AddFallbackEffectsFor(ref disappearances, VisibilityMode.OnHiding, databaseAppearances, defaultDisappearancesTags);

        //Initializes only animations that are being used
        foreach (var behavior in behaviors) behavior.animation.InitializeOnce();
        foreach (var appearance in appearances) appearance.animation.InitializeOnce();
        foreach (var disappearance in disappearances) disappearance.animation.InitializeOnce();

        //Prepares Characters
        PopulateCharacters(true);
        CopyMeshFromSource(ref characters); //초기 데이터 세팅
    }
    void AnimateText(float deltaTime)
    {
        UpdateUniformIntensity();

        if (IsAppearanceStart)
        {
            for (int i = 0; i < characters.Length; i++)
            {
                characters[i].ResetAnimation();

                if (characters[i].isVisible)
                    characters[i].appearanceTime += deltaTime;
            }
            ProcessAnimationRegions(appearances, false);
        }
        else if (IsDisAppearanceStart)
        {
            for (int i = 0; i < characters.Length; i++)
            {
                characters[i].ResetAnimation();

                if (characters[i].isVisible == false)
                    characters[i].appearanceTime -= deltaTime;

                if (characters[i].appearanceTime <= 0) // "<=" to force hiding characters when TimeScale = 0 
                {
                    characters[i].appearanceTime = 0;
                    characters[i].Hide();
                }
            }
            ProcessAnimationRegions(disappearances, false);
        }
        else
        {
            for (int i = 0; i < characters.Length; i++)
            {
                characters[i].ResetAnimation();

                if (characters[i].isVisible)
                    characters[i].passedTime += deltaTime;
            }
            ProcessAnimationRegions(behaviors, true);
            ChildObjectMove();
        }

        //updates source -> 메쉬데이터 컴포넌트로 전달
        PasteMeshToSource(characters);
    }

    public virtual void GetTagsData()
    {
        ResetTimeAllCharacter();

        for (int i = 0; i < animationData.appearanceAnimation.Count; i++)
        {
            AppearanceAnimationScriptible tmp = animationData.appearanceAnimation[i];
            if (IsFirstAppearance)
            {
                if (tmp.appearanceAnimationType == AppearanceAnimationType.First)
                    currentAppearance = tmp;
            }
            else
            {
                if (tmp.appearanceAnimationType == AppearanceAnimationType.Default)
                    currentAppearance = tmp;
            }
        }

        for (int i = 0; i < animationData.behaviorAnimation.Count; i++)
        {
            BehaviorAnimationScriptible tmp = animationData.behaviorAnimation[i];
            if (tmp.behaviorAnimationType == BehaviorAnimationType.Default)
                currentBehavior = tmp;
        }

        for (int i = 0; i < animationData.disAppearanceAnimation.Count; i++)
        {
            DisAppearanceAnimationScriptible tmp = animationData.disAppearanceAnimation[i];
            if (tmp.disAppearanceAnimationType == DisAppearanceAnimationType.Default)
                currentDisAppearance = tmp;
        }

        defaultAppearancesTags = currentAppearance.animationString;
        defaultBehaviorsTags = currentBehavior.animationString;
        defaultDisappearancesTags = currentDisAppearance.animationString;
    }

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
    void ProcessAnimationRegions(AnimationRegion[] regions, bool isBehavior)
    {
        foreach (var region in regions)
        {
            foreach (var range in region.ranges)
            {
                region.SetupContextFor(range.modifiers); //TODO index instead of passing modifier by value

                for (int i = range.indexes.x; i < range.indexes.y && i < charactersCount; i++)
                {
                    if (isBehavior)
                    {
                        if (characters[i].passedTime < 0)
                            continue;
                    }
                    else
                    {
                        if (characters[i].appearanceTime < 0)
                            continue;
                    }

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
                characters[i].init.positions = characters[i].source.positions;
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
    void HideAllCharactersTime()    //모든 텍스트 숨김
    {
        ResetTimeAllCharacter();
        for (int i = 0; i < charactersCount; i++)
        {
            var c = characters[i];
            c.isVisible = false;
            c.Hide();
            characters[i] = c;
        }
    }
    void ResetTimeAllCharacter()
    {
        for (int i = 0; i < charactersCount; i++)
        {
            characters[i].passedTime = 0;
            characters[i].appearanceTime = 0;
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
            if (characters[i].isVisible == false)
            {
                characters[i].passedTime = 0;
                characters[i].Hide();
                continue;
            }

            middlePos += characters[i].current.positions.GetMiddlePos();
            count++;
        }
        middlePos /= count;

        foreach (var kvp in childData)
            kvp.Key.anchoredPosition = kvp.Value + new Vector3(0, middlePos.y, 0);
        
        box.center = new Vector3(box.center.x, middlePos.y, box.center.z);
    }
    public void SetVisibilityChar(int index, bool isVisible)
    {
        if (index < 0 || index >= charactersCount) 
            return;
        characters[index].isVisible = isVisible;
    }

    #endregion

    #region VillageCommand
    public virtual bool IsInitCircleEnabled { get; set; }
    #endregion
}