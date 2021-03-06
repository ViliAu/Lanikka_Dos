﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    private PlayerController pc;
    public PlayerController Player_Controller {
        get {
            if (pc == null) {
                pc = transform.GetComponent<PlayerController>();
            }
            return pc;
        }
    }

    private PlayerInput pi;
    public PlayerInput Player_Input {
        get {
            if (pi == null) {
                pi = transform.GetComponent<PlayerInput>();
            }
            return pi;
        }
    }

    private PlayerCamera pcam;
    public PlayerCamera Player_Camera {
        get {
            if (pcam == null) {
                pcam = transform.GetComponent<PlayerCamera>();
            }
            return pcam;
        }
    }

    private PlayerInventory pinv;
    public PlayerInventory Player_Inventory {
        get {
            if (pinv == null) {
                pinv = transform.GetComponent<PlayerInventory>();
            }
            return pinv;
        }
    }

    private PlayerEquipment peq;
    public PlayerEquipment Player_Equipment {
        get {
            if (peq == null) {
                peq = transform.GetComponent<PlayerEquipment>();
            }
            return peq;
        }
    }

    private PlayerUI pui;
    public PlayerUI Player_UI {
        get {
            if (pui == null) {
                pui = transform.GetComponent<PlayerUI>();
            }
            return pui;
        }
    }

    private PlayerInteraction pint;
    public PlayerInteraction Player_Interaction {
        get {
            if (pint == null) {
                pint = transform.GetComponent<PlayerInteraction>();
            }
            return pint;
        }
    }

    private PlayerWallet pwal;
    public PlayerWallet Player_Wallet {
        get {
            if (pwal == null) {
                pwal = transform.GetComponent<PlayerWallet>();
            }
            return pwal;
        }
    }

    private PlayerGrab pgrab;
    public PlayerGrab Player_Grab {
        get {
            if (pgrab == null) {
                pgrab = transform.GetComponent<PlayerGrab>();
            }
            return pgrab;
        }
    }
}