using UnityEngine;
using System.Collections.Generic;

// abstract class không thể được kéo thả vào object, nó chỉ để cho các class khác kế thừa
public abstract class BaseAttackHitbox : MonoBehaviour
{
    [Header("Base Settings")]
    public int damage = 10;
    public ActionDirection attackDirection;

    public bool isDealingDamage;
    public bool isAttackActive; // CỜ MỚI!

    protected List<CharacterStats> processedCharacters;
    [HideInInspector] public CharacterStats ownerStats;


    protected virtual void Awake()
    {
        processedCharacters = new List<CharacterStats>();
        ownerStats = GetComponentInParent<CharacterStats>();

    }

    public virtual void ResetState()
    {
        processedCharacters.Clear();
        isAttackActive = false; 
        isDealingDamage = false;
    }
    // Hàm public để bên ngoài có thể "ghi sổ"
    public void AddProcessedCharacter(CharacterStats stats)
    {
        if (processedCharacters != null && !processedCharacters.Contains(stats))
        {
            processedCharacters.Add(stats);
        }
    }
    public void StartAttack()
    {
        isAttackActive = true;
    }
    public void EndAttack()
    {
        isAttackActive = false;
    }
    public void EnableDamage() { isDealingDamage = true; }
    public void DisableDamage() { isDealingDamage = false; }
}