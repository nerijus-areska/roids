using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Spaceship : MonoBehaviour
{

    public GameObject gameCamera;

    public GameObject moveTargetLine;

    public GameObject canvas;

    private LineRenderer lineRenderer;

    public GameObject sparksEffect;
    private CanvasController canvasController;
    public int coins;

    public float miningPower = 0.03f;
    public int miningPowerUpgrade = 10;

    public float shipSpeed = 1.0f;
    public int shipSpeedUpgrade = 10;

    public float shipRotation = 1.0f;
    public int shipRotationUpgrade = 10;

    public bool directMoving = false;

    public Vector3 currentRotation;
    
    public enum STATE {
        ROTATING,
        MOVING,
        IDLE,
        MINING
    }

//TODO: temp, move into function
    private float diff;

    private float DIST_TO_ROID = 3f;

    private Vector3 miningPoint;
    private Vector3 miningRoidPoint;
    private GameObject miningRoid;

    public STATE state = STATE.IDLE;
    // Start is called before the first frame update

    public float dragSpeed = 1.0f;
    // TODO: it's in screen pixels, probably dependant on screen size
    private float dragThreshold = 4f;
    private Vector3? dragOrigin = null;
    public bool cameraIsDragged = false;

// Boosts:
    public float? speedBoostUntil = null;
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        sparksEffect.SetActive(false);
        state = STATE.IDLE;
        canvasController = canvas.GetComponent<CanvasController>();
    }

    // Update is called once per frame
    void Update()
    {
        invalidateBoosts();
        handleControls();

        if(directMoving == true) {
            //draw directional line
            var moveTargetLineRenderer = moveTargetLine.GetComponent<LineRenderer>();
            Vector3 tmp1 = transform.position;
            tmp1.z = 5f;
            Vector3[] positions = new Vector3[2] { tmp1, miningPoint};
            moveTargetLineRenderer.positionCount = 2;
            moveTargetLineRenderer.SetPositions(positions);
        }

        if(state == STATE.IDLE) {
            //if doing nothing - just find our closest roid
            float minDistance = 99999;
            GameObject closestRoid = null;
            foreach(GameObject roid in Init.roidList) {
                float dist = Vector3.Distance(transform.position, roid.transform.position);
                if(minDistance > dist) {
                    minDistance = dist;
                    closestRoid = roid;
                }
            }
            //Debug.Log("Found roid " + closestRoid.transform.position + " min distance: " + minDistance);
            Vector3 dir = closestRoid.transform.position - transform.position;
            Vector3 normDir = Vector3.Normalize(dir);
            miningPoint = transform.position + normDir * (minDistance - DIST_TO_ROID);
            //Debug.Log("Dir " + dir + " norm dir: " + normDir + " miningPoint: " + miningPoint + " my pos: " + transform.position);
            miningRoidPoint = closestRoid.transform.position;
            miningRoid = closestRoid;

            state = STATE.ROTATING;
        } else if(state == STATE.ROTATING) {
            // float singleStep = 1.0f * Time.deltaTime;
            // Vector3 target = miningPoint - transform.position;
            // if(Math.Abs(target.x) < 0.1f && Math.Abs(target.y) < 0.1f && Math.Abs(target.z) < 0.1f) {
            //     // bugfix for situation, when two roids spawned at same point, and ship was already in mining position
            //     state = STATE.MOVING;
            // } else {
            //     Vector3 newDirection = Vector3.RotateTowards(transform.up, target, singleStep, 0.0f);
            //     newDirection.z = 0f;
            //     //transform.rotation = Quaternion.LookRotation(Vector3.forward, newDirection);
            //     transform.rotation = Quaternion.Euler(newDirection);
            //     diff = Vector3.Angle(target, transform.up);
            //     if(diff < 0.5f) {
            //         state = STATE.MOVING;
            //     }
            // }

            float singleStep = 40.0f * shipRotation * Time.deltaTime;
            Vector3 target = miningPoint - transform.position;
            //Debug.Log("Mining point: " + miningPoint + " pos: " + transform.position + " Target: " + target + " current up: " + transform.up);
            float prevDiff = diff;
            diff = Vector3.SignedAngle(target, transform.up, Vector3.forward);
            if(Math.Abs(diff) < 1f) {
                state = STATE.MOVING;
                singleStep = prevDiff;
            }
            if(diff > 0) {
                currentRotation.z -= singleStep;
            } else {
                currentRotation.z += singleStep;
            }
            transform.rotation = Quaternion.Euler(currentRotation);
            
        } else if(state == STATE.MOVING) {
            float currentShipSpeed = 0.01f * shipSpeed;
            if(speedBoostUntil != null) {
                //TODO: make this variable?
                currentShipSpeed *= 4;
            }
            transform.position = Vector3.MoveTowards(transform.position, miningPoint, currentShipSpeed);
            if(dragOrigin == null && !cameraIsDragged) {
                updateCameraPosition();
            }

            float dist = Vector3.Distance(transform.position, miningPoint);
            if(dist <= 0) {
                if(directMoving == true) {
                    state = STATE.IDLE;
                    directMoving = false;
                    var moveTargetLineRenderer = moveTargetLine.GetComponent<LineRenderer>();
                    moveTargetLineRenderer.positionCount = 0;
                } else {
                    state = STATE.MINING;
                    Vector3[] positions = new Vector3[2] { miningPoint, miningRoidPoint};
                    lineRenderer.positionCount = 2;
                    lineRenderer.SetPositions(positions);
                    sparksEffect.SetActive(true);
                    Vector3 tmp2 = miningRoidPoint;
                    tmp2.z = -1f;
                    sparksEffect.transform.position = tmp2;
                }
            }
        } else if(state == STATE.MINING) {
            var roid = miningRoid.GetComponent<Roid>();
            roid.oreAmount -= miningPower;
            roid.refreshOreAmount();
            if(roid.oreAmount <= 0) {
                Destroy(miningRoid);
                Init.roidList.Remove(miningRoid);
                clearEffects();
                state = STATE.IDLE;
                coins += (int)Math.Round(roid.initialOreAmount, 0);
                canvasController.refreshCoinsText();
            }
        }
    }

    public void clearEffects() {
        lineRenderer.positionCount = 0;
        sparksEffect.SetActive(false);
    }

    public bool upgradeMiningPower() {
        if(coins - miningPowerUpgrade >= 0) {
            miningPower += 0.1f;
            coins -= miningPowerUpgrade;
            miningPowerUpgrade *= 2;
            return true;
        } else {
            return false;
        }
    }

    public bool upgradeShipSpeed() {
        if(coins - shipSpeedUpgrade >= 0) {
            shipSpeed += 0.1f;
            coins -= shipSpeedUpgrade;
            shipSpeedUpgrade *= 2;
            return true;
        } else {
            return false;
        }
    }

    public bool upgradeShipRotation() {
        if(coins - shipRotationUpgrade >= 0) {
            shipRotation += 0.2f;
            coins -= shipRotationUpgrade;
            shipRotationUpgrade *= 2;
            return true;
        } else {
            return false;
        }
    }

    private void handleControls() {
        if(!Globals.inputDisabled && !Globals.panelOpened) {
            if (Input.GetMouseButtonDown(0)) {
                Vector3 tmp = Input.mousePosition;
                tmp.z = 0f;
                dragOrigin = tmp;
                return;
            } else if (Input.GetMouseButtonUp(0) && dragOrigin != null) {
                Vector3 tmp = Input.mousePosition;
                tmp.z = 0f;
                float dist = Vector3.Distance(dragOrigin.Value, tmp);
//                Debug.Log("Got dist after mouse down and up: " + dist);
                if(dist < dragThreshold) {
                    tmp = Camera.main.ScreenToWorldPoint(tmp);
                    tmp.z = 0f;
                    // Direct move command
                    miningPoint = tmp;
                    state = STATE.ROTATING;
                    directMoving = true;
                    clearEffects();
                }
                dragOrigin = null;
                return;
            }
            if (!Input.GetMouseButton(0)) return;
            Vector3 tmp2 = Input.mousePosition;
            tmp2.z = 0f;
            float dist2 = Vector3.Distance(dragOrigin.Value, tmp2);
            if(dist2 > dragThreshold) {
                canvasController.startDrag();
                cameraIsDragged = true;
                //Debug.Log("dist from origin: " + dist2 + " cur mouse pos: " + tmp2 + " origin: " + dragOrigin + " diff: " + (tmp2 - dragOrigin.Value));
                Vector3 pos = Camera.main.ScreenToViewportPoint(tmp2 - dragOrigin.Value);
                Vector3 move = new Vector3(pos.x * dragSpeed, pos.y * dragSpeed, 0);
    
                Camera.main.transform.Translate(move, Space.World);  
            }
        }

    }

    private void updateCameraPosition() {
        Vector3 tmp = transform.position;
        tmp.z = -10f;
        gameCamera.transform.position = tmp;
    }

    public void resetDrag() {
        cameraIsDragged = false;
        updateCameraPosition();
    }

    public void invalidateBoosts() {
        if(speedBoostUntil != null && Time.time > speedBoostUntil) {
            speedBoostUntil = null;
        }
    }

}
