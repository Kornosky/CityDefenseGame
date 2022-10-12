using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // base components
    [HideInInspector] public Transform myTransform;
    [HideInInspector] public GameObject myGameObject;

    // ref transform
    [HideInInspector] public Transform refTransform;

    // tech
    Vector3 surfaceFwd;
    Vector3 surfaceUp;

    Vector3 fwdOnSurfaceTarget, fwdOnSurface;
    Vector3 rightOnSurfaceTarget, rightOnSurface;
    Vector3 upOnSurfaceTarget, upOnSurface;

    // settings
    [Header("settings")]
    public float gravity;
    public float jumpForce;
    public float wallJumpForce;
    public float terminalVelocity;
    public float maxSpeed;
    public float turnSpeed;
    public float slopeForce;
    public float slopeForceRayLength;
    float slopeAngle;
    float speedFacTarget, speedFacCur;
    float turnSpeedFacTarget, turnSpeedFacCur;
    Vector3 upDir;

    // forced focus point

    // camera
    [Header("camera")]
    public Transform camTransform;

    [HideInInspector] public Vector3 totalMoveDir;
    [HideInInspector] public Vector3 gravDir;
    [HideInInspector] public float yVelocity;
    [HideInInspector] public float velocity;
    float airForward;
    float airRight;
    float acceleration;
    float extraSpeed;

    [HideInInspector] public bool grounded;
    bool startedJump;
    bool isHoldingJump;
    bool isSliding;
    bool canSlide;
    bool canWallJump;
    bool movingIntoCollision;
    bool wasGrounded;

    [HideInInspector] public Movement myCharacterController;
    [HideInInspector] public bool isGrounded;
    [HideInInspector] public float characterHeight;
    [HideInInspector] public Vector3 originalPos, localMove, localMovePrev, colNormal, airMovement;

    Vector3 wallRunLocalMove;
    bool inHorizontalWallRun;
    bool wallRunHorizontalLimit, wallRunVerticalLimit;
    float wallRunDir;

    // extra force
    Vector3 extraForceTarget;
    Vector3 extraForceCur;

    // visuals
    [Header("visuals")]
    public Material bodyMat;
    public Material skinMat;
    public Material[] legMat;
    public Material[] armMat;
    public Mesh legMidMesh;
    public Mesh armMidMesh;

    // head
    [HideInInspector] public GameObject headObject;
    [HideInInspector] public Transform headTransform;

    // body
    [HideInInspector] public GameObject bodyObject;
    [HideInInspector] public Transform bodyTransform;
    float bodyHeightTarget, bodyHeight;
    float bodyAngularRotTarget, bodyAngularRot;
    float bodyForwardRotTarget, bodyForwardRotCur;

    // legs
    [HideInInspector] public List<List<TubeRenderer>> legTubeRenderers;
    [HideInInspector] public List<List<Vector3>> legPoints, legPointsTarget;
    [HideInInspector] public List<List<Transform>> legMidTransforms;
    [HideInInspector] public List<bool> legHitWall, legHitOtherLedge;
    [HideInInspector] public List<GameObject> legDustSlideParticleObjects;
    [HideInInspector] public List<Transform> legDustSlideParticleTransforms;
    [HideInInspector] public List<ParticleSystem> legDustSlideParticleSystems;

    // arms
    [HideInInspector] public List<List<TubeRenderer>> armTubeRenderers;
    [HideInInspector] public List<List<Vector3>> armPoints, armPointsTarget;
    [HideInInspector] public List<List<Transform>> armMidTransforms;
    [HideInInspector] public List<GameObject> armDustSlideParticleObjects;
    [HideInInspector] public List<Transform> armDustSlideParticleTransforms;
    [HideInInspector] public List<ParticleSystem> armDustSlideParticleSystems;

    // animation
    bool moving;
    bool running;
    bool hamtaroMode;

    int legTurn;
    int stepRate, stepCounter;

    int climbTurn;
    int climbRate, climbCounter;

    int wallRunTurn;
    int wallRunRate, wallRunCounter;
    int stopWallRunDur, stopWallRunCounter;

    float jumpMotionFacTarget, jumpMotionFacCur;
    float fallMotionFacTarget, fallMotionFacCur;

    int hardSetJointsDur, hardSetJointsCounter;

    // stunned
    Vector3 stunnedFwd;
    int stunnedDur, stunnedCounter;

    // looking
    [HideInInspector] public Transform lookPointTarget;

    // state
    public enum State { Normal, Jumping, Falling, Stunned, Dead };
    public State curState;

    // layerMasks
    [Header("layerMasks")]
    public LayerMask climbLayerMask;
    public LayerMask solidLayerMask;
    public LayerMask collideLayerMask;
    public LayerMask surfaceMask;
    public LayerMask deadLayerMask;

    // audio
    [HideInInspector] public AudioSource movementAudioSource0;
    [HideInInspector] public AudioSource playerAudioSource0;

    void Start()
    {
        //hamtaroMode = true;

        // get base components
        myTransform = transform;
        myGameObject = gameObject;

        upDir = Vector3.up;

        // starting parameters
        totalMoveDir = Vector3.zero;
        acceleration = 2.5f;
        extraSpeed = 1f;

        // animation
        legTurn = 0;
        stepRate = 14;
        stepCounter = 0;

        climbTurn = 0;
        climbRate = 10;
        climbCounter = climbRate;

        wallRunTurn = 0;
        wallRunRate = 6;
        wallRunCounter = wallRunRate;

        // hard set?
        hardSetJointsDur = 10;
        hardSetJointsCounter = 0;

        // speed
        speedFacTarget = 1f;
        speedFacCur = speedFacTarget;

        turnSpeedFacTarget = 1f;
        turnSpeedFacCur = turnSpeedFacTarget;

        // get character controller
        myCharacterController = GetComponent<Movement>();
        characterHeight =12f;
        isGrounded = true;

        //Get position that character starts game at 
        originalPos = myTransform.position;
        colNormal = Vector3.zero;

        // create ref transform
        GameObject refO = new GameObject("refO");
        refTransform = refO.transform;

        // create body
        bodyObject = PrefabManager.instance.SpawnPrefabAsGameObject(PrefabManager.instance.playerBodyPrefabs[0], myTransform.position, Quaternion.identity, .25f);
        bodyTransform = bodyObject.transform;

        // create head
        headObject = PrefabManager.instance.SpawnPrefabAsGameObject(PrefabManager.instance.playerHeadPrefabs[0], myTransform.position, Quaternion.identity, .1f);
        headTransform = headObject.transform;

        // create legs
        legTubeRenderers = new List<List<TubeRenderer>>();
        legPoints = new List<List<Vector3>>();
        legPointsTarget = new List<List<Vector3>>();
        legMidTransforms = new List<List<Transform>>();
        legHitWall = new List<bool>();
        legHitOtherLedge = new List<bool>();
        legDustSlideParticleObjects = new List<GameObject>();
        legDustSlideParticleTransforms = new List<Transform>();
        legDustSlideParticleSystems = new List<ParticleSystem>();

        for (int i = 0; i < 2; i++)
        {
            legTubeRenderers.Add(new List<TubeRenderer>());
            legPoints.Add(new List<Vector3>());
            legPointsTarget.Add(new List<Vector3>());
            legMidTransforms.Add(new List<Transform>());
            legHitWall.Add(false);
            legHitOtherLedge.Add(false);

            // leg settings
            float legStartRadius = .0575f;
            float legEndRadius = .0325f;

            // tubes
            for (int ii = 0; ii < 3; ii++)
            {
                GameObject legO = new GameObject("legO" + i.ToString());
                Transform legTr = legO.transform;
                BasicFunctions.ResetTransform(legTr);

                TubeRenderer legTubeRenderer = legO.AddComponent<TubeRenderer>();
                legTubeRenderer.crossSegments = 6;
                legTubeRenderer.material = (ii == 0) ? legMat[0] : legMat[1];

                float legTubeRadius = (ii == 0) ? legStartRadius : legEndRadius;
                legTubeRenderer.radius = legTubeRadius;

                legTubeRenderer.receiveShadows = true;
                legTubeRenderer.castShadows = true;

                legTubeRenderer.forceDraw = true;

                legTubeRenderers[i].Add(legTubeRenderer);
                legPoints[i].Add(Vector3.zero);
                legPointsTarget[i].Add(Vector3.zero);
            }

            // mid transforms
            for (int ii = 0; ii < 4; ii++)
            {
                // create leg mid transform
                GameObject legMidO = new GameObject("legMidO");
                Transform legMidTr = legMidO.transform;

                float legMidScl = (ii <= 1) ? legStartRadius : legEndRadius;
                legMidTr.localScale = Vector3.one * legMidScl;

                MeshFilter legMidMf = legMidO.AddComponent<MeshFilter>();
                legMidMf.mesh = legMidMesh;

                MeshRenderer legMidMr = legMidO.AddComponent<MeshRenderer>();
                legMidMr.material = (ii <= 1) ? legMat[0] : legMat[1];

                legMidTransforms[i].Add(legMidTr);
            }

            // leg dust slide particles
            GameObject legDustSlideO = PrefabManager.instance.SpawnPrefabAsGameObject(PrefabManager.instance.dustSlidePrefabs[1], myTransform.position, Quaternion.identity, .25f);
            Transform legDustSlideTr = legDustSlideO.transform;

            legDustSlideParticleTransforms.Add(legDustSlideTr);
            legDustSlideParticleObjects.Add(legDustSlideO);
            legDustSlideParticleSystems.Add(legDustSlideO.GetComponent<ParticleSystem>());
        }

        // create arms
        armTubeRenderers = new List<List<TubeRenderer>>();
        armPoints = new List<List<Vector3>>();
        armPointsTarget = new List<List<Vector3>>();
        armMidTransforms = new List<List<Transform>>();
        armDustSlideParticleObjects = new List<GameObject>();
        armDustSlideParticleTransforms = new List<Transform>();
        armDustSlideParticleSystems = new List<ParticleSystem>();

        for (int i = 0; i < 2; i++)
        {
            armTubeRenderers.Add(new List<TubeRenderer>());
            armPoints.Add(new List<Vector3>());
            armPointsTarget.Add(new List<Vector3>());
            armMidTransforms.Add(new List<Transform>());

            // arm settings
            float armStartRadius = .0575f;
            float armEndRadius = .0325f;

            // tubes
            for (int ii = 0; ii < 3; ii++)
            {
                GameObject armO = new GameObject("armO" + i.ToString());
                Transform armTr = armO.transform;
                BasicFunctions.ResetTransform(armTr);

                TubeRenderer armTubeRenderer = armO.AddComponent<TubeRenderer>();
                armTubeRenderer.crossSegments = 6;
                armTubeRenderer.material = (ii == 0) ? armMat[0] : armMat[1];

                float armTubeRadius = (ii == 0) ? armStartRadius : armEndRadius;
                armTubeRenderer.radius = armTubeRadius;

                armTubeRenderer.receiveShadows = true;
                armTubeRenderer.castShadows = true;

                armTubeRenderer.forceDraw = true;

                armTubeRenderers[i].Add(armTubeRenderer);
                armPoints[i].Add(Vector3.zero);
                armPointsTarget[i].Add(Vector3.zero);
            }

            // mid transforms
            for (int ii = 0; ii < 3; ii++)
            {
                // create arm mid transform
                GameObject armMidO = new GameObject("armMidO");
                Transform armMidTr = armMidO.transform;

                float armMidScl = (ii <= 1) ? armStartRadius : armEndRadius;
                armMidTr.localScale = Vector3.one * armMidScl;

                MeshFilter armMidMf = armMidO.AddComponent<MeshFilter>();
                armMidMf.mesh = armMidMesh;

                MeshRenderer armMidMr = armMidO.AddComponent<MeshRenderer>();
                armMidMr.material = (ii <= 1) ? armMat[0] : armMat[1];

                armMidTransforms[i].Add(armMidTr);
            }

            // arm dust slide particles
            GameObject armDustSlideO = PrefabManager.instance.SpawnPrefabAsGameObject(PrefabManager.instance.dustSlidePrefabs[1], myTransform.position, Quaternion.identity, .25f);
            Transform armDustSlideTr = armDustSlideO.transform;

            armDustSlideParticleTransforms.Add(armDustSlideTr);
            armDustSlideParticleObjects.Add(armDustSlideO);
            armDustSlideParticleSystems.Add(armDustSlideO.GetComponent<ParticleSystem>());
        }


        // state
        SetState(State.Normal);
        //SetState(State.Sitting);



        // audio
        //movementAudioSource0 = AudioManager.instance.CreateAudioSource(true,false,false,myTransform,0f,10f);
        // playerAudioSource0 = AudioManager.instance.CreateAudioSource(true, false, false, myTransform, 0f, 10f);
    }

    void Update()
    {
        // Cast a sphere wrapping character controller 10 meters forward
        // to see if it is about to hit anything.
        grounded = isGrounded;

        float maxVel = 5f;
        velocity = Mathf.Clamp(velocity, -maxVel, maxVel);

        if (!GameManager.Instance.paused)
        {
            // are we moving into a collision?
            if (curState == State.Normal || curState == State.Jumping || curState == State.Falling)
            {
                float cHeight = (characterHeight * .325f);
                float cDst = .375f;
                if (!grounded)
                {
                    cDst = .25f;
                }
                Vector3 c0 = myTransform.position;
                c0.y += cHeight;
                Vector3 cDir = totalMoveDir.normalized;
                cDir.y = 0f;
                Vector3 c1 = myTransform.position + (cDir * cDst);
                c1.y += cHeight;
                c1.y = c0.y;
                RaycastHit cHit;
                if (Physics.Linecast(c0, c1, out cHit, collideLayerMask))
                {
                    if (!movingIntoCollision)
                    {
                        movingIntoCollision = true;

                        // running into something solid? get stunned?
                        if ((running && (maxSpeed >= 2.75f)) || (curState == State.Jumping || curState == State.Falling))
                        {
                            // get stunned surface normal
                            stunnedFwd = -cHit.normal.normalized;
                            stunnedFwd.y = 0f;
                            myTransform.forward = stunnedFwd;

                            // enter stunned state?
                            if (!grounded)
                            {
                                float velFac = .375f;

                                velocity *= -velFac;
                            }
                            SetState(State.Stunned);

                            // white orb impact particle
                            GameObject whiteOrbO = PrefabManager.instance.SpawnPrefabAsGameObject(PrefabManager.instance.whiteOrbPrefabs[0], cHit.point, Quaternion.identity, .375f);
                            Destroy(whiteOrbO, .1f);
                        }
                    }
                }
                else
                {
                    movingIntoCollision = false;
                }

                // debug
                //Debug.DrawLine(c0, c1, Color.white, 5f);
            }

            // update speed factor
            float speedFacLerpie = 30f;
            speedFacCur = Mathf.Lerp(speedFacCur, speedFacTarget, speedFacLerpie * Time.deltaTime);

            // update turn speed factor
            float turnSpeedFacLerpie = 20f;
            turnSpeedFacCur = Mathf.Lerp(turnSpeedFacCur, turnSpeedFacTarget, turnSpeedFacLerpie * Time.deltaTime);

            // stunned?
            if (curState == State.Stunned)
            {
                //Debug.Log("stunned! || " + stunnedCounter.ToString() + "/" + stunnedDur.ToString() + " || " + Time.time.ToString());

                if (stunnedCounter < stunnedDur)
                {
                    stunnedCounter++;
                }
                else
                {
                    SetState(State.Normal);
                }
            }

            // hard set joints?
            if (hardSetJointsCounter < hardSetJointsDur)
            {
                hardSetJointsCounter++;
            }

            // handle jump motion
            jumpMotionFacTarget = Mathf.Lerp(jumpMotionFacTarget, 0f, 20f * Time.deltaTime);
            jumpMotionFacCur = Mathf.Lerp(jumpMotionFacCur, jumpMotionFacTarget, 10f * Time.deltaTime);

            // handle fall motion
            //fallMotionFacTarget = Mathf.Lerp(fallMotionFacTarget, 0f, 20f * Time.deltaTime);
            fallMotionFacCur = Mathf.Lerp(fallMotionFacCur, fallMotionFacTarget, 10f * Time.deltaTime);


            // transform movement direction
            TransformMoveDirection();

            // move animation?
            float moveThreshold = .1f;
            moving = true;

            float maxSpeedTarget = 1.75f; //HERE

            maxSpeed = Mathf.Lerp(maxSpeed, maxSpeedTarget, 10f * Time.deltaTime);
            if (curState == State.Stunned && grounded)
            {
                maxSpeed *= speedFacCur;
            }

            turnSpeed = 30f * turnSpeedFacCur;

            stepRate = (running) ? 8 : 14;

            if (stepCounter < stepRate)
            {
                stepCounter++;
            }
            if (true)//moving
            {
                if (stepCounter >= stepRate && (curState == State.Normal))
                {
                    // pin foot
                    legPoints[legTurn][2] = legPointsTarget[legTurn][2];

                    // spawn dust particle
                    if (hardSetJointsCounter >= hardSetJointsDur)
                    {
                        if (running)
                        {
                            //Vector3 particlePos = legPoints[legTurn][2];
                            Vector3 particlePos = legTubeRenderers[legTurn][1].vertices[1].point;
                            Quaternion particleRot = Quaternion.identity;
                            particleRot = Quaternion.LookRotation(-myTransform.forward) * Quaternion.Euler(60f, 0f, 0f);
                            //PrefabManager.instance.SpawnPrefab(PrefabManager.instance.dustPrefabs[0], particlePos, particleRot, .175f);
                        }
                    }

                    // next leg
                    legTurn = (legTurn == 0) ? 1 : 0;
                    stepCounter = 0;

                    // footstep audio source
                    // AudioManager.instance.PlaySound(movementAudioSource0, BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.footstepClips0), .4f, .7f, .225f, .25f, Mathf.Abs(slopeAngle) * 2f);
                }
            }

            // no input at all?
            if (curState == State.Dead)
            {
                moving = false;
                running = false;
                localMove = myTransform.forward;
                localMovePrev = myTransform.forward;
                //totalMoveDir = Vector3.zero;
                totalMoveDir.x = 0f;
                totalMoveDir.z = 0f;
                velocity = 0f;
            }
        }
    }

    private void LateUpdate()
    {
        if (localMove.magnitude > .025f)
        {
            localMovePrev = localMove;
        }
    }

    void FixedUpdate()
    {
        if (!GameManager.Instance.paused)
        {
            // update visuals?
            UpdateVisuals();
        }
    }

    void UpdateVisuals()
    {
        Vector3 bodyUp = upOnSurface;
        Vector3 bodyFwd = fwdOnSurface;
        Vector3 bodyRight = rightOnSurface;

        if (lookPointTarget != null /*&& GameManager.instance.inCutscene*/ )
        {
            //bodyFwd = myTransform.forward;

            Vector3 r0 = bodyTransform.position;
            Vector3 r1 = lookPointTarget.position;
            r1.y = r0.y;
            Vector3 r2 = (r1 - r0).normalized;
            bodyFwd = r2;
        }

        // update body
        if (bodyTransform != null)
        {
            Vector3 bodyPos = myTransform.position;

            bodyHeightTarget = .275f;

            // moving motion
            if (moving && curState == State.Normal)
            {
                float stepP = BasicFunctions.ConvertRange(stepCounter, 0f, stepRate, 0f, 1f);
                stepP = Easing.Quintic.In(stepP);

                bodyHeightTarget += .225f;
                bodyHeightTarget += (Mathf.Cos((stepP - .5f) * 2f) * -.275f);
            }

            float bodyHeightLerpie = 10f;
            bodyHeight = Mathf.Lerp(bodyHeight, bodyHeightTarget, bodyHeightLerpie * Time.deltaTime);

            bodyPos += bodyUp * bodyHeight;
            bodyTransform.position = bodyPos;

            Quaternion bodyTargetRot = Quaternion.LookRotation(bodyFwd, bodyUp);

            float bodyAngularRotFac = 40f;
            if (curState == State.Jumping || curState == State.Falling)
            {
                bodyAngularRotFac = 10f;
            }

            if (curState == State.Dead)
            {
                bodyAngularRotFac = 10f;
            }

            bodyAngularRotTarget = Vector3.Dot(localMove.normalized, bodyRight) * -bodyAngularRotFac;


            if (lookPointTarget != null)
            {
                bodyAngularRotTarget = 0f;//localMove.x * -20f;
            }

            float bodyAngularRotLerpie = 10f;


            bodyAngularRot = Mathf.Lerp(bodyAngularRot, bodyAngularRotTarget, bodyAngularRotLerpie * Time.deltaTime);

            bodyForwardRotTarget = (velocity * 12.5f);

            if (curState == State.Jumping || curState == State.Falling)
            {
                bodyForwardRotTarget = 25f;


            }
            if (curState == State.Dead)
            {
                //bodyForwardRotTarget = -10f;
                float tt0 = Time.time * 10f;
                float ff0 = 20f;

                float ss0 = Mathf.Sin(tt0) * ff0;
                bodyForwardRotTarget = ss0;
            }


            float bodyForwardRotLerpie = 20f;



            bodyForwardRotCur = Mathf.Lerp(bodyForwardRotCur, bodyForwardRotTarget, bodyForwardRotLerpie * Time.deltaTime);

            float bodyRotTargetY = 0f;


            //ResetBodyRot();
            bodyTargetRot *= Quaternion.Euler(bodyForwardRotCur, bodyRotTargetY, bodyAngularRot);

            // extra rot to do the hamtaro run
            if (hamtaroMode && running)
            {
                bodyTargetRot *= Quaternion.Euler(10f, 0f, 0f);
            }


            bodyTransform.rotation = bodyTargetRot;
        }

        // update head
        if (headTransform != null)
        {
            Vector3 headPos = bodyTransform.position;
            headPos += bodyTransform.up * .2f;
            headTransform.position = headPos;

            Quaternion headTargetRot = bodyTransform.rotation; //Quaternion.LookRotation(myTransform.forward);

            headTransform.rotation = Quaternion.Lerp(headTransform.rotation, headTargetRot, 10f * Time.deltaTime);
        }

        // update legs
        UpdateLegs();

        // update arms
        UpdateArms();
    }

    void UpdateLegs()
    {
        Vector3 legUp = upOnSurface;
        Vector3 legFwd = fwdOnSurface;
        Vector3 legRight = rightOnSurface;

        if (lookPointTarget != null)
        {
            legFwd = bodyTransform.forward.normalized;
            legRight = bodyTransform.right.normalized;
            legFwd.y = 0f;
            legRight.y = 0f;
        }

        for (int i = 0; i < 2; i++)
        {
            float legDir = (i == 0) ? 1f : -1f;

            legHitWall[i] = false;
            legHitOtherLedge[i] = false;

            Vector3 pStart, pMid, pEnd, pFoot;

            // leg percentage
            float legP = BasicFunctions.ConvertRange(stepCounter, 0f, stepRate, 0f, 1f);
            legP = Easing.Quintic.Out(legP);

            // define points
            pStart = bodyTransform.position;
            pStart += (bodyTransform.up * -.025f);
            pStart += (bodyTransform.right * (.045f * legDir));

            pEnd = myTransform.position;
            pEnd += (bodyTransform.right * (.0675f * legDir));

            // animation?
            if (moving && (curState == State.Normal))
            {
                float moveStepFac = 1f;
                if (movingIntoCollision)
                {
                    moveStepFac = .1f;
                }

                Vector3 legMoveFwd = legFwd;
                Vector3 legMoveUp = legUp;

                if (lookPointTarget != null)
                {
                    legMoveFwd = localMove;
                }

                if (i == legTurn)
                {
                    if (!running)
                    {
                        float s0 = Mathf.Sin((legP - .5f) * Mathf.PI) * .25f;
                        float s1 = Mathf.Cos((legP - .5f) * Mathf.PI) * .25f;
                        pEnd += (legMoveFwd * (s0 * moveStepFac));
                        pEnd += (legMoveUp * (s1 * moveStepFac));
                    }
                    else
                    {
                        float s0 = Mathf.Sin((legP - .5f) * Mathf.PI) * .375f;
                        float s1 = Mathf.Cos((legP - .5f) * Mathf.PI) * .325f;
                        pEnd += (legMoveFwd * (s0 * moveStepFac));
                        pEnd += (legMoveUp * (s1 * moveStepFac));
                        pEnd += (legMoveFwd * (-.0675f * moveStepFac));
                    }
                }
            }

            // falling?
            if (curState == State.Falling)
            {
                pEnd = myTransform.position;
                pEnd += (bodyTransform.right * (.125f * legDir));
                //pEnd += (bodyTransform.up * .1f);
                pEnd += (bodyTransform.forward * (.175f * legDir));

                // fall motion?
                pEnd += (bodyTransform.forward * .05f);

                float jumpMotionFacCurMax = .25f;
                float jumpMotionFacCurUse = Mathf.Clamp(jumpMotionFacCur, -jumpMotionFacCurMax, jumpMotionFacCurMax);

                pEnd += (bodyTransform.forward * -(jumpMotionFacCurUse * .5f));
                pEnd += (bodyTransform.up * -(jumpMotionFacCurUse * .125f));

                float t0 = (Time.time * 15f) + (float)(legDir);
                float f0 = .1f * fallMotionFacCur;
                f0 = Mathf.Clamp(f0, -.125f, .125f);
                float s0 = Mathf.Sin(t0) * -f0;
                float s1 = Mathf.Cos(t0) * -f0;
                pEnd += (bodyTransform.forward * s0);
                pEnd += (bodyTransform.up * s1);
            }

            // dead?
            if (curState == State.Dead)
            {
                pEnd = myTransform.position;
                pEnd += (bodyTransform.right * (.1f * legDir));
                pEnd += (bodyTransform.up * .125f);

                float t0 = (Time.time * 20f) + (float)(legDir);
                float f0 = .125f;
                float s0 = Mathf.Sin(t0) * -f0;
                float s1 = Mathf.Cos(t0) * -f0;
                pEnd += (bodyTransform.forward * s0);
                pEnd += (bodyTransform.up * s1);
            }

            // stunned?
            if (curState == State.Stunned)
            {
                pEnd = myTransform.position;
                pEnd += (bodyTransform.right * (.0675f * legDir));
                pEnd += (legFwd * .05f);

                if (i == legTurn)
                {
                    pEnd += (legFwd * .125f);
                    pEnd += (legUp * .125f);

                    float t0 = Time.time * 20f;
                    float f0 = .1f;
                    float s0 = Mathf.Sin(t0) * f0;
                    float s1 = Mathf.Cos(t0) * f0;
                    pEnd += (legFwd * s0);
                    pEnd += (legUp * s1);
                }
            }


            pMid = BasicFunctions.LerpByDistance(pStart, legPoints[i][2], Vector3.Distance(pStart, legPoints[i][2]) * .5f);
            pMid += (legFwd * (.075f * legP));


            // falling
            if (curState == State.Falling)
            {
                pMid = BasicFunctions.LerpByDistance(pStart, legPoints[i][2], Vector3.Distance(pStart, legPoints[i][2]) * .5f);
                pMid += (legFwd * .125f);
            }

            // dead
            if (curState == State.Dead)
            {
                pMid = BasicFunctions.LerpByDistance(pStart, legPoints[i][2], Vector3.Distance(pStart, legPoints[i][2]) * .5f);
                pMid += (legFwd * .05f);
            }


            // stunned?
            if (curState == State.Stunned)
            {
                pMid = BasicFunctions.LerpByDistance(pStart, legPoints[i][2], Vector3.Distance(pStart, legPoints[i][2]) * .5f);
                pMid += (legFwd * .05f);
                pMid += (bodyTransform.up * .025f);
            }

            // store points
            legPointsTarget[i][0] = pStart;
            legPointsTarget[i][1] = pMid;
            legPointsTarget[i][2] = pEnd;

            // lerp points
            float legLerpie = 20f;


            //if ( curState == State.WallSlide )
            //{
            //    legLerpie = 30f;
            //}

            legPoints[i][0] = legPointsTarget[i][0];
            legPoints[i][1] = legPointsTarget[i][1];

            bool doLerp = (curState != State.Normal);
            if (i == legTurn /*&& curState != State.Hanging*/ && curState == State.Normal)
            {
                doLerp = true;
            }

            // hard set?
            bool hardSet = false;
            if (hardSetJointsCounter < hardSetJointsDur)
            {
                hardSet = true;
            }

            if (curState == State.Dead)
            {
                hardSet = true;
                doLerp = false;
            }

            if (doLerp)
            {
                legPoints[i][2] = Vector3.Slerp(legPoints[i][2], legPointsTarget[i][2], legLerpie * Time.deltaTime);
            }

            if (hardSet)
            {
                legPoints[i][2] = legPointsTarget[i][2];
            }

            // foot?
            Vector3 footDir = legFwd;

            float footLength = .075f;
            pFoot = legPoints[i][2] + (footDir * footLength);

            // set points
            if (legTubeRenderers != null && legTubeRenderers.Count > 0)
            {
                if (legTubeRenderers[i][0].vertices != null && legTubeRenderers[i][0].vertices.Length > 0)
                {
                    legTubeRenderers[i][0].vertices[0].point = legPoints[i][0];
                    legTubeRenderers[i][0].vertices[1].point = legPoints[i][1];
                }
                if (legTubeRenderers[i][1].vertices != null && legTubeRenderers[i][1].vertices.Length > 0)
                {
                    legTubeRenderers[i][1].vertices[0].point = legPoints[i][1];
                    legTubeRenderers[i][1].vertices[1].point = legPoints[i][2];
                }
                if (legTubeRenderers[i][2].vertices != null && legTubeRenderers[i][2].vertices.Length > 0)
                {
                    legTubeRenderers[i][2].vertices[0].point = legPoints[i][2];
                    legTubeRenderers[i][2].vertices[1].point = pFoot;
                }
            }

            // leg mid transforms
            if (legMidTransforms != null && legMidTransforms.Count > 0)
            {
                legMidTransforms[i][0].position = legPoints[i][0];
                legMidTransforms[i][1].position = legPoints[i][1];
                legMidTransforms[i][2].position = legPoints[i][2];
                legMidTransforms[i][3].position = pFoot;
            }
        }
    }

    void UpdateArms()
    {
        Vector3 armUp = upOnSurface;
        Vector3 armFwd = fwdOnSurface;
        Vector3 armRight = rightOnSurface;

        if (lookPointTarget != null)
        {
            armUp = bodyTransform.up;
            armFwd = bodyTransform.forward;
            armRight = bodyTransform.right;
        }
        for (int i = 0; i < 2; i++)
        {
            float armDir = (i == 0) ? 1f : -1f;
            float armTurnDir = (i == legTurn) ? -1f : 1f;

            Vector3 pStart, pMid, pEnd;

            // define points
            pStart = bodyTransform.position;
            pStart += (bodyTransform.up * .15f);
            pStart += (bodyTransform.right * (.0825f * armDir));

            pEnd = bodyTransform.position;
            pEnd += (bodyTransform.right * (.25f * armDir));
            //pEnd += (bodyTransform.up * .025f);


            // animation?
            if (moving)
            {
                float moveStepFac = 1f;
                if (movingIntoCollision)
                {
                    moveStepFac = .1f;
                }

                pEnd += (armFwd * (.1f * moveStepFac));

                if (running)
                {
                    if (!hamtaroMode)
                    {
                        pEnd += (armUp * (.075f * moveStepFac));
                        pEnd += (armRight * ((.05f * armDir) * moveStepFac));
                        pEnd += (armFwd * ((.1f * armTurnDir) * moveStepFac));
                    }
                    else
                    {
                        pEnd += (armUp * (.175f * moveStepFac));
                        pEnd += (armRight * ((-.025f * armDir) * moveStepFac));
                        pEnd += (armFwd * ((.025f * armTurnDir) * moveStepFac));
                        pEnd += (armFwd * (-.1675f * moveStepFac));
                    }
                }
                else
                {
                    pEnd += (armFwd * ((.15f * armTurnDir) * moveStepFac));
                }
            }

            // falling?
            if (curState == State.Falling)
            {
                pEnd = bodyTransform.position;
                pEnd += (bodyTransform.right * (.25f * armDir));
                pEnd += (bodyTransform.up * .125f);
                pEnd += (bodyTransform.forward * (.125f * -armDir));

                // fall motion?
                pEnd += (bodyTransform.forward * .125f);
                pEnd += (bodyTransform.forward * -(jumpMotionFacCur * .25f));
                pEnd += (bodyTransform.up * -(jumpMotionFacCur * .075f));

                float armVelocityAdd = (yVelocity * .0175f);
                float armVelocityAddMax = .1f;
                pEnd += (bodyTransform.up * -Mathf.Clamp(armVelocityAdd, -armVelocityAddMax, armVelocityAddMax));

                // circular motion
                float t0 = (Time.time * 20f) - (float)(armDir);
                float f0 = .05f * fallMotionFacCur;
                float s0 = Mathf.Sin(t0) * -f0;
                float s1 = Mathf.Cos(t0) * -f0;
                pEnd += (bodyTransform.forward * s0);
                pEnd += (bodyTransform.up * s1);
            }

            // dead?
            if (curState == State.Dead)
            {
                pEnd = bodyTransform.position;
                pEnd += (bodyTransform.right * (.325f * armDir));
                pEnd += (bodyTransform.up * .25f);

                // circular motion
                float t0 = (Time.time * 20f) - (float)(armDir);
                float f0 = .125f;
                float s0 = Mathf.Sin(t0) * -f0;
                float s1 = Mathf.Cos(t0) * -f0;
                pEnd += (bodyTransform.forward * s0);
                pEnd += (bodyTransform.up * s1);
            }

            // stunned?
            if (curState == State.Stunned)
            {
                pEnd = bodyTransform.position;
                pEnd += (bodyTransform.right * (.325f * armDir));
                pEnd += (bodyTransform.up * .125f);
                pEnd += (bodyTransform.forward * .025f);

                float t0 = Time.time * 20f;
                float f0 = .1f;
                float s0 = Mathf.Sin(t0) * f0;
                float s1 = Mathf.Cos(t0) * f0;
                pEnd += (bodyTransform.forward * s0);
                pEnd += (bodyTransform.up * (s1 * armDir));
            }


            pMid = BasicFunctions.LerpByDistance(pStart, armPoints[i][2], Vector3.Distance(pStart, armPoints[i][2]) * .5f);
            pMid += (armFwd * -.025f);
            pMid += (armRight * (.0125f * armDir));

            // elbow when moving?
            if (moving)
            {
                pMid += (armFwd * -.05f);// * (1f - armP)));
                if (running)
                {
                    pMid += (armFwd * .075f);
                }
            }

            // elbow when jumping?
            if (curState == State.Jumping)
            {
                pMid += (armFwd * -.075f);
            }

            // elbow when falling?
            if (curState == State.Falling)
            {
                pMid += (armFwd * -.075f);
            }

            // elbow when dead?
            if (curState == State.Dead)
            {
                pMid = BasicFunctions.LerpByDistance(pStart, armPoints[i][2], Vector3.Distance(pStart, armPoints[i][2]) * .5f);
                pMid += (armUp * -.05f);
            }

            // elbow when stunned?
            if (curState == State.Stunned)
            {
                pMid = BasicFunctions.LerpByDistance(pStart, armPoints[i][2], Vector3.Distance(pStart, armPoints[i][2]) * .5f);
            }

            // store points
            armPointsTarget[i][0] = pStart;
            armPointsTarget[i][1] = pMid;
            armPointsTarget[i][2] = pEnd;

            // lerp points
            float armLerpie = 20f;

            if (curState == State.Jumping || curState == State.Falling || curState == State.Dead)
            {
                armLerpie = 30f;
            }

            armPoints[i][0] = armPointsTarget[i][0];
            armPoints[i][1] = armPointsTarget[i][1];

            bool doLerp = true;

            // hard set?
            bool hardSet = false;

            if (doLerp)
            {
                armPoints[i][2] = Vector3.Slerp(armPoints[i][2], armPointsTarget[i][2], armLerpie * Time.deltaTime);
            }

            if (hardSet)
            {
                armPoints[i][2] = armPointsTarget[i][2];
            }

            // set points
            if (armTubeRenderers != null && armTubeRenderers.Count > 0)
            {
                if (armTubeRenderers[i][0].vertices != null && armTubeRenderers[i][0].vertices.Length > 0)
                {
                    armTubeRenderers[i][0].vertices[0].point = armPoints[i][0];
                    armTubeRenderers[i][0].vertices[1].point = armPoints[i][1];
                }
                if (armTubeRenderers[i][1].vertices != null && armTubeRenderers[i][1].vertices.Length > 0)
                {
                    armTubeRenderers[i][1].vertices[0].point = armPoints[i][1];
                    armTubeRenderers[i][1].vertices[1].point = armPoints[i][2];
                }
            }

            // arm mid transforms
            if (armMidTransforms != null && armMidTransforms.Count > 0)
            {
                armMidTransforms[i][0].position = armPoints[i][0];
                armMidTransforms[i][1].position = armPoints[i][1];
                armMidTransforms[i][2].position = armPoints[i][2];
            }
        }
    }
    //void ResetBodyRot()
    //{
    //            bodyAngularRotTarget = 0f;
    //            bodyForwardRotTarget = 0f;
    //            bodyRotTargetY = 0f;
    //            bodyForwardRotCur = 0f;
    //            bodyAngularRot = 0f;
    //}

    void TransformMoveDirection()
    {
        // get forward
        Vector3 lookDir = Vector2.right;
        lookDir.y = 0;
        lookDir = lookDir.normalized;

        // get right vector
        Vector3 right = new Vector3(0,0,1);
        right.y = 0;
        right = right.normalized;

        // wall jump exploit?
        bool updateLocalMove = true;

        if (updateLocalMove)
        {
            //localMove.x = (PlayerInput.instance.inputH * right.x) + (PlayerInput.instance.inputV * lookDir.x);
            //localMove.z = (PlayerInput.instance.inputH * right.z) + (PlayerInput.instance.inputV * lookDir.z);
        }
        else
        {
            localMove.x = Mathf.Lerp(localMove.x,0f,20f * Time.deltaTime);
            localMove.z = Mathf.Lerp(localMove.z,0f,20f * Time.deltaTime);
        }
        localMove.y = 0;


        // normalize
        localMove = localMove.normalized;

        // debug
        //Debug.Log("uhh hallo? || " + localMove + " || " + Time.time.ToString());
    }

    public void SetState ( State _to )
    {
        GameObject gameObjectSet = myGameObject;

        speedFacTarget = 1f;
        speedFacCur = speedFacTarget;

        //upDir = Vector3.up;

        // do something specific?
        switch ( _to )
        {
                default:
            
                    gameObjectSet.layer = 10;

                    speedFacTarget = 1f;

                    turnSpeedFacTarget = 1f;

                break;

            case State.Dead:

                gameObjectSet.layer = 11;

                // player dead audio
                //AudioManager.instance.PlaySound(playerAudioSource0, BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.playerHitClips0), 1.2f, 1.6f, .5f, .525f, 0f);

                break;


            case State.Normal:

                gameObjectSet.layer = 10;

                speedFacTarget = 1f;

                turnSpeedFacTarget = 1f;

                // reset upDir here?
                upDir = Vector3.up;

                break;

            case State.Falling:

                gameObjectSet.layer = 10;

                fallMotionFacTarget = 1f;
                fallMotionFacCur = 0;

                upDir = Vector3.up;

            break;

            case State.Stunned:

                gameObjectSet.layer = 10;

                speedFacTarget = 0f;
                speedFacCur = speedFacTarget;

                stunnedDur = 20;
                stunnedCounter = 0;

                // player stunned audio
               // AudioManager.instance.PlaySound(playerAudioSource0, BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.playerHitClips0), 1.4f, 1.8f, .3f, .325f, 0f);

                // player stunned audio?
                //AudioManager.instance.PlaySound(playerAudioSource0, BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.playerStunnedClips0), 1.4f, 1.8f, .425f, .45f, 0f);

                break;

        }

        // set
        curState = _to;

        // debug
        //Debug.Log("speed factor: " + speedFacCur.ToString() + "/" + speedFacTarget.ToString() + " || " + Time.time.ToString());

        // debug
        //Debug.Log("set state to: " + _to + " || " + Time.time.ToString());
    }

}
