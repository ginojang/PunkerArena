//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// wt.shin: Enumeration utilities.
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEngine.UI;

namespace Devil.Common
{
    public static class GameObjectUtility
    {
		public static void Reset(this Transform trans)
        {
            trans.position = Vector3.zero;
            trans.rotation = Quaternion.identity;
            trans.localScale = Vector3.one;
        }

        public static GameObject InstantiateGameObject(string prefabPath, GameObject objectForParenting = null, bool worldPositionStays = false)
        {
            GameObject retObject = null;
            //retObject = Object.Instantiate(Resources.Load("Prefabs/" + prefabPath, typeof(GameObject)) as GameObject);
            retObject = Object.Instantiate(Resources.Load(prefabPath, typeof(GameObject)) as GameObject);

            if (objectForParenting != null)
                retObject.transform.SetParent(objectForParenting.transform, worldPositionStays);

            return retObject;
        }        

		public static GameObject InstantiateGameObject(GameObject prefab, GameObject objectForParenting = null, bool worldPositionStays = false)
        {
            GameObject retObject = Object.Instantiate<GameObject>(prefab);
            if (objectForParenting != null)
                retObject.transform.SetParent(objectForParenting.transform, worldPositionStays);

            return retObject;
        }        

        public static GameObject InstantiateUIGameObject(GameObject prefab, Transform parent)
        {
            return Object.Instantiate(prefab, parent);
        }        

        public static T GetBaseClass<T>(this GameObject gameObject) where T : class
        {
            T returnValue = null;

            foreach (var c in gameObject.GetComponents<Component>())
            {
                if (c is T)
                    return c as T;
            }

            return returnValue;
        }

        public static bool IsDestroyed(this GameObject go)
        {
            // Checks if a GameObject has been destroyed.
            // UnityEngine overloads the == opeator for the GameObject type and returns null when the object has been destroyed, However,
            // actually the object is still there but has not been cleaned up yet if we test both we can determine if the object has been destroyed.
            return go == null && !ReferenceEquals(go, null);
        }

        #region Find game objects
        /// <summary>
        /// FindChildByName tries to find it's first child siblings
        /// </summary>
        /// <param name="go"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static GameObject FindChildByName(this GameObject go, string name)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                if (go.transform.GetChild(i).gameObject.name.CompareTo(name) == 0)
                    return go.transform.GetChild(i).gameObject;
            }

            return null;
        }

        // FindChildByName doesn't go through it's game object's child game objects but FindChildByNameInChildren does.
        public static GameObject FindChildByNameInChildren(this GameObject go, string name)
        {
            if (go.name.CompareTo(name) == 0)
                return go;

            for (int i = 0; i < go.transform.childCount; i++)
            {
                GameObject ret = go.transform.GetChild(i).gameObject.FindChildByNameInChildren(name);
                if (ret != null)
                    return ret;
            }

            return null;
        }

        public static GameObject GetParentGameObject(this GameObject goChild)
        {
            return goChild.transform.parent.gameObject;
        }
        #endregion

        #region Fade
        /// <summary>
        /// fade the game object including all of it's children only if they have an image component
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="endValue"></param>
        /// <param name="duration"></param>
        public static void Fade(this GameObject gameObject, float endValue, float duration)
        {       
			/*
            Image theImage = gameObject.GetComponent<Image>();
            if (theImage != null)
            {
                theImage.DOFade(endValue, duration);
            }

            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                gameObject.transform.GetChild(i).gameObject.Fade(endValue, duration);
            }*/
        }

        /// <summary>
        /// fade-out the game object immediately including all of it's children only if they have an image component
        /// </summary>
        /// <param name="gameObject"></param>
        public static void FadeOutImmediate(this GameObject gameObject)
        {        
            Image theImage = gameObject.GetComponent<Image>();
            if (theImage != null)
            {
                theImage.color = new Color(theImage.color.r, theImage.color.g, theImage.color.b, 0f);
            }

            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                gameObject.transform.GetChild(i).gameObject.FadeOutImmediate();
            }
        }
        #endregion

        public static bool Pick(this GameObject go, Camera camera, ref Vector3 pickedPos, float rayMaxDist = 100f)
        {
            RaycastHit hit;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, rayMaxDist, 1 << go.layer))
            {
                if (hit.collider.gameObject.name.Equals(go.name))
                {
                    pickedPos = hit.point;
                    return true;
                }                                
            }

            return false;
        }

		#region Collision
		public static bool CollisionTestRayToSkinMeshBounds(this GameObject go, Ray ray, Camera camera, ref Vector3 pickedPos, float rayMaxDist)
        {
            SkinnedMeshRenderer[] theRenderers = go.GetComponentsInChildren<SkinnedMeshRenderer>();
            int theClosestRendererIndex = -1;
            float theClosestDistance = -1;
            for (int i = 0; i < theRenderers.Length; i++)
            {
                float distance;
                if (theRenderers[i].bounds.IntersectRay(ray, out distance))
                {
                    if (theClosestRendererIndex == -1)
                    {
                        theClosestRendererIndex = i;
                        theClosestDistance = distance;
                    }
                    else
                    {
                        if (distance < theClosestDistance)
                        {
                            theClosestRendererIndex = i;
                            theClosestDistance = distance;
                        }
                    }
                }
            }

            if (theClosestRendererIndex > -1)
            {
                pickedPos = theRenderers[theClosestRendererIndex].bounds.ClosestPoint(ray.origin);
                return true;
            }

            return false;
            /*
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, rayMaxDist, 1 << go.layer))
            {
                if (hit.collider.gameObject.name.Equals(go.name))
                {
                    pickedPos = hit.point;
                    return true;
                }
            }

            return false;
            */
        }

        public static bool CollisionTestRayToSpawnColliders(this GameObject go, Ray ray, Camera camera, ref Vector3 pickedPos, float rayMaxDist)
        {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, rayMaxDist, (1 << LayerMask.NameToLayer("PlayerSpawnSpot"))))
            {
                pickedPos = hit.point;
                return true;
            }

            return false;
        }
        #endregion collision
    }
}