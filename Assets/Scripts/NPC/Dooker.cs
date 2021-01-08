using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/* Dookers have 4 states: IDLE, HUNGRY, DEFECATING, CRYING
 * If they have no food slots in their stomach => They're hungry
 * If their shitmass counter is over a threshold => They start shitting until no shit mass is left
 * If they're hungry, they check if a food through exists. If not, they start crying. If it is empty, same thing.
 * If they need to take a shit, they check if a bucket exists regardless if it is full or not. If not, they shit themselves.
 */

public class Dooker : NPC {
    [Header("AI settings")]
    [SerializeField] private float stateUpdateInterval = 2f;
    [SerializeField] private float pathFindInterval = 0.5f;
    [SerializeField] private float eatingInterval = 1.5f;

    [Header("Food data")]
    [SerializeField] private List <Edible> foodSlots = new List<Edible>();
    [Tooltip("How many food slots can be occupied at max")]
    [SerializeField] private int maxFoodCount = 5;

    [Header("Shit data")]
    [SerializeField] private float maxShitmass = 100;
    [Tooltip("How much shitmass does the dooker need to have in order to take a doodie")]
    [SerializeField] private float currentShitmass = 0;
    [Tooltip("How much shitmass does the dooker need to have in order to take a doodie")]
    [SerializeField] private float shitmassThreshold = 75f;
    [Tooltip("How much shitmass is one turd gonna cost")]
    [SerializeField] private float shitMassPerShit = 20f;
    [Tooltip("How long it takes to push out a turd")]
    [SerializeField] private float shitTime = 3f;
    [Tooltip("Gameobject that indicates where the shit will spawn from.")]
    [SerializeField] private Transform shitSpawn = null;

    private NavMeshAgent agent;

    [SerializeField] private State currentState = State.IDLE;
    public enum State {
        IDLE,
        HUNGRY,
        EATING,
        DEFECATING,
        CRYING
    }

    private void Awake() {
        agent = GetComponent<NavMeshAgent>();
        InvokeRepeating("UpdateState", 0, stateUpdateInterval);
    }

    private void UpdateState() {
        // Check if we can digest food
        if (foodSlots.Count > 0 && !IsInvoking("Digest")) {
            Invoke("Digest", foodSlots[0].DigestTime);
        }
        // If we're idling check what we have to do
        if (currentState == State.IDLE || currentState == State.CRYING) {
            // Try to do something
            currentState = GetNewState();

            // Digesting food
            if (currentState == State.IDLE) {
                if (agent != null)
                    agent.destination = EntityManager.DookerPen.GetNewWalkPos(transform.position.y);
                else
                    Debug.LogError("No nav mesh agent found for dooker!");
                return;
            }

            // Nothing to do
            else if (currentState == State.CRYING) {
                //SoundSystem.PlaySoundGroup("dooker_cry", transform.position);
                return;
            }
        }

        // If we're shitting
        if (currentState == State.DEFECATING && (!IsInvoking("Defecate") || !IsInvoking("BucketDefecate"))) {
            // TODO: KYYKKY ANIMI

            // Bucket shit
            if (EntityManager.DookerPen.shitBucket != null && EntityManager.DookerPen.shitBucket.CanAddShit()) {
                // We're close enough
                if (Vector3.Distance(transform.position, EntityManager.DookerPen.shitBucket.transform.position) <
                    EntityManager.DookerPen.shitBucket.dookerDistance) {
                    agent.destination = transform.position;
                    Invoke("BucketDefecate", shitTime);
                }
                // Go closer
                else {
                    agent.destination = EntityManager.DookerPen.shitBucket.transform.position;
                }
            }

            // Shit on the ground
            else {
                Invoke("Defecate", shitTime);
            }
        }

        // If we're going to get some food
        if (currentState == State.HUNGRY && !IsInvoking("SeekFoodContainer")) {
            if (EntityManager.DookerPen.foodContainer != null) {
                Invoke("SeekFoodContainer", pathFindInterval);
            }
        }
    }

    // Functions for trying to find food or bucket; cannot pass params to invokes
    private void SeekFoodContainer() {
        // If food container is lost start idling
        if (EntityManager.DookerPen.foodContainer == null) {
            currentState = State.IDLE;
            return;
        }
        // If we're close enough
        if (Vector3.Distance(transform.position, EntityManager.DookerPen.foodContainer.transform.position) <
                    EntityManager.DookerPen.foodContainer.dookerDistance) {
            EatFood();
        }
        // if not
        else {
            agent.destination = EntityManager.DookerPen.foodContainer.transform.position;
            Invoke("SeekFoodContainer", pathFindInterval);
        }
    }

