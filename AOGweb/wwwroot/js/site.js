function togglePasswordVisibility() {
    var x = document.getElementById("inputPwd");
    if (x.type === "password") {
        x.type = "text";
    } else {
        x.type = "password";
    }
}

function backToHome() {
    window.location.href = "https://" + window.location.hostname + "/Home";
}
