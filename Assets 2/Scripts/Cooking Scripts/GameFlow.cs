using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//GameFlow manages the cooking aspect of the game. It generates random orders and checks if the player made the correct order.

public class GameFlow : MonoBehaviour
{
    public static int orderValue = 0; //target value
    public static int plateValue = 0; //the player's value

    public static GameFlow instance; 

    [Header("UI Elements")]
    public TextMeshProUGUI orderText; 
    public GameObject orderTicket; 

    //This dictionary maps the item name to it's numeric value. This corresponds to the order and plate value to track if they match.
    private static Dictionary<string, int> foodValues = new Dictionary<string, int>()
    {
        { "burger_bread_1", 10000 },
        { "burger_bread_2", 1000 },
        { "burger_patty", 100 },
        { "cheese_slice", 1 },
        { "lettuce_slice", 100000 },
        { "onion_slice", 10 },
        { "tomato_slice", 1000000 }
    };

    //Changes the prefab names to names the player will understand
    private static Dictionary<string, string> foodNames = new Dictionary<string, string>()
    {
        { "burger_bread_1", "Bottom Bun" },
        { "burger_bread_2", "Top Bun" },
        { "burger_patty", "Patty" },
        { "cheese_slice", "Cheese" },
        { "lettuce_slice", "Lettuce" },
        { "onion_slice", "Onion" },
        { "tomato_slice", "Tomato" }
    };

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        GenerateRandomOrder();
    }

    void Update()
    {

    }

    //Generates a random order value by adding up the ingreident's numeric values.
        //Ex. 2 cheese slices --> 2, 2 onion slices --> 20
    public static void GenerateRandomOrder()
    {
        plateValue = 0; 
        orderValue = 0;

        //these will always be included
        orderValue += foodValues["burger_bread_1"];
        orderValue += foodValues["burger_bread_2"];

        //Generate how many patties, but there will always be at least one
        int pattyCount = Random.value > 0.2f ? 1 : 2;
        orderValue += foodValues["burger_patty"] * pattyCount;

        //list of possible toppings
        List<string> toppings = new List<string>()
        {
            "cheese_slice",
            "lettuce_slice",
            "onion_slice",
            "tomato_slice"
        };

        int numToppings = Random.Range(1, 5);

        for (int i = 0; i < numToppings; i++)
        {
            int randomIndex = Random.Range(0, toppings.Count);
            orderValue += foodValues[toppings[randomIndex]];
        }

        Debug.Log("New Order Value: " + orderValue);
        instance.DisplayOrder();
    }

    //Displays the order to the player. Divides the order value by the ingredients to know what names to display.
    private void DisplayOrder()
    {
        Dictionary<string, int> orderCounts = new Dictionary<string, int>();

        int remainingValue = orderValue;

        int tomatoCount = remainingValue / 1000000;
        if (tomatoCount > 0) orderCounts["Tomato"] = tomatoCount;
        remainingValue %= 1000000;

        int lettuceCount = remainingValue / 100000;
        if (lettuceCount > 0) orderCounts["Lettuce"] = lettuceCount;
        remainingValue %= 100000;

        int topBunCount = remainingValue / 10000;
        if (topBunCount > 0) orderCounts["Top Bun"] = topBunCount;
        remainingValue %= 10000;

        int bottomBunCount = remainingValue / 1000;
        if (bottomBunCount > 0) orderCounts["Bottom Bun"] = bottomBunCount;
        remainingValue %= 1000;

        int pattyCount = remainingValue / 100;
        if (pattyCount > 0) orderCounts["Patty"] = pattyCount;
        remainingValue %= 100;

        int onionCount = remainingValue / 10;
        if (onionCount > 0) orderCounts["Onion"] = onionCount;
        remainingValue %= 10;

        int cheeseCount = remainingValue;
        if (cheeseCount > 0) orderCounts["Cheese"] = cheeseCount;

        string orderDisplay = "— ORDER —\n\n";

        foreach (var item in orderCounts)
        {
            if (item.Value == 1)
                orderDisplay += $"• {item.Key}\n";
            else
                orderDisplay += $"• {item.Key} x{item.Value}\n";
        }

        orderText.text = orderDisplay;
    }
}