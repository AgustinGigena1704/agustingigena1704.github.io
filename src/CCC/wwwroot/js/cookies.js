export function setCookie(key, value, expire) {
    document.cookie = `${key}=${value}; expires=${expire}; path=/; Secure; SameSite=Strict`;
}
window.setCookie = setCookie;
export function deleteCookie(key) {
    document.cookie = `${key}=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/; Secure; SameSite=Strict`;
}

window.deleteCookie = deleteCookie;
export function getCookie(key) {
    const cookies = document.cookie.split("; ");
    for (const cookie of cookies) {
        const [cookieKey, cookieValue] = cookie.split("=");
        if (decodeURIComponent(cookieKey) === key) {
            return decodeURIComponent(cookieValue); 
        }
    }
    return null;
}
window.getCookie = getCookie;