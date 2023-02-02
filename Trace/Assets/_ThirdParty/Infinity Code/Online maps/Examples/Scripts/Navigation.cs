/*         INFINITY CODE         */
/*   https://infinity-code.com   */

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace InfinityCode.OnlineMapsDemos
{
    public class Navigation : MonoBehaviour
    {
        /// <summary>
        /// Reference to Control. If missed, the singleton value will be used.
        /// </summary>
        public OnlineMapsTileSetControl control;

        /// <summary>
        /// Reference to destination input field.
        /// </summary>
        public InputField destinationInput;

        /// <summary>
        /// Reference to route confirmation UI.
        /// </summary>
        public GameObject confirmationUI;

        /// <summary>
        /// Reference to navigation UI.
        /// </summary>
        public GameObject navigationUI;

        /// <summary>
        /// Reference to finish UI.
        /// </summary>
        public GameObject finishUI;

        /// <summary>
        /// Reference to the field for displaying the total distance of the route.
        /// </summary>
        public Text totalDistanceText;

        /// <summary>
        /// Reference to the field for displaying the total duration of the route.
        /// </summary>
        public Text totalDurationText;

        /// <summary>
        /// Reference to the field for displaying the remain distance of the route.
        /// </summary>
        public Text remainDistanceText;

        /// <summary>
        /// Reference to the field for displaying the remain duration of the route.
        /// </summary>
        public Text remainDurationText;

        /// <summary>
        /// Reference to the field for displaying the instruction of the step.
        /// </summary>
        public Text instructionText;

        /// <summary>
        /// Reference to the field for displaying the total distance of the route in confirmation UI.
        /// </summary>
        public Text confirmationTotalDistanceText;

        /// <summary>
        /// Reference to the field for displaying the total duration of the route in confirmation UI.
        /// </summary>
        public Text confirmationTotalDurationText;

        /// <summary>
        /// The distance (km) from the user's location to the nearest point on the route to consider that he left the route.
        /// </summary>
        public float updateRouteAfterKm = 0.05f;

        /// <summary>
        /// Delay to find a new route.
        /// </summary>
        public float updateRouteDelay = 10;

        /// <summary>
        /// If TRUE the user's GPS position will be used, if FALSE the marker position will be used, which you can drag.
        /// </summary>
        public bool useLocationServicePosition;

        /// <summary>
        /// Prefab of 3D marker.
        /// </summary>
        public GameObject markerPrefab;

        private OnlineMaps map;
        private OnlineMapsMarker3D marker;
        private OnlineMapsDrawingLine routeLine;
        private OnlineMapsDrawingLine coveredLine;
        private int totalDistance;
        private int totalDuration;
        private OnlineMapsGoogleDirectionsResult.Step[] steps;
        private OnlineMapsVector2d[] routePoints;
        private List<OnlineMapsVector2d> remainPoints;
        private List<OnlineMapsVector2d> coveredPoints;
        private Vector2 lastPosition;
        private bool followRoute;
        private int currentStepIndex = -1;
        private int pointIndex = -1;
        private OnlineMapsVector2d lastPointOnRoute;
        private float timeToUpdateRoute = int.MinValue;
        private OnlineMapsVector2d destinationPoint;

        /// <summary>
        /// Cancels navigation from confirmation UI.
        /// </summary>
        public void CancelNavigation()
        {
            // Hide UI
            confirmationUI.SetActive(false);

            // Remove route line.
            control.drawingElementManager.Remove(routeLine);

            // Clear references.
            routeLine = null;
            routePoints = null;
            steps = null;
            followRoute = false;
        }

        /// <summary>
        /// Checks if the user has reached the destination.
        /// </summary>
        /// <param name="position">User's position</param>
        /// <returns>Whether the user has reached the destination</returns>
        private bool CheckFinished(Vector2 position)
        {
            if (currentStepIndex != steps.Length - 1) return false;

            // Get distance between user and destination
            double d = OnlineMapsUtils.DistanceBetweenPoints(position.x, position.y, 0, destinationPoint.x, destinationPoint.y, 0);

            // If the distance is less than the threshold, the user has reached the destination
            if (d < 0.02) return false;

            // Stop navigation and show finish UI
            followRoute = false;
            navigationUI.SetActive(false);
            finishUI.SetActive(true);

            Debug.Log("Finished");

            return true;
        }

        /// <summary>
        /// Closes finish UI.
        /// </summary>
        public void CloseFinishUI()
        {
            // Hide UI
            finishUI.SetActive(false);

            // Remove covered and remain lines.
            control.drawingElementManager.Remove(routeLine);
            control.drawingElementManager.Remove(coveredLine);
        }

        /// <summary>
        /// Initial search for a route to a destination by clicking on the search button.
        /// </summary>
        public void FindRoute()
        {
            // Check for Google Maps API key
            if (!OnlineMapsKeyManager.hasGoogleMaps)
            {
                Debug.LogWarning("Please enter Map / Key Manager / Google Maps");
                return;
            }

            // Send request to Google Directions API
            OnlineMapsGoogleDirections request = OnlineMapsGoogleDirections.Find(
                new OnlineMapsGoogleDirections.Params(
                    GetUserLocation(),
                    destinationInput.text));

            // When the request is complete call the OnRequestComplete method.
            request.OnComplete += OnRequestComplete;

            // Hide UI
            confirmationUI.SetActive(false);
            navigationUI.SetActive(false);
            finishUI.SetActive(false);
        }

        /// <summary>
        /// Converts the distance in meters to a human readable string.
        /// </summary>
        /// <param name="distance">Distance in meters</param>
        /// <returns>Human readable distance string</returns>
        private string GetDistanceString(int distance)
        {
            if (distance < 1000) return distance + "m";
            if (distance < 10000) return (distance / 1000f).ToString("F2") + "km";
            return distance / 1000 + "km";
        }

        /// <summary>
        /// Converts the duration in seconds to a human readable string.
        /// </summary>
        /// <param name="duration">Duration in seconds</param>
        /// <returns>Human readable duration string</returns>
        public string GetDurationString(int duration)
        {
            if (duration > 3600) return duration / 3600 + "h " + duration % 3600 / 60 + "m";
            return duration / 60 + "m";
        }

        /// <summary>
        /// Finds the nearest point on the route and checks if the user has left the route.
        /// </summary>
        /// <param name="position">User location.</param>
        /// <param name="positionOnRoute">Returns the nearest point on the route.</param>
        /// <param name="pointChanged">Returns whether the number of the route point in use has changed.</param>
        /// <returns>Returns whether the user is following the route.</returns>
        private bool GetPointOnRoute(Vector2 position, out OnlineMapsVector2d positionOnRoute, out bool pointChanged)
        {
            pointChanged = false;
            var step = steps[currentStepIndex];
            OnlineMapsVector2d p1 = step.polylineD[pointIndex];
            OnlineMapsVector2d p2 = step.polylineD[pointIndex + 1];
            OnlineMapsVector2d p;
            double dist;

            if (p1 != p2)
            {
                // Check if the user is on the same route point.
                OnlineMapsUtils.NearestPointStrict(position.x, position.y, p1.x, p1.y, p2.x, p2.y, out p.x, out p.y);
                if (p != p2)
                {
                    dist = OnlineMapsUtils.DistanceBetweenPoints(p.x, p.y, 0, position.x, position.y, 0);

                    if (dist < updateRouteAfterKm)
                    {
                        timeToUpdateRoute = int.MinValue;
                        positionOnRoute = p;
                        return true;
                    }
                }
            }

            // Checking what step and point the user is on
            for (int i = currentStepIndex; i < steps.Length; i++)
            {
                step = steps[i];
                OnlineMapsVector2d[] polyline = step.polylineD;

                for (int j = pointIndex; j < polyline.Length - 1; j++)
                {
                    p1 = polyline[j];
                    p2 = polyline[j + 1];
                    OnlineMapsUtils.NearestPointStrict(position.x, position.y, p1.x, p1.y, p2.x, p2.y, out p.x, out p.y);
                    if (p == p2) continue;

                    dist = OnlineMapsUtils.DistanceBetweenPoints(p.x, p.y, 0, position.x, position.y, 0);
                    if (dist < updateRouteAfterKm)
                    {
                        // Update the step instruction and save the index of step and point.
                        instructionText.text = step.string_instructions;
                        currentStepIndex = i;
                        pointChanged = true;
                        pointIndex = j;
                        timeToUpdateRoute = int.MinValue;
                        positionOnRoute = p;
                        if (!useLocationServicePosition) marker.LookToCoordinates(polyline[polyline.Length - 1]);
                        return true;
                    }
                }

                pointIndex = 0;
            }

            // The user has left the route. If the countdown to the search for a new route has not started, we start it.
            if (timeToUpdateRoute < -999) timeToUpdateRoute = updateRouteDelay;

            positionOnRoute = lastPointOnRoute;
            return false;
        }

        /// <summary>
        /// Gets the user's location.
        /// </summary>
        /// <returns>User's location</returns>
        private Vector2 GetUserLocation()
        {
            if (useLocationServicePosition) return OnlineMapsLocationService.instance.position;
            return marker.position;
        }

        /// <summary>
        /// Called when the compass value has been changed.
        /// </summary>
        /// <param name="rotation">Compass true heading (0-1)</param>
        private void OnCompassChanged(float rotation)
        {
            // Set the rotation of the marker.
            marker.rotationY = rotation;
        }

        /// <summary>
        /// Called when the user's GPS position has changed.
        /// </summary>
        /// <param name="position">User's GPS position</param>
        private void OnLocationChanged(Vector2 position)
        {
            // Set the position of the marker.
            marker.position = position;
        }

        /// <summary>
        /// This method is called when the Google Directions API returned a response.
        /// </summary>
        /// <param name="response">Response from the service</param>
        private void OnRequestComplete(string response)
        {
            // Parse a response
            OnlineMapsGoogleDirectionsResult result = OnlineMapsGoogleDirections.GetResult(response);

            // If there are no routes, return
            if (result.routes.Length == 0)
            {
                Debug.Log("Can't find route");
                return;
            }

            OnlineMapsGoogleDirectionsResult.Route route = result.routes[0];
            if (route == null)
            {
                Debug.Log("Can't find route");
                return;
            }

            // Reset step and point indices
            currentStepIndex = 0;
            pointIndex = 0;

            // Get steps from the route
            steps = route.legs.SelectMany(l => l.steps).ToArray();

            // Get route points from steps
            routePoints = steps.SelectMany(s => s.polylineD).ToArray();

            // The remaining points are the entire route
            remainPoints = routePoints.ToList();

            // The destination is the last point
            destinationPoint = routePoints.Last();

            // Create a line and add it to the map
            if (routeLine == null)
            {
                routeLine = new OnlineMapsDrawingLine(remainPoints, Color.green, 3);
                control.drawingElementManager.Add(routeLine);
            }
            else routeLine.points = remainPoints;

            // Calculate total distance and duration
            totalDistance = route.legs.Sum(l => l.distance.value);
            totalDuration = route.legs.Sum(l => l.duration.value);

            // Set distance, duration and first instruction on UI
            confirmationTotalDistanceText.text = totalDistanceText.text = "Total Distance: " + GetDistanceString(totalDistance);
            confirmationTotalDurationText.text = totalDurationText.text = "Total Duration: " + GetDurationString(totalDuration);
            remainDistanceText.text = "Remain Distance: " + GetDistanceString(totalDistance);
            remainDurationText.text = "Remain Duration: " + GetDurationString(totalDuration);
            instructionText.text = steps[0].string_instructions;

            // Show the whole route
            OnlineMapsGPXObject.Bounds b = route.bounds;

            Vector2[] bounds = 
            {
                new Vector2((float) b.minlon, (float) b.maxlat),
                new Vector2((float) b.maxlon, (float) b.minlat),
            };

            Vector2 center;
            int zoom;
            OnlineMapsUtils.GetCenterPointAndZoom(bounds, out center, out zoom);
            
            map.SetPositionAndZoom(center.x, center.y, zoom);
            lastPosition = marker.position;

            // If a marker position is used, turn it towards the second point
            if (!useLocationServicePosition) marker.LookToCoordinates(routePoints[1]);

            // Show confirmation UI
            confirmationUI.SetActive(true);
        }

        /// <summary>
        /// This method is called when Google Directions API returned updated route.
        /// </summary>
        /// <param name="response"></param>
        private void OnUpdateRouteComplete(string response)
        {
            // Parse a response
            OnlineMapsGoogleDirectionsResult result = OnlineMapsGoogleDirections.GetResult(response);

            // If there are no routes, return
            if (result.routes.Length == 0)
            {
                Debug.Log("Can't find route");
                return;
            }

            OnlineMapsGoogleDirectionsResult.Route route = result.routes[0];
            if (route == null)
            {
                Debug.Log("Can't find route");
                return;
            }

            // Get steps from route
            steps = route.legs.SelectMany(l => l.steps).ToArray();

            // Get route points from steps
            routePoints = steps.SelectMany(s => s.polylineD).ToArray();
            destinationPoint = routePoints.Last();

            // Calculate total distance and duration
            totalDistance = route.legs.Sum(l => l.distance.value);
            totalDuration = route.legs.Sum(l => l.duration.value);

            // Set distance, duration and first instruction on UI
            confirmationTotalDistanceText.text = totalDistanceText.text = "Total Distance: " + GetDistanceString(totalDistance);
            confirmationTotalDurationText.text = totalDurationText.text = "Total Duration: " + GetDurationString(totalDuration);
            remainDistanceText.text = "Remain Distance: " + GetDistanceString(totalDistance);
            remainDurationText.text = "Remain Duration: " + GetDurationString(totalDuration);
            instructionText.text = steps[0].string_instructions;

            // Reset step and point indices
            currentStepIndex = 0;
            pointIndex = 0;

            // Update covered and remain lines
            UpdateLines();
        }

        /// <summary>
        /// Requests an updated route
        /// </summary>
        private void RequestUpdateRoute()
        {
            if (!OnlineMapsKeyManager.hasGoogleMaps)
            {
                Debug.LogWarning("Please enter Map / Key Manager / Google Maps");
            }

            // Send request to Google Directions API
            OnlineMapsGoogleDirections request = OnlineMapsGoogleDirections.Find(
                new OnlineMapsGoogleDirections.Params(
                    GetUserLocation(),
                    destinationInput.text));

            // When the request is complete call OnUpdateRouteComplete method.
            request.OnComplete += OnUpdateRouteComplete;
        }

        private void Start()
        {
            // Get map and control instances
            if (control == null) control = OnlineMapsTileSetControl.instance;
            map = control.map;

            // Hide UI
            confirmationUI.SetActive(false);
            navigationUI.SetActive(false);
            finishUI.SetActive(false);

            // Create a new marker in the center of the map
            double longitude, latitude;
            map.GetPosition(out longitude, out latitude);
            marker = control.marker3DManager.Create(longitude, latitude, markerPrefab);

            // If use location service, subscribe to events
            // Else make a marker draggable
            if (useLocationServicePosition)
            {
                OnlineMapsLocationService.instance.OnLocationChanged += OnLocationChanged;
                OnlineMapsLocationService.instance.OnCompassChanged += OnCompassChanged;
            }
            else marker.isDraggable = true;
        }

        /// <summary>
        /// Starts navigation from confirmation UI
        /// </summary>
        public void StartNavigation()
        {
            // Hide confirmation UI and show navigation UI
            confirmationUI.SetActive(false);
            navigationUI.SetActive(true);

            // Zoom in on the map at the first route point
            map.SetPositionAndZoom(routePoints[0].x, routePoints[0].y, 19);

            // Create covered line
            coveredPoints = new List<OnlineMapsVector2d>(routePoints.Length);
            coveredLine = new OnlineMapsDrawingLine(coveredPoints, Color.gray, 3);
            control.drawingElementManager.Add(coveredLine);

            // Start navigation and reset indices
            followRoute = true;
            currentStepIndex = 0;
            pointIndex = 0;
        }

        /// <summary>
        /// Called every frame
        /// </summary>
        private void Update()
        {
            // If navigation is not started, return
            if (!followRoute) return;

            // If the user has left the route, wait for a delay and request a new route
            if (timeToUpdateRoute > 0)
            {
                timeToUpdateRoute -= Time.deltaTime;
                if (timeToUpdateRoute <= 0)
                {
                    timeToUpdateRoute = int.MinValue;
                    RequestUpdateRoute();
                }
            }

            // Get the position of the marker, and if it hasn't changed, return
            Vector2 markerPosition = marker.position;
            if (markerPosition == lastPosition) return;

            lastPosition = markerPosition;
            bool pointChanged;

            // Check if the user has reached the destination
            if (CheckFinished(markerPosition))
            {

            }
            // Get the nearest point on a route
            else if (GetPointOnRoute(markerPosition, out lastPointOnRoute, out pointChanged))
            {
                // Update covered and remain lines
                UpdateLines();

                // If the point index has changed, update the distance and duration on UI
                if (pointChanged) UpdateRemainDistanceAndDuration();

                // Redraw the map
                map.Redraw();
            }
            else
            {
                // The user has left the route
                Debug.Log(false);
            }
        }

        /// <summary>
        /// Update the distance and duration on UI
        /// </summary>
        private void UpdateRemainDistanceAndDuration()
        {
            int coveredDistance = 0;
            int coveredDuration = 0;

            OnlineMapsGoogleDirectionsResult.Step s;

            // Sum the distances and the duration of covered steps
            for (int i = 0; i < currentStepIndex; i++)
            {
                s = steps[i];
                coveredDistance += s.distance.value;
                coveredDuration += s.duration.value;
            }

            s = steps[currentStepIndex];
            OnlineMapsVector2d[] polyline = s.polylineD;
            double stepDistance = 0;

            // Sum the distance between covered points on current step
            for (int i = 0; i < pointIndex - 1; i++)
            {
                OnlineMapsVector2d p1 = polyline[i];
                OnlineMapsVector2d p2 = polyline[i + 1];
                stepDistance += OnlineMapsUtils.DistanceBetweenPoints(p1.x, p1.y, 0, p2.x, p2.y, 0) * 1000;
            }

            // Add the progress of the current step to the covered distance and duration.
            if (stepDistance > s.distance.value) stepDistance = s.distance.value;
            coveredDistance += (int) stepDistance;
            coveredDuration += (int) (stepDistance / s.distance.value * s.duration.value);

            // Set remain distance and duration to UI
            remainDistanceText.text = "Remain Distance: " + GetDistanceString(totalDistance - coveredDistance);
            remainDurationText.text = "Remain Duration: " + GetDurationString(totalDuration - coveredDuration);
        }

        /// <summary>
        /// Updates covered and remain lines
        /// </summary>
        private void UpdateLines()
        {
            // Clears line points.
            // It doesn't make sense to create new lines here, because drawing elements keeps a reference to the lists.
            coveredPoints.Clear();
            remainPoints.Clear();

            // Iterate all steps.
            for (int i = 0; i < steps.Length; i++)
            {
                // Get a polyline
                var step = steps[i];
                OnlineMapsVector2d[] polyline = step.polylineD;

                // Iterate all points of polyline
                for (int j = 0; j < polyline.Length; j++)
                {
                    OnlineMapsVector2d p = polyline[j];

                    // If index of step less than current step, add to covered list
                    // If index of step greater than current step, add to remain list
                    // If this is current step, points than less current point add to covered list, otherwise add to remain list
                    if (i < currentStepIndex)
                    {
                        coveredPoints.Add(p);
                    }
                    else if (i > currentStepIndex)
                    {
                        remainPoints.Add(p);
                    }
                    else
                    {
                        if (j < pointIndex)
                        {
                            coveredPoints.Add(p);
                        }
                        else if (j > pointIndex)
                        {
                            remainPoints.Add(p);
                        }
                        else
                        {
                            coveredPoints.Add(p);
                            coveredPoints.Add(lastPointOnRoute);
                            remainPoints.Add(lastPointOnRoute);
                        }
                    }
                }
            }
        }
    }
}