/* eslint-disable */
const functions = require("firebase-functions");
const admin = require("firebase-admin");

admin.initializeApp();

exports.sendNotification = functions.database.ref("/allFriendRequests/{notificationId}").onWrite((change, context) => {
    const notificationId = context.params.notificationId;

    // Retrieve the notification data from the Realtime Database
    const notificationData = change.after.val();

    // Get the FCM Server Key from Firebase Console
    const serverKey = "AAAA0A2ku6o:APA91bFT-j6XeI1Nfdwk6KNSqfnEuGtvm-Qzv1Y3A5oEbPMgoIGNqqI4F_8DAcEasTkz1rN71p53dOwrKe26cf4RI96cyzjOIJuF6PDb790cdcXTl2R7SXILbmxTiBqTGDS_RUfiMRH_";

    // Create a new Firebase Cloud Messaging client
    const fcm = admin.messaging();

    // Build the notification payload
    const payload = {
        notification: {
            title: "MEssage fom function",
            body: "Message body from function"
        }
    };

const fcm_key = "cE2JRs7i20TpgXPNeOngCk:APA91bGMEdZi67ULSA47rPjJGfCPNpuaAWDcW6K_y5WXRf89Y1X1y_8WnJPu8Wu-eVtGkelb4Sp5SxPEf-0_tg2KfY_ZAPGl35owqqrVS9aDa3AyqFPolY8tfawK1K0peBpiXjJ_4ycJ";

    // Send the notification to the device(s) with the FCM token(s)
    return fcm.sendToDevice(fcm_key, payload, { priority: "high", timeToLive: 60 * 60 * 24 })
        .then((response) => {
            console.log("Notification sent successfully:", response);
            // Update the notification status in the Realtime Database
            // return admin.database().ref(`/allFriendRequests/${notificationId}/status`).set("sent");
        })
        .catch((error) => {
            console.error("Error sending notification:", error);
            // Update the notification status in the Realtime Database
            // return admin.database().ref(`/allFriendRequests/${notificationId}/status`).set("error");
        });
});
