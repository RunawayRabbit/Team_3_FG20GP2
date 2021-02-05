using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CauldronMixing : MonoBehaviour
{
    [SerializeField] Animator thrownObjectAnimator;
    [SerializeField] GameObject[] objectsToActivate;
    [SerializeField] GameObject[] objectsToDeactivate;
    [SerializeField] ParticleSystem correctParticle;
    [SerializeField] ParticleSystem wrongParticle;
    [SerializeField] Transform potionSpawnPoint;
    [SerializeField] float particleDelay = .2f;

    [SerializeField] AudioSource correctAudio;
    [SerializeField] AudioSource wrongAudio;

    public List<PotionRecipe> potionRecipes = new List<PotionRecipe>();

    List<Ingredient> currentIngredients = new List<Ingredient>();

    public static CauldronMixing activeCauldron = null;

    private void Start()
    {
        foreach (var item in objectsToActivate)
        {
            item.SetActive(false);
        }
    }

    public void AddIngredient(Ingredient ingredient)
    {
        StartCoroutine(_AddIngredient(ingredient));
    }

    public IEnumerator _AddIngredient(Ingredient ingredient)
    {
        Debug.Log(ingredient + " being added to cauldron");
        bool validIngredient = false;
        PotionRecipe corRecipe = null;
        currentIngredients.Add(ingredient);
        foreach (PotionRecipe recipe in potionRecipes)
        {
            if (currentIngredients.Count <= recipe.ingredients.Count)
            {
                for (int i = 0; i < currentIngredients.Count; i++)
                {
                    if (currentIngredients[i] != recipe.ingredients[i])
                    {
                        break;
                    }
                    else if (i == currentIngredients.Count - 1)
                    {
                        validIngredient = true;
                        if (currentIngredients.Count == recipe.ingredients.Count) corRecipe = recipe;
                    }
                }
            }
        }


        if (thrownObjectAnimator.transform.childCount != 0)
            Destroy(thrownObjectAnimator.transform.GetChild(0).gameObject);


        Instantiate(ingredient.ingredientPrefab, thrownObjectAnimator.transform.position, thrownObjectAnimator.transform.rotation, thrownObjectAnimator.transform.transform);
        thrownObjectAnimator.Play("FlyingIntoCauldron");
        if (validIngredient)
        {

            yield return new WaitForSeconds(particleDelay);
            correctParticle.Play();
            correctAudio.Play();
            if (corRecipe != null)
            {
                PotionDone(corRecipe);
            }

        }
        else
        {
            yield return new WaitForSeconds(particleDelay);
            wrongParticle.Play();
            wrongAudio.Play();
            currentIngredients = new List<Ingredient>();
        }

    }

    GameObject spawnedPotion;

    public void PotionDone(PotionRecipe recipe)
    {
        currentIngredients = new List<Ingredient>();
        if (spawnedPotion == null)
            spawnedPotion = Instantiate(recipe.potionObject, potionSpawnPoint.position, Quaternion.identity, null);

    }


    public void BeginMixing()
    {
        PlayerManager.CurrentPlayer.GetComponent<PlayerMovement>().ChangeState(PlayerMovement.PlayerState.Locked);
        activeCauldron = this;
        foreach (GameObject gameObject in objectsToActivate)
        {
            gameObject.SetActive(true);
        }
        foreach (var item in objectsToDeactivate)
        {
            item.SetActive(false);
        }
    }

    public void StopMixing()
    {
        activeCauldron = null;
        foreach (var item in objectsToDeactivate)
        {
            item.SetActive(true);
        }
        foreach (GameObject gameObject in objectsToActivate)
        {
            gameObject.SetActive(false);
        }

        InventoryManager.InventoryIsOpen = false;
        PlayerManager.CurrentPlayer.GetComponent<PlayerMovement>().ChangeState(PlayerMovement.PlayerState.Waiting);
    }


}


[System.Serializable]
public class PotionRecipe
{
    public List<Ingredient> ingredients;
    public GameObject potionObject;
}
