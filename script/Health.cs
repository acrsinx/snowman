public partial class Health {
    private int maxHealth;
    public delegate void Void();
    public Void die;
    public Void healthChanged;
    public int MaxHealth {
        set {
            maxHealth = value;
            if (currentHealth > maxHealth) {
                currentHealth = maxHealth;
            }
        }
        get {
            return maxHealth;
        }
    }
    private int currentHealth;
    public int CurrentHealth {
        set{
            if (value > maxHealth) {
                currentHealth = maxHealth;
            } else if (value <= 0) {
                currentHealth = 0;
                die?.Invoke();
            } else {
                currentHealth = value;
            }
            healthChanged?.Invoke();
        }
        get{
            return currentHealth;
        }
    }
    public Health(int maxHealth) {
        this.maxHealth = maxHealth;
        currentHealth = maxHealth;
    }
    public void SetFullHealth() {
        CurrentHealth = maxHealth;
    }
    public bool IsFullHealth() {
        return currentHealth == maxHealth;
    }
    public override string ToString() {
        return currentHealth.ToString() + "/" + maxHealth.ToString();
    }
}