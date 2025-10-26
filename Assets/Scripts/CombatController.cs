using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CombatController : MonoBehaviour
{

    [Header("Flat Defense Settings")]
    [Tooltip("Multiplier applied to Strength/Magic when computing flat defense.")]
    public float flatDefenseMultiplier = 0.5f;

    [Header("Percent Defense Settings")]
    [Tooltip("Controls how strongly % defenses stack (lower = stronger stacking).")]
    public float defenseSoftness = 2f; // 1 = linear, 2 = soft curve, 3+ = stronger soft cap

    [Tooltip("Maximum achievable % reduction (after soft cap).")]
    [Range(0f, 0.99f)]
    public float maxReduction = 0.85f;

    [Header("Shield Settings")]
    [Tooltip("Multiplier for shield reduction effectiveness (0.5 = half effective).")]
    [Range(0f, 1f)]
    public float shieldEfficiency = 1.0f;

    [Tooltip("How much shield absorbs before breaking.")]
    public float shieldHpFactor = 1.0f;

    [Header("Damage Randomness")]
    [Range(0f, 0.5f)]
    public float damageVariance = 0.1f;

    [Header("Critical Hits")]
    [Range(0f, 1f)] public float critChance = 0.1f;
    [Range(1f, 3f)] public float critMultiplier = 1.5f;

    [Header("Music")]
    [SerializeField] private AudioClip[] battleMusicClips;
    [SerializeField] private AudioClip victoryMusic;
    [SerializeField] private AudioClip defeatMusic;

    [Header("References")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform player1Position, player2Position, enemy1Position, enemy2Position;

    [Header("UI References")]
    [SerializeField] private GameObject pleaseWait;
    [SerializeField] private GameObject buttons;
    [SerializeField] private GameObject introScreen;
    [SerializeField] private Text introTitle, introLevel;
    [SerializeField] private CombatEndScreenController endScreen;
    [SerializeField] private CanvasGroup introEffect;
    [SerializeField] private Text manatext, maxmanatext;
    [SerializeField] private Text lifepotionstext, manapotionstext;
    [SerializeField] private Slider manaslider;
    [SerializeField] private AnimatedImage manabaranimation;
    [SerializeField] private CharacterMonitor playerMonitor, allyMonitor, enemy1Monitor, enemy2Monitor;
    [SerializeField] private DamageNumberDisplayController damageNumberDisplay;
    [SerializeField] private SpriteRenderer marker1, marker2;
    [SerializeField] private GameObject trainingInfo;
    [SerializeField] private Text currentEng, expGain;
    [SerializeField] private Slider engBar;

    private bool isAnimating = false;
    private static bool isTraining = false;
    private int trainingXp = 0;
    private AudioSource audioSource;

    // Globals
    class Combatant { public Character ch; public GameObject go; public Animator animator; }
    private static Combatant player, ally, enemy1, enemy2, target;
    private static Stage currentStage;

    public bool IsAnimating
    {
        get => isAnimating; set
        {
            isAnimating = value;
            pleaseWait.SetActive(isAnimating);
            buttons.SetActive(!isAnimating);
            if (!isAnimating) // animation has finished, trigger end screen now if needed
                CheckEnd();
        }
    }
    public static void InitTraining(int level)
    {
        currentStage = Resources.Load<Stage>("Stages/Training");
        currentStage.currentLevel = level;
        InitCombat(currentStage);
        isTraining = true;
    }
    public static void InitCombat(Stage stage)
    {
        currentStage = stage;

        isTraining = false;
        player = new Combatant();
        player.ch = GameController.PlayerChar;
        ally = null; // always start without ally

        Character[] enemys = currentStage.GetCurrentStageCharacters();
        Character enemy1Character = enemys[0];
        Character enemy2Character = enemys[1];

        // Reset characters
        // For player only reset shield hp, take over remaining stats from world
        GameController.PlayerChar.currentShieldHp = GameController.Shield != null ? GameController.Shield.shieldHP : 0;
        if (enemy1Character)
        {
            enemy1 = new Combatant();
            enemy1.ch = enemy1Character;
            enemy1.ch.life = enemy1.ch.maxlife;
            enemy1.ch.mana = enemy1.ch.maxmana;
            enemy1.ch.currentShieldHp = enemy1.ch.shield != null ? enemy1.ch.shield.shieldHP : 0;
        }
        if (enemy2Character)
        {
            enemy2 = new Combatant();
            enemy2.ch = enemy2Character;
            enemy2.ch.life = enemy2.ch.maxlife;
            enemy2.ch.mana = enemy2.ch.maxmana;
            enemy2.ch.currentShieldHp = enemy2.ch.shield != null ? enemy2.ch.shield.shieldHP : 0;
        }
    }

    public void Start()
    {
        // Just for debugging!
        //GameController.LoadAllPrefs();
        //InitCombat(Resources.Load<Character>("Character/Thief"), Resources.Load<Character>("Character/Thief 1"));
        //trainingXp = 200;

        audioSource = GetComponent<AudioSource>();
        PlayRandomMusic();
        StartCoroutine(PlayIntro());
        // Player must always be there
        playerMonitor.MChar = player.ch;
        player.go = Instantiate(playerPrefab, player1Position);
        player.animator = player.go.GetComponentInChildren<Animator>();

        // Is an ally there?
        if (ally != null)
        {
            ally.go = Instantiate(ally.ch.combatprefab, player2Position);
            ally.animator = ally.go.GetComponentInChildren<Animator>();
            allyMonitor.MChar = ally.ch;
        }
        else
        {
            player2Position.gameObject.SetActive(false);
            allyMonitor.MChar = null;
        }

        // instantiate first enemy if available
        if (enemy1 != null)
        {
            enemy1.go = Instantiate(enemy1.ch.combatprefab, enemy1Position);
            enemy1Monitor.MChar = enemy1.ch;
            enemy1.animator = enemy1.go.GetComponentInChildren<Animator>();
            SetTarget(0);
        }
        else
        {
            enemy1Position.gameObject.SetActive(false);
            enemy1Monitor.MChar = null;
        }
        // instantiate second enemy if available
        if (enemy2 != null)
        {
            enemy2.go = Instantiate(enemy2.ch.combatprefab, enemy2Position);
            enemy2Monitor.MChar = enemy2.ch;
            enemy2.animator = enemy2.go.GetComponentInChildren<Animator>();
            SetTarget(1);
        }
        else
        {
            enemy2Position.gameObject.SetActive(false);
            enemy2Monitor.MChar = null;
        }
        UpdateTexts();
    }

    public void UpdateTexts()
    {
        float currentMana = (int)(GameController.Mana);
        float maxMana = (int)(GameController.MaxMana);
        manatext.text = currentMana.ToString("F0");
        maxmanatext.text = "/ " + maxMana.ToString("F0");
        manaslider.value = currentMana / maxMana;
        lifepotionstext.text = GameController.LifePotions.ToString();
        manapotionstext.text = GameController.ManaPotions.ToString();
        trainingInfo.SetActive(isTraining);
        expGain.text = "EXP earned: " + trainingXp;
        currentEng.text = GameController.Eng.ToString("F0");
        engBar.value = GameController.Eng / GameController.MaxEng;
    }

    public void PlayRandomMusic()
    {
        int idx = Random.Range(0, battleMusicClips.Length);
        audioSource.clip = battleMusicClips[idx];
        audioSource.loop = true;
        audioSource.Play();
    }

    public void CheckEnd()
    {
        bool end = GameController.Life <= 0; // player dead
        end |= ((enemy1 == null || enemy1.ch.life <= 0) && (enemy2 == null || enemy2.ch.life <= 0)); // both enemys dead
        end |= (isTraining && GameController.Eng <= 0); // Out of energy in training
        if (end) EndCombat();
        else
        {
            // check if target still valid
            if (target.ch.life <= 0)
            {
                // change target
                if (target.Equals(enemy1))
                    SetTarget(1);
                else
                    SetTarget(0);
            }
        }
    }

    public void OnAttackButtonClicked()
    {
        Debug.Log("Normal Attack");
        StartCoroutine(PlayTurnAnimations(CombatAction.ATTACK));
    }
    public void OnFleeButtonClicked()
    {
        Debug.Log("Flee");
        EndCombat();
    }
    public void OnLifepotionButtonClicked()
    {
        Debug.Log("Use Lifepotion");
        if (GameController.UseLifePotion())
            StartCoroutine(PlayTurnAnimations(CombatAction.LIFE_POTION));
    }
    public void OnManapotionButtonClicked()
    {
        Debug.Log("Use Manapotion");
        if (GameController.UseManaPotion())
            StartCoroutine(PlayTurnAnimations(CombatAction.MANA_POTION));
        UpdateTexts();// updates mana bar
    }
    public void OnSkillButtonClicked(Skill s)
    {
        Debug.Log("Use Skill " + s);
        if (s.currentlevel > 0
            && GameController.Mana >= s.manacost)
        {
            GameController.Mana -= s.manacost;
            UpdateTexts();
            StartCoroutine(PlayTurnAnimations(s.associatedAction));
        }
        else
        {
            Debug.Log("Not enough Mana");
        }
    }

    public void EndCombat()
    {
        if (GameController.Life <= 0)
        {
            // Gameover
            audioSource.clip = defeatMusic;
            audioSource.loop = false;
            audioSource.Play();
            endScreen.ShowEndscreen(true, 0, 0, null);
        }
        else if ((enemy1 == null || enemy1.ch.life <= 0)
            && (enemy2 == null || enemy2.ch.life <= 0))
        {
            // Victory
            currentStage.currentLevel++;
            audioSource.clip = victoryMusic;
            audioSource.loop = false;
            audioSource.Play();
            // calculate loot
            int xp = 0;
            int gold = 0;
            Item itemdrop = null;
            if (!isTraining)
            {
                int itemidx = Random.Range(0, 10);
                if (enemy1 != null)
                {
                    xp += (int)enemy1.ch.expNext;
                    gold += (int)enemy1.ch.gold;
                    if (itemidx == 0 || itemidx == 1)
                        itemdrop = enemy1.ch.weapon;
                    else if (itemidx == 2)
                        itemdrop = enemy1.ch.shield;
                    else if (itemidx == 3)
                        itemdrop = enemy1.ch.suit;
                    else if (itemidx == 4)
                        itemdrop = enemy1.ch.headgear;
                }
                if (enemy2 != null)
                {
                    xp += (int)enemy2.ch.expNext;
                    gold += (int)enemy2.ch.gold;
                    if (itemidx == 5 || itemidx == 6)
                        itemdrop = enemy2.ch.weapon;
                    else if (itemidx == 7)
                        itemdrop = enemy2.ch.shield;
                    else if (itemidx == 8)
                        itemdrop = enemy2.ch.suit;
                    else if (itemidx == 9)
                        itemdrop = enemy2.ch.headgear;
                }
            }
            endScreen.ShowEndscreen(false, gold, xp + trainingXp, itemdrop);
        }
        else
        {
            // Neutral end
            audioSource.clip = victoryMusic;
            audioSource.loop = false;
            audioSource.Play();
            endScreen.ShowEndscreen(false, 0, trainingXp, null);
        }
    }

    public void SetTarget(int idx)
    {
        Debug.Log("SetTarget: " + idx);
        if (idx == 0 && enemy1 != null)
        {
            target = enemy1;
            Color c = marker1.color;
            c.a = 1.0f;
            marker1.color = c;
            c = marker2.color;
            c.a = 0.4f;
            marker2.color = c;
        }
        else if (idx == 1 && enemy2 != null)
        {
            target = enemy2;
            Color c = marker1.color;
            c.a = 0.4f;
            marker1.color = c;
            c = marker2.color;
            c.a = 1.0f;
            marker2.color = c;
        }
    }

    public void OnEndScreenExit()
    {
        GameController.SaveAllPrefs();
        GameController.LoadWorldScene();
    }

    public int CalculateFinalDamage(
        float rawDamage,
        int strengthOrMagic,
        float armorPercent)
    {
        // === Raw damage must be greater than 0 ===
        if (rawDamage <= 0)
            return 0;

        // === 1. Apply flat defense (stats) ===
        float flatDefense = strengthOrMagic * flatDefenseMultiplier;
        float reduced = Mathf.Max(1, rawDamage - flatDefense);

        // === 2. Combine armor % and apply soft cap ===
        float adjustedReduction = ApplySoftCap(armorPercent);
        reduced *= (1f - adjustedReduction);

        // === 3. Variance and critical hit ===
        reduced *= Random.Range(1f - damageVariance, 1f + damageVariance);
        if (Random.value < critChance)
            reduced *= critMultiplier;

        // === 4. Clamp and return ===
        return Mathf.Max(1, Mathf.RoundToInt(reduced));
    }

    // Soft cap function for diminishing returns
    private float ApplySoftCap(float reduction)
    {
        // Example: 0.5 -> 0.35 when softness = 2
        float soft = 1f - Mathf.Pow(1f - reduction, defenseSoftness);
        return Mathf.Min(soft, maxReduction);
    }

    private void DealDamage(Combatant attacker, Combatant defender, float extraPhysDmg, float extraMagicDmg)
    {
        if (attacker == null || defender == null) return;

        if (defender.ch.currentShieldHp > 0)
        {
            // shield takes damage
            int physdmg = CalculateFinalDamage(
                attacker.ch.GetPhysDmg() + extraPhysDmg,
               attacker.ch.weapon ? attacker.ch.weapon.shieldDmg * -1 : 0,
                defender.ch.GetShieldPhysDefPercent()
            );
            int magicdmg = CalculateFinalDamage(
                attacker.ch.GetMagicDmg() + extraMagicDmg,
                attacker.ch.weapon ? attacker.ch.weapon.shieldDmg * -1 : 0,
                defender.ch.GetShieldMagicDefPercent()
            );
            Debug.Log(defender.ch.name + "'s shield receives " + physdmg + " physical and " + magicdmg + " magical damage");
            ShowDamageNumber(defender.go.transform.position, physdmg, magicdmg);
            int totaldmg = physdmg + magicdmg;
            defender.ch.currentShieldHp -= totaldmg;
            if (defender.ch.currentShieldHp < 0)
            {
                defender.ch.currentShieldHp = 0;
                defender.animator?.SetTrigger("ShieldBreak");
            }
            else
            {
                defender.animator?.SetTrigger("ShieldHit");
            }
            // earn training xp
            if(isTraining && attacker.ch.IsPlayerChar() && totaldmg > 0)
            {
                // default easy
                int xp = 3;
                int eng = 2;
                if(currentStage.currentLevel == 1) // medium
                {
                    xp = 5;
                    eng = 3;
                }
                if (currentStage.currentLevel == 2) // hard
                {
                    xp = 20;
                    eng = 10;
                }
                trainingXp += xp;
                GameController.Eng -= eng;
            }
        }
        else
        {
            // character takes damage
            int physdmg = CalculateFinalDamage(
                attacker.ch.GetPhysDmg(),
                defender.ch.strength,
                defender.ch.GetPhysDefPercent()
            );
            int magicdmg = CalculateFinalDamage(
                attacker.ch.GetMagicDmg(),
                defender.ch.magic,
                defender.ch.GetMagicDefPercent()
            );
            Debug.Log(defender.ch.name + " receives " + physdmg + " physical and " + magicdmg + " magical damage");
            ShowDamageNumber(defender.go.transform.position, physdmg, magicdmg);
            int totaldmg = physdmg + magicdmg;
            defender.ch.life -= totaldmg;
            if (defender.ch.life < 0)
            {
                defender.ch.life = 0;
                defender.animator?.SetTrigger("Die");
            }
            else
            {
                defender.animator?.SetTrigger("Hit");
            }
            // earn training xp
            if (isTraining && attacker.ch.IsPlayerChar() && totaldmg > 0)
            {
                // default easy
                int xp = 6;
                int eng = 2;
                if (currentStage.currentLevel == 1) // medium
                {
                    xp = 10;
                    eng = 3;
                }
                if (currentStage.currentLevel == 2) // hard
                {
                    xp = 40;
                    eng = 10;
                }
                trainingXp += xp;
                GameController.Eng -= eng;
            }
        }
        UpdateTexts();
    }

    public void ShowDamageNumber(Vector3 position, int physdmg, int magicdmg)
    {
        damageNumberDisplay.transform.position = position + new Vector3(0, 2, -5);
        Color textcolor = Color.orange;
        if (physdmg < 0)
            textcolor = Color.green;
        else if (magicdmg > physdmg)
            textcolor = Color.cyan;
        damageNumberDisplay.Show(Mathf.Abs(physdmg + magicdmg), textcolor);
    }


    // Animation Sequnce Coroutines

    public IEnumerator PlayIntro()
    {
        IsAnimating = true;
        introTitle.text = currentStage.title;
        introLevel.text = "Level " + (currentStage.currentLevel+1);
        introScreen.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        // Fade-in and Scale-in
        yield return FadeAndScale(introEffect, 0f, 1f, new Vector3(0.1f, 0.1f, 1), new Vector3(1.3f, 1.1f, 1f), 0.75f);
        // Fade-out and Scale-out
        yield return FadeAndScale(introEffect, 1f, 0f, new Vector3(1.3f, 1.1f, 1f), new Vector3(0.1f, 0.1f, 1f), 0.75f);

        yield return new WaitForSeconds(0.5f);

        introScreen.SetActive(false);
        yield return new WaitForSeconds(2f);
        IsAnimating = false;
    }

    private IEnumerator PlayTurnAnimations(CombatAction playerAction)
    {
        IsAnimating = true;

        // Player turn
        yield return PerformCombatAction(playerAction, player, target);
        UpdateTexts();

        // Ally turn
        if (ally != null && target != null)
            yield return PerformNormalAttack(ally, target);

        // Now enemy turn
        if (enemy1 != null && enemy1.ch.life > 0)
        {
            // TODO make a random move
            yield return PerformNormalAttack(enemy1, player);
        }
        if (enemy2 != null && enemy2.ch.life > 0)
        {
            // TODO make a random move
            yield return PerformNormalAttack(enemy2, player);
        }

        IsAnimating = false;
    }

    private IEnumerator PerformCombatAction(CombatAction a, Combatant user, Combatant target)
    {
        switch (a)
        {
            case CombatAction.ATTACK:
                if (target != null)
                    yield return PerformNormalAttack(user, target);
                break;
            case CombatAction.LIFE_POTION:
                ShowDamageNumber(user.go.transform.position, -80, 0);
                yield return new WaitForSeconds(0.5f);
                break;
            case CombatAction.MANA_POTION:
                ShowDamageNumber(user.go.transform.position, 0, 80);
                yield return new WaitForSeconds(0.5f);
                break;
            case CombatAction.SKILL_STAB:
                yield return PerformStab(user, target);
                break;
            case CombatAction.SKILL_SHURIKEN:
                yield return PerformShuriken(user, target);
                break;
            case CombatAction.SKILL_DOUBLESTRIKE:
                Debug.Log("Action not supoorted yet: " + a);
                break;
            case CombatAction.SKILL_SPEEDSTRIKE:
                Debug.Log("Action not supoorted yet: " + a);
                break;
            case CombatAction.SKILL_VERTICALSTRIKE:
                Debug.Log("Action not supoorted yet: " + a);
                break;
            case CombatAction.SKILL_AVENGER:
                Debug.Log("Action not supoorted yet: " + a);
                break;
            case CombatAction.SKILL_SPLIT:
                Debug.Log("Action not supoorted yet: " + a);
                break;
            case CombatAction.SKILL_EXECUTION:
                Debug.Log("Action not supoorted yet: " + a);
                break;
            case CombatAction.SKILL_CHARGE:
                yield return PerformManaHeal(user, GameController.FindSkillByActionId(CombatAction.SKILL_CHARGE).GetCurrentLevelValue());
                break;
            case CombatAction.SKILL_HEAL:
                yield return PerformHeal(user, GameController.FindSkillByActionId(CombatAction.SKILL_HEAL).GetCurrentLevelValue());
                break;
            case CombatAction.SKILL_ENERGYSHOT:
                Debug.Log("Action not supoorted yet: " + a);
                break;
            case CombatAction.SKILL_FIRESHOT:
                Debug.Log("Action not supoorted yet: " + a);
                break;
            case CombatAction.SKILL_MANABOMB:
                Debug.Log("Action not supoorted yet: " + a);
                break;
            case CombatAction.SKILL_ANNIHILATE:
                Debug.Log("Action not supoorted yet: " + a);
                break;
            case CombatAction.SKILL_SHADOWSTRIKE:
                Debug.Log("Action not supoorted yet: " + a);
                break;
            case CombatAction.SKILL_REPLICATE:
                Debug.Log("Action not supoorted yet: " + a);
                break;
        }
    }

    private IEnumerator PerformNormalAttack(Combatant attacker, Combatant defender)
    {

        Vector3 start = attacker.go.transform.position;
        Vector3 forward = defender.go.transform.position;

        forward += (start - forward).normalized * 2;

        // Move forward
        attacker.animator?.SetTrigger("Move");
        float t = 0;
        while (t < 1.0f)
        {
            t += Time.deltaTime;
            attacker.go.transform.position = Vector3.Lerp(start, forward, t);
            yield return null;
        }

        // Wait for attack animation
        attacker.animator?.SetTrigger("Strike");
        yield return new WaitForSeconds(0.25f);
        DealDamage(attacker, defender, 0, 0);
        yield return new WaitForSeconds(0.25f);

        // Return back
        attacker.animator?.SetTrigger("Move");
        t = 0;
        while (t < 1.0f)
        {
            t += Time.deltaTime;
            attacker.go.transform.position = Vector3.Lerp(forward, start, t);
            yield return null;
        }
        attacker.animator?.SetTrigger("Idle");
    }

    private IEnumerator PerformHeal(Combatant user, int healamount)
    {
        // Play the heal effect
        user.animator?.SetTrigger("Heal");
        Debug.Log(user.ch.name + " heals " + healamount);
        user.ch.life += healamount;
        if (user.ch.life > user.ch.maxlife) user.ch.life = user.ch.maxlife;
        ShowDamageNumber(user.go.transform.position, -healamount, 0);
        // wait for the animation to finish
        yield return new WaitForSeconds(0.5f);
        user.animator?.SetTrigger("Idle");
        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator PerformManaHeal(Combatant user, int healamount)
    {
        // Play the mana restore effect
        user.animator?.SetTrigger("Heal");
        Debug.Log(user.ch.name + " recovers mana " + healamount);
        user.ch.mana += healamount;
        if (user.ch.mana > user.ch.maxmana) user.ch.mana = user.ch.maxmana;
        ShowDamageNumber(user.go.transform.position, 0, healamount);
        // wait for the animation to finish
        yield return new WaitForSeconds(0.5f);
        user.animator?.SetTrigger("Idle");
        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator FadeAndScale(CanvasGroup canvasGroup, float fromAlpha, float toAlpha, Vector3 fromScale, Vector3 toScale, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float normalized = t / duration;
            canvasGroup.alpha = Mathf.Lerp(fromAlpha, toAlpha, normalized);
            transform.localScale = Vector3.Lerp(fromScale, toScale, normalized);
            yield return null;
        }
        canvasGroup.alpha = toAlpha;
        transform.localScale = toScale;
    }

    // Skill Animations
    private IEnumerator PerformStab(Combatant user, Combatant target)
    {

        Vector3 start = user.go.transform.position;
        Vector3 forward = target.go.transform.position;

        forward += (start - forward).normalized * 2;

        // Move forward
        user.animator?.SetTrigger("Move");
        float t = 0;
        while (t < 1.0f)
        {
            t += Time.deltaTime;
            user.go.transform.position = Vector3.Lerp(start, forward, t);
            yield return null;
        }

        // Wait for attack animation
        user.animator?.SetTrigger("Stab");
        yield return new WaitForSeconds(0.25f);
        if (user.ch.IsPlayerChar())
            DealDamage(user, target, GameController.FindSkillByActionId(CombatAction.SKILL_STAB).GetCurrentLevelValue(), 0);
        else
            DealDamage(user, target, user.ch.level * 4, 0);
        yield return new WaitForSeconds(0.25f);

        // Return back
        user.animator?.SetTrigger("Move");
        t = 0;
        while (t < 1.0f)
        {
            t += Time.deltaTime;
            user.go.transform.position = Vector3.Lerp(forward, start, t);
            yield return null;
        }
        user.animator?.SetTrigger("Idle");
    }
    private IEnumerator PerformShuriken(Combatant user, Combatant target)
    {
        // throw
        user.animator?.SetTrigger("Throw");
        yield return new WaitForSeconds(0.5f);

        // Wait for attack animation
        yield return new WaitForSeconds(0.25f);
        if (user.ch.IsPlayerChar())
        {
            DealDamage(user, enemy1, 0, GameController.FindSkillByActionId(CombatAction.SKILL_SHURIKEN).GetCurrentLevelValue());
            DealDamage(user, enemy2, 0, GameController.FindSkillByActionId(CombatAction.SKILL_SHURIKEN).GetCurrentLevelValue());
        }
        else
        {
            DealDamage(user, player, 0, user.ch.level * 4);
            DealDamage(user, ally, 0, user.ch.level * 4);
        }
        // Wait for hurt animation
        user.animator?.SetTrigger("Idle");
        yield return new WaitForSeconds(0.5f);
    }

}