    private void SeekShitBucket() {

    }

    private void Digest() {
        // If the ass is full of shit don't digest food anymore;
        // rather halt the digestion until more shit space is acquired
        // Shit mass can overcharge in order to prevent softlocks
        if (/*foodSlots[0].ShitmassAmount + */currentShitmass > maxShitmass) {
            Invoke("Digest", foodSlots[0].DigestTime);
            return;
        }
        currentShitmass += foodSlots[0].ShitmassAmount;
        foodSlots.RemoveAt(0);
        if (foodSlots.Count > 0) {
            Invoke("Digest", foodSlots[0].DigestTime);
        }
    }

    private void Defecate() {
        // Stop the agent
        agent.destination = transform.position;
        SoundSystem.PlaySoundGroup("dooker_efe", transform.position);
        currentShitmass -= shitMassPerShit;

        Doodie doodie = Instantiate(Database.Singleton.GetEntityPrefab("doodie_normal") as Doodie, shitSpawn.position, transform.rotation, null) as Doodie;
        // TODO: Paska paukkumis animi

        // If we're out of power walk to a random spot and start idling
        if (currentShitmass < shitMassPerShit) {
            currentState = State.IDLE;
        }
    }

    private void BucketDefecate() {
        // If the bucket is full shit on the ground
        if (!EntityManager.DookerPen.shitBucket.CanAddShit()) {
            Invoke("Defecate", 0);
            return;
        }

        // Stop the agent
        agent.destination = transform.position;
        SoundSystem.PlaySoundGroup("dooker_efe", transform.position);
        currentShitmass -= shitMassPerShit;
        EntityManager.DookerPen.shitBucket.AddShit("normal"); // TODO: ANIMI JA DROPTABLE
        // TODO: Paska paukkumis animi

        // If we're out of power walk to a random spot and start idling
        if (currentShitmass < shitMassPerShit) {
            currentState = State.IDLE;
        }
    }

    private void EatFood() {
        if (EntityManager.DookerPen.foodContainer != null ) {
            if (EntityManager.DookerPen.foodContainer.edibles.Count > 0) {
                if (foodSlots.Count < maxFoodCount) {
                    // Eating
                    // TODO: IMUTUS ANIMI TÄHÄ
                    AddFood(EntityManager.DookerPen.foodContainer.edibles[EntityManager.DookerPen.foodContainer.edibles.Count-1]);
                    EntityManager.DookerPen.foodContainer.edibles.RemoveAt(EntityManager.DookerPen.foodContainer.edibles.Count-1);
                    Invoke("SeekFoodContainer", eatingInterval);
                    return;
                }
            }
        }
        // Not eating
        agent.destination = EntityManager.DookerPen.GetNewWalkPos(transform.position.y);
        currentState = State.IDLE;
    }

    public void AddFood(Edible e) {
        if (foodSlots.Count < maxFoodCount) {
            foodSlots.Add(e);
            SoundSystem.PlaySoundGroup("dooker_eat", transform.position);
        }
    }

    private State GetNewState() {
        // Hungry
        if (foodSlots.Count == 0) {
            // Check for food container
            if (EntityManager.DookerPen.foodContainer != null) {
                // Check if it has food
                if (EntityManager.DookerPen.foodContainer.edibles.Count > 0) {
                    return State.HUNGRY;
                }
            }
            if (currentShitmass > shitMassPerShit) {
                return State.DEFECATING;
            }
            else {
                // Couldn't do anything!
                if (currentState != State.CRYING) // TODO: Move this somewhere else?
                    agent.destination = EntityManager.DookerPen.GetNewWalkPos(transform.position.y);
                return State.CRYING;
            }
        }
        // Not hungry
        if (currentShitmass > shitmassThreshold) {
            return State.DEFECATING;
        }
        return State.IDLE;
    }

    public override void PlayerInteract() {
        base.PlayerInteract();
        // FEEDING
        if (EntityManager.Player.Player_Equipment.equippedItem != null) {
            if (EntityManager.Player.Player_Equipment.equippedItem as Edible) {
                if (foodSlots.Count < maxFoodCount) {
                    AddFood((Edible)EntityManager.Player.Player_Equipment.equippedItem);
                    EntityManager.Player.Player_Inventory.DecrementStackSize(EntityManager.Player.Player_Equipment.itemIndex);
                }
            }
        }
        // OPENING UI
        else {
            // TODO: DOOKER UI
        }
    }
}