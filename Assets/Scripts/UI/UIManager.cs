using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Enemy enemy;

    [SerializeField] private TextMeshProUGUI playerHPText;
    [SerializeField] protected TextMeshProUGUI enemyHPText;

    private void Update()
    {
        playerHPText.text = $"Player HP  {player.CurrentHP}";

        enemyHPText.text = $"Enemy HP : {enemy.CurrentHP}";
    }
}
