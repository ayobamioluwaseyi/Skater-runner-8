using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
   public static LevelManager Instance { get; set; }

    private Transform cameraContainer;

    public const float DISTANCE_BEFORE_SPAWN = 100f;
    private const int INITIAL_SEGMENTS = 10;
    private const int INITIAL_TRANSITION_SEGMENTS = 2;
    private const int MAX_SEGMENTS_ON_SCREEN = 15;
    public bool SHOW_COLLIDER = true; //$$

    private int amountOfActiveSegments;
    private int continiousSegments;
    private int currentSpawnZ;
    private int currentLevel;
    private int y1,y2,y3;

    // Level Spawning - List of pieces
    public List<Piece> ramps = new List<Piece>();
    public List<Piece> longBlocks = new List<Piece>();
    public List<Piece> jumps = new List<Piece>();
    public List<Piece> slides = new List<Piece>();
    [HideInInspector]
    public List<Piece> pieces = new List<Piece>(); // All the pieces in the pool

    //List of Segments
    public List<Segment> availableSegments = new List<Segment> ();
    public List<Segment> availableTransitions = new List<Segment> ();
    [HideInInspector]
    public List<Segment> segments = new List<Segment>();

    //Game Play
    //private bool isMoving = false;

    private void Awake()
    {
        Instance = this;
        cameraContainer = Camera.main.transform;
        currentSpawnZ = 0;
        currentLevel = 0;
    }

    private void Start()
    {
        //spawn segments
        for (int i = 0; i < INITIAL_SEGMENTS; i++)
        {
            if (i < INITIAL_TRANSITION_SEGMENTS)
            {
                // spawn empty spaces in front so the player can have some space before game start
                SpawnTransition();
            }
            else
            {
                // Generate Segments
                GenerateSegment();
            }
        }
    }

    private void Update()
    {
        // show objects when camera gets closer to them
        if(currentSpawnZ - cameraContainer.position.z < DISTANCE_BEFORE_SPAWN)
        {
            GenerateSegment();
        }
        // delete spawned elements when we pass them
        if (amountOfActiveSegments >= MAX_SEGMENTS_ON_SCREEN)
        {
            segments[amountOfActiveSegments - 1].Despawn();
            amountOfActiveSegments--;
        }
    }

    private void GenerateSegment()
    {
        SpawnSegment();

        if (Random.Range(0f,1f) < (continiousSegments * 0.25f))
        {
            // Spawn Transition Segment
            continiousSegments = 0;
            SpawnTransition();
        } else
        {
            continiousSegments++;
        }
    }

    private void SpawnSegment()
    {
        // the segments spawned should fit the last one spawned
        List<Segment> possibleSegments = availableSegments.FindAll(x=> x.beginY1 == y1 || x.beginY2 == y2 || x.beginY3 == y3);
        int id = Random.Range(0, possibleSegments.Count);

        Segment s = GetSegment(id, false);

        y1 = s.endY1;
        y2 = s.endY2;
        y3 = s.endY3;

        s.transform.SetParent(transform);
        s.transform.localPosition = Vector3.forward * currentSpawnZ;

        currentSpawnZ += s.length;
        amountOfActiveSegments++;
        s.Spawn();
    }

    private void SpawnTransition()
    {
        List<Segment> possibleTransition = availableTransitions.FindAll(x => x.beginY1 == y1 || x.beginY2 == y2 || x.beginY3 == y3);
        int id = Random.Range(0, possibleTransition.Count);

        Segment s = GetSegment(id, true);

        y1 = s.endY1;
        y2 = s.endY2;
        y3 = s.endY3;

        s.transform.SetParent(transform);
        s.transform.localPosition = Vector3.forward * currentSpawnZ;

        currentSpawnZ += s.length;
        amountOfActiveSegments++;
        s.Spawn();
    }

    public Segment GetSegment(int id, bool transition)
    {
        Segment s = null;
        s = segments.Find(x => x.SegId == id && x.transition == transition && !x.gameObject.activeSelf);
        if(s == null)
        {
            GameObject go = Instantiate((transition) ? availableTransitions[id].gameObject : availableSegments[id].gameObject) as GameObject;
            s = go.GetComponent<Segment>();

            s.SegId = id;
            s.transition = transition;

            segments.Insert(0, s);
        }
        else
        {
            // remove it from the list
            segments.Remove(s);
            // add it to the list but this time at the first index
            segments.Insert(0, s);
        }

        return s;
    }
    public Piece GetPiece(PieceType pt, int visualIndex)
    {
        Piece p = pieces.Find(x => x.type == pt && x.visualIndex == visualIndex && !x.gameObject.activeSelf); // an object that has the same type, visual index and is not active
        if(p == null)
        {
            GameObject go = null;
            //spawn it for reuse later
            if(pt == PieceType.ramp)
            {
                go = ramps[visualIndex].gameObject;
            } else if (pt == PieceType.longBlock)
            {
                go = longBlocks[visualIndex].gameObject;
            }
            else if (pt == PieceType.jump)
            {
                go = jumps[visualIndex].gameObject;
            }
            else if (pt == PieceType.slide)
            {
                go = slides[visualIndex].gameObject;
            }
            
            go = Instantiate(go);
            p = go.GetComponent<Piece>();
            pieces.Add(p);
        }
        return p;
    }
}
