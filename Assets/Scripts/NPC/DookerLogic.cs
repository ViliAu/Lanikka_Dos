using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Dookers have 3 states: IDLE, HUNGRY, DEFECATING
 * If they have no food slots in their stomach => They're hungry
 * If their shitmass counter is over a threshold => They start shitting until no shit mass is left
 * They priorize eating over anything => If they're hungry they start crying and can't shit.
 * If they're hungry, they check if a food through exists. If not, they start crying. If it is empty, same thing.
 * If they need to take a shit, they check if a bucket exists regardless if it is full or not. If not, they shit themselves.
 */

public class DookerLogic : MonoBehaviour {



    public enum State {
        IDLE,
        EATING,
        DEFECATING
    }

}