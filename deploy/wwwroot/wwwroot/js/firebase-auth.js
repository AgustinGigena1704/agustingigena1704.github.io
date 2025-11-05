// wwwroot/firebase-auth.js

import { initializeApp } from "https://www.gstatic.com/firebasejs/12.4.0/firebase-app.js";
import {
    getAuth,
    signInWithEmailAndPassword,
    signOut,
    createUserWithEmailAndPassword,
    onAuthStateChanged,
    signInWithPopup,
    GoogleAuthProvider,
    EmailAuthProvider,
    getIdToken,
    linkWithCredential,
    linkWithPopup
} from "https://www.gstatic.com/firebasejs/12.4.0/firebase-auth.js";

const firebaseConfig = {
    apiKey: "AIzaSyCNCKgn1N7rflnLxJb6lQiVsNT4J4ehtfE",
    authDomain: "ccc-web-5a445.firebaseapp.com",
    projectId: "ccc-web-5a445",
    storageBucket: "ccc-web-5a445.firebasestorage.app",
    messagingSenderId: "420857686145",
    appId: "1:420857686145:web:fe63026897c2469de34580",
    measurementId: "G-Q3RCQ6CKK1"
};

const app = initializeApp(firebaseConfig);
const auth = getAuth(app);
auth.useDeviceLanguage();

let dotNetAuthRef = null;

export function registerAuthStateDotNetRef(dotNetRef) {
    dotNetAuthRef = dotNetRef;
}
window.registerAuthStateDotNetRef = registerAuthStateDotNetRef;

const googleProvider = new GoogleAuthProvider();

export async function linkPasswordAccount(email, password) {
    var credential = EmailAuthProvider.credential(email, password);
    var result = await linkWithCredential(auth.currentUser, credential);
    const user = result.user;
    let token = await user.getIdToken(false);
    if (dotNetAuthRef) {
        dotNetAuthRef.invokeMethodAsync('FireBaseOnAuthStateChanged', token);
    }
    return {
        user: user,
        token: token
    }
}
window.linkPasswordAccount = linkPasswordAccount;

export async function linkGoogleAccount() {
    try {
        var result = await linkWithPopup(auth.currentUser, googleProvider);
        const user = result.user;
        let token = await user.getIdToken(false);
        if (dotNetAuthRef) {
            dotNetAuthRef.invokeMethodAsync('FireBaseOnAuthStateChanged', token);
        }
        return {
            user: user,
            token: token
        }
    } catch (error) {
        console.error("Error vinculando la cuenta de Google:", error);
        return { error: error.message };
    }
}
window.linkGoogleAccount = linkGoogleAccount


onAuthStateChanged(auth, async (user) => {
    let token = null; 
    if (user) { 
        token = await user.getIdToken(false); 
    }

    if (dotNetAuthRef) {
        dotNetAuthRef.invokeMethodAsync('FireBaseOnAuthStateChanged', token);
    }
});

export async function signInWithGoogle() {
    try {
        const result = await signInWithPopup(auth, googleProvider);
        const user = result.user;
        const token = await getIdToken(user, false);
        if (dotNetAuthRef) {
            await dotNetAuthRef.invokeMethodAsync('RegisterNewUser', user.uid);
        }
        return {
            user: user,
            token: token,
        };
    } catch (error) {
        const errorMessage = error?.message ?? String(error);
        return { error: errorMessage };
    }
}
window.signInWithGoogle = signInWithGoogle;

export async function firebaseLogin(email, password) {
    try {
        const userCredential = await signInWithEmailAndPassword(auth, email, password);
        const token = await getIdToken(userCredential.user, false)
        return {
            user: userCredential.user,
            token: token,
        };
    } catch (error) {
        return { error: error.message };
    }
}
window.firebaseLogin = firebaseLogin;

export async function firebaseLogout() {
    await signOut(auth);
}
window.firebaseLogout = firebaseLogout

export async function firebaseRegister(email, password) {
    try {
        const userCredential = await createUserWithEmailAndPassword(auth, email, password);
        const user = userCredential.user; 
        const token = await user.getIdToken(false);
        if (dotNetAuthRef) {
            dotNetAuthRef.invokeMethodAsync('RegisterNewUser', user.Uid);
        }
        return {
            user: user,
            token: token,
        };
    } catch (error) {
        return { error: error.message };
    }
}
window.firebaseRegister = firebaseRegister;
