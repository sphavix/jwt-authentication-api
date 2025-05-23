﻿//cookies.js
function setCookie(name, value, days) {
    var expiresAt = "";

    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expiresAt = "; expiresAt=" + date.toUTCString();
    }
    document.cookie = name + "=" + (value || "") + expiresAt + "; path=/";
}

function deleteCookie(name) {
    document.cookie = name + '=; Max-Age=-99999999;';
}

function getCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');

    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];

        while (c.charAt(0) == ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
    }

    return null;
}