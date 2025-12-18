using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("Burger Icons")]
    public Image[] burgerImages;          // assign all burger Images here, in order
    public Sprite fullBurgerSprite;       // burger when you have health
    public Sprite emptyBurgerSprite;      // optional: faded/empty burger

    public void SetHearts(int current, int max)
    {
        // clamp in case something gets weird
        if (max < 0) max = 0;
        if (current < 0) current = 0;
        if (current > max) current = max;

        for (int i = 0; i < burgerImages.Length; i++)
        {
            bool withinMax = i < max;

            // only show burgers up to maxHearts
            burgerImages[i].enabled = withinMax;
            if (!withinMax) continue;

            // full vs empty burger
            if (i < current)
            {
                if (fullBurgerSprite != null)
                    burgerImages[i].sprite = fullBurgerSprite;
            }
            else
            {
                if (emptyBurgerSprite != null)
                    burgerImages[i].sprite = emptyBurgerSprite;
                else if (fullBurgerSprite != null)
                    burgerImages[i].color = new Color(1f, 1f, 1f, 0.3f); // faded if no empty sprite
            }
        }
    }
}
