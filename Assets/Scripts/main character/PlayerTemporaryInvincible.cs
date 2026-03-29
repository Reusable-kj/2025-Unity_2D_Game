using UnityEngine;

public class PlayerTemporaryInvincible : MonoBehaviour
{
    float invincibleUntil = 0f;
    public bool IsInvincible => Time.time < invincibleUntil;

    public void SetInvincible(float seconds)
    {
        invincibleUntil = Mathf.Max(invincibleUntil, Time.time + seconds);
    }
}