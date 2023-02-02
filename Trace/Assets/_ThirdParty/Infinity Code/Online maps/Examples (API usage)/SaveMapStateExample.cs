/*         INFINITY CODE         */
/*   https://infinity-code.com   */

using System.Collections.Generic;
using UnityEngine;

namespace InfinityCode.OnlineMapsExamples
{
    /// <summary>
    /// Example of runtime saving map state.
    /// </summary>
    [AddComponentMenu("Infinity Code/Online Maps/Examples (API Usage)/SaveMapStateExample")]
    public class SaveMapStateExample : MonoBehaviour
    {
        public List<Texture2D> markerTextures;
        public List<GameObject> marker3DPrefabs;

        private string key = "MapSettings";

        private void LoadMarkerManager(OnlineMapsMarkerManager manager, OnlineMapsJSONItem json)
        {
            manager.RemoveAll();
            foreach (OnlineMapsJSONItem jitem in json)
            {
                double mx = jitem.ChildValue<double>("longitude");
                double my = jitem.ChildValue<double>("latitude");
                int textureIndex = jitem.ChildValue<int>("texture");
                Texture2D texture = null;
                if (textureIndex > -1 && textureIndex < markerTextures.Count) texture = markerTextures[textureIndex];
                string label = jitem.ChildValue<string>("label");

                OnlineMapsMarker marker = manager.Create(mx, my, texture, label);
                
                marker.range = jitem.ChildValue<OnlineMapsRange>("range");
                marker.align = (OnlineMapsAlign)jitem.ChildValue<int>("align");
                marker.rotation = jitem.ChildValue<float>("rotation");
                marker.enabled = jitem.ChildValue<bool>("enabled");
            }
        }

        private void LoadMarker3DManager(OnlineMapsMarker3DManager manager, OnlineMapsJSONItem json)
        {
            manager.RemoveAll();
            foreach (OnlineMapsJSONItem jitem in json)
            {
                double mx = jitem.ChildValue<double>("longitude");
                double my = jitem.ChildValue<double>("latitude");
                int prefabIndex = jitem.ChildValue<int>("prefab");
                GameObject prefab = null;
                if (prefabIndex > -1 && prefabIndex < marker3DPrefabs.Count) prefab = marker3DPrefabs[prefabIndex];

                OnlineMapsMarker3D marker = manager.Create(mx, my, prefab);

                marker.range = jitem.ChildValue<OnlineMapsRange>("range");
                marker.label = jitem.ChildValue<string>("label");
                marker.rotationY = jitem.ChildValue<float>("rotationY");
                marker.scale = jitem.ChildValue<float>("scale");
                marker.enabled = jitem.ChildValue<bool>("enabled");
                marker.sizeType = (OnlineMapsMarker3D.SizeType)jitem.ChildValue<int>("sizeType");
            }
        }

        /// <summary>
        /// Loading saved state.
        /// </summary>
        private void LoadState()
        {
            if (!PlayerPrefs.HasKey(key)) return;

            OnlineMaps map = OnlineMaps.instance;

            // Load map position and zoom
            string settings = PlayerPrefs.GetString(key);
            OnlineMapsJSONItem json = OnlineMapsJSON.Parse(settings);
            OnlineMapsJSONItem jpos = json["Map/Coordinates"];
            map.position = jpos.Deserialize<Vector2>();
            map.floatZoom = json["Map/Zoom"].V<float>();

            // Load 2D and 3D markers
            LoadMarkerManager(OnlineMapsMarkerManager.instance, json["Markers"]);
            LoadMarker3DManager(OnlineMapsMarker3DManager.instance, json["Markers3D"]);
        }

        private void OnGUI()
        {
            // By clicking on the button to save the current state.
            if (GUI.Button(new Rect(5, 5, 150, 30), "Save State")) SaveState();
        }

        private void SaveState()
        {
            OnlineMapsJSONObject json = new OnlineMapsJSONObject();

            // Save position and zoom
            OnlineMaps map = OnlineMaps.instance;
            OnlineMapsJSONObject jmap = new OnlineMapsJSONObject();
            json.Add("Map", jmap);
            jmap.Add("Coordinates", map.position);
            jmap.Add("Zoom", map.floatZoom);

            // Save 2D markers
            OnlineMapsJSONArray jmarkers = new OnlineMapsJSONArray();
            foreach (OnlineMapsMarker marker in OnlineMapsMarkerManager.instance)
            {
                OnlineMapsJSONObject jmarker = marker.ToJSON() as OnlineMapsJSONObject;
                jmarker.Add("texture", markerTextures.IndexOf(marker.texture));
                jmarkers.Add(jmarker);
            }
            json.Add("Markers", jmarkers);

            // Save 3D markers
            OnlineMapsJSONArray jmarkers3d = new OnlineMapsJSONArray();
            foreach (OnlineMapsMarker3D marker in OnlineMapsMarker3DManager.instance)
            {
                OnlineMapsJSONObject jmarker = marker.ToJSON() as OnlineMapsJSONObject;
                jmarker.Add("prefab", marker3DPrefabs.IndexOf(marker.prefab));
                jmarkers3d.Add(jmarker);
            }
            json.Add("Markers3D", jmarkers3d);

            Debug.Log(json.ToString());
            
            // Save settings to PlayerPrefs
            PlayerPrefs.SetString(key, json.ToString());
        }

        private void Start()
        {
            LoadState();
        }
    }
}