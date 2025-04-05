using UnityEngine;

namespace EasyParallax
{
    /**
 * Creates copies of this object and arranges them seamlessly, so that one is right next to the other.
 */
    [RequireComponent(
        typeof(SpriteRenderer))] //Make sure we have a SpriteRenderer, because we need the width of the sprite
    public class SpriteDuplicator : MonoBehaviour
    {
        /**
     * The number of copies of the object that we will have at any given time, including this object
     */
        [SerializeField] private int poolSize = 5;

        [Tooltip(
            "Determines how long to wait until a sprite is repositioned. If you increase it by 1, the sprites need to go to the left 1 sprite width more before they get repositioned")]
        [SerializeField]
        private int spriteRepositionIndex = 2;

        [Tooltip(
            "Adjust this if you have gaps between your sprites. Smaller correction -> more gap. Bigger correction -> less gap")]
        [SerializeField]
        private float spriteRepositionCorrection = 0.03f;

        /**
     * An array of all current copies of this object, including this object
     */
        private Transform[] duplicatesPool;

        /**
     * Here we keep the width of the sprite, so we can later calculate where to place other objects
     */
        private float spriteWidth;

        private void Start()
        {
            //First, we instantiate a number of objects that we will reuse. This is also called pooling. 
            //We do this, because it's more efficient than creating and destroying objects over and over again.
            duplicatesPool = new Transform[poolSize];

            //Now, we need to get the width of our sprite, because we will position the objects based on how wide they are
            spriteWidth = GetComponent<SpriteRenderer>().bounds.size.x;

            //Let's populate our array with enough items. First add this object, as it's part of the pool.
            duplicatesPool[0] = transform;

            var startingPos = transform.position;

            //Next duplicate this and add the objects, until we fill our pool
            for (var i = 1; i < poolSize; i++)
            {
                var position = new Vector2(CalculateX(startingPos), startingPos.y);
                startingPos = position;
                duplicatesPool[i] = Instantiate(gameObject, position, Quaternion.identity, transform.parent).transform;
                //It's very important to remove the sprite duplicator script from the copied object
                //Otherwise we will get an infinite loop of sprite duplication
                Destroy(duplicatesPool[i].GetComponent<SpriteDuplicator>());
            }
        }


        private void Update()
        {
            //We need to check if any of our sprites has gone of screen. 
            //We will reposition it to be on the right side if it is
            foreach (var duplicate in duplicatesPool)
            {
                if (duplicate.transform.position.x < -spriteWidth * spriteRepositionIndex)
                {
                    //In order to reposition it, we need to get which sprite is the rightmost currently
                    var rightmostSprite = GetRightMostSprite();

                    //Let's position it to the right of that sprite
                    var startingPos = rightmostSprite.position;
                    var position =
                        new Vector2(CalculateX(startingPos), startingPos.y);
                    duplicate.transform.position = position;
                }
            }
        }

        private float CalculateX(Vector3 startingPos)
        {
            return Mathf.FloorToInt(startingPos.x + spriteWidth) -
                   spriteRepositionCorrection * transform.lossyScale.magnitude;
        }

        private Transform GetRightMostSprite()
        {
            var rightmostX = Mathf.NegativeInfinity;
            Transform rightmostSprite = null;
            foreach (var duplicate in duplicatesPool)
            {
                if (!(duplicate.position.x > rightmostX)) continue;

                rightmostSprite = duplicate;
                rightmostX = duplicate.position.x;
            }

            return rightmostSprite;
        }
    }
}