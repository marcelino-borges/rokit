const functions = require('firebase-functions');

// // Create and Deploy Your First Cloud Functions
// // https://firebase.google.com/docs/functions/write-firebase-functions
//
// exports.helloWorld = functions.https.onRequest((request, response) => {
//  response.send("Hello from Firebase!");
// });

const admin = require('firebase-admin');

var serviceAccount = require('E:\Meus_Projetos_Unity\projeto-madonna\functions\firebaseServiceAccount.json');

//admin.initializeApp({
//    credential: admin.credential.cert(serviceAccount),
//    databaseURL: 'https://rokit-b819e.firebaseio.com'
//});

//// Initialize the default app
//admin.initializeApp(defaultAppConfig);

//// Initialize another app with a different config
//var otherApp = admin.initializeApp(otherAppConfig, 'other');

//console.log(admin.app().name);  // '[DEFAULT]'
//console.log(otherApp.name);     // 'other'

//// Use the shorthand notation to retrieve the default app's services
//var defaultAuth = admin.auth();
//var defaultDatabase = admin.database();

//// Use the otherApp variable to retrieve the other app's services
//var otherAuth = otherApp.auth();
//var otherDatabase = otherApp.database();


